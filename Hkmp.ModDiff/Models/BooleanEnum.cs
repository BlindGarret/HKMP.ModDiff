using System.ComponentModel;
using JetBrains.Annotations;

namespace Hkmp.ModDiff.Models
{
    /// <summary>
    /// Simplem enum representing a boolean for easy menu creation
    /// </summary>
    public enum BooleanEnum
    {
        /// <summary>
        /// Truthy state
        /// </summary>
        [Description("True")]
        [UsedImplicitly]
        True,

        /// <summary>
        /// Falsy state
        /// </summary>
        [Description("False")]
        [UsedImplicitly]
        False
    }
}
