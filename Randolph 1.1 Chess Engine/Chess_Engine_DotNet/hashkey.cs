using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public struct HashKey
    {
        public static System.Random randomNumberGen = new System.Random();
        //this is for the pieces and the squares that they could possibly be on
        //it generates a unique key for every piece, and for every possible square that they could be on
        public static long[,] PieceKeys = new long[13, 120];
        public static long SideKey;
        //the bits for representing castle permissions are layed out like this -0 0 0 0 which is four bits or 15 as the largest possible value so we need 16 keys
        public static long[] CastleKey = new long[16];

        public static long GetRandom64BitHash()
        {
            /*long Long64BitRandomNumber = (long)((random.NextDouble() * 2.0 - 1.0) * long.MaxValue);

            return Long64BitRandomNumber;*/

            byte[] buffer = new byte[8];

            randomNumberGen.NextBytes(buffer);

            return System.BitConverter.ToInt64(buffer, 0);
        }

        public static long GeneratePositionalKey(ref Chessboard position)
        {
            int Piece = (int)PieceID.EMPTY;
            long PositionalKey = 0;

            for (int square = 0; square < Global.BOARD_SQUARE_NUMBER; ++square)
            {
                Piece = position.Pieces[square];

                if ((Piece != (int)Coordinate.NO_SQ) && (Piece != (int)PieceID.EMPTY) && (Piece != (int)Coordinate.OFFBOARD))
                {
                    PositionalKey ^= PieceKeys[Piece, square];
                }
            }

            if (position.Side == (int)Color.WHITE)
            {
                PositionalKey ^= SideKey;
            }

            if (position.EnPassant != (int)Coordinate.NO_SQ)
            {
                PositionalKey ^= PieceKeys[(int)PieceID.EMPTY, position.EnPassant];
            }

            PositionalKey ^= CastleKey[position.CastlePermission];

            return PositionalKey;
        }
    };
}