using System;

namespace Engine
{
    /*To those reading this, I must give a shout out to Bluefever Software and there Vice engine written in C. Ive learned loads of knowledge,
     concepts, and techniques from them, and they heavily influenced the beginnings of this engine. This engine is similar to vice albiet being a C# engine,
     and being much different in the way of structuring*/

    class Program
    {
        //makes the engine throw an error --> Q7/5p2/5P1p/5PPN/6Pk/4N1Rp/7P/6K1 w - - 0 1
        //mate in 3 = "8/1Kn1p3/1p5N/4p1q1/4k1N1/3R2p1/Qn2B3/7R w - - 0 1"

        //public static string TestFEN = "r1b1k2r/ppppnppp/2n2q2/2b5/3NP3/2P1B3/PP3PPP/RN1QKB1R w KQkq - 0 1";
        //public static string TestFENArray = TestFEN;

        static void Main()
        {
            Engine.Initializers.InitializeAll();
            //uncomment the uci loop statement and get rid of everything else
            Engine.UCI.UCILoop();

            /*Engine.Chessboard TestBoard = new Engine.Chessboard();
            Engine.MoveList TestMoveList = new Engine.MoveList();

            Engine.HashTable.InitializeHashTable(ref TestBoard.PrincipleVariationTable);

            Engine.Chessboard.ParseFEN(ref TestFENArray, ref TestBoard);
            //Engine.MoveGen.GenerateAllMoves(ref TestBoard, ref TestMoveList);

            //Engine.PVTable test;
            //Engine.PVEntry test1;

            Engine.SearchInfo info = new Engine.SearchInfo();
            //depth is in plys, so if you want it to solve a mate in 3 you need depth 6, or 6 plys
            info.Depth = 6;
            info.StartTime = Engine.Search.GetTimeMilliSeconds();
            info.StopTime = info.StartTime + 200000;

            Engine.Search.SearchPosition(ref TestBoard, ref info);

            //Engine.Debug.PerftTest(4, ref TestBoard); */
        }
    }
}
