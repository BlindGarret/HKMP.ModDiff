using Hkmp.Api.Client;
using Hkmp.Api.Server;
using Modding;

namespace Hkmp.ModDiff
{
    internal class HkMod : Mod
    {
        public HkMod() : base(ModInfo.Name)
        {
        }

        public override void Initialize()
        {
            ClientAddon.RegisterAddon(new Client());
            ServerAddon.RegisterAddon(new Server());
            base.Initialize();
        }

        public override string GetVersion()
        {
            return ModInfo.Version;
        }
    }
}
