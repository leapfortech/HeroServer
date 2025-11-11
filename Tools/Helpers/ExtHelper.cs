using System;
using System.Collections.Generic;

namespace HeroServer
{
    public static class ExtHelper
    {
        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return new List<T>(items).Contains(item);
        }

        public static bool In<T>(this T item, List<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            return items.Contains(item);
        }
    }
}