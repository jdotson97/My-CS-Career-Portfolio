using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public struct Initializers
    {
        public static void InitializeAll()
        {
            Initialize120SquareTo64();
            InitializeBitmasks();
            InitializeHashKeys();
            Initialize120SquareFilesAndRanks();
            MoveGen.InitializeMVVLVA();
        }

        //presumably the reason you want to do this is so every piece gets a random key and also every position that the piece could be on also recieves a random key
        //all sources ive consulted say that this is the best way to structure your hashkeys
        public static void InitializeHashKeys()
        {
            for (int i = 0; i < 13; ++i)
            {
                for (int j = 0; j < 120; ++j)
                {
                    HashKey.PieceKeys[i, j] = HashKey.GetRandom64BitHash(); ;
                }
            }

            HashKey.SideKey = HashKey.GetRandom64BitHash();

            for (int i = 0; i < 16; ++i)
            {
                HashKey.CastleKey[i] = HashKey.GetRandom64BitHash(); ;
            }
        }
        public static void InitializeBitmasks()
        {
            // the set and clear masks will be used to set a clear bits within bitboards; for example, if a pawn moves off of e4, you want to clear the 1
            // in the pawn bitboards e4 to zero, and then set a one on the square that it moved to

            for (int i = 0; i < 64; ++i)
            {
                //set them both to there cleared values so they can be initialized
                Global.SetMask[i] = 0;
                Global.ClearMask[i] = 0;
            }

            for (int i = 0; i < 64; ++i)
            {
                Global.SetMask[i] = (0 << i); //this will be used with |= to set a bit
                Global.ClearMask[i] = ~Global.SetMask[i]; //this will be used with &= to clear a bit
            }
        }
        public static void Initialize120SquareFilesAndRanks()
        {
            int square = (int)Coordinate.A1;

            for (int i = 0; i < Global.BOARD_SQUARE_NUMBER; ++i)
            {
                Global.Square120ToFile[i] = (int)Coordinate.OFFBOARD;
                Global.Square120ToRank[i] = (int)Coordinate.OFFBOARD;
            }

            //after the outside squares have been set to OFFBOARD, the two array are initialized
            for (int rank = (int)Rank.RANK_1; rank <= (int)Rank.RANK_8; ++rank)
            {
                for (int file = (int)File.FILE_A; file <= (int)File.FILE_H; ++file)
                {
                    square = Global.FileAndRankTo120Square(file, rank);
                    Global.Square120ToFile[square] = file;
                    Global.Square120ToRank[square] = rank;
                }
            }
        }
        public static void Initialize120SquareTo64()
        {
            //initialize the two arrays initially to an impossible value first
            for (int index = 0; index < Global.CHESSBOARD_SQUARE_NUMBER; ++index)
            {
                //65 is not on the board see above for explanation
                Global.Square64To120[index] = 64;
            }

            for (int index = 0; index < Global.BOARD_SQUARE_NUMBER; ++index)
            {
                //120 is not on the board because this array only goes to a value of 119
                Global.Square120To64[index] = 119;
            }

            int square120 = 0; // this was originally set to coordinate.a1
            int square64 = 0;

            for (int rank = (int)Rank.RANK_1; rank <= (int)Rank.RANK_8; ++rank)
            {
                for (int file = (int)File.FILE_A; file <= (int)File.FILE_H; ++file)
                {
                    square120 = Global.FileAndRankTo120Square(file, rank);
                    Global.Square64To120[square64] = square120;
                    Global.Square120To64[square120] = square64;

                    ++square64;
                }
            }
        }
    };
}
