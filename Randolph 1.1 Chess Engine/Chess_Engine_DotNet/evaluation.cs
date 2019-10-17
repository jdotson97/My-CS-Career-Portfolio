using System;
using System.Diagnostics;

namespace Engine
{
    public struct Evaluate
    {
        public static readonly int[] PawnTable =
        {
            0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
            10  ,   10  ,   0   ,   -10 ,   -10 ,   0   ,   10  ,   10  ,
            5   ,   0   ,   0   ,   5   ,   5   ,   0   ,   0   ,   5   ,
            0   ,   0   ,   10  ,   20  ,   20  ,   10  ,   0   ,   0   ,
            5   ,   5   ,   5   ,   10  ,   10  ,   5   ,   5   ,   5   ,
            10  ,   10  ,   10  ,   20  ,   20  ,   10  ,   10  ,   10  ,
            20  ,   20  ,   20  ,   30  ,   30  ,   20  ,   20  ,   20  ,
            0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
        };
        public static readonly int[] KnightTable =
        {
            0   ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   0   ,
            0   ,   0   ,   0   ,   5   ,   5   ,   0   ,   0   ,   0   ,
            0   ,   0   ,   10  ,   10  ,   10  ,   10  ,   0   ,   0   ,
            0   ,   0   ,   10  ,   20  ,   20  ,   10  ,   5   ,   0   ,
            5   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   5   ,
            5   ,   10  ,   10  ,   20  ,   20  ,   10  ,   10  ,   5   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
        };
        public static readonly int[] BishopTable =
        {
            0   ,   0   ,   -10 ,   0   ,   0   ,   -10 ,   0   ,   0   ,
            0   ,   0   ,   0   ,   10  ,   10  ,   0   ,   0   ,   0   ,
            0   ,   0   ,   10  ,   15  ,   15  ,   10  ,   0   ,   0   ,
            0   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   0   ,
            0   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   0   ,
            0   ,   0   ,   10  ,   15  ,   15  ,   10  ,   0   ,   0   ,
            0   ,   0   ,   0   ,   10  ,   10  ,   0   ,   0   ,   0   ,
            0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
        };
        public static readonly int[] RookTable =
        {
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            25  ,   25  ,   25  ,   25  ,   25  ,   25  ,   25  ,   25  ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0
        };
        /// <summary>
        /// Mirrors board coordinates to allow black to use the piece tables correctly
        /// </summary>
        public static readonly int[] Mirror =
        {
            56  ,   57  ,   58  ,   59  ,   60  ,   61  ,   62  ,   63  ,
            48  ,   49  ,   50  ,   51  ,   52  ,   53  ,   54  ,   55  ,
            40  ,   41  ,   42  ,   43  ,   44  ,   45  ,   46  ,   47  ,
            32  ,   33  ,   34  ,   35  ,   36  ,   37  ,   38  ,   39  ,
            24  ,   25  ,   26  ,   27  ,   28  ,   29  ,   30  ,   31  ,
            16  ,   17  ,   18  ,   19  ,   20  ,   21  ,   22  ,   23  ,
            8   ,   9   ,   10  ,   11  ,   12  ,   13  ,   14  ,   15  ,
            0   ,   1   ,   2   ,   3   ,   4   ,   5   ,   6   ,   7
        };

        /// <summary>
        /// Evaluates a position, and returns a score in hundreths of pawns.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int EvaluateThisPosition(ref Chessboard position)
        {
            int piece;
            //int pieceNumber;
            int square;
            int score = (position.Material[(int)Color.WHITE] - position.Material[(int)Color.BLACK]);

            piece = (int)PieceID.PAWN_W;
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                square = position.PieceList[piece, i];
                score += PawnTable[Global.Square120To64[square]];
            }
            piece = (int)PieceID.PAWN_B;
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                square = position.PieceList[piece, i];
                score -= PawnTable[Mirror[Global.Square120To64[square]]];
            }

            piece = (int)PieceID.KNIGHT_W;
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                square = position.PieceList[piece, i];
                score += KnightTable[Global.Square120To64[square]];
            }
            piece = (int)PieceID.KNIGHT_B;
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                square = position.PieceList[piece, i];
                score -= KnightTable[Mirror[Global.Square120To64[square]]];
            }

            piece = (int)PieceID.BISHOP_W;
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                square = position.PieceList[piece, i];
                score += BishopTable[Global.Square120To64[square]];
            }
            piece = (int)PieceID.BISHOP_B;
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                square = position.PieceList[piece, i];
                score -= BishopTable[Mirror[Global.Square120To64[square]]];
            }

            piece = (int)PieceID.ROOK_W;
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                square = position.PieceList[piece, i];
                score += RookTable[Global.Square120To64[square]];
            }
            piece = (int)PieceID.ROOK_B;
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                square = position.PieceList[piece, i];
                score -= RookTable[Mirror[Global.Square120To64[square]]];
            }

            if (position.Side == (int)Color.WHITE)
            {
                return score;
            }
            else
            {
                return -score;
            }
        }
    }
}