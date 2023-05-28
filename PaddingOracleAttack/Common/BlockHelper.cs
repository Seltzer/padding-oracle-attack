using System;
using System.Collections.Generic;
using System.Linq;

namespace PaddingOracleAttack.Common
{

    public static class BlockHelper
    {

        public static IList<byte[]> DivideIntoBlocks(IList<byte> bytes)
        {
            if (!bytes.Any())
                return new List<byte[]>();

            var newBlock = bytes.Take(Constants.BlockSizeInBytes).ToArray();

            return new[] { newBlock }
                .Concat(DivideIntoBlocks(bytes.Skip(Constants.BlockSizeInBytes).ToList()))
                .ToList();
        }


        public static byte[] CombineBlocks(byte[] block1, byte[] block2)
        {
            var result = new byte[Constants.BlockSizeInBytes * 2];

            Array.Copy(block1, 0, result, 0, Constants.BlockSizeInBytes);
            Array.Copy(block2, 0, result, Constants.BlockSizeInBytes, Constants.BlockSizeInBytes);

            return result;
        }

    }

}