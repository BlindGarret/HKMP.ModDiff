using System.Linq;
using Hkmp.Api.Client;
using Hkmp.ModDiff.Extensions;
using Hkmp.ModDiff.Models;
using Hkmp.ModDiff.Packets;
using Modding;
using ILogger = Hkmp.Logging.ILogger;

namespace Hkmp.ModDiff.Services
{
    internal class ClientNetService
    {
        public ClientNetService(ILogger logger, Client addon, IClientApi clientApi)
        {
            var sender = clientApi.NetClient.GetNetworkSender<ModListPacketId>(addon);
            clientApi.ClientManager.ConnectEvent += () =>
            {
                logger.Info("Player connected, sending modlist to server");

                sender.SendSingleData(ModListPacketId.ModListClientData, new ModListPacket
                {
                    PlayerInfo = new PlayerInformation
                    {
                        InstalledMods = ModHooks.GetAllMods(true).Select(m => new ModVersion
                        {
                            Name = m.GetName(),
                            Version = m.GetVersion().UnifyVersionString()
                        }).ToList(),
                        PlayerName = clientApi.ClientManager.Username
                    }
                });
            };
        }
    }
}
