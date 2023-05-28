﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaddingOracleAttack.Common;
using PaddingOracleAttack.PaddingOracles;

namespace PaddingOracleAttack
{

    /// <summary>
    /// Written based on https://en.wikipedia.org/wiki/Padding_oracle_attack
    ///
    /// Note that this fundamental algorithm remains the same regardless of which oracle we choose to use.
    /// </summary>
    class Haxor
    {

        readonly PaddingOracle oracle;


        public Haxor(PaddingOracle oracle)
        {
            this.oracle = oracle;
        }


        public void DecryptCipherBytes(byte[] cipher)
        {
            // Divide cipher bytes into blocks
            var blocks = BlockHelper.DivideIntoBlocks(cipher);

            Console.WriteLine("Skipping block 1 as it cannot be decrypted without knowing/guessing the IV");
            Console.WriteLine();

            var plaintexts = blocks
                .GetAllConsecutiveNTuples(2)
                .Select((twoTuple, i) =>
                {
                    // Decrypt
                    Console.WriteLine($"Decrypting block {i + 1} / {blocks.Count}");
                    var decrypted = DecryptBlock(twoTuple.First(), twoTuple.Skip(1).First());

                    // Print decrypted plaintext
                    var decryptedPlaintext = Encoding.UTF8.GetString(decrypted, 0, decrypted.Length);
                    Console.WriteLine(decryptedPlaintext);
                    Console.WriteLine();

                    return decryptedPlaintext;
                })
                .ToList();

            var plaintext = string.Join("", plaintexts);
            Console.WriteLine();
            Console.WriteLine($"Full plaintext minus first block: {plaintext}");
        }


        /// <summary>
        /// Variables referred to in formulae below:
        ///     - F = fabricated cipher block
        ///     - Cn = block we're trying to decrypt
        ///     - Cn-1 = precedingCipherBlock
        ///     - Pn = plaintext to which Cn decrypts
        ///     - (F,Cn) = the double-block cipher which we repeatedly submit to the oracle after manipulating F
        ///     - P' = plaintext which the oracle computes when decrypting (F,Cn)
        ///     - P'2 = second block of P' which ideally contains a valid padding value
        ///
        /// In the general case, AES CBC decryption of (Cn-1, Cn) is performed by:
        ///     - Computing Pn = D(Cn) ^ Cn-1
        ///     - Checking that the padding of Pn is valid
        ///
        /// We perform decryption of Cn by:
        ///     - Coming up with an F which satisfies F ^ D(Cn) = P'2 where P'2 has some expected padding value. This allows us to obtain D(Cn)
        ///     - Substituting D(Cn) from above into Pn = D(Cn) ^ Cn-1
        /// </summary>
        ///
        /// <param name="precedingCipherBlock">Cn-1</param>
        /// <param name="cipherBlockToDecrypt">Cn</param>
        /// <returns>Pn</returns>
        byte[] DecryptBlock(byte[] precedingCipherBlock, byte[] cipherBlockToDecrypt)
        {
            var result = new byte[Constants.BlockSizeInBytes];

            // This is a fabricated cipher, F, which we'll continually mutate throughout and submit to the oracle along with cipherBlockToDecrypt
            var fabricatedCipherBlock = new byte[Constants.BlockSizeInBytes];

            // Iterate backwards over the block we want to decrypt (Cn). current represents the index of the byte we want to decrypt
            for (var current = Constants.BlockSizeInBytes - 1; current >= 0; current--)
            {
                // With each iteration, our padding size increases
                var paddingSize = Constants.BlockSizeInBytes - current;

                // Only applies from second iteration onwards
                if (paddingSize > 1)
                {
                    // After the last iteration, we have an F which when fed into the oracle, results in Cn decrypting to P'2 which has a
                    // valid padding of size (paddingSize - 1). For this iteration, we want it to yield a new padding of size paddingSize
                    for (var rhsIndex = current + 1; rhsIndex < Constants.BlockSizeInBytes; rhsIndex++)
                    {
                        // Take the exist by in F and...
                        fabricatedCipherBlock[rhsIndex] = (byte)(fabricatedCipherBlock[rhsIndex]
                            // XOR with the old padding to zero out the yielded byte in P'2. This works since A ^ A = 0
                            ^ (byte)(paddingSize - 1)
                            // XOR with the new padding to yield that padding. This works since 0 ^ A = A
                            ^ (byte)paddingSize
                        );
                    }
                }

                // After this invocation, we'll have a fabricated cipher block which satisfies F ^ D(Cn) = P'2 where P'2 has valid padding
                // matching paddingSize above. Logically, we want to rearrange this to: D(Cn) = P'2 ^ F
                TryAllByteValues(oracle, fabricatedCipherBlock, cipherBlockToDecrypt, current);

                // Now we'll plug D(Cn) from above into Pn = D(Cn) ^ Cn-1, leaving us with:
                //      Pn = P'2 (padding) ^ F (fabricated cipher block) ^ Cn-1 (precedingCipherBlock)
                result[current] = (byte)(precedingCipherBlock[current] ^ (byte)paddingSize ^ fabricatedCipherBlock[current]);
            }

            return result;
        }


        /// <summary>
        /// Sets fabricatedCipherBlock[current (F[k]) to the byte value which satisfies F ^ D(Cn) = P'2 where P'2 not only has valid padding
        /// (according to the oracle) but also the exact padding we expect.
        ///
        /// WARNING! This is buggy since it doesn't handle the case where we try to set the padding to 1, but unknowingly set it to 2,3,4... etc.
        /// </summary>
        ///
        /// <param name="fabricatedCipherBlock">Gets mutated</param>
        static void TryAllByteValues(PaddingOracle oracle, byte[] fabricatedCipherBlock, byte[] cipherBlockToDecrypt, int current)
        {
            // Create a double-block array up front and repeatedly mutate it as we try different values. Here we construct (F,Cn) where the
            // first block is the fabricated block we want to mutate and the second is the fixed block we want to decrypt.
            var cipherToAttempt = BlockHelper.CombineBlocks(fabricatedCipherBlock, cipherBlockToDecrypt);

            foreach (byte v in Enumerable.Range(byte.MinValue, byte.MaxValue + 1))
            {
                cipherToAttempt[current] = v;
                if (oracle.IsPaddingValid(cipherToAttempt))
                {
                    // Mutate fabricatedCipherBlock
                    fabricatedCipherBlock[current] = v;
                    return;
                }
            }

            throw new Exception("computer says no");
        }

    }

}