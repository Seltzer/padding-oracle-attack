using System;
using System.Collections.Generic;
using System.Linq;

namespace PaddingOracleAttack.Common
{

    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExt
    {

        public static IEnumerable<IEnumerable<T>> GetAllConsecutiveNTuples<T>(this IEnumerable<T> input, int n)
        {
            if (n < 1)
                throw new ArgumentOutOfRangeException(nameof(n), "Must be larger than 0");

            var tupleUnderContruction = new List<T>();

            foreach (var x in input)
            {
                tupleUnderContruction.Add(x);

                if (tupleUnderContruction.Count == n)
                {
                    var tupleToReturn = tupleUnderContruction.ToList();
                    tupleUnderContruction.RemoveAt(0);

                    yield return tupleToReturn;
                }
            }
        }

    }
}
