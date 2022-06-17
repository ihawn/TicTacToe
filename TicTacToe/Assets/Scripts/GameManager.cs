using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public Player CurrentPlayer;
    public TicTacToe Game;
    public PlayerType Player1Type;
    public PlayerType Player2Type;
    public int aiRecursionDepth = 5;
    public bool gameOver;

    public GameObject x;
    public GameObject o;
    public static GameObject X;
    public static GameObject O;

    TakeTurnsOverloads TakeTurns; //object instance of overload container

    void Start()
    {
        TakeTurns = new TakeTurnsOverloads();
        X = x;
        O = o;
        Game = new TicTacToe(Player1Type, Player2Type);
        CurrentPlayer = Game.Player1;
    }

    void Update()
    {
        if(!gameOver)
            GetInput();
    }

    void GetInput()
    {
        switch(CurrentPlayer.PlayerType)
        {
            case PlayerType.Human:
                GetHumanInput();
                break;

            case PlayerType.AI:
                GetAiInput();
                break;
        }
    }

    void GetHumanInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                GameObject g = hit.transform.gameObject;
                if(!Game.Board.Pieces.Any(p => p.x == (int)g.transform.position.x && p.y == (int)g.transform.position.z))
                {
                    Piece p = new Piece()
                    {
                        x = (int)g.transform.position.x,
                        y = (int)g.transform.position.z,
                        isX = CurrentPlayer.isX
                    };
                    Game.Board.Pieces.Add(p);
                    SpawnAgent(g.transform.position);
                    SwitchPlayer();
                }
            }
        }
    }

    void GetAiInput()
    {
        var result = TakeTurns.GetBestMove(Game.Board, aiRecursionDepth, CurrentPlayer.isX);

        Piece p = new Piece()
        {
            x = result.Move.x,
            y = result.Move.y,
            isX = CurrentPlayer.isX
        };
        Game.Board.Pieces.Add(p);
        SpawnAgent(new Vector3(p.x, 0, p.y));
        SwitchPlayer();
    }

    void SpawnAgent(Vector3 pos)
    {
        Instantiate(CurrentPlayer.isX ? X : O, pos, Quaternion.identity);
    }

    void SwitchPlayer()
    {
        gameOver = TakeTurns.EndGameReached(Game.Board).Item1;
        CurrentPlayer = CurrentPlayer == Game.Player1 ? Game.Player2 : Game.Player1;
    }

    public static GameObject GetGamePiece(bool isX)
    {
        return isX ? X : O;
    }
}
