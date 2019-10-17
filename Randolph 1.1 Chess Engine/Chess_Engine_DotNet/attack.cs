using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public struct Attack
    {
        public static readonly int[] KingDirections = { -1, -10, 1, 10, -9, -11, 11, 9 };
        public static readonly int[] RookDirections = { -1, -10, 1, 10 };
        public static readonly int[] BishopDirections = { -9, -11, 11, 9 };
        public static readonly int[] KnightDirections = { -8, -19, -21, -12, 8, 19, 21, 12 };

        //convert this to a bool in the future -- square is the square in question, side is the side the square may be attacked by
        public static int IsThisSquareAttacked(int square, int side, ref Chessboard position)
        {
            //maybe define i here because setting a variable costs less than declaring new ones over and over
            int piece, tempsquare, direction;

            //the pawns
            if (side == (int)Color.WHITE)
            {
                //take the square in question and see if there is a white pawn  on one square in either diagonal direction in front of the
                //square, and if there is, that means that this square is definilty being attacked by white, and we should return true
                if ((position.Pieces[square - 11] == (int)PieceID.PAWN_W) || (position.Pieces[square - 9] == (int)PieceID.PAWN_W))
                    return (int)Conditional.TRUE;
            }
            else
            {
                if ((position.Pieces[square + 11] == (int)PieceID.PAWN_B) || (position.Pieces[square + 9] == (int)PieceID.PAWN_B))
                    return (int)Conditional.TRUE;
            }

            //the knights
            for (int i = 0; i < 8; ++i)
            {
                if (position.Pieces[square + KnightDirections[i]] == (int)Coordinate.OFFBOARD)
                    continue;

                piece = position.Pieces[square + KnightDirections[i]];
                if ((Global.IsPieceKnight[piece] == (int)Conditional.TRUE) && (Global.PieceColor[piece] == side))
                    return (int)Conditional.TRUE;
            }

            //the rooks/queens -- vertical and horizontal directions
            for (int i = 0; i < 4; ++i)
            {
                direction = RookDirections[i];
                tempsquare = square + direction;
                piece = position.Pieces[tempsquare];

                while (piece != (int)Coordinate.OFFBOARD)
                {
                    if (piece != (int)PieceID.EMPTY)
                    {
                        if ((Global.IsPieceRookOrQueen[piece] == (int)Conditional.TRUE) && (Global.PieceColor[piece] == side))
                        {
                            return (int)Conditional.TRUE;
                        }
                        break;
                    }
                    tempsquare += direction;
                    piece = position.Pieces[tempsquare];
                }
            }

            //the bishops/queens -- diagonal directions
            for (int i = 0; i < 4; ++i)
            {
                direction = BishopDirections[i];
                tempsquare = square + direction;
                piece = position.Pieces[tempsquare];

                while (piece != (int)Coordinate.OFFBOARD)
                {
                    if (piece != (int)PieceID.EMPTY)
                    {
                        if ((Global.IsPieceBishopOrQueen[piece] == (int)Conditional.TRUE) && (Global.PieceColor[piece] == side))
                        {
                            return (int)Conditional.TRUE;
                        }
                        break;
                    }
                    tempsquare += direction;
                    piece = position.Pieces[tempsquare];
                }
            }

            //the kings
            for (int i = 0; i < 8; ++i)
            {
                piece = position.Pieces[square + KingDirections[i]];
                if (position.Pieces[square + KingDirections[i]] == (int)Coordinate.OFFBOARD)
                    continue;

                if ((Global.IsPieceKing[piece] == (int)Conditional.TRUE) && (Global.PieceColor[piece] == side))
                    return (int)Conditional.TRUE;
            }

            return (int)Conditional.FALSE;
        }
    };
}

