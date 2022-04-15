using System.IO;
using System.Linq;
using System.Reflection;
using Hkmp.Api.Client;
using Hkmp.ModDiff.Models;
using Hkmp.ModDiff.Services;
using Modding;
using Newtonsoft.Json;

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

            var mods = ModHooks.GetAllMods(true).Select(m => new ModVersion
            {
                Name = m.GetName(),
                Version = m.GetVersion()
            }).ToList();
            var dllDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.WriteAllText(Path.Combine(dllDir ?? string.Empty, "modlist.json"), JsonConvert.SerializeObject(mods, Formatting.Indented));
            Logger.Info(this, "modlist.json created");

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
