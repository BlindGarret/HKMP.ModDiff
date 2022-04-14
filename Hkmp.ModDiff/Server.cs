using Hkmp.Api.Server;
using Hkmp.ModDiff.Services;

namespace Hkmp.ModDiff
{
    /// <summary>
    /// Server addon for checking mod load difference
    /// </summary>
    public class Server : ServerAddon
    {
        /// <inheritdoc />
        public override void Initialize(IServerApi serverApi)
        {
            Logger.Info(this, "Server initialized");
            // ReSharper disable once ObjectCreationAsStatement
            new ServerNetService(Logger, this, serverApi);
        }

        /// <inheritdoc />
        protected override string Name => ModInfo.Name;

        /// <inheritdoc />
        protected override string Version => ModInfo.Version;

        /// <inheritdoc />
        public override bool NeedsNetwork => true;
    }
}
