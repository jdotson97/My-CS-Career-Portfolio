using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public struct Debug
    {
        public static long leafnodes;

        public static  void Perft(int depth, ref Chessboard position)
        {
            if (depth == 0)
            {
                leafnodes++;
                return;
            }

            MoveList list = new MoveList();
            MoveGen.GenerateAllMoves(ref position, ref list);

            for (int i = 0; i < list.count; ++i)
            {
                if (MakeMove.Move(ref position, (list.moves[i]).move) == (int)Conditional.FALSE)
                {
                    continue;
                }
                Perft(depth - 1, ref position);
                MakeMove.TakeBackMove(ref position);
            }

            return;
        }

        public static void PerftTest(int depth, ref Chessboard position)
        {
            leafnodes = 0;

            MoveList list = new MoveList();
            MoveGen.GenerateAllMoves(ref position, ref list);

            int move;
            for (int i = 0; i < list.count; ++i)
            {
                move = list.moves[i].move;
                if (MakeMove.Move(ref position, (list.moves[i]).move) == (int)Conditional.FALSE) //it thinks the king is attacked on every movee
                {
                    continue;
                }

                long cumulativenodes = leafnodes;
                Perft(depth - 1, ref position);
                MakeMove.TakeBackMove(ref position);
                long oldnodes = leafnodes - cumulativenodes;
            }

            //print out how may nodes were visited;

            return;
        }
    }
}