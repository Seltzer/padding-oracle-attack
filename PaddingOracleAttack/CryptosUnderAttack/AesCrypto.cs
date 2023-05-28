using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PaddingOracleAttack.CryptosUnderAttack
{

    public static class AesCrypto
    {

        static readonly byte[] key = Encoding.UTF8.GetBytes("aad7fa2ac48c46f29ad7d7adda918b73");


        public static byte[] Encrypt(byte[] plaintext)
        {
            using (var aes = Aes.Create())
            using (var memoryStream = new MemoryStream())
            {
                aes.Key = key;

                memoryStream.Write(aes.IV, 0, aes.IV.Length);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plaintext, 0, plaintext.Length);
                    cryptoStream.FlushFinalBlock();

                    return memoryStream.ToArray();
                }
            }
        }


        public static byte[] Decrypt(byte[] cipher)
        {
            var iv = new byte[16];

            using (var aes = Aes.Create())
            using (var memoryStream = new MemoryStream(cipher))
            {
                aes.Key = key;
                memoryStream.Read(iv, 0, 16);

                using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (var binaryReader = new BinaryReader(cryptoStream))
                    return binaryReader.ReadBytes(cipher.Length);
            }
        }

    }
}
