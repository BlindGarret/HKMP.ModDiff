using Hkmp.ModDiff.Models;

namespace Hkmp.ModDiff
{
    /// <summary>
    /// App configuration
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Whether the system should consider extra mods a mismatch
        /// </summary>
        public bool MismatchOnExtraMods { get; set; }

        /// <summary>
        /// Whether the client should be kicked after a mismatch
        /// </summary>
        public bool KickOnMistmatch { get; set; }
    }
}
