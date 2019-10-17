using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public struct Bitboard
    {
        //credit to the chessprogramming.wiki and my other sources for this code snipet, as it would be a serious
        // pain in the ass to put this all together from scrath

        public static readonly int[] BitTable = new int[Global.CHESSBOARD_SQUARE_NUMBER]
        {
            63, 30, 3, 32, 25, 41, 22, 33, 15, 50, 42, 13, 11, 53, 19, 34, 61, 29, 2,
            51, 21, 43, 45, 10, 18, 47, 1, 54, 9, 57, 0, 35, 62, 31, 40, 4, 49, 5, 52,
            26, 60, 6, 23, 44, 46, 27, 56, 16, 7, 39, 48, 24, 59, 14, 12, 55, 38, 28,
            58, 20, 37, 17, 36, 8
        };

        public static int PopBit(ref long bitboard)
        {
            long x = bitboard ^ (bitboard - 1);

            long fold = ((x & 0xffffffff) ^ (x >> 32));
            bitboard &= (bitboard - 1);

            return BitTable[(fold * 0x783a9b23) >> 26];
        }
        public static int CountBits(ref long bitboard)
        {
            int i;

            for (i = 0; bitboard == 0; ++i)
            {
                bitboard &= bitboard - 1;
            }

            return i;
        }

        public static void ClearBit(ref long bitboard, int square)
        {
            (bitboard) &= Global.ClearMask[(square)];
        }
        public static void SetBit(ref long bitboard, int square)
        {
            (bitboard) |= Global.SetMask[(square)];
        }
    };
}
