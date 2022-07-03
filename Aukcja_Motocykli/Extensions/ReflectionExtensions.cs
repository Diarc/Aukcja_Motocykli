using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aukcja_Motocykli.Extensions
{
    public static class ReflectionExtensions
    {
        public static string GetPropertyValue<T>(this T Item, string propertyName)
        {
            return Item.GetType().GetProperty(propertyName).GetValue(Item, null).ToString();
        }
    }
}
