using System;
using System.Linq;

namespace PaddingOracleAttack.PaddingOracles
{

    public abstract class PaddingOracle
    {

        int numDecryptionInvocations;


        public bool IsPaddingValid(byte[] cipherBytes)
        {
            ++numDecryptionInvocations;

            return IsPaddingValidImpl(cipherBytes);
        }


        protected abstract bool IsPaddingValidImpl(byte[] cipherBytes);


        public virtual void OutputSummary()
        {
            Console.WriteLine($"Number of decryption invocations: {numDecryptionInvocations}");
            Console.WriteLine();
        }

    }

}
