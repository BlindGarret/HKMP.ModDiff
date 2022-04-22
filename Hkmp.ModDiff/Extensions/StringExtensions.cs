using System.Globalization;

namespace Hkmp.ModDiff.Extensions
{
    internal static class StringExtensions
    {
        private const string UnifiedSeperator = ".";

        /// <summary>
        /// Unifys how we seperate versions to a '.' for comparison as the built in Version class doesn't handle this.
        /// </summary>
        /// <param name="str">The string to replace decimal sepertators on.</param>
        /// <returns>The original string with all decimal seperators replaced by '.'</returns>
        public static string UnifyVersionString(this string str)
        {
            return str.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, UnifiedSeperator);
        }

        /// <summary>
        /// This changes the unified version seperator back to the current cultural seperator for the version string.
        /// </summary>
        /// <param name="str">The version string to change</param>
        /// <returns>The version string seperated again by the cultural seperator</returns>
        public static string ToCulturalDecimalSeperatorForVersions(this string str)
        {
            return str.Replace(UnifiedSeperator, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }
    }
}
