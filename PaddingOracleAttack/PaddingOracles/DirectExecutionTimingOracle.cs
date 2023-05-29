using System;
using System.Diagnostics;
using System.Linq;

namespace PaddingOracleAttack.PaddingOracles
{

    /// <summary>
    /// Uses an arbitrary timing threshold which probably only works on my machine!
    /// </summary>
    class DirectExecutionTimingOracle : PaddingOracle
    {

        readonly Func<byte[], byte[]> decrypt;

        const long thresholdInTicks = 20000;


        public DirectExecutionTimingOracle(Func<byte[], byte[]> decrypt)
        {
            this.decrypt = decrypt;
        }


        protected override bool IsPaddingValidImpl(byte[] cipherBytes)
        {
            var sw = Stopwatch.StartNew();

            // The repetition is intentional
            try
            {
                decrypt(cipherBytes);

                return sw.ElapsedTicks < thresholdInTicks;
            }
            catch (Exception)
            {
                return sw.ElapsedTicks < thresholdInTicks;
            }
        }

    }

}
