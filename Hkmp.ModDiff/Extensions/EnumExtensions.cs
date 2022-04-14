using System;
using System.Linq;

namespace Hkmp.ModDiff.Extensions
{
    internal static class EnumExtensions
    {
        public static string GetDescription(this Enum genericEnum)
        {
            var genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString());
            if ((memberInfo.Length > 0))
            {
                var attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((attribs.Any()))
                {
                    return ((System.ComponentModel.DescriptionAttribute)attribs.ElementAt(0)).Description;
                }
            }
            return genericEnum.ToString();
        }
    }
}
