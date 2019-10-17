using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine
{
    public struct IO
    {
        //define two functions in here that take in a square and output the square and algabraic notation,
        // and take in a move and output the move in algabraic notation

        public static int ParseMove(ref string Notation, ref Chessboard position)
        {
            if (Notation[1] > '8' || Notation[1] < '1') return Global.NO_MOVE;
            if (Notation[3] > '8' || Notation[3] < '1') return Global.NO_MOVE;
            if (Notation[0] > 'h' || Notation[0] < 'a') return Global.NO_MOVE;
            if (Notation[2] > 'h' || Notation[2] < 'a') return Global.NO_MOVE;

            int from = Global.FileAndRankTo120Square(Notation[0] - 'a', Notation[1] - '1');
            int to = Global.FileAndRankTo120Square(Notation[2] - 'a', Notation[3] - '1');

            //System.Diagnostics.Debug.Assert(/*make sure the square is actually on the board*/);

            MoveList list = new MoveList();
            MoveGen.GenerateAllMoves(ref position, ref list);
            int MoveHash;
            int PromotedPiece = (int)PieceID.EMPTY;

            for (int i = 0; i < list.count; ++i)
            {
                MoveHash = (list.moves[i]).move;
                if ((Global.MoveFromSquare(MoveHash) == from) && (Global.MoveToSquare(MoveHash) == to))
                {
                    PromotedPiece = Global.MovePromotedPiece(MoveHash);
                    if (PromotedPiece != (int)PieceID.EMPTY)
                    {
                        if ((Global.IsPieceRookOrQueen[PromotedPiece] == (int)Conditional.TRUE) && (Global.IsPieceBishopOrQueen[PromotedPiece] == (int)Conditional.TRUE) && (Notation[4] == 'q'))
                        {
                            return MoveHash;
                        }
                        else if ((Global.IsPieceKnight[PromotedPiece] == (int)Conditional.TRUE) && (Notation[4] == 'n'))
                        {
                            return MoveHash;
                        }
                        else if ((Global.IsPieceRookOrQueen[PromotedPiece] == (int)Conditional.TRUE) && (Global.IsPieceBishopOrQueen[PromotedPiece] == (int)Conditional.FALSE) && (Notation[4] == 'r'))
                        {
                            return MoveHash;
                        }
                        else if ((Global.IsPieceRookOrQueen[PromotedPiece] == (int)Conditional.FALSE) && (Global.IsPieceBishopOrQueen[PromotedPiece] == (int)Conditional.TRUE) && (Notation[4] == 'b'))
                        {
                            return MoveHash;
                        }                       
                        continue;
                    }
                    return MoveHash;
                }
            }

            return Global.NO_MOVE;
        }

        public static string TranslateMove(int move) 
        {
            string translatedMove = null;

            int FileFrom = Global.Square120ToFile[Global.MoveFromSquare(move)];
            int RankFrom = Global.Square120ToRank[Global.MoveFromSquare(move)];

            int FileTo = Global.Square120ToFile[Global.MoveToSquare(move)];
            int RankTo = Global.Square120ToRank[Global.MoveToSquare(move)];

            int promoted = Global.MovePromotedPiece(move);

            translatedMove += (char)('a' + FileFrom); //+= (char)('1' + RankFrom) += (char)('a' + FileTo) += (char)('1' + RankTo)
            translatedMove += (char)('1' + RankFrom);
            translatedMove += (char)('a' + FileTo);
            translatedMove += (char)('1' + RankTo);

            if (promoted != 0)
            {
		        char promotionSuffix = 'q';
		        if(Global.IsPieceKnight[promoted] == (int)Conditional.TRUE)
                {
                    promotionSuffix = 'n';
		        }
                else if((Global.IsPieceRookOrQueen[promoted] == (int)Conditional.TRUE) && (Global.IsPieceRookOrQueen[promoted] == (int)Conditional.FALSE))
                {
                    promotionSuffix = 'r';
		        }
                else if((Global.IsPieceRookOrQueen[promoted] == (int)Conditional.FALSE) && (Global.IsPieceRookOrQueen[promoted] == (int)Conditional.TRUE))
                {
                    promotionSuffix = 'b';
		        }

                translatedMove += promotionSuffix;           
	        }
 
            return translatedMove;
        }
    }
}