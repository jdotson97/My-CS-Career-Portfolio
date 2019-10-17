using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public class Chessboard
    {
        //this will be used when attaching this engine to unity
        //public Square[] Chessboard = new Square[Global.BOARD_SQUARE_NUMBER];

        public int[] Pieces = new int[Global.BOARD_SQUARE_NUMBER];

        //pawns and kings; black white and both; these will be bitboards
        //sometime in the far future find a way to represent all pieces using bitboards
        /*
        public long[] Queens = new long [3];
        public long[] Rooks = new long[3];
        public long[] Knights = new long[3];
        public long[] Bishops = new long[3];
        */
        //the kings[] may want to be a long and not an int NOTE it is int because its not a bitboard; it just holds the 2 squares that has kings on them
        public long[] Pawns = new long[3];
        public int[] Kings = new int[2];

        //whos side it is to play (could potentially be bool)
        public int Side;

        //what en passent squares are active if any
        public int EnPassant;
        public int FiftyMove;

        //how many half moves are we into the current search, and how manyt total plys have been made in the current game so far
        public int Ply;
        public int TotalPly;

        //read the comment next to the enum for castling; this may have been able to be stored in bools, but it is easier to store
        //all of the 4 bools values inside one int
        public int CastlePermission;

        //this will keep track of how the board looks at a very basic level; great for keeping track of repeated positions in search, fifty mvoe rule threefold rep
        public long PositionalKey;

        //the number of pieces that we have on the baord; indexed by particular piece type;
        //for example if we want the number of knights on the board, we just simply would put in the PieceID value of 2
        public int[] NumberOfPieces = new int[13];

        //a big piece is anything that is not a pawn; this will store the value of the number of big pieces for white balck and both
        public int[] BigPieces = new int[2];   // bishops and knights
        public int[] MajorPieces = new int[2]; // rooks and queens
        public int[] MinorPieces = new int[2]; // bishops and knights

        //holds the value for the material score of black and white
        public int[] Material = new int[2];

        //before every move in the game, we will store in this array all of the information about the position before the move has
        //been made to allow the user the ability to take the move back
        public UndoMove[] BoardHistory = new UndoMove[Global.MAX_GAME_MOVE_LENGTH];

        public PVTable PrincipleVariationTable;
        public int[] PrincipleVariationArray = new int[Global.MAX_DEPTH];

        public int[,] SearchHistory = new int[13, Global.BOARD_SQUARE_NUMBER];
        public int[,] SearchKillers = new int[2, Global.MAX_DEPTH];

        //13 is the number of different types of pieces, and 10 is the largest possible number of that type of piece that
        //could be on the Board at any one point in time; ex. there could only ever be 10 rooks because 2 to start and 8 Pawns
        public int[,] PieceList = new int[13, 10];

        //to add 1 rook to the board for example you would do PieceList[4][0] = Coordinate.D4 PieceList[4][1] will still equal NO_SQ

        public Chessboard()
        {
            //initialize BoardHistory; creating an array in C# doesnt initialize things in the array so you have to do that explicitly
            for (int i = 0; i < Global.MAX_GAME_MOVE_LENGTH; ++i)
            {
                BoardHistory[i] = new UndoMove();               
            }
            PrincipleVariationTable = new PVTable();
        }

        //WORKING
        public static void ResetBoard(ref Chessboard position)
        {
            //working perfectly TESTED  
            for (int i = 0; i < Global.BOARD_SQUARE_NUMBER; ++i)
            {
                position.Pieces[i] = (int)Coordinate.OFFBOARD;
            }

            //tested and is working changed form using 120to64 to 64to120
            for (int i = 0; i < Global.CHESSBOARD_SQUARE_NUMBER; ++i)
            {
                //if i happen in the future to be pulling my hair out over a bug, make sure that the below function Square120To64[i] is the correct one and not the square64... one
                position.Pieces[Global.Square64To120[i]] = (int)PieceID.EMPTY;
            }

            //was throwing an out of range expection when i realized that i had the pawns[3] in the same loop as the [2]; fixed by adding if statement
            for (int i = 0; i < 3; ++i)
            {
                if (i < 2)
                {
                    position.BigPieces[i] = 0;
                    position.MajorPieces[i] = 0;
                    position.MinorPieces[i] = 0;
                    position.Material[i] = 0;
                }
                
                position.Pawns[i] = 0;
            }

            for (int i = 0; i < 13; ++i)
            {
                position.NumberOfPieces[i] = 0;
            }

            //setting the positions of both kings to no square
            position.Kings[(int)Color.WHITE] = position.Kings[(int)Color.BLACK] = (int)Coordinate.NO_SQ;

            //setting the side to both colors, eliminating any residual en passet squares, and resetign the fifty move rule
            position.Side = (int)Color.BOTH;
            position.EnPassant = (int)Coordinate.NO_SQ;
            position.FiftyMove = 0;

            //reseting both of the plys to zero to reset the move counter totally
            position.Ply = 0;
            position.TotalPly = 0;

            //all castling permissions are reset
            position.CastlePermission = 0;

            //the position is returned to its default null state
            position.PositionalKey = 0;
        }
        //look to make this more efficient in the future
        public static int CheckBoard(ref Chessboard position)
        {
            int[] tempPieceNumber = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] tempBigPieces = { 0, 0 };
            int[] tempMajorPieces = { 0, 0 };
            int[] tempMinorPieces = { 0, 0 };
            int[] tempMaterial = { 0, 0 };

            int square64, square120, tempPiece, tempNumberOfPieces, color, piececount;

            long[] tempPawns = { 0, 0, 0 };

            tempPawns[(int)Color.WHITE] = position.Pawns[(int)Color.WHITE];
            tempPawns[(int)Color.BLACK] = position.Pawns[(int)Color.BLACK];
            tempPawns[(int)Color.BOTH] = position.Pawns[(int)Color.BOTH];

            for (tempPiece = (int)PieceID.PAWN_W; tempPiece <= (int)PieceID.KING_B; ++tempPiece)
            {
                for (tempNumberOfPieces = 0; tempNumberOfPieces <= position.NumberOfPieces[tempPiece]; ++tempNumberOfPieces)
                {
                    square120 = position.PieceList[tempPiece, tempNumberOfPieces];
                    //Debug.Assert(position.Pieces[square120] == tempPiece);
                }
            }

            for (square64 = 0; square64 < 64; ++square64)
            {
                square120 = Global.Square64To120[square64];

                tempPiece = position.Pieces[square120];
                ++tempPieceNumber[tempPiece];
                color = Global.PieceColor[tempPiece];

                if (Global.IsPieceBig[tempPiece] == (int)Conditional.TRUE)
                    ++tempBigPieces[color];
                if (Global.IsPieceMajor[tempPiece] == (int)Conditional.TRUE)
                    ++tempMajorPieces[color];
                if (Global.IsPieceMinor[tempPiece] == (int)Conditional.TRUE)
                    ++tempMinorPieces[color];

                tempMaterial[color] += Global.PieceValue[tempPiece];
            }

            //check to make sure the values inside of position.NumberOfPieces are correct by comparing them to a freshly generated piece number array
            for (tempPiece = (int)PieceID.PAWN_W; tempPiece <= (int)PieceID.KING_B; ++tempPiece)
            {
                //Debug.Assert(tempPieceNumber[tempPiece] == position.NumberOfPieces[tempPiece]);
            }

            //make sure the number of pawns that are contained within the parn bitboards match their repective values in position.NumberOfPieces[]
            piececount = Bitboard.CountBits(ref tempPawns[(int)Color.WHITE]);
            //Debug.Assert(piececount == position.NumberOfPieces[(int)PieceID.PAWN_W]);
            piececount = Bitboard.CountBits(ref tempPawns[(int)Color.BLACK]);
            //Debug.Assert(piececount == position.NumberOfPieces[(int)PieceID.PAWN_B]);
            piececount = Bitboard.CountBits(ref tempPawns[(int)Color.BOTH]);
            //Debug.Assert(piececount == position.NumberOfPieces[(int)PieceID.PAWN_W] + position.NumberOfPieces[(int)PieceID.PAWN_B]);

            //make sure the coordinates that the pawns occupy in the pawn bitboards are accurate to respective pawn positions in position.NumberOfPieces[] 
            while (tempPawns[(int)Color.WHITE] != 0)
            {
                square64 = Bitboard.PopBit(ref tempPawns[(int)Color.WHITE]);
                //Debug.Assert(position.Pieces[Global.Square64To120[square64]] == (int)PieceID.PAWN_W);
            }
            while (tempPawns[(int)Color.BLACK] != 0)
            {
                square64 = Bitboard.PopBit(ref tempPawns[(int)Color.BLACK]);
                //Debug.Assert(position.Pieces[Global.Square64To120[square64]] == (int)PieceID.PAWN_B);
            }
            while (tempPawns[(int)Color.BOTH] != 0)
            {
                square64 = Bitboard.PopBit(ref tempPawns[(int)Color.BOTH]);
                //Debug.Assert((position.Pieces[Global.Square64To120[square64]] == (int)PieceID.PAWN_W) || (position.Pieces[Global.Square64To120[square64]] == (int)PieceID.PAWN_B));
            }

            //assert that based on the position, position has the correct amount of material accounted for.
            //Debug.Assert(tempMaterial[(int)Color.WHITE] == position.Material[(int)Color.WHITE] && tempMaterial[(int)Color.BLACK] == position.Material[(int)Color.BLACK]);
            //Debug.Assert(tempMinorPieces[(int)Color.WHITE] == position.MinorPieces[(int)Color.WHITE] && tempMinorPieces[(int)Color.BLACK] == position.MinorPieces[(int)Color.BLACK]);
            //Debug.Assert(tempMajorPieces[(int)Color.WHITE] == position.MajorPieces[(int)Color.WHITE] && tempMajorPieces[(int)Color.BLACK] == position.MajorPieces[(int)Color.BLACK]);
            //Debug.Assert(tempBigPieces[(int)Color.WHITE] == position.BigPieces[(int)Color.WHITE] && tempBigPieces[(int)Color.BLACK] == position.BigPieces[(int)Color.BLACK]);

            //assert that it is either white or blacks turn and not both
            //Debug.Assert(position.Side == (int)Color.WHITE || position.Side == (int)Color.BLACK);

            //assert that the positional key is correct for the position inside of position; check generates a fresh hashkey and compares it to the one already inside of position
            //Debug.Assert(HashKey.GeneratePositionalKey(ref position) == position.PositionalKey);

            //verify that the en passet squares are set corectly, as they can only be on the sixth rank if its whites turn, or on the third rank if it is blacks turn
            //Debug.Assert((position.EnPassant == (int)Coordinate.NO_SQ) || (Global.Square120ToRank[position.EnPassant] == (int)Rank.RANK_6 && position.Side == (int)Color.WHITE)
            //     || (Global.Square120ToRank[position.EnPassant] == (int)Rank.RANK_3 && position.Side == (int)Color.BLACK));

            //verify that the position of the kings in position.pieces matches the one present in position.kings
            //Debug.Assert(position.Pieces[position.Kings[(int)Color.WHITE]] == (int)PieceID.KING_W);
            //Debug.Assert(position.Pieces[position.Kings[(int)Color.BLACK]] == (int)PieceID.KING_B);

            return (int)Conditional.TRUE;
        }
        //try to take out inefficiencies in this because this will likley have to be called very frequently (every move)
        public static void UpdatePieceListsAndMaterial(ref Chessboard position)
        {
            int piece, square, color;

            for (int i = 0; i < Global.BOARD_SQUARE_NUMBER; ++i)
            {
                square = i;
                piece = position.Pieces[i];

                if ((piece != (int)Coordinate.OFFBOARD) && (piece != (int)PieceID.EMPTY)) //may need to add a NO_SQ check into this if statement at some point
                {
                    color = Global.PieceColor[piece];

                    //if the piece is a big, major, or minor pioece, increment the number of correspnding pieces' counter inside of Chessboard
                    if (Global.IsPieceBig[piece] == (int)Conditional.TRUE)
                        ++position.BigPieces[color];
                    if (Global.IsPieceMajor[piece] == (int)Conditional.TRUE)
                        ++position.MajorPieces[color];
                    if (Global.IsPieceMinor[piece] == (int)Conditional.TRUE)
                        ++position.MinorPieces[color];

                    position.Material[color] += Global.PieceValue[piece];

                    //PieceList[PAWN_W, 0] = a2
                    //PieceList[PAWN_W, 1] = b2

                    //this takes the piece and the number of those pieces on the board and initialzes piece list with this info
                    position.PieceList[piece, position.NumberOfPieces[piece]] = square;
                    //now the number of that type of piece must be incremented

                    ++(position.NumberOfPieces[piece]);
                    
                    if (piece == (int)PieceID.KING_W)
                        position.Kings[(int)Color.WHITE] = square;
                    if (piece == (int)PieceID.KING_B)
                        position.Kings[(int)Color.BLACK] = square;
                    
                    //use actual set and clear mask function defined in global
                    if (piece == (int)PieceID.PAWN_W)
                    {
                        position.Pawns[(int)Color.WHITE] |= Global.SetMask[Global.Square120To64[square]];
                        position.Pawns[(int)Color.BOTH] |= Global.SetMask[Global.Square120To64[square]];
                    }
                    else if (piece == (int)PieceID.PAWN_B)
                    {
                        position.Pawns[(int)Color.BLACK] |= Global.SetMask[Global.Square120To64[square]];
                        position.Pawns[(int)Color.BOTH] |= Global.SetMask[Global.Square120To64[square]];
                    }
                }
            }
        }
        //takes a string from a gui front end like arena and parses it so the engine can set up a position //WORKING
        public static int ParseFEN(ref string FENString, ref Chessboard position) //<--once this is up and running enough to attach to arena, test the SHIT out of this
        {
            ResetBoard(ref position);

            int rank = (int)Rank.RANK_8;
            int file = (int)File.FILE_A;

            int piece = 0;
            int count = 0;

            int square64 = 0;
            int square120 = 0;

            int i = 0;
            while (rank >= (int)Rank.RANK_1 /*&& (FENString[i].Length)*/) //the second paramter in this while definition may cause this funciton to fail
            {
                count = 1;

                switch (FENString[i])
                {
                    case 'p': piece = (int)PieceID.PAWN_B; break;
                    case 'r': piece = (int)PieceID.ROOK_B; break;
                    case 'n': piece = (int)PieceID.KNIGHT_B; break;
                    case 'b': piece = (int)PieceID.BISHOP_B; break;
                    case 'k': piece = (int)PieceID.KING_B; break;
                    case 'q': piece = (int)PieceID.QUEEN_B; break;

                    case 'P': piece = (int)PieceID.PAWN_W; break;
                    case 'R': piece = (int)PieceID.ROOK_W; break;
                    case 'N': piece = (int)PieceID.KNIGHT_W; break;
                    case 'B': piece = (int)PieceID.BISHOP_W; break;
                    case 'K': piece = (int)PieceID.KING_W; break;
                    case 'Q': piece = (int)PieceID.QUEEN_W; break;

                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                        piece = (int)PieceID.EMPTY;
                        count = FENString[i] - '0';
                        break;

                    case '/':
                    case ' ':
                        rank--;
                        file = (int)File.FILE_A;
                        ++i;
                        continue;

                    default:
                        //there was an error
                        return -1;
                }

                

                //if piece is empty, this will simply leave the values on those squares alone as they are empty from calling reset square
                //if piece is not empty, count will equal one, and this will simply assign the position.Pieces[square120] to that particular piece id
                for (int j = 0; j < count; j++)
                {
                    //use fileandrankto120square for square64
                    //could get rid of square64 and 120 all together if its proven to be more efficient in the future
                    square64 = rank * 8 + file;
                    square120 = Global.Square64To120[square64]; //changed to 64to120
                    if (piece != (int)PieceID.EMPTY)
                    {
                        position.Pieces[square120] = piece;
                    }
                    file++;
                }

                ++i;
            }

            //ASSERT(FENString[i] == 'w' || FENString[i] == 'b');

            position.Side = (FENString[i] == 'w') ? (int)Color.WHITE : (int)Color.BLACK;
            i += 2;

            for (int j = 0; j < 4; j++)
            {
                if (FENString[i] == ' ')
                {
                    break;
                }
                switch (FENString[i])
                {
                    case 'K': position.CastlePermission |= (int)Castling.WKING_CASTLE; break;
                    case 'Q': position.CastlePermission |= (int)Castling.WQUEEN_CASTLE; break;
                    case 'k': position.CastlePermission |= (int)Castling.BKING_CASTLE; break;
                    case 'q': position.CastlePermission |= (int)Castling.BQUEEN_CASTLE; ; break;

                    default: break;
                }

                ++i;
            }

            ++i;

            //ASSERT(position->CastlePermission >= 0 && position->CastlePermission <= 15);

            if (FENString[i] != '-')
            {
                file = FENString[i] - 'a';
                rank = FENString[i + 1] - '1';

                //ASSERT(file >= (int)File.FILE_A && file <= (int)File.FILE_H);
                //ASSERT(rank >= (int)Rank.RANK_1 && rank <= (int)Rank.RANK_8);

                //file and rank are close to 61 WTF?!?
                position.EnPassant = Global.FileAndRankTo120Square(file, rank);
                ++i;
            }

            i += 2;

            if (FENString[i] != 0)
            {
                var debug = FENString[i];

                position.FiftyMove = (FENString[i] - '0');

                position.TotalPly = Convert.ToInt32(FENString.Substring(i + 2));

                //position.TotalPly = (FENString[i + 2] - '0');
            }

            position.PositionalKey = HashKey.GeneratePositionalKey(ref position);
            UpdatePieceListsAndMaterial(ref position);
            return 0;
        }
    };

    //stores a move integer and a score
    public class Move //think about making this into a class
    {
        public int move;
        public int score;

        public Move()
        {
            move = 0;
            score = 0;
        }
    }
    //this may need to be a struct if i can figure out how
    public class MoveList
    {
        public Move[] moves;
        public int count;

        public MoveList()
        {
            moves = new Move[Global.MAX_POSITIONAL_MOVES];
            for (int i = 0; i < Global.MAX_POSITIONAL_MOVES; ++i)
            {
                moves[i] = new Move();
            }

            count = 0;
        }
    }
    
    public class UndoMove
    {
        public int MoveNumber;
        public int CastlePermission;

        public int EnPassant;
        public int FiftyMove;

        //i must check whether or not a position has repeated itself 3 times to check for three-fold repetition
        public long PositionalKey;
    };
    //make entry into a class at some point
    public struct PVEntry
    {
        public long Positionalkey;
        public int move;
    }
    //the below doesnt just make a table of 50000 because the size of the table could change
    public struct PVTable //THE POSITIONAL KEY IS THE INDEX OF THE HASH WHICH DIRECTS YOU TO THE BEST MOVE FOR THAT POSITIONAL KEY
    {   //THIS GREATLY SPEEDS UP THE SEARCH
        public PVEntry[] PrincipleTable;
        public int NumberOfEntries;

        /*public PVTable()
        {
            PrincipleTable = null;
            NumberOfEntries = 0;
        }*/
    }
    //the below structure is in place to allow the engine to know how long it has to think for when searching a position
    public class SearchInfo
    {
        public int StartTime;
        public int StopTime;

        public int Depth;
        public int DepthSetting;

        public bool TimeSetting;
        public int MovesToGo;
        public int infinite;

        public long nodes;

        public bool quit;
        public bool stopped;

        public float FailHigh;
        public float FailHighFirst;

        public SearchInfo()
        {
            TimeSetting = false;
            stopped = false;

            quit = false;
        }
    }
};
