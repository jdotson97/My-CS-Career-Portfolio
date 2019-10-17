﻿using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    //THESE DAMN ENUMS DO MAKE THE CODE MORE CLEAR BUT ALL THE CASTING MIGHT SLOW THIS BITCH DOWN
    //MAY HAVE TO CONSIDER THAT FOR THE FUTURE
    public enum Coordinate : int
    {
        A1 = 21, B1, C1, D1, E1, F1, G1, H1,
        A2 = 31, B2, C2, D2, E2, F2, G2, H2,
        A3 = 41, B3, C3, D3, E3, F3, G3, H3,
        A4 = 51, B4, C4, D4, E4, F4, G4, H4,
        A5 = 61, B5, C5, D5, E5, F5, G5, H5,
        A6 = 71, B6, C6, D6, E6, F6, G6, H6,
        A7 = 81, B7, C7, D7, E7, F7, G7, H7,
        A8 = 91, B8, C8, D8, E8, F8, G8, H8, NO_SQ, OFFBOARD
    };

    public enum File : int { FILE_A, FILE_B, FILE_C, FILE_D, FILE_E, FILE_F, FILE_G, FILE_H, FILE_NONE };
    public enum Rank : int { RANK_1, RANK_2, RANK_3, RANK_4, RANK_5, RANK_6, RANK_7, RANK_8, RANK_NONE };

    public enum PieceID : int
    {
        EMPTY,

        PAWN_W, KNIGHT_W, BISHOP_W, ROOK_W, QUEEN_W, KING_W,
        PAWN_B, KNIGHT_B, BISHOP_B, ROOK_B, QUEEN_B, KING_B
    };

    public enum Color : int
    {
        WHITE, BLACK, BOTH
    };

    //the idea behind this is you may be able to store all of the castling permissions in a single bit string  0 0 0 0
    public enum Castling : int
    {
        WKING_CASTLE = 1, WQUEEN_CASTLE = 2, BKING_CASTLE = 4, BQUEEN_CASTLE = 8
    };

    public enum Conditional
    {
        TRUE, FALSE
    };
}