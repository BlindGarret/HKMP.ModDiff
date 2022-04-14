using System.Collections.Generic;
using Hkmp.ModDiff.Models;
using Hkmp.Networking.Packet;
using Newtonsoft.Json;

namespace Hkmp.ModDiff.Packets
{
    /// <summary>
    /// Packet Ids for modlist data
    /// </summary>
    public enum ModListPacketId
    {
        /// <summary>
        /// Client data packet for sending the modlist to the server for comparison
        /// </summary>
        ModListClientData,
    }

    internal class ModListPacket : IPacketData
    {
        public bool IsReliable => true;

        public bool DropReliableDataIfNewerExists => true;

        public PlayerInformation PlayerInfo { get; set; }

        public void WriteData(IPacket packet)
        {
            packet.Write(JsonConvert.SerializeObject(PlayerInfo));
        }

        public void ReadData(IPacket packet)
        {
            PlayerInfo = JsonConvert.DeserializeObject<PlayerInformation>(packet.ReadString());
        }
    }

    internal class PlayerInformation
    {
        public string PlayerName { get; set; }
        public List<ModVersion> InstalledMods { get; set; }
    }
}
