using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PaddingOracleAttack.CryptosUnderAttack
{

    public static class RijndaelCrypto
    {

        const string key = "llamaseatmuesliforbreakfast1234";

        static readonly byte[] iv = { 0x25, 0x22, 0x39, 0x4e, 0x21, 0x2a, 0xbb, 0xc2, 0x33, 0xcc, 0x54, 0x22, 0x41 };


        public static byte[] Encrypt(byte[] clearData)
        {
            var pdb = new PasswordDeriveBytes(key, iv);

            var ms = new MemoryStream();
            var alg = Rijndael.Create();

            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(clearData, 0, clearData.Length);
            cs.Close();

            return ms.ToArray();
        }


        public static byte[] Decrypt(byte[] cipherData)
        {
            var pdb = new PasswordDeriveBytes(key, iv);

            var ms = new MemoryStream();
            var alg = Rijndael.Create();

            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();

            return ms.ToArray();
        }

    }

}