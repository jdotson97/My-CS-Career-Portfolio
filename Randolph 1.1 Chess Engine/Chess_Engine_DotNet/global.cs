using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public struct Global
    {
        public static string STARTING_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /// <summary>
        /// Sets max game length: 1536 plys/768 moves
        /// </summary>
        public const int MAX_GAME_MOVE_LENGTH = 1536;

        public const int BOARD_SQUARE_NUMBER = 120;
        public const int CHESSBOARD_SQUARE_NUMBER = 64;

        public const int MAX_POSITIONAL_MOVES = 256;

        public const int MAX_DEPTH = 64;

        public const int NO_MOVE = 0;

        public const int CHECKMATE = 8388608;

        public static int[] Square120To64 = new int[BOARD_SQUARE_NUMBER];
        public static int[] Square64To120 = new int[CHESSBOARD_SQUARE_NUMBER];

        /// <summary>
        /// ClearMask is an array of 64 longs, with each element being the btiwise compliment of SetMask[]: all 0s except
        /// one 1 shifted it's index's number of bits. |=
        /// </summary>
        public static long[] SetMask = new long[CHESSBOARD_SQUARE_NUMBER];       
        /// <summary>
        /// ClearMask is an array of 64 longs, with each element being the btiwise compliment of SetMask[]: all 1s except
        /// one zero shifted it's index's number of bits. &=
        /// </summary>
        public static long[] ClearMask = new long[CHESSBOARD_SQUARE_NUMBER];

        //For debugging: Note: Convert to strings
        public static char[] PieceString = ".PNBRQKpnbrqk".ToCharArray();
        public static char[] SideString = "wb-".ToCharArray();
        public static char[] RankString = "12345678".ToCharArray();
        public static char[] FileString = "abcdefgh".ToCharArray();

        //Indexed using a 10x12 board square
        public static int[] Square120ToFile = new int[BOARD_SQUARE_NUMBER];
        public static int[] Square120ToRank = new int[BOARD_SQUARE_NUMBER];
        
        //Indexed by PieceID
        public static readonly int[] IsPieceBig =
        {
            (int)Conditional.FALSE,

            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE,
            (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE
        };
        public static readonly int[] IsPieceMajor =
        {
            (int)Conditional.FALSE,
            
            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE,
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE
        };
        public static readonly int[] IsPieceMinor =
        {
            (int)Conditional.FALSE,
            
            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE,
            (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE
        };

        public static readonly int[] PieceValue =
        {
            0,

            100, 325, 325, 550, 1000, 50000,
            100, 325, 325, 550, 1000, 50000
        };
        public static readonly int[] PieceColor =
        {
            (int)Color.BOTH,

            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Color.WHITE, (int)Color.WHITE, (int)Color.WHITE, (int)Color.WHITE, (int)Color.WHITE, (int)Color.WHITE,
            (int)Color.BLACK, (int)Color.BLACK, (int)Color.BLACK, (int)Color.BLACK, (int)Color.BLACK, (int)Color.BLACK
        };

        //Indexed by PieceID
        public static readonly int[] IsPiecePawn =
        {
            (int)Conditional.FALSE,
            
            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.TRUE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE,
            (int)Conditional.TRUE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE
        };
        public static readonly int[] IsPieceKnight =
        {
            (int)Conditional.FALSE,
            
            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE,
            (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE
        };
        public static readonly int[] IsPieceKing =
        {
            (int)Conditional.FALSE,
            
            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE,
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE
        };
        public static readonly int[] IsPieceRookOrQueen =
        {
            (int)Conditional.FALSE,
            
            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.FALSE,
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.FALSE
        };
        public static readonly int[] IsPieceBishopOrQueen =
        {
            (int)Conditional.FALSE,
            
            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.FALSE,
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.FALSE
        };
        public static readonly int[] IsPieceASlider =
        {
            (int)Conditional.FALSE,
            
            //pawns                 //knights               //bishops               //rooks                 //queens                //kings
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.FALSE,
            (int)Conditional.FALSE, (int)Conditional.FALSE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.TRUE, (int)Conditional.FALSE
        };

        /* Open to see bitmapping summary
         *
         * 0000 0000 0000 0000 0000 0000 0000
         * 
         * FROM     - 7 bits
         * TO       - 7 bits
         * CAPTURE  - 4 bits
         * EN PASS  - 1 bit
         * PAWN SRT - 1 bit
         * PROMOTE  - 4 bits
         * CASTLE   - 1 bit
         */

        static readonly public int MoveFlagEnPassant = 0x40000;
        static readonly public int MoveFlagPawnStart = 0x80000;
        static readonly public int MoveFlagCastling = 0x1000000;
        static readonly public int MoveFlagCapture = 0x7C000;
        static readonly public int MoveFlagPromotion = 0xF00000;

        //you are shoving the move int a certain number of bits right so that it trims all the information from the right side of the information that you want,
        //and then it simply ands with 0x7f in order to trim the excess information from the left side
        public static int MoveFromSquare(int move)
        {
            return (move & 0x7F);
        }
        public static int MoveToSquare(int move)
        {
            //bitwise anding move >> 7 with 0x7F effectily trims the other bits off of move
            return ((move >> 7) & 0x7F);
        }
        public static int MovePieceCaptured(int move)
        {
            return ((move >> 14) & 0xF);
        }
        public static int MovePromotedPiece(int move)
        {
            return ((move >> 20) & 0xF);
        }

        //Hashtable 2MB in size
        public const int HashTableSize = 0x100000 * 2;

        //this will allow me to give a file and a rank, and this function will return the 120 square board coordinate for that square
        public static int FileAndRankTo120Square(int file, int rank) //this being static may cause some kind of issue in the future so just be minful for that
        {
            return (21 + (file)) + ((rank) * 10);
        }
    };
}
