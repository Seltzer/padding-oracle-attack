using System;
using System.Text;
using PaddingOracleAttack.CryptosUnderAttack;
using PaddingOracleAttack.PaddingOracles;

namespace PaddingOracleAttack
{

    class Program
    {

        static void Main()
        {
            // Crypto under attack
            var crypto = InstantiateCryptoToAttack();

            // Ask the user for plaintext, encrypt it, print both, obtain cipher
            var cipherBytes = DemandAndEncryptPlaintext(crypto);

            // Choose a padding oracle
            var oracle = InstantiateOracle(crypto);

            // Fun time
            new Decryptor(oracle).DecryptCiphertext(cipherBytes);

            // Print results
            OutputResultsAndWait(oracle);
        }


        static PaddingOracle InstantiateOracle(CryptoWrapper crypto)
        {
            return new DirectExecutionErrorOracle(crypto);
            //return new DirectExecutionTimingOracle(crypto);
        }


        static CryptoWrapper InstantiateCryptoToAttack()
        {
            return new CryptoWrapper(RijndaelCrypto.Encrypt, RijndaelCrypto.Decrypt);
            //return new CryptoWrapper(AesCrypto.Encrypt, AesCrypto.Decrypt);
        }


        /// <returns>Cipher bytes</returns>
        static byte[] DemandAndEncryptPlaintext(CryptoWrapper crypto)
        {
            Console.WriteLine("Enter some plaintext to encrypt or hit <ENTER> for the default. Longer is better since we can't decrypt "
                + $"the first {Constants.BlockSizeInBytes} bytes");

            var plaintext = Console.ReadLine();

            // Apply default if null/empty
            if (string.IsNullOrEmpty(plaintext))
            {
                //plaintext = "In cryptography, a padding oracle attack is an attack which uses the padding validation of a cryptographic "
                    //+ "message to decrypt the ciphertext.";

                plaintext = "Cats and dogs and birds and hogs and prawns and frogs.";

                Console.WriteLine($"plaintext string = {plaintext}");
            }

            // Check that the crypto actually functions correctly WRT the specified plaintext before we attempt to attack it
            crypto.AssertCanRoundTrip(plaintext);

            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            var cipherBytes = crypto.Encrypt(plaintextBytes);
            var cipherString = Convert.ToBase64String(cipherBytes).Replace('+', '-').Replace('/', '_');

            Console.WriteLine();
            Console.WriteLine($"cipher string = {cipherString}");
            Console.WriteLine();

            return cipherBytes;
        }


        static void OutputResultsAndWait(PaddingOracle oracle)
        {
            oracle.OutputSummary();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

    }

}
