using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Helpers
{
    /// <summary>
    /// Helper class for classes that implement IEnumberable.
    /// </summary>
    public static class EnumberableHelper
    {
        /// <summary>
        /// Allows users to iterate over a collection in a round-robin fashion.
        /// Be careful, can loop indefinitely.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="collection">Collection to iterate over.</param>
        /// <returns>A round-robin enumerable.</returns>
        public static IEnumerable<T> ToRoundRobinList<T>(this IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any())
            {
                yield break;
            }

            var enumerator = collection.GetEnumerator();

            while (true)
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }

                enumerator.Reset();
            }
        }
    }
}
