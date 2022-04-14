using Hkmp.Api.Client;
using Hkmp.ModDiff.Services;

namespace Hkmp.ModDiff
{
    /// <summary>
    /// Client addon for checking mod load difference
    /// </summary>
    public class Client : ClientAddon
    {
        /// <inheritdoc />
        public override void Initialize(IClientApi clientApi)
        {
            Logger.Info(this, "Client initialized");
            // ReSharper disable once ObjectCreationAsStatement
            new ClientNetService(Logger, this, clientApi);
        }

        /// <inheritdoc />
        protected override string Name => ModInfo.Name;

        /// <inheritdoc />
        protected override string Version => ModInfo.Version;

        /// <inheritdoc />
        public override bool NeedsNetwork => true;
    }
}
