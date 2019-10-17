using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public struct MoveGen
    {
        //CASTLE POSSIBLY NEEDS TO BE RENAMED CAPTURE
        //this function takes all neccessry bit string to build a move int and packs them all inot one 64 bit integer
        public static int GenerateMoveInteger(int from, int to, int castle, int promotion, int moveflag)
        {
            return ((from) | (to << 7) | (castle << 14) | (promotion << 20) | (moveflag));
        }

        //convert to bool
        public static int IsSquareOffBoard(int square)
        {
            return (Global.Square120ToFile[square] == (int)Coordinate.OFFBOARD ? 1 : 0);
        }

        //an array that holds all of pieces that are slider pieces
        public static int[] LoopThroughSliderPieces =
        {
            (int)PieceID.BISHOP_W, (int)PieceID.ROOK_W, (int)PieceID.QUEEN_W, 0, (int)PieceID.BISHOP_B,(int)PieceID.ROOK_B,(int)PieceID.QUEEN_B, 0
        };
        //this array will be indexed by piece color, and will tell the move generator what index to start at in the above array based on whos turn it is
        public static int[] LoopThroughSliderPiecesStartIndex = { 0, 4 };
        
        //this array will loop through all the pieces on the board that can slide, white being the first that is listed
        public static int[] LoopThroughNonSliderPieces =
        {
            (int)PieceID.KNIGHT_W, (int)PieceID.KING_W, 0, (int)PieceID.KNIGHT_B, (int)PieceID.KING_B, 0
        };
        //this array will be indexed by piece color, and will tell the move generator what index to start at in the above array based on whos turn it is
        public static int[] LoopThroughSliderNonPiecesStartIndex = { 0, 3 };

        //the pawns have not been set because you do not have to use this array for the pawns
        public static int[,] PieceDirections =
        {
            { 0, 0, 0, 0, 0, 0, 0, 0 },             //empty

            { 0, 0, 0, 0, 0, 0, 0, 0 },             //white pawn
            { -8, -19, -21, -12, 8, 19, 21, 12},    //white knight
            { -9, -11, 11, 9, 0, 0, 0, 0},          //white bishop
            { -1, -10, 1, 10, 0, 0, 0, 0},          //white rook
            { -1, -10, 1, 10, -9, -11, 11, 9},      //white queen
            { -1, -10, 1, 10, -9, -11, 11, 9},      //white king

            { 0, 0, 0, 0, 0, 0, 0, 0 },             //black pawn
            { -8, -19, -21, -12, 8, 19, 21, 12},    //black knight
            { -9, -11, 11, 9, 0, 0, 0, 0},          //black bishop
            { -1, -10, 1, 10, 0, 0, 0, 0},          //black rook
            { -1, -10, 1, 10, -9, -11, 11, 9},      //black queen
            { -1, -10, 1, 10, -9, -11, 11, 9}       //black king
        };
        //used with the above so that a loop would know that a particular index in the above array has as many elements as reflected in thisn one
        public static int[] NumberOfPieceDirectionsByPiece =
        {
            0, 0, 8, 4, 4, 8, 8, 0, 8, 4, 4, 8, 8
        };
        //this takes in a position and a move and will tell you whether or not a move is valid for that position

        public static readonly int[] VictimScore =
        {
            0, 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600
        };
        public static int[,] MVVLVAScore = new int[13, 13];

        public static void InitializeMVVLVA()
        {
            for (int i = (int)PieceID.PAWN_W; i <= (int)PieceID.KING_B; ++i)
            {
                for (int j = (int)PieceID.PAWN_W; j <= (int)PieceID.KING_B; ++j)
                {
                    MVVLVAScore[j, i] = ((VictimScore[j] + 6) - (VictimScore[i] / 100));
                }
            }
        }

        public static int MoveExists(ref Chessboard position, int move)
        {
            MoveList list = new MoveList();
            GenerateAllMoves(ref position, ref list);

            for (int i = 0; i < list.count; ++i)
            {
                //the legendary marker of the most elusive bug i have ever found; this was == 0 should have been == 1
                if (MakeMove.Move(ref position, list.moves[i].move) == (int)Conditional.FALSE)
                {
                    continue;
                }
                MakeMove.TakeBackMove(ref position);
                if (list.moves[i].move == move)
                {
                    return (int)Conditional.TRUE;
                }
            }

            return (int)Conditional.FALSE;
        }

        //the reason that there is different functions right now is not clear, however, that will all change when we start
        //actually implementing that search algorithm; my goal is to efficiently multithread alpha beta search
        public static void AddQuietMove(ref Chessboard position, int move, ref MoveList list)
        {
            //exceptoin is being thrown because all elements inside of movelist are null values; fixed by explicitly calling move's default constructor
            list.moves[list.count].move =  move;

            if (position.SearchKillers[0, position.Ply] == move)
            {
                list.moves[list.count].score = 900000;
            }
            else if (position.SearchKillers[1, position.Ply] == move)
            {
                list.moves[list.count].score = 800000;
            }
            else
            {
                //this could possibly be causing a bug, so if it does, make this equal to zero instead
                list.moves[list.count].score = position.SearchHistory[position.Pieces[Global.MoveFromSquare(move)], /*position.Pieces[*/Global.MoveToSquare(move)/*]*/];
            }
            
            ++list.count;
        }
        public static void AddCaptureMove(ref Chessboard position, int move, ref MoveList list)
        {
            list.moves[list.count].move =  move;
            list.moves[list.count].score = MVVLVAScore[Global.MovePieceCaptured(move), position.Pieces[Global.MoveFromSquare(move)]] + 1000000;
            ++list.count;
        }
        public static void AddEnPassantMove(ref Chessboard position, int move, ref MoveList list)
        {
            list.moves[list.count].move =  move;
            list.moves[list.count].score = 105 + 1000000;
            ++list.count;
        }
        
        //the following functions could be combined in order to make all of this more efficient
        public static void AddWhitePawnCaptureMove  (ref Chessboard position, int from, int to, int capture, ref MoveList list)
        {
            if (Global.Square120ToRank[from] == (int)Rank.RANK_7)
            {
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.QUEEN_W, 0), ref list);
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.ROOK_W, 0), ref list);
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.BISHOP_W, 0), ref list);
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.KNIGHT_W, 0), ref list);
            }
            else
            {
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.EMPTY, 0), ref list);
            }
        }
        public static void AddWhitePawnMove         (ref Chessboard position, int from, int to, ref MoveList list)
        {
            if (Global.Square120ToRank[from] == (int)Rank.RANK_7)
            {
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.QUEEN_W, 0), ref list);
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.ROOK_W, 0), ref list);
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.BISHOP_W, 0), ref list);
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.KNIGHT_W, 0), ref list);
            }
            else
            {
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.EMPTY, 0), ref list);
            }
        }
        public static void AddBlackPawnCaptureMove  (ref Chessboard position, int from, int to, int capture, ref MoveList list)
        {
            if (Global.Square120ToRank[from] == (int)Rank.RANK_2)
            {
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.QUEEN_B, 0), ref list);
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.ROOK_B, 0), ref list);
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.BISHOP_B, 0), ref list);
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.KNIGHT_B, 0), ref list);
            }
            else
            {
                AddCaptureMove(ref position, GenerateMoveInteger(from, to, capture, (int)PieceID.EMPTY, 0), ref list);
            }
        }
        public static void AddBlackPawnMove         (ref Chessboard position, int from, int to, ref MoveList list)
        {
            //if a black pawn is on the second rank then add promotion moves to the rules list for it
            if (Global.Square120ToRank[from] == (int)Rank.RANK_2)
            {
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.QUEEN_B, 0), ref list);
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.ROOK_B, 0), ref list);
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.BISHOP_B, 0), ref list);
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.KNIGHT_B, 0), ref list);
            }
            else
            {
                AddQuietMove(ref position, GenerateMoveInteger(from, to, (int)PieceID.EMPTY, (int)PieceID.EMPTY, 0), ref list);
            }
        }

        //this fucntion will be calling the above funcitons to add moves into the move list
        public static void GenerateAllMoves(ref Chessboard position, ref MoveList list)
        {
            list.count = 0;

            int piece = (int)PieceID.EMPTY;
            int side = position.Side;
            int square = 0;
            int tempsquare = 0;

            int piecenumber = 0;

            int direction = 0;
            int pieceindex = 0;
            int index;

            //try and combine this is into one function so there isnt so much code bloat
            //white and black pawns
            if (side == (int)Color.WHITE)
            {
                for (piecenumber = 0; piecenumber < position.NumberOfPieces[(int)PieceID.PAWN_W]; ++piecenumber)
                {
                    //make sure that the square is being fed in valid on the board moves, as if not there may be a probelm with checkboard()
                    square = position.PieceList[(int)PieceID.PAWN_W, piecenumber];

                    if (position.Pieces[square + 10] == (int)PieceID.EMPTY)
                    {
                        AddWhitePawnMove(ref position, square, square + 10, ref list);

                        if ((Global.Square120ToRank[square] == (int)Rank.RANK_2) && (position.Pieces[square + 20] == (int)PieceID.EMPTY))
                        {
                            AddQuietMove(ref position, GenerateMoveInteger(square, square + 20, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagPawnStart), ref list);
                        }
                    }

                    //check to see if there is a black piece to capture either to the diagonal right of left of the white pawn, and if there is, add capture moves to the movelist
                    if (!(IsSquareOffBoard(square + 9) == 1) && (Global.PieceColor[position.Pieces[square + 9]] == (int)Color.BLACK))
                    {
                        AddWhitePawnCaptureMove(ref position, square, square + 9, position.Pieces[square + 9], ref list);
                    }
                    if (!(IsSquareOffBoard(square + 11) == 1) && (Global.PieceColor[position.Pieces[square + 11]] == (int)Color.BLACK))
                    {
                        AddWhitePawnCaptureMove(ref position, square, square + 11, position.Pieces[square + 11], ref list);
                    }

                    //check and see if a square on square diagonal to a white pawn is an en passant square, if it is, then add the possible en passant capture to the movelist
                    if ((square + 9 == position.EnPassant) && (position.Pieces[square + 9] != (int)Coordinate.OFFBOARD))
                    {
                        AddEnPassantMove(ref position, GenerateMoveInteger(square, square + 9, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagEnPassant), ref list);
                    }
                    if ((square + 11 == position.EnPassant) && (position.Pieces[square + 11] != (int)Coordinate.OFFBOARD))
                    {
                        AddEnPassantMove(ref position, GenerateMoveInteger(square, square + 11, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagEnPassant), ref list);
                    }
                }
                
                //if the castlepermission & whtie kingsie castle 0001 are anded, if true will produce 1 and make this statment true
                if ((position.CastlePermission & (int)Castling.WKING_CASTLE) > 0000)
                {
                    if (position.Pieces[(int)Coordinate.F1] == (int)PieceID.EMPTY && position.Pieces[(int)Coordinate.G1] == (int)PieceID.EMPTY)
                    {
                        //the below statement will return true as 0 and false as 1 because thats how conditional is set up
                        if ((Attack.IsThisSquareAttacked((int)Coordinate.E1, (int)Color.BLACK, ref position) == 1) && (Attack.IsThisSquareAttacked((int)Coordinate.F1, (int)Color.BLACK, ref position) == 1) && (Attack.IsThisSquareAttacked((int)Coordinate.G1, (int)Color.BLACK, ref position) == 1))
                        {
                            AddQuietMove(ref position, GenerateMoveInteger((int)Coordinate.E1, (int)Coordinate.G1, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagCastling), ref list);
                        }
                    }
                }

                if ((position.CastlePermission & (int)Castling.WQUEEN_CASTLE) > 0000)
                {
                    if (position.Pieces[(int)Coordinate.D1] == (int)PieceID.EMPTY && position.Pieces[(int)Coordinate.C1] == (int)PieceID.EMPTY && position.Pieces[(int)Coordinate.B1] == (int)PieceID.EMPTY)
                    {
                        //the below statement will return true as 0 and false as 1 because thats how conditional is set up
                        if ((Attack.IsThisSquareAttacked((int)Coordinate.E1, (int)Color.BLACK, ref position) == 1) && (Attack.IsThisSquareAttacked((int)Coordinate.D1, (int)Color.BLACK, ref position) == 1) && (Attack.IsThisSquareAttacked((int)Coordinate.C1, (int)Color.BLACK, ref position) == 1))
                        {
                            AddQuietMove(ref position, GenerateMoveInteger((int)Coordinate.E1, (int)Coordinate.C1, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagCastling), ref list);
                        }
                    }
                }
            }
            else
            {
                for (piecenumber = 0; piecenumber < position.NumberOfPieces[(int)PieceID.PAWN_B]; ++piecenumber)
                {
                    //make sure that the square is being fed in valid on the board moves, as if not there may be a probelm with checkboard()
                    square = position.PieceList[(int)PieceID.PAWN_B, piecenumber];

                    if (position.Pieces[square - 10] == (int)PieceID.EMPTY)
                    {
                        AddBlackPawnMove(ref position, square, square - 10, ref list);

                        //promotion
                        if ((Global.Square120ToRank[square] == (int)Rank.RANK_7) && (position.Pieces[square - 20] == (int)PieceID.EMPTY))
                        {
                            AddQuietMove(ref position, GenerateMoveInteger(square, square - 20, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagPawnStart), ref list);
                        }
                    }

                    //check to see if there is a black piece to capture either to the diagonal right of left of the black pawn, and if there is, add capture moves to the movelist
                    if (!(IsSquareOffBoard(square - 9) == 1) && (Global.PieceColor[position.Pieces[square - 9]] == (int)Color.WHITE))
                    {
                        AddBlackPawnCaptureMove(ref position, square, square - 9, position.Pieces[square - 9], ref list);
                    }
                    if (!(IsSquareOffBoard(square - 11) == 1) && (Global.PieceColor[position.Pieces[square - 11]] == (int)Color.WHITE))
                    {
                        AddBlackPawnCaptureMove(ref position, square, square - 11, position.Pieces[square - 11], ref list);
                    }

                    //check and see if a square on square diagonal to a white pawn is an en passant square, if it is, then add the possible en passant capture to the movelist
                    if ((square - 9 == position.EnPassant) && (position.EnPassant != (int)Coordinate.NO_SQ))
                    {
                        AddEnPassantMove(ref position, GenerateMoveInteger(square, square - 9, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagEnPassant), ref list);
                    }
                    if ((square - 11 == position.EnPassant) && (position.EnPassant != (int)Coordinate.NO_SQ))
                    {
                        AddEnPassantMove(ref position, GenerateMoveInteger(square, square - 11, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagEnPassant), ref list);
                    }
                }

                //if the castlepermission & whtie kingsie castle 0001 are anded, the result will be greater than one
                if ((position.CastlePermission & (int)Castling.BKING_CASTLE) > 0000)
                {
                    if (position.Pieces[(int)Coordinate.F8] == (int)PieceID.EMPTY && position.Pieces[(int)Coordinate.G8] == (int)PieceID.EMPTY)
                    {
                        //the below statement will return true as 0 and false as 1 because thats how conditional is set up
                        if ((Attack.IsThisSquareAttacked((int)Coordinate.E8, (int)Color.WHITE, ref position) == 1) && (Attack.IsThisSquareAttacked((int)Coordinate.F8, (int)Color.WHITE, ref position) == 1) && (Attack.IsThisSquareAttacked((int)Coordinate.G8, (int)Color.WHITE, ref position) == 1))
                        {
                            AddQuietMove(ref position, GenerateMoveInteger((int)Coordinate.E8, (int)Coordinate.G8, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagCastling), ref list);
                        }
                    }
                }

                if ((position.CastlePermission & (int)Castling.BQUEEN_CASTLE) > 0000)
                {
                    if (position.Pieces[(int)Coordinate.D8] == (int)PieceID.EMPTY && position.Pieces[(int)Coordinate.C8] == (int)PieceID.EMPTY && position.Pieces[(int)Coordinate.B8] == (int)PieceID.EMPTY)
                    {
                        //the below statement will return true as 0 and false as 1 because thats how conditional is set up
                        if ((Attack.IsThisSquareAttacked((int)Coordinate.E8, (int)Color.WHITE, ref position) == 1) && (Attack.IsThisSquareAttacked((int)Coordinate.D8, (int)Color.WHITE, ref position) == 1) && (Attack.IsThisSquareAttacked((int)Coordinate.C8, (int)Color.WHITE, ref position) == 1))
                        {
                            AddQuietMove(ref position, GenerateMoveInteger((int)Coordinate.E8, (int)Coordinate.C8, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagCastling), ref list);
                        }
                    }
                }
            }

            //SLIDING pieces bishops rooks and queens
            pieceindex = LoopThroughSliderPiecesStartIndex[side];
            piece = LoopThroughSliderPieces[pieceindex++]; //increment piece index after this statement
            //piece will equal zero when it has looped through all available piece types for a particualr color
            while (piece != 0)
            {
                for (piecenumber = 0; piecenumber < position.NumberOfPieces[piece]; ++piecenumber)
                {
                    //the below statement is checking whether or not PieceList has a bullcrap value in an element
                    square = position.PieceList[piece, piecenumber];
                    //ASSERT that the square is on the board

                    for (index = 0; index < NumberOfPieceDirectionsByPiece[piece]; ++index)
                    {
                        //PieceDirections is indexed by piece type first and then the particular direciton in question (index)
                        direction = PieceDirections[piece, index];
                        tempsquare = square + direction;

                        //loop through the available square ina certain direction for a sliding piece until it hits the edge of the board or another piece
                        while ((IsSquareOffBoard(tempsquare)) != 1)
                        {
                            //checking if its running into a piece
                            if ((position.Pieces[tempsquare] != (int)PieceID.EMPTY))
                            {
                                // 0 ^ 1 = 1  1 ^ 1 = 0 (if the piece it ran into is of the opposite color, then make a capture)
                                if (Global.PieceColor[position.Pieces[tempsquare]] == (side ^ 1))
                                {
                                    AddCaptureMove(ref position, GenerateMoveInteger(square, tempsquare, position.Pieces[tempsquare], (int)PieceID.EMPTY, 0), ref list);
                                }
                                break;
                            }

                            AddQuietMove(ref position, GenerateMoveInteger(square, tempsquare, (int)PieceID.EMPTY, (int)PieceID.EMPTY, 0), ref list);
                            tempsquare += direction;
                        }
                    }
                }

                piece = LoopThroughSliderPieces[pieceindex++];
            }

            //NON-SLIDING pieces knights and kings (possibly turn this into a for loop)
            pieceindex = LoopThroughSliderNonPiecesStartIndex[side];
            piece = LoopThroughNonSliderPieces[pieceindex++];
            while (piece != 0)
            {
                for (piecenumber = 0; piecenumber < position.NumberOfPieces[piece]; ++piecenumber)
                {
                    //the below statement is checking whether or not PieceList has a bullcrap value in an element
                    square = position.PieceList[piece, piecenumber];
                    //ASSERT that the square is on the board

                    for (index = 0; index < NumberOfPieceDirectionsByPiece[piece]; ++index)
                    {
                        //PieceDirections is indexed by piece type first and then the particular direciton in question (index)
                        direction = PieceDirections[piece, index];
                        tempsquare = square + direction;

                        //bad coding practive to use continue perhaps change
                        if (IsSquareOffBoard(tempsquare) == 1)
                        {
                            continue;
                        }

                        //checking if its running into a piece
                        if ((position.Pieces[tempsquare] != (int)PieceID.EMPTY))
                        {
                            // 0 ^ 1 = 1  1 ^ 1 = 0 (if the piece it ran into is of the opposite color, then make a capture)
                            if (Global.PieceColor[position.Pieces[tempsquare]] == (side ^ 1))
                            {
                                AddCaptureMove(ref position, GenerateMoveInteger(square, tempsquare, position.Pieces[tempsquare], (int)PieceID.EMPTY, 0), ref list);
                            }

                            continue;
                        }

                        AddQuietMove(ref position, GenerateMoveInteger(square, tempsquare, (int)PieceID.EMPTY, (int)PieceID.EMPTY, 0), ref list);
                    }
                }
                //increment the piece to another non slider so it can be referenced from piecelist and generate moves for it
                piece = LoopThroughNonSliderPieces[pieceindex++];
            }
        }
        //this function is a near replica of GenerateAllMoves(), however, this one only generates capture moves
        public static void GenerateAllCaptureMoves(ref Chessboard position, ref MoveList list)
        {
            list.count = 0;

            int piece = (int)PieceID.EMPTY;
            int side = position.Side;
            int square = 0;
            int tempsquare = 0;

            int piecenumber = 0;

            int direction = 0;
            int pieceindex = 0;
            int index;

            //try and combine this is into one function so there isnt so much code bloat
            //white and black pawns
            if (side == (int)Color.WHITE)
            {
                for (piecenumber = 0; piecenumber < position.NumberOfPieces[(int)PieceID.PAWN_W]; ++piecenumber)
                {
                    //make sure that the square is being fed in valid on the board moves, as if not there may be a probelm with checkboard()
                    square = position.PieceList[(int)PieceID.PAWN_W, piecenumber];


                    //check to see if there is a black piece to capture either to the diagonal right of left of the white pawn, and if there is, add capture moves to the movelist
                    if (!(IsSquareOffBoard(square + 9) == 1) && (Global.PieceColor[position.Pieces[square + 9]] == (int)Color.BLACK))
                    {
                        AddWhitePawnCaptureMove(ref position, square, square + 9, position.Pieces[square + 9], ref list);
                    }
                    if (!(IsSquareOffBoard(square + 11) == 1) && (Global.PieceColor[position.Pieces[square + 11]] == (int)Color.BLACK))
                    {
                        AddWhitePawnCaptureMove(ref position, square, square + 11, position.Pieces[square + 11], ref list);
                    }

                    //check and see if a square on square diagonal to a white pawn is an en passant square, if it is, then add the possible en passant capture to the movelist
                    if ((square + 9 == position.EnPassant) && (position.Pieces[square + 9] != (int)Coordinate.OFFBOARD))
                    {
                        AddEnPassantMove(ref position, GenerateMoveInteger(square, square + 9, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagEnPassant), ref list);
                    }
                    if ((square + 11 == position.EnPassant) && (position.Pieces[square + 11] != (int)Coordinate.OFFBOARD))
                    {
                        AddEnPassantMove(ref position, GenerateMoveInteger(square, square + 11, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagEnPassant), ref list);
                    }
                }
            }
            else
            {
                for (piecenumber = 0; piecenumber < position.NumberOfPieces[(int)PieceID.PAWN_B]; ++piecenumber)
                {
                    //make sure that the square is being fed in valid on the board moves, as if not there may be a probelm with checkboard()
                    square = position.PieceList[(int)PieceID.PAWN_B, piecenumber];

                    //check to see if there is a black piece to capture either to the diagonal right of left of the black pawn, and if there is, add capture moves to the movelist
                    if (!(IsSquareOffBoard(square - 9) == 1) && (Global.PieceColor[position.Pieces[square - 9]] == (int)Color.WHITE))
                    {
                        AddBlackPawnCaptureMove(ref position, square, square - 9, position.Pieces[square - 9], ref list);
                    }
                    if (!(IsSquareOffBoard(square - 11) == 1) && (Global.PieceColor[position.Pieces[square - 11]] == (int)Color.WHITE))
                    {
                        AddBlackPawnCaptureMove(ref position, square, square - 11, position.Pieces[square - 11], ref list);
                    }

                    //check and see if a square on square diagonal to a white pawn is an en passant square, if it is, then add the possible en passant capture to the movelist
                    if ((square - 9 == position.EnPassant) && (position.EnPassant != (int)Coordinate.NO_SQ))
                    {
                        AddEnPassantMove(ref position, GenerateMoveInteger(square, square - 9, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagEnPassant), ref list);
                    }
                    if ((square - 11 == position.EnPassant) && (position.EnPassant != (int)Coordinate.NO_SQ))
                    {
                        AddEnPassantMove(ref position, GenerateMoveInteger(square, square - 11, (int)PieceID.EMPTY, (int)PieceID.EMPTY, Global.MoveFlagEnPassant), ref list);
                    }
                }
            }

            //SLIDING pieces bishops rooks and queens
            pieceindex = LoopThroughSliderPiecesStartIndex[side];
            piece = LoopThroughSliderPieces[pieceindex++]; //increment piece index after this statement
            //piece will equal zero when it has looped through all available piece types for a particualr color
            while (piece != 0)
            {
                for (piecenumber = 0; piecenumber < position.NumberOfPieces[piece]; ++piecenumber)
                {
                    //the below statement is checking whether or not PieceList has a bullcrap value in an element
                    square = position.PieceList[piece, piecenumber];
                    //ASSERT that the square is on the board

                    for (index = 0; index < NumberOfPieceDirectionsByPiece[piece]; ++index)
                    {
                        //PieceDirections is indexed by piece type first and then the particular direciton in question (index)
                        direction = PieceDirections[piece, index];
                        tempsquare = square + direction;

                        //loop through the available square ina certain direction for a sliding piece until it hits the edge of the board or another piece
                        while ((IsSquareOffBoard(tempsquare)) != 1)
                        {
                            //checking if its running into a piece
                            if ((position.Pieces[tempsquare] != (int)PieceID.EMPTY))
                            {
                                // 0 ^ 1 = 1  1 ^ 1 = 0 (if the piece it ran into is of the opposite color, then make a capture)
                                if (Global.PieceColor[position.Pieces[tempsquare]] == (side ^ 1))
                                {
                                    AddCaptureMove(ref position, GenerateMoveInteger(square, tempsquare, position.Pieces[tempsquare], (int)PieceID.EMPTY, 0), ref list);
                                }
                                break;
                            }

                            tempsquare += direction;
                        }
                    }
                }

                piece = LoopThroughSliderPieces[pieceindex++];
            }

            //NON-SLIDING pieces knights and kings (possibly turn this into a for loop)
            pieceindex = LoopThroughSliderNonPiecesStartIndex[side];
            piece = LoopThroughNonSliderPieces[pieceindex++];
            while (piece != 0)
            {
                for (piecenumber = 0; piecenumber < position.NumberOfPieces[piece]; ++piecenumber)
                {
                    //the below statement is checking whether or not PieceList has a bullcrap value in an element
                    square = position.PieceList[piece, piecenumber];
                    //ASSERT that the square is on the board

                    for (index = 0; index < NumberOfPieceDirectionsByPiece[piece]; ++index)
                    {
                        //PieceDirections is indexed by piece type first and then the particular direciton in question (index)
                        direction = PieceDirections[piece, index];
                        tempsquare = square + direction;

                        //bad coding practive to use continue perhaps change
                        if (IsSquareOffBoard(tempsquare) == 1)
                        {
                            continue;
                        }

                        //checking if its running into a piece
                        if ((position.Pieces[tempsquare] != (int)PieceID.EMPTY))
                        {
                            // 0 ^ 1 = 1  1 ^ 1 = 0 (if the piece it ran into is of the opposite color, then make a capture)
                            if (Global.PieceColor[position.Pieces[tempsquare]] == (side ^ 1))
                            {
                                AddCaptureMove(ref position, GenerateMoveInteger(square, tempsquare, position.Pieces[tempsquare], (int)PieceID.EMPTY, 0), ref list);
                            }

                            continue;
                        }
                    }
                }
                //increment the piece to another non slider so it can be referenced from piecelist and generate moves for it
                piece = LoopThroughNonSliderPieces[pieceindex++];
            }
        }
    }
}
