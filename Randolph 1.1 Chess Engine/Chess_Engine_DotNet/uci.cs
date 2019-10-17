using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Engine
{
    public struct UCI
    {
        public static string START_POSITION_COMMAND = "startpos";
        public static readonly int INPUT_BUFFER = (400 * 6);

        public static string INPUT_COMMAND;

        public static async Task ParseGoCommand(string line, SearchInfo info, Chessboard position)
        {
            int depth = -1, movestogo = 30, movetime = -1, time = -1, increment = 0;
            //adding a space at the end do indexOf(" ") can be used in every context
            line += " ";

            //Time
            if ((line.IndexOf("wtime") != -1) && (position.Side == (int)Color.WHITE))
            {
                string tempString = (line.Substring(line.IndexOf("wtime") + 6));
                string extractedTime = tempString.Substring(0, tempString.IndexOf(" "));

                time = Convert.ToInt32(extractedTime);
            }
            if ((line.IndexOf("btime") != -1) && (position.Side == (int)Color.BLACK))
            {
                string tempString = (line.Substring(line.IndexOf("btime") + 6));
                string extractedTime = tempString.Substring(0, tempString.IndexOf(" "));

                time = Convert.ToInt32(extractedTime);
            }
            //Increment
            if ((line.IndexOf("winc") != -1) && (position.Side == (int)Color.WHITE))
            {
                string tempString = (line.Substring(line.IndexOf("winc") + 5));
                string extractedInc = tempString.Substring(0, tempString.IndexOf(" "));

                increment = Convert.ToInt32(extractedInc);
            }
            if ((line.IndexOf("binc") != -1) && (position.Side == (int)Color.BLACK))
            {
                string tempString = (line.Substring(line.IndexOf("binc") + 5));
                string extractedInc = tempString.Substring(0, tempString.IndexOf(" "));

                increment = Convert.ToInt32(extractedInc);
            }

            if ((line.IndexOf("movetime") != -1))
            {
                string tempString = (line.Substring(line.IndexOf("movetime") + 9));
                string extractedMovetime = tempString.Substring(0, tempString.IndexOf(" "));

                movetime = Convert.ToInt32(extractedMovetime);
            }
            if ((line.IndexOf("movestogo") != -1))
            {
                string tempString = (line.Substring(line.IndexOf("movestogo") + 10));
                string extractedMovesToGo = tempString.Substring(0, tempString.IndexOf(" "));

                movestogo = Convert.ToInt32(extractedMovesToGo);
            }

            if ((line.IndexOf("depth") != -1))
            {
                string tempString = (line.Substring(line.IndexOf("depth") + 6));
                string extractedDepth = tempString.Substring(0, tempString.IndexOf(" "));

                depth = Convert.ToInt32(extractedDepth);
            }
            if ((line.IndexOf("infinite") != -1))
            {
                depth = Global.MAX_DEPTH;
            }

            //---------------------------------

            if (movetime != -1)
            {
                time = movetime;
                movestogo = 1;
            }

            info.StartTime = Search.GetTimeMilliSeconds();
            info.Depth = depth;

            if (time != -1)
            {
                info.TimeSetting = true;
                time /= movestogo;
                time -= 50; //we dont want to overrun our time
                info.StopTime = info.StartTime + time + increment;
            }

            if (depth == -1)
            {
                info.Depth = Global.MAX_DEPTH;
            }

            //Console.In.Close();
            await Search.SearchPosition(position, info).ConfigureAwait(false);
        }
        public static void ParsePosition(string line, ref Chessboard position)
        {
            string parsedFEN = null;

            if (String.Compare(line, 0, "startpos", 0, 8) == 0)
            {
                Chessboard.ParseFEN(ref Global.STARTING_FEN, ref position);
            }
            else
            {
                if (line.IndexOf("fen") == -1)
                {
                    Chessboard.ParseFEN(ref Global.STARTING_FEN, ref position);
                }
                else
                {
                    var temp = (line.TrimStart(line.Substring(0, line.IndexOf("fen") + 3).ToCharArray()));
                    parsedFEN = 
                        temp.Substring(0, 
                        ((temp.IndexOf("moves") != -1) ? temp.IndexOf("moves") - 1 : temp.Length));

                    Chessboard.ParseFEN(ref parsedFEN, ref position);
                }
            }

            if (line.IndexOf("moves") != -1)
            {
                string extractedMove;
                int parsedMove;
                string tempMovesString = line.Substring(line.IndexOf("moves") + 6, line.Length - (line.IndexOf("moves") + 6)) + " ";

                int i = 0;
                while (tempMovesString != "")
                {
                    extractedMove = tempMovesString.Substring(0, tempMovesString.IndexOf(" "));
                    parsedMove = IO.ParseMove(ref extractedMove, ref position);

                    if (parsedMove == Global.NO_MOVE)
                    {
                        break;
                    }
                    MakeMove.Move(ref position, parsedMove);
                    //trim off the piece we just parsed so we can get the next piece
                    tempMovesString = tempMovesString.Substring(extractedMove.Length + 1, tempMovesString.Length - extractedMove.Length - 1);
                    position.Ply = 0;
                    ++i;
                }
            }
        }

        /*public static Task GetUserInput()
        {
            return Task.Factory.StartNew(() =>
            {
                Console.In.Close();
                INPUT_COMMAND = Console.ReadLine();
            });
        }

        public static async void AwaitUserInputAsync()
        {
            do
            {
                INPUT_COMMAND = null;
                await GetUserInput();

                Search.STOP_SEARCHING = true;

                if (string.Compare(INPUT_COMMAND, 0, "quit", 0, 4) == 0)
                {
                    Search.QUIT = true;
                }

            } while (!Search.QUIT);
        }*/

        public static void UCILoop()
        {
            //string inputBuffer;

            Console.WriteLine("id name Walt");
            Console.WriteLine("id author Josh Dotson");
            Console.WriteLine("uciok");

            Chessboard position = new Chessboard();
            SearchInfo info = new SearchInfo();
            HashTable.InitializeHashTable(ref position.PrincipleVariationTable);

            //we will only break out of this loop when the quit command is sent.
            do
            {
                Console.In.Close();
                INPUT_COMMAND = Console.ReadLine();

                if (INPUT_COMMAND == null)
                {
                    Task.Delay(25);
                    continue;
                }

                if (INPUT_COMMAND == "\n")
                {
                    continue;
                }

                if (String.Compare(INPUT_COMMAND, 0, "isready", 0, 7) == 0)
                {
                    Console.WriteLine("readyok\n");
                    continue;
                }
                else if (String.Compare(INPUT_COMMAND, 0, "position", 0, 8) == 0)
                {
                    ParsePosition(INPUT_COMMAND, ref position);
                }
                else if (String.Compare(INPUT_COMMAND, 0, "ucinewgame", 0, 10) == 0)
                {
                    ParsePosition(START_POSITION_COMMAND, ref position);
                }
                else if (String.Compare(INPUT_COMMAND, 0, "go", 0, 2) == 0)
                {
                    Task.Run(() => ParseGoCommand(INPUT_COMMAND, info, position));
                }
                else if (String.Compare(INPUT_COMMAND, 0, "stop", 0, 4) == 0)
                {
                    info.stopped = true;
                }
                else if (String.Compare(INPUT_COMMAND, 0, "quit", 0, 4) == 0)
                {
                    info.quit = true;
                    break;
                }
                else if (String.Compare(INPUT_COMMAND, 0, "uci", 0, 3) == 0)
                {
                    Console.WriteLine("id name Walt");
                    Console.WriteLine("id author Josh Dotson");
                    Console.WriteLine("uciok");
                }
                //we can take this thing out later
                if (info.quit)
                {
                    break;
                }

            } while (!Search.QUIT);

            return;
        }
    }
}