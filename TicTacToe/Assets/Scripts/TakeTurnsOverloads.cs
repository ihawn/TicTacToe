using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TakeTurns;
using TakeTurns.Containers;
using TakeTurns.Enumerations;
using System.Linq;

public class TakeTurnsOverloads : TakeTurns<TicTacToeBoard, Piece, Move>
{
    // X is maximizing player
    // 8 possible 3-in-a-rows
    // This function is extremely game specific and will play a large part in determining how good your AI is
    public override float GetGameEvaluation(TicTacToeBoard space)
    {
        var endStateEvaluation = EndGameReached(space);
        if (endStateEvaluation.Item1) //It's recommended to put an infinite penalty for the other player winning
            return endStateEvaluation.Item2 == EndState.MaxPlayerWins ? 1000000 : (endStateEvaluation.Item2 == EndState.MinPlayerWins ? -1000000 : 0);
     
        float eval = 0;
        int xCount;
        int oCount;
        int emptyCount;

        //horizontal, vertical
        for(int i = 0; i < 3; i++)
        {
            xCount = space.Pieces.Count(p => p.x == i && p.isX);
            oCount = space.Pieces.Count(p => p.x == i && !p.isX);
            emptyCount = 3 - (xCount + oCount);

            xCount = xCount == 3 ? 100 : xCount;
            oCount = oCount == 3 ? 100 : oCount;
            xCount = oCount > 0 ? 0 : xCount * xCount;
            oCount = xCount > 0 ? 0 : oCount * oCount;
            eval += xCount - oCount;


            xCount = space.Pieces.Count(p => p.y == i && p.isX);
            oCount = space.Pieces.Count(p => p.y == i && !p.isX);

            xCount = xCount == 3 ? 100 : xCount;
            oCount = oCount == 3 ? 100 : oCount;
            xCount = oCount > 0 ? 0 : xCount * xCount;
            oCount = xCount > 0 ? 0 : oCount * oCount;
            eval += xCount - oCount;
        }


        //diagonal 1
        xCount = space.Pieces.Count(p => p.x == p.y && p.isX);
        oCount = space.Pieces.Count(p => p.x == p.y && !p.isX);

        xCount = xCount == 3 ? 100 : xCount;
        oCount = oCount == 3 ? 100 : oCount;
        xCount = oCount > 0 ? 0 : xCount * xCount;
        oCount = xCount > 0 ? 0 : oCount * oCount;

        eval += xCount - oCount;

        //diagonal 2
        xCount = space.Pieces.Count(p => p.x == 3 - p.y && p.isX);
        oCount = space.Pieces.Count(p => p.x == 3 - p.y && !p.isX);

        xCount = xCount == 3 ? 100 : xCount;
        oCount = oCount == 3 ? 100 : oCount;
        xCount = oCount > 0 ? 0 : xCount * xCount;
        oCount = xCount > 0 ? 0 : oCount * oCount;

        eval += xCount - oCount;

        return eval;
    }

    //Returns if the game is over and who won (or if there is a tie)
    public override (bool, EndState) EndGameReached(TicTacToeBoard space)
    {
        int xCount;
        int oCount;
        for (int i = 0; i < 3; i++)
        {
            //vertical
            xCount = space.Pieces.Count(p => p.x == i && p.isX);
            oCount = space.Pieces.Count(p => p.x == i && !p.isX);
            if (xCount == 3 || oCount == 3)
                return (true, xCount == 3 ? EndState.MaxPlayerWins : EndState.MinPlayerWins);

            //horizontal
            xCount = space.Pieces.Count(p => p.y == i && p.isX);
            oCount = space.Pieces.Count(p => p.y == i && !p.isX);
            if (xCount == 3 || oCount == 3)
                return (true, xCount == 3 ? EndState.MaxPlayerWins : EndState.MinPlayerWins);
        }

        //diagonal 1
        xCount = space.Pieces.Count(p => p.y == p.x && p.isX);
        oCount = space.Pieces.Count(p => p.y == p.x && !p.isX);
        if (xCount == 3 || oCount == 3)
            return (true, xCount == 3 ? EndState.MaxPlayerWins : EndState.MinPlayerWins);

        //diagonal 2
        xCount = space.Pieces.Count(p => p.y == 2 - p.x && p.isX);
        oCount = space.Pieces.Count(p => p.y == 2 - p.x && !p.isX);
        if (xCount == 3 || oCount == 3)
            return (true, xCount == 3 ? EndState.MaxPlayerWins : EndState.MinPlayerWins);

        return (space.Pieces.Count() == 9, EndState.Tie);
    }

    public override IList<MinimaxInput<TicTacToeBoard, Piece, Move, float>> GetPositions(TicTacToeBoard space, bool isMaxPlayer)
    {
        IList<MinimaxInput<TicTacToeBoard, Piece, Move, float>> positionList = new List<MinimaxInput<TicTacToeBoard, Piece, Move, float>>();

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                if(!space.Pieces.Select(i => (i.x, i.y)).Contains((i, j))) // empty space
                {
                    Piece p = new Piece()
                    {
                        x = i,
                        y = j,
                        isX = isMaxPlayer
                    };
                    TicTacToeBoard copyBoard = new TicTacToeBoard(space);
                    copyBoard.Pieces.Add(p);

                    positionList.Add(new MinimaxInput<TicTacToeBoard, Piece, Move, float>(copyBoard, p, new Move { x = i, y = j }));
                }
            }
        }

        positionList.Shuffle();
        return positionList;
    }
}

public class TicTacToeBoard
{
    public List<Piece> Pieces { get; set; }

    public TicTacToeBoard()
    {
        Pieces = new List<Piece>();
    }

    // deep copy constructor
    public TicTacToeBoard(TicTacToeBoard board)
    {
        Pieces = new List<Piece>();
        foreach(Piece piece in board.Pieces)
        {
            Piece p = new Piece()
            {
                x = piece.x,
                y = piece.y,
                isX = piece.isX
            };
            Pieces.Add(p);
        }
    }
}

public class Piece
{
    public int x { get; set; }
    public int y { get; set; }
    public bool isX { get; set; }

    public Piece()
    {
        x = -1;
        y = -1;
    }
}

public class Move
{
    public int x { get; set; }
    public int y { get; set; }
    public Move()
    {
        x = -1;
        y = -1;
    }
}

//not needed but recommended to shuffle the move lists (prevents the same move from being made for the same situation)
//from: https://stackoverflow.com/questions/273313/randomize-a-listt
static class ShuffleExtension
{
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}