using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hkmp.Api.Server;
using Hkmp.ModDiff.Models;
using Hkmp.ModDiff.Packets;
using Hkmp.Networking.Packet.Data;
using Modding;

namespace Hkmp.ModDiff.Services
{
    internal class ServerNetService
    {
        public ServerNetService(ILogger logger, Server addon, IServerApi serverApi)
        {
            var receiver = serverApi.NetServer.GetNetworkReceiver<ModListPacketId>(addon, (_) => new ModListPacket());
            receiver.RegisterPacketHandler<ModListPacket>(
                ModListPacketId.ModListClientData, (id, data) => HandleModlist(id, data, serverApi, logger));
        }

        private void HandleModlist(ushort id, ModListPacket data, IServerApi serverApi, ILogger logger)
        {
            var clientMods = data.PlayerInfo.InstalledMods;
            var serverMods = ModHooks.GetAllMods(true).Select(m => new ModVersion
            {
                Name = m.GetName(),
                Version = m.GetVersion()
            }).ToList();

            var modInfo = CalculateModDiff(serverMods, clientMods);

            var isMatch = CheckIsMatch(modInfo, MenuMod.Configuration.MatchType);

            logger.Info(this,
                !isMatch
                    ? $"{data.PlayerInfo.PlayerName}'s mods DO NOT match!"
                    : $"{data.PlayerInfo.PlayerName} joined with matching mods!");

            MessageModDiscrepancies(serverApi, id, modInfo, isMatch);
            // Todo: Setup config for "Strict Diff" and "Disconnect on Mismatch"

            if (!isMatch && MenuMod.Configuration.KickOnMistmatch == BooleanEnum.True)
            {
                // Disconnecting in real time causes a weird race condition as the player is still in it's connection event.
                // delaying this action arbitrarily fixes it.
                Task.Run(async () =>
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    serverApi.ServerManager.DisconnectPlayer(id, DisconnectReason.Kicked);
                });
            }
        }

        private static Models.ModInfo CalculateModDiff(List<ModVersion> serverMods, List<ModVersion> clientMods)
        {
            // dicts for faster comparisons assuming huge mod lists.
            var serverModsTable = serverMods.ToDictionary(m => m.Name, m => m.Version);
            var clientModTable = clientMods.ToDictionary(m => m.Name, m => m.Version);

            var missingMods = serverMods.Where(m => !clientModTable.ContainsKey(m.Name)).ToList();
            var extraMods = new List<ModVersion>();
            var wrongVersionMods = new List<ModVersionMismatch>();

            foreach (var clientMod in clientMods)
            {
                if (serverModsTable.TryGetValue(clientMod.Name, out var serverModVersion))
                {
                    if (serverModVersion != clientMod.Version)
                    {
                        wrongVersionMods.Add(new ModVersionMismatch
                        {
                            Name = clientMod.Name,
                            Version = clientMod.Version,
                            Expected = serverModVersion
                        });
                    }
                }
                else
                {
                    extraMods.Add(clientMod);
                }
            }

            return new Models.ModInfo
            {
                ExtraMods = extraMods,
                MissingMods = missingMods,
                WrongVersions = wrongVersionMods
            };
        }

        private static bool CheckIsMatch(Models.ModInfo info, MatchType type)
        {
            switch (type)
            {
                case MatchType.Strict:
                    return !info.MissingMods.Any() && !info.WrongVersions.Any() && !info.ExtraMods.Any();
                case MatchType.Loose:
                default:
                    return !info.MissingMods.Any() && !info.WrongVersions.Any();
            }
        }

        private static void MessageModDiscrepancies(IServerApi api, ushort playerId, Models.ModInfo info, bool isMatch)
        {
            var playerFound = api.ServerManager.TryGetPlayer(playerId, out var player);

            if (!playerFound)
            {
                // Already DC'd
                return;
            }

            api.ServerManager.SendMessage(player, isMatch ? "Mod Diff Check: Match!" : "Mod Diff Check: Mismatch!");

            if (info.ExtraMods.Any())
            {
                api.ServerManager.SendMessage(player, "Extra Mods:");
                foreach (var mod in info.ExtraMods)
                {
                    api.ServerManager.SendMessage(player, $"- {mod.Name} @ {mod.Version}");
                }
            }

            if (info.MissingMods.Any())
            {
                api.ServerManager.SendMessage(player, "Missing Mods:");
                foreach (var mod in info.MissingMods)
                {
                    api.ServerManager.SendMessage(player, $"- {mod.Name} @ {mod.Version}");
                }
            }

            if (info.WrongVersions.Any())
            {
                api.ServerManager.SendMessage(player, "Wrong Versions:");
                foreach (var mod in info.WrongVersions)
                {
                    api.ServerManager.SendMessage(player, $"- {mod.Name} @ {mod.Version} expected {mod.Expected}");
                }
            }
        }
    }
}
