using System;
using System.Diagnostics;

namespace Engine
{
    public struct HashTable
    {
        public static int GetPvLine(int depth, ref Chessboard position) 
        {
            int move = ProbeHashTable(ref position);
            int count = 0;
	
	        while(move != Global.NO_MOVE && count < depth)
            {
		        if (MoveGen.MoveExists(ref position, move) == (int)Conditional.TRUE)
                {
			        MakeMove.Move(ref position, move);
                    position.PrincipleVariationArray[count++] = move;
		        }
                else
                {
			        break;
		        }
                move = ProbeHashTable(ref position);	
	        }
	
	        while(position.Ply > 0)
            {
		        MakeMove.TakeBackMove(ref position);
	        }
	
	        return count;	
        }
        
        //in the future try to send Hashtable size as an argument and not a ocnst value
        public static void InitializeHashTable(ref PVTable hashtable)
        {
            unsafe
            {
                //this will do integer division which will cut off the decimal
                hashtable.NumberOfEntries = Global.HashTableSize / sizeof(PVEntry);
                hashtable.NumberOfEntries -= 2;
                hashtable.PrincipleTable = null;

                hashtable.PrincipleTable = new PVEntry[hashtable.NumberOfEntries];
            }
             //get rid of this it is a needless check to make sure your not outside the bounds of our 2MB

            ClearHashTable(ref hashtable);
        }

        public static void ClearHashTable(ref PVTable hashtable)
        {
            for (int i = 0; i < hashtable.NumberOfEntries; ++i)
            {
                hashtable.PrincipleTable[i].Positionalkey = 0;
                hashtable.PrincipleTable[i].move = Global.NO_MOVE;
            }
        }

        public static void StoreHashTableMove(ref Chessboard position, int move)
        {
            //you mod positional key here because that way index will never be larger than the number of entries
            long index = (position.PositionalKey % position.PrincipleVariationTable.NumberOfEntries);

            if (index < 0)
            {
                //we want the absolute value of index
                index = -index;
            }

            position.PrincipleVariationTable.PrincipleTable[index].move = move;
            position.PrincipleVariationTable.PrincipleTable[index].Positionalkey = position.PositionalKey;
        }

        //the problem with below is that mod on two different positional keys may give the same index
        public static int ProbeHashTable(ref Chessboard position)
        {
            //you mod positional key here because that way index will never be larger than the number of entries
            long index = (position.PositionalKey % position.PrincipleVariationTable.NumberOfEntries);

            if (index < 0)
            {
                //we want the absolute value of index
                index = -index;
            }

            if ((position.PrincipleVariationTable.PrincipleTable[index]).Positionalkey == position.PositionalKey)
            {
                return position.PrincipleVariationTable.PrincipleTable[index].move;
            }

            return Global.NO_MOVE;
        }
    }
}