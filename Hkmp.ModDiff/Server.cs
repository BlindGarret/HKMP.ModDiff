using System.IO;
using System.Reflection;
using Hkmp.Api.Server;
using Hkmp.ModDiff.Services;
using Newtonsoft.Json;

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

            var dllDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configPath = Path.Combine(dllDir ?? string.Empty, "moddiff_config.json");
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(new Configuration()));
                Logger.Info(this, "Created configuration file");
            }

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
