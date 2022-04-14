using Hkmp.ModDiff.Models;

namespace Hkmp.ModDiff
{
    /// <summary>
    /// App configuration
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The type of match to check the mod list with
        /// </summary>
        public MatchType MatchType { get; set; }

        /// <summary>
        /// Whether the client should be kicked after a mismatch
        /// </summary>
        public BooleanEnum KickOnMistmatch { get; set; }
    }
}
