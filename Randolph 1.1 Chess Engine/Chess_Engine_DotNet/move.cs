using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public struct MakeMove
    {
        /*
        1111 (castle permission) &= would equal 15 with all except certain squares. If the balck king moves, you would 'and' its from (3) square with
        castle perm (15), and get 0011, which has removed the kings ability to castle on either side. Works with white; Make this filled on runtime
        -this also keeps track of whether or not the rooks have moved
        */
        public static int[] CastlePermissions =
        {
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 13, 15, 15, 15, 12, 15, 15, 14, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 7,  15, 15, 15, 3,  15, 15, 11, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15
        };

        //position.PositionalKey ^= (HashKey.PieceKeys[piece, square]);

        public static void ClearPiece(int square, ref Chessboard position)
        {
            //assert that the piece is on the board

            int piece = position.Pieces[square];

            int color = Global.PieceColor[piece];
            //int index = 0;
            int temppiecenumber = -1;

            //hash the piece out of the position
            position.PositionalKey ^= (HashKey.PieceKeys[piece, square]);

            position.Pieces[square] = (int)PieceID.EMPTY;
            position.Material[color] -= Global.PieceValue[piece];

            if (Global.IsPieceBig[piece] == (int)Conditional.TRUE)
            {
                --position.BigPieces[color];

                if (Global.IsPieceMajor[piece] == (int)Conditional.TRUE)
                {
                    --position.MajorPieces[color];
                }
                else
                {
                    --position.MinorPieces[color];
                }
            }
            else
            {
                //if its not a big piece then it must be a pawn, and therefore the 'pawns' array chessboard must be adjusted to account for this
                position.Pawns[color]           &= (Global.ClearMask[Global.Square120To64[square]]);
                position.Pawns[(int)Color.BOTH] &= (Global.ClearMask[Global.Square120To64[square]]);
            }

            //next the piece must be removed from the piecelist
            for (int i = 0; i < position.NumberOfPieces[piece]; ++i)
            {
                if (position.PieceList[piece, i] == square)
                {
                    temppiecenumber = i;
                    break;
                }
            }
            //this below statement takes the piece at the end of the array and puts into the empty space left so there is no wasted space in PieceList
            position.PieceList[piece, temppiecenumber] = position.PieceList[piece, --position.NumberOfPieces[piece]];
            position.PieceList[piece, position.NumberOfPieces[piece]] = 0;
        }

        public static void AddPiece(int square, ref Chessboard position, int piece)
        {
            int color = Global.PieceColor[piece];

            //shad the piece inot the position
            position.PositionalKey ^= (HashKey.PieceKeys[piece, square]);

            position.Pieces[square] = piece;

            if (Global.IsPieceBig[piece] == (int)Conditional.TRUE)
            {
                ++position.BigPieces[color];

                if (Global.IsPieceMajor[piece] == (int)Conditional.TRUE)
                {
                    ++position.MajorPieces[color];
                }
                else
                {
                    ++position.MinorPieces[color];
                }
            }
            else
            {
                //if its not a big piece then it must be a pawn, and therefore the 'pawns' array chessboard must be adjusted to account for this
                position.Pawns[color] &= (Global.ClearMask[Global.Square120To64[square]]);
                position.Pawns[(int)Color.BOTH] &= (Global.ClearMask[Global.Square120To64[square]]);
            }

            position.Material[color] += Global.PieceValue[piece];
            //the following statement puts the piece into the last spot in the piece list array and then increments the number of pieces for that particular piece
            position.PieceList[piece, position.NumberOfPieces[piece]++] = square;
        }

        public static void MovePiece(int from, int to, ref Chessboard position)
        {
            int index = 0;
            int piece = position.Pieces[from];
            int color = Global.PieceColor[piece];

            //int temppiecenumber = (int)Conditional.FALSE;

            //hash the piece out of the position at the from square,
            position.PositionalKey ^= (HashKey.PieceKeys[piece, from]);
            position.Pieces[from] = (int)PieceID.EMPTY;
            //and hash it back in at the to square
            position.PositionalKey ^= (HashKey.PieceKeys[piece, to]);
            position.Pieces[to] = piece;

            if (Global.IsPieceBig[piece] == (int)Conditional.FALSE)
            {
                //take the pawn out of the piece list by &=ing it with clearmask
                position.Pawns[color] &= (Global.ClearMask[Global.Square120To64[from]]);
                position.Pawns[(int)Color.BOTH] &= (Global.ClearMask[Global.Square120To64[from]]);

                position.Pawns[color] |= (Global.SetMask[Global.Square120To64[to]]);
                position.Pawns[(int)Color.BOTH] |= (Global.SetMask[Global.Square120To64[to]]);
            }

            for (index = 0; index < position.NumberOfPieces[piece]; ++index)
            {
                //look for the piece in the piece list that is set to the from square,
                if (position.PieceList[piece, index] == from)
                {
                    //and change that value to the to squares value to reflect the mvoe in the piece list
                    position.PieceList[piece, index] = to;
                    break;
                }
            }
        }
        
        public static int Move(ref Chessboard position, int move)
        {
            int from = Global.MoveFromSquare(move);
            int to = Global.MoveToSquare(move);
            int side = position.Side;

            //check some stuff

            position.BoardHistory[position.TotalPly].PositionalKey = position.PositionalKey;

            if ((move & Global.MoveFlagEnPassant) > 0)
            {
                //this was an en passet capture, and therefore you must remove the pawn that it captured: to - 10 for white, to + 10 for black on a 120 board
                if (position.Side == (int)Color.WHITE)
                {                  
                    ClearPiece(to - 10, ref position);
                }
                else
                {
                    ClearPiece(to + 10, ref position);
                }
            }
            else if ((move & Global.MoveFlagCastling) > 0)
            {
                switch (to)
                {
                    case (int)Coordinate.C1:
                        MovePiece((int)Coordinate.A1, (int)Coordinate.D1, ref position);
                        break;
                    case (int)Coordinate.C8:
                        MovePiece((int)Coordinate.A8, (int)Coordinate.D8, ref position);
                        break;
                    case (int)Coordinate.G1:
                        MovePiece((int)Coordinate.H1, (int)Coordinate.F1, ref position);
                        break;
                    case (int)Coordinate.G8:
                        MovePiece((int)Coordinate.H8, (int)Coordinate.F8, ref position);
                        break;
                    default: //throw an exeption or something
                        break;
                }
            }

            if (position.EnPassant != (int)Coordinate.NO_SQ)
            {
                //hash out the enpasset sqaure from before because we'll either be adding one this move or not doing anything at all
                position.PositionalKey ^= (HashKey.PieceKeys[(int)PieceID.EMPTY, position.EnPassant]);
            }           
            //hash the castling permission out of the key as well
            position.PositionalKey ^= (HashKey.CastleKey[position.CastlePermission]);

            position.BoardHistory[position.TotalPly].MoveNumber = move;
            position.BoardHistory[position.TotalPly].FiftyMove = position.FiftyMove;
            position.BoardHistory[position.TotalPly].EnPassant = position.EnPassant;
            position.BoardHistory[position.TotalPly].CastlePermission = position.CastlePermission;

            position.CastlePermission &= CastlePermissions[from];
            position.CastlePermission &= CastlePermissions[to];
            position.EnPassant = (int)Coordinate.NO_SQ;

            //hash back in the castling permissions to the positional key
            position.PositionalKey ^= (HashKey.CastleKey[(position.CastlePermission)]);

            int captured = Global.MovePieceCaptured(move);
            ++position.FiftyMove;

            //if this is a capture move, clear the piece captured from a square, and reset the fifty move counter
            if (captured != (int)PieceID.EMPTY)
            {
                ClearPiece(to, ref position);
                position.FiftyMove = 0;
            }

            ++position.TotalPly;
            ++position.Ply;

            //the following is to find out whether or not we have to set another en passet square
            if (Global.IsPiecePawn[position.Pieces[from]] == (int)Conditional.TRUE)
            {
                position.FiftyMove = 0;

                if ((move & Global.MoveFlagPawnStart) > 0)
                {
                    if (side == (int)Color.WHITE)
                    {
                        position.EnPassant = from + 10;
                    }
                    else
                    {
                        position.EnPassant = from - 10;
                    }

                    position.PositionalKey ^= (HashKey.PieceKeys[(int)PieceID.EMPTY, position.EnPassant]);
                }
            }

            //finally move the dang piece
            MovePiece(from, to, ref position);

            int promotedpiece = Global.MovePromotedPiece(move);
            if (promotedpiece != (int)PieceID.EMPTY)
            {
                ClearPiece(to, ref position);
                AddPiece(to, ref position, promotedpiece);
            }

            if (Global.IsPieceKing[position.Pieces[to]] == (int)Conditional.TRUE)
            {
                position.Kings[position.Side] = to;
            }

            //switch the side
            position.Side ^= 1;
            //hash the side into the positional key
            position.PositionalKey ^= (HashKey.SideKey);

            //this statement is here to check and make sure the side did not put themselves in check
            if (Attack.IsThisSquareAttacked(position.Kings[side], position.Side, ref position) == (int)Conditional.TRUE)
            {
                TakeBackMove(ref position);
                return (int)Conditional.FALSE;
            }

            //Chessboard.UpdatePieceListsAndMaterial(ref position);
            return (int)Conditional.TRUE;
        }

        public static void TakeBackMove(ref Chessboard position)
        {
            --position.TotalPly;
            --position.Ply;

            int move = position.BoardHistory[position.TotalPly].MoveNumber;
            int from = Global.MoveFromSquare(move);
            int to = Global.MoveToSquare(move);
            //int side = position.Side;

            //check some things to make sure that those squares are on the board

            if (position.EnPassant != (int)Coordinate.NO_SQ)
            {
                //hash out the enpasset sqaure from before because we'll either be adding one this move or not doing anything at all
                position.PositionalKey ^= (HashKey.PieceKeys[(int)PieceID.EMPTY, position.EnPassant]);
            }
            //hash the castling permission out of the key as well
            position.PositionalKey ^= (HashKey.CastleKey[(position.CastlePermission)]);

            position.CastlePermission = position.BoardHistory[position.TotalPly].CastlePermission;
            position.FiftyMove = position.BoardHistory[position.TotalPly].FiftyMove;
            position.EnPassant = position.BoardHistory[position.TotalPly].EnPassant;

            if (position.EnPassant != (int)Coordinate.NO_SQ)
            {
                //hash back in en passet from the value dreived from the board history array above
                position.PositionalKey ^= (HashKey.PieceKeys[(int)PieceID.EMPTY, position.EnPassant]);
            }
            //now that castle has been set to the move in the history we must also hash back in the castle permissions
            position.PositionalKey ^= (HashKey.CastleKey[(position.CastlePermission)]);

            position.Side ^= 1;
            //hash the side into the positional key
            position.PositionalKey ^= (HashKey.SideKey);

            if ((move & Global.MoveFlagEnPassant) > 0)
            {
                //we need to add the pawn back on the board were it was if this was an en passet capture
                if (position.Side == (int)Color.WHITE)
                {
                    AddPiece(to - 10, ref position, (int)PieceID.PAWN_B);
                }
                else
                {
                    AddPiece(to + 10, ref position, (int)PieceID.PAWN_W);
                }
            }
            else if ((move & Global.MoveFlagCastling) > 0)
            {
                switch (to)
                {
                    //based on what color castled and wether it was king/queen, we must move the rook on that side to the other side of the king
                    case (int)Coordinate.C1:
                        MovePiece((int)Coordinate.D1, (int)Coordinate.A1, ref position);
                        break;
                    case (int)Coordinate.C8:
                        MovePiece((int)Coordinate.D8, (int)Coordinate.A8, ref position);
                        break;
                    case (int)Coordinate.G1:
                        MovePiece((int)Coordinate.F1, (int)Coordinate.H1, ref position);
                        break;
                    case (int)Coordinate.G8:
                        MovePiece((int)Coordinate.F8, (int)Coordinate.H8, ref position);
                        break;
                    default: //throw an exeption or something
                        break;
                }
            }
            //move the piece back to wence it came
            MovePiece(to, from, ref position);

            if (Global.IsPieceKing[position.Pieces[from]] == (int)Conditional.TRUE)
            {
                position.Kings[position.Side] = from;
            }

            int captured = Global.MovePieceCaptured(move);
            if (captured != (int)PieceID.EMPTY)
            {
                AddPiece(to, ref position, captured);
                //System.Diagnostics.Debug.Assert(IsPieceValid(captured));
            }

            if (Global.MovePromotedPiece(move) != (int)PieceID.EMPTY)
            {
                ClearPiece(from, ref position);
                AddPiece(from, ref position, (Global.PieceColor[Global.MovePromotedPiece(move)] == (int)Color.WHITE ? (int)PieceID.PAWN_W : (int)PieceID.PAWN_B));
            }

            //call assert with system.diagnostics.debug on the checkboard(ref position)
        }
    }
}