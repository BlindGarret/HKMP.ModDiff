using System.Collections.Generic;

namespace Hkmp.ModDiff.Models
{
    internal class ModInfo
    {
        public List<ModVersion> ExtraMods { get; set; }

        public List<ModVersion> MissingMods { get; set; }

        public List<ModVersionMismatch> WrongVersions { get; set; }
    }
}
