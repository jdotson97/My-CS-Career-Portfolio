using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Engine
{
    public struct Search
    {
        //public static readonly int SEARCH_WAS_INTERRUPTED = 100000;

        //it seems that the way to make this not block the program is to put the whole thing on a different threads

        //public static Task GetUserInput;

        public static bool STOP_SEARCHING = false;
        public static bool QUIT = false;

        /*public static Task IsInputWaiting()
        {
            /*GetUserInput = return Task.Factory.StartNew(() =>
            {
                Console.In.Close();
                INPUT_COMMAND = Console.ReadLine();
            });

            //return GetUserInput;
        }

        public static async void AwaitUserInputAsync()
        {
            do
            {
                await IsInputWaiting();

                STOP_SEARCHING = true; 

                if (string.Compare(INPUT_COMMAND, 0, "quit", 0, 4) == 0)
                {
                    QUIT = true;
                }

            } while (!QUIT);
        }*/

        public static int GetTimeMilliSeconds()
        {
            return System.Environment.TickCount;
        }
        public static void CheckOnTime(ref SearchInfo info)
        {
            if ((info.TimeSetting) && (GetTimeMilliSeconds() > info.StopTime))
            {
                info.stopped = true;
            }
        }
        public static void PickNextMove(int moveNumber, ref MoveList list)
        {
            Move temp;
            int index = 0;
            int bestScore = 0;
            int bestNum = moveNumber;

            for (index = moveNumber; index < list.count; ++index)
            {
                if (list.moves[index].score > bestScore)
                {
                    bestScore = list.moves[index].score;
                    bestNum = index;
                }
            }
            temp = list.moves[moveNumber];
            list.moves[moveNumber] = list.moves[bestNum];
            list.moves[bestNum] = temp;
        }
        public static bool IsRepetition(ref Chessboard position)
        {
            for (int i = (position.TotalPly - position.FiftyMove); i < position.TotalPly - 1; ++i)
            {
                if (position.PositionalKey == position.BoardHistory[i].PositionalKey)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Cleans up search stats, as well as the hashtable to make way for a new search
        /// </summary>
        /// <param name="position"></param>
        /// <param name="info"></param>
        public static void CleanUpSearch(ref Chessboard position, ref SearchInfo info)
        {
            for (int i = 0; i < 13; ++i)
            {
                for (int j = 0; j < Global.BOARD_SQUARE_NUMBER; ++j)
                {
                    position.SearchHistory[i, j] = 0;
                }
            }
            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < Global.MAX_DEPTH; ++j)
                {
                    position.SearchKillers[i, j] = 0;
                }
            }

            HashTable.ClearHashTable(ref position.PrincipleVariationTable);
            position.Ply = 0;

            STOP_SEARCHING = false;
            QUIT = false;

            info.stopped = false;
            info.nodes = 0;
            info.FailHigh = 0;
            info.FailHighFirst = 0;
        }
        /// <summary>
        /// Used to remove the horizon effect from an alpha beta search.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="position"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int Quiescence(int alpha, int beta, ref Chessboard position, ref SearchInfo info)
        {
            if ((info.nodes & 8191) == 0)
            {
                CheckOnTime(ref info);
            }

            ++info.nodes;
            //repitition - this may not require the position.ply line
            if (((IsRepetition(ref position)) || (position.FiftyMove >= 100)) && (position.Ply > 0))
            {
                return 0;
            }
            //not greater than max depth
            if (position.Ply > (Global.MAX_DEPTH - 1))
            {
                return Evaluate.EvaluateThisPosition(ref position);
            }

            int score = Evaluate.EvaluateThisPosition(ref position);

            //if our position is already better than beta, it is safe to say that we are not going to make any move the reduces it
            if (score >= beta)
            {
                return beta;
            }
            if (score > alpha)
            {
                alpha = score;
            }

            MoveList list = new MoveList();
            MoveGen.GenerateAllCaptureMoves(ref position, ref list);

            //int moveNumber = 0;
            int legal = 0;
            int oldAlpha = alpha;
            int bestMove = Global.NO_MOVE;
            int pvMove = HashTable.ProbeHashTable(ref position);

            score = -int.MaxValue;

            for (int i = 0; i < list.count; ++i)
            {
                PickNextMove(i, ref list);

                if (MakeMove.Move(ref position, list.moves[i].move) == (int)Conditional.FALSE)
                {
                    continue;
                }
                ++legal;
                score = -Quiescence(-beta, -alpha, ref position, ref info); //NegaMax
                MakeMove.TakeBackMove(ref position);
                //check on time
                if (info.stopped || STOP_SEARCHING)
                {
                    return 0;
                }

                if (score > alpha)
                {
                    //if score is greater than beta, it will be pruned, and thus we will return beta
                    if (score >= beta)
                    {
                        if (legal == 1)
                        {
                            ++info.FailHighFirst;
                        }
                        ++info.FailHigh;

                        return beta;
                    }
                    alpha = score;
                    //this move beat alpha, and therefore we must store it in bestMove
                    bestMove = list.moves[i].move;
                }
            }

            if (alpha != oldAlpha)
            {
                //we found a new, higher alpha! therefore we must store it in the hashtable
                HashTable.StoreHashTableMove(ref position, bestMove);
            }

            return alpha;
        }
        /// <summary>
        /// Recursively searches a position to a provided depth, using an evaluation funciton to produce an overall score.
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="depth">How deep to search a position.</param>
        /// <param name="position"></param>
        /// <param name="info">Contains info about a particular search.</param>
        /// <param name="makeNullMove"></param>
        /// <returns></returns>
        public static int AlphaBeta(int alpha, int beta, int depth, ref Chessboard position, ref SearchInfo info, int makeNullMove)
        {
            if (depth == 0)
            {
                return Quiescence(alpha, beta, ref position, ref info);
            }

            if ((info.nodes & 2047) == 0)
            {
                CheckOnTime(ref info);
            }

            ++info.nodes;

            if (((IsRepetition(ref position)) || (position.FiftyMove >= 100)) && (position.Ply > 0))
            {
                return 0;
            }
            if (position.Ply > (Global.MAX_DEPTH - 1))
            {
                return Evaluate.EvaluateThisPosition(ref position);
            }

            MoveList list = new MoveList();
            MoveGen.GenerateAllMoves(ref position, ref list);

            //int moveNumber = 0;
            int legal = 0;
            int oldAlpha = alpha;
            int bestMove = Global.NO_MOVE;
            int score = -int.MaxValue;
            int pvMove = HashTable.ProbeHashTable(ref position);

            //just as a note this may cause som strange bugs in the future ---- POSSIBLE BUG ALERT!
            if (pvMove != Global.NO_MOVE)
            {
                for (int i = 0; i < list.count; ++i)
                {
                    if (list.moves[i].move == pvMove)
                    {
                        //if there is a move for this line that is already stored in the hashtable, prioritize it above all others
                        list.moves[i].score = 2000000;
                        break;
                    }
                }
            }

            for (int i = 0; i < list.count; ++i)
            {
                PickNextMove(i, ref list);

                if (MakeMove.Move(ref position, list.moves[i].move) == (int)Conditional.FALSE)
                {
                    continue;
                }

                ++legal;
                score = -AlphaBeta(-beta, -alpha, depth - 1, ref position, ref info, (int)Conditional.TRUE); //NegaMax
                MakeMove.TakeBackMove(ref position);

                if (info.stopped || STOP_SEARCHING)
                {
                    return 0;
                }

                if (score > alpha)
                {
                    //if score is greater than beta, it will be pruned, and thus we will return beta
                    if (score >= beta)
                    {
                        if (legal == 1)
                        {
                            ++info.FailHighFirst;
                        }
                        ++info.FailHigh;
                        
                        if ((list.moves[i].move & Global.MoveFlagCapture) == 0)
                        {
                            position.SearchKillers[1, position.Ply] = position.SearchKillers[0, position.Ply];
                            position.SearchKillers[0, position.Ply] = list.moves[i].move;
                        }

                        return beta;
                    }
                    alpha = score;
                    //this move beat alpha, and therefore we must store it in bestMove
                    bestMove = list.moves[i].move;

                    if ((list.moves[i].move & Global.MoveFlagCapture) == 0)
                    {
                        //this will prioritize moves that occur closer to the root of the tree
                        position.SearchHistory[position.Pieces[Global.MoveFromSquare(bestMove)], /*position.Pieces[*/Global.MoveToSquare(bestMove)/*]*/] += depth;
                    }
                }
            }
            //if there are no legal moves
            if (legal == 0)
            {
                if (Attack.IsThisSquareAttacked(position.Kings[position.Side], position.Side ^ 1, ref position) == (int)Conditional.TRUE)
                {
                    //if the kings square is attacked, checkmate
                    return -(Global.CHECKMATE/* + position.Ply*/);
                }
                else
                {
                    //if the kings square is not attacked, stalemate
                    return 0; 
                }
            }

            if (alpha != oldAlpha)
            {
                //we found a new, higher alpha! therefore we must store it in the hashtable
                HashTable.StoreHashTableMove(ref position, bestMove);
            }

            return alpha;
        }
        /// <summary>
        /// Manages iterative deepening, as well as initializing search
        /// </summary>
        /// <param name="position"></param>
        /// <param name="info"></param>
        public static async Task SearchPosition(Chessboard position, SearchInfo info)
        {
            int bestMove = Global.NO_MOVE;
            int bestScore = -int.MaxValue;
            int pvMoves = 0;
            CleanUpSearch(ref position, ref info);

            //AwaitUserInputAsync();

            for (int i = 0; i <= info.Depth; ++i)
            {
                bestScore = AlphaBeta(-int.MaxValue, int.MaxValue, i, ref position, ref info, (int)Conditional.TRUE);

                if (info.stopped || STOP_SEARCHING)
                {
                    break;
                }

                pvMoves = HashTable.GetPvLine(i, ref position);
                bestMove = position.PrincipleVariationArray[0];

                Console.Write(string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", "info score cp ", (bestScore).ToString(), " depth ", (i).ToString(), " nodes ", (info.nodes).ToString(), " time ", (info.StartTime).ToString(), " pv "));
                
                pvMoves = HashTable.GetPvLine(i, ref position);
                for (int j = 0; j < pvMoves; ++j)
                {
                    Console.Write(string.Format("{0} {1}", IO.TranslateMove(position.PrincipleVariationArray[j]), " "));
                }

                Console.Write("\n");
            }

            Console.WriteLine("bestmove " + IO.TranslateMove(bestMove));

            /*if (bestScore >= 290 || bestScore <= -290)
            {
                Console.WriteLine(string.Format("{0} {1}", "Score: ", ("m" + (bestScore - Global.CHECKMATE))));
            }
            else
            {
                Console.WriteLine($"score {bestScore}");
            }*/

            return;
        }
    }
}