using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameMiniBoardPlayers
{
    public int boardNumber;
    public GameMiniBoardPlayers[] nineBoard = new GameMiniBoardPlayers[9];

    // Buttons used in unity UIf
    public Text[] buttonList;

    public StateNode currentNode;


    private bool gameRunning = true;

    public GameMiniBoardPlayers(Text[] buttonList, int boardNumber, GameMiniBoardPlayers[] nineBoard)
    {
        allGameRunning = true;

        this.boardNumber = boardNumber;
        this.buttonList = buttonList;
        this.nineBoard = nineBoard;
        SetGameControllerReferenceOnButtons();
        currentNode = new StateNode();
        currentNode.gs = new GameState();
        currentNode.gs.state = new char[3, 3];
        currentNode.player = 'x';
        currentNode.alpha = int.MinValue;
        currentNode.betta = int.MaxValue;
        currentNode.depth = 0;
        // free memory
        GC.Collect();
    }

    void GameOver()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = false;
        }
    }

    void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<GridSpacePlayers>().SetGameControllerReference(this);
        }
    }


    private bool IsTerminalState(GameState gs)
    {
        return IsWinningState('x', gs) == true || IsWinningState('o', gs) == true || IsDraw(gs) == true;
    }








    private int IndexToButtonNumber(int row, int col)
    {
        return row * 3 + col + 1;
    }

    /// Function that checks if a gamestate is game over because of a player winning or a tie
    private bool IsWinningState(char player, GameState gs)
    {
        // check horizontally
        for (var i = 0; i < 3; i++)
        {
            var horizontalCount = 0;
            for (var j = 0; j < 3; j++)
            {
                if (gs.state[i, j] == player)
                {
                    horizontalCount++;
                }
            }

            if (horizontalCount == 3)
            {
                return true;
            }
        }

        // check veritcally
        for (var i = 0; i < 3; i++)
        {
            var verticalCount = 0;

            for (var j = 0; j < 3; j++)
            {
                if (gs.state[j, i] == player)
                {
                    verticalCount++;
                }
            }

            if (verticalCount == 3)
            {
                return true;
            }
        }

        // check both diagonals

        if (gs.state[0, 0] == player && gs.state[1, 1] == player && gs.state[2, 2] == player)
        {
            return true;
        }
        else if (gs.state[2, 0] == player && gs.state[1, 1] == player && gs.state[0, 2] == player)
        {
            return true;
        }

        return false;
    }

    private bool IsDraw(GameState gs)
    {
        var filledInSpots = 0;

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (gs.state[i, j] != 0)
                {
                    filledInSpots++;
                }
            }
        }

        if (filledInSpots == 9 && IsWinningState('x', gs) == false && IsWinningState('o', gs) == false)
        {
            return true;
        }

        return false;
    }

    private int CheckState()
    {

        if (IsWinningState('o', currentNode.gs) == true)
        {

            gameRunning = false;
            GameOver();
            GameControllerPlayers.mainBoard.Fill(this.boardNumber);

            return 1;

        }
        else if (IsWinningState('x', currentNode.gs) == true)
        {

            gameRunning = false;

            GameOver();
            GameControllerPlayers.mainBoard.Fill(this.boardNumber);

            return 2;

        }
        else if (IsDraw(currentNode.gs) == true)
        {

            gameRunning = false;
            GameOver();

            GameControllerPlayers.mainBoard.FillWithT(this.boardNumber);

            return 3;

        }

        return 0;
    }


    static String player = "X";
    public void Fill(int buttonNumber)
    {
        if (gameRunning == true && allGameRunning == true)
        {


            var row = (buttonNumber - 1) / 3;
            var col = (buttonNumber - 1) % 3;

            int number = IndexToButtonNumber(row, col);




           

            if (this.boardNumber == 10)
            {
                
                if (player.Equals("X"))
                {
                    currentNode.gs.state[row, col] = 'o';

                    buttonList[buttonNumber - 1].text = "O";

                }
                else
                {
                    currentNode.gs.state[row, col] = 'x';

                    buttonList[buttonNumber - 1].text = "X";

                }
            }
            else
            {
                buttonList[buttonNumber - 1].text = player;

                if (player.Equals("X"))
                {
                    currentNode.gs.state[row, col] = 'x';

                   
                        player = "O";
                }
                else
                {
                    currentNode.gs.state[row, col] = 'o';

                    
                        player = "X";
                }
            }

            int s = CheckState();


            buttonList[buttonNumber - 1].GetComponentInParent<Button>().interactable = false;

            

            if (boardNumber != 10 && allGameRunning == true)
            {

                if (nineBoard[buttonNumber - 1].gameRunning == true)
                {
                    for (int i = 0; i < nineBoard.Length; i++)
                    {
                        if (i == (buttonNumber - 1))
                        {
                            nineBoard[i].enableAllButton();
                            continue;
                        }
                        nineBoard[i].disbleAllButton();
                    }

                }
                else
                {
                    for (int i = 0; i < nineBoard.Length; i++)
                    {
                        if (nineBoard[i].gameRunning == false) continue;

                        nineBoard[i].enableAllButton();
                    }
                }

            }

            if (this.boardNumber == 10 && s != 0)
            {
                allGameRunning = false;
                for (int i = 0; i < nineBoard.Length; i++)
                {
                    

                    nineBoard[i].disbleAllButton();
                }
            }
        }

    }


    static Boolean allGameRunning = true;

    public void FillWithT(int buttonNumber)
    {
        if (gameRunning == true && allGameRunning == true)
        {


            var row = (buttonNumber - 1) / 3;
            var col = (buttonNumber - 1) % 3;
            int number = IndexToButtonNumber(row, col);



            buttonList[buttonNumber - 1].text = "T";
            currentNode.gs.state[row, col] = 't';


            buttonList[buttonNumber - 1].GetComponentInParent<Button>().interactable = false;

            int s = CheckState();

            if (this.boardNumber == 10 && s != 0)
            {
                allGameRunning = false;
                for (int i = 0; i < nineBoard.Length; i++)
                {


                    nineBoard[i].disbleAllButton();
                }
            }
        }

    }


    public void disbleAllButton()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = false;
        }
    }

    public void enableAllButton()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            if (buttonList[i].text.Equals(""))
                buttonList[i].GetComponentInParent<Button>().interactable = true;
        }
    }










}
