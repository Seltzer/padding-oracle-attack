using System;
using System.Linq;
using System.Text;

namespace PaddingOracleAttack.CryptosUnderAttack
{

    class CryptoWrapper
    {

        public Func<byte[], byte[]> Encrypt { get; }

        public Func<byte[], byte[]> Decrypt { get; }


        public CryptoWrapper(Func<byte[], byte[]> encrypt, Func<byte[], byte[]> decrypt)
        {
            Encrypt = encrypt;
            Decrypt = decrypt;
        }


        /// <summary>
        /// Sanity check
        /// </summary>
        public void AssertCanRoundTrip(string plaintextString)
        {
            var plaintext = Encoding.UTF8.GetBytes(plaintextString);

            var decrypted = Decrypt(Encrypt(plaintext));
            if (!plaintext.SequenceEqual(decrypted))
                throw new InvalidOperationException("Couldn't round-trip the specified plaintext");
        }

    }


}
