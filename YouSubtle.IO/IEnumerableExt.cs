using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouSubtle
{
    internal static class IEnumerable
    {
        public static IEnumerable<T> FilterIf<T>(this IEnumerable<T> sourceEnumerable,
                                                 Func<bool> filterationCondition,
                                                 Func<T, bool> filter)
        {
            if (filterationCondition == null) return sourceEnumerable;

            return sourceEnumerable.Where(filter);
        }
    }
}
