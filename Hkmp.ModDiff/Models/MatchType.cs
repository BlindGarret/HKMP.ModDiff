using System.ComponentModel;
using JetBrains.Annotations;

namespace Hkmp.ModDiff.Models
{
    /// <summary>
    /// Enum representing what kind of match the diff checker is using
    /// </summary>
    public enum MatchType
    {
        /// <summary>
        /// A loose match only checking for missing mods and mis-versioned mods
        /// </summary>
        [Description("Missing/Version Check")]
        [UsedImplicitly]
        Loose,

        /// <summary>
        /// A strict check including all loose checks while also disallowing extra mods on the client.
        /// </summary>
        [Description("Include Extra Mods")]
        [UsedImplicitly]
        Strict
    }
}
