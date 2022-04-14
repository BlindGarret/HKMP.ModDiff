using System;
using System.Linq;
using Hkmp.ModDiff.Extensions;
using Modding;

namespace Hkmp.ModDiff.Utils
{
    internal static class MenuUtils  
    {
        public static IMenuMod.MenuEntry BuildEntry<T>(string name, string description, Action<T> saver, Func<T> loader) where T: Enum
        {
            
            var vals = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            return new IMenuMod.MenuEntry(
                name,
                vals.Select(v => v.GetDescription()).ToArray(),
                description,
                i => saver(vals[i]),
                () => Array.IndexOf(vals, loader())
            );
        }
    }
}
