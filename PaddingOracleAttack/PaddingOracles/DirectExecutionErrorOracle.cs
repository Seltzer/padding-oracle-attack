using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using PaddingOracleAttack.CryptosUnderAttack;

namespace PaddingOracleAttack.PaddingOracles
{

    /// <summary>
    /// Assumes that the decryption method emits <see cref="CryptographicException"/> so only really works with some cryptos.
    /// </summary>
    class DirectExecutionErrorOracle : PaddingOracle
    {

        readonly CryptoWrapper crypto;

        readonly IList<long> successTimes = new List<long>();
        readonly IList<long> paddingFailureTimes = new List<long>();
        readonly IList<long> miscFailTimes = new List<long>();


        public DirectExecutionErrorOracle(CryptoWrapper crypto)
        {
            this.crypto = crypto;
        }


        protected override bool IsPaddingValidImpl(byte[] cipherBytes)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                crypto.Decrypt(cipherBytes);

                successTimes.Add(sw.ElapsedTicks);
                return true;
            }
            catch (CryptographicException ex)
            {
                if (ex.Message.Contains("padding", StringComparison.InvariantCultureIgnoreCase))
                {
                    paddingFailureTimes.Add(sw.ElapsedTicks);
                    return false;
                }

                miscFailTimes.Add(sw.ElapsedTicks);
                return true;

            }
            catch (Exception)
            {
                miscFailTimes.Add(sw.ElapsedTicks);
                return true;
            }
        }


        public override void OutputSummary()
        {
            base.OutputSummary();

            Report(nameof(successTimes), successTimes);
            Report(nameof(paddingFailureTimes), paddingFailureTimes);
            Report(nameof(miscFailTimes), miscFailTimes);
        }


        static void Report(string name, IList<long> times)
        {
            if (times.Any())
                Console.WriteLine($"avg {name} = {times.Average()} based on {times.Count} results, min = {times.Min()}, max = {times.Max()}");
        }

    }

}
