using System;
using System.Linq;

namespace PaddingOracleAttack.PaddingOracles
{

    /// <summary>
    /// Is capable of telling the caller whether their specified ciphertext has valid padding
    /// </summary>
    public abstract class PaddingOracle
    {

        int numDecryptionInvocations;


        public bool IsPaddingValid(byte[] ciphertextBytes)
        {
            ++numDecryptionInvocations;

            return IsPaddingValidImpl(ciphertextBytes);
        }


        protected abstract bool IsPaddingValidImpl(byte[] cipherBytes);


        public virtual void OutputSummary()
        {
            Console.WriteLine($"Number of decryption invocations: {numDecryptionInvocations}");
            Console.WriteLine();
        }

    }

}
