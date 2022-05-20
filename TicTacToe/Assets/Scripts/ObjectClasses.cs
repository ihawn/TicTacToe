using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TicTacToe
{
    public TicTacToeBoard Board { get; set; }
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }

    public TicTacToe(PlayerType player1Type, PlayerType player2Type)
    {
        Board = new TicTacToeBoard();
        Player1 = new Player(true, player1Type);
        Player2 = new Player(false, player2Type);
    }
}

public class Player
{
    public bool isX { set; get; }
    public PlayerType PlayerType { get; set; }

    public Player(bool isx, PlayerType playerType)
    {
        isX = isx;
        PlayerType = playerType;
    }
}

public enum PlayerType
{
    Human = 0,
    AI = 1,
}
