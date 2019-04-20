using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouSubtle
{
    internal static class IEnumerable
    {
        public static IEnumerable<T> FilterIf<T>(this IEnumerable<T> sourceEnumerable,
                                                 bool filterationCondition,
                                                 Func<T, bool> filter)
        {
            if (filterationCondition == false) return sourceEnumerable;

            return sourceEnumerable.Where(filter);
        }
    }
}
