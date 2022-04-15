using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hkmp.Api.Server;
using Hkmp.ModDiff.Models;
using Hkmp.ModDiff.Packets;
using Hkmp.Networking.Packet.Data;
using Newtonsoft.Json;

namespace Hkmp.ModDiff.Services
{
    internal class ServerNetService
    {
        private List<ModVersion> _knownMods = new List<ModVersion>();
        private Configuration _configuration;

        // This is technically IDisposable but this is a notional singleton so we should be fine.
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FileSystemWatcher _modListWatcher;
        private readonly ILogger _logger;

        private static readonly string DllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private const string ModlistFileName = "modlist.json";
        private const string ConfigFileName = "moddiff_config.json";

        public ServerNetService(ILogger logger, Server addon, IServerApi serverApi)
        {
            _logger = logger;
            var receiver = serverApi.NetServer.GetNetworkReceiver<ModListPacketId>(addon, (_) => new ModListPacket());
            receiver.RegisterPacketHandler<ModListPacket>(
                ModListPacketId.ModListClientData, (id, data) => HandleModlist(id, data, serverApi));

            _modListWatcher = new FileSystemWatcher(DllDirectory ?? string.Empty);
            _modListWatcher.IncludeSubdirectories = false;
            _modListWatcher.Changed += OnFileChanged;
            _modListWatcher.EnableRaisingEvents = true;
            HandleConfigChange();
            HandleModlistChange();
        }

        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // just do a best attempt at getting the file before passing it off to readers
            const int maxAttempts = 50;
            var attempts = 0;
            while (IsFileLocked(new FileInfo(e.FullPath)) && attempts <= maxAttempts)
            {
                // We don't want to mess around with half copied files, wait for the lock to end
                await Task.Delay(10);
                attempts++;
            }

            if (attempts > maxAttempts)
            {
                // We'll call this unreadable
                _logger.Warn(this, $"Was unable to read locked file: {e.Name}");
                return;
            }

            switch (e.Name)
            {
                case ModlistFileName:
                    HandleModlistChange();
                    break;
                case ConfigFileName:
                    HandleConfigChange();
                    break;
            }
        }

        private void HandleModlistChange()
        {
            if (!File.Exists(Path.Combine(DllDirectory, ModlistFileName)))
            {
                _logger.Warn(this, "No Modlist Provided.");
                return;
            }

            var fileContents = ReadAllTextNoLock(Path.Combine(DllDirectory, ModlistFileName));
            if (fileContents == null)
            {
                return;
            }

            _knownMods = JsonConvert.DeserializeObject<List<ModVersion>>(fileContents);
            _logger.Info(this, "ModList Updated");
        }

        private void HandleConfigChange()
        {
            if (!File.Exists(Path.Combine(DllDirectory, ConfigFileName)))
            {
                _logger.Warn(this, "No Configuration Provided.");
                return;
            }

            var fileContents = ReadAllTextNoLock(Path.Combine(DllDirectory, ConfigFileName));
            if (fileContents == null)
            {
                return;
            }

            _configuration = JsonConvert.DeserializeObject<Configuration>(fileContents);
            _logger.Info(this, "Configuration Updated");
        }

        private void HandleModlist(ushort id, ModListPacket data, IServerApi serverApi)
        {
            var clientMods = data.PlayerInfo.InstalledMods;

            var modInfo = CalculateModDiff(_knownMods, clientMods);

            var isMatch = CheckIsMatch(modInfo, _configuration.MismatchOnExtraMods);

            _logger.Info(this,
                !isMatch
                    ? $"{data.PlayerInfo.PlayerName}'s mods DO NOT match!"
                    : $"{data.PlayerInfo.PlayerName} joined with matching mods!");

            MessageModDiscrepancies(serverApi, id, modInfo, isMatch);

            if (!isMatch && _configuration.KickOnMistmatch)
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

        private static bool CheckIsMatch(Models.ModInfo info, bool countExtraMods)
        {
            return !info.MissingMods.Any() && !info.WrongVersions.Any() && (!info.ExtraMods.Any() || !countExtraMods);
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

        private string ReadAllTextNoLock(string path)
        {
            try
            {
                using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(file))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
                _logger.Error(this, $"Unable to read text from file {path}. File is locked.");
                return null;
            }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (var _ = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}
