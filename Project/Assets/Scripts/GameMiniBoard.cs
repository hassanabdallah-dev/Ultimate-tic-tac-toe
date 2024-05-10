using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameMiniBoard
{
    public int boardNumber;
    public GameMiniBoard[] nineBoard = new GameMiniBoard[9];
    public int previousBoard = -1;

    // Buttons used in unity UI
    public Text[] buttonList;
    int count = 0;

    System.Random r = new System.Random();
    // Current decision node
    public StateNode currentNode;


    private bool gameRunning = true;

    public GameMiniBoard(Text[] buttonList, int boardNumber, GameMiniBoard[] nineBoard)
    {
        allGameRunning = true;

        this.boardNumber = boardNumber;
        this.buttonList = buttonList;
        this.nineBoard = nineBoard;
        SetGameControllerReferenceOnButtons();

        currentNode = new StateNode();
        currentNode.gs = new GameState();
        currentNode.gs.state = new char[3, 3];
        currentNode.player = 'o';
        currentNode.alpha = int.MinValue;
        currentNode.betta = int.MaxValue;
        currentNode.depth = 0;

        // permute over all possible game states with alpha and betta pruning.
        GenerateStates(currentNode, 'o');

        Debug.Log(count + " Node generated with alpha betta prunning" +
            "!");
        // free memory
        GC.Collect();
    }

    public void firstMove()
    {
        //Tuple<int, int> firstAction = new Tuple<int, int>(0, 4);
        FillWithO(5);
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
            buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }


    private bool IsTerminalState(GameState gs)
    {
        return IsWinningState('x', gs) == true || IsWinningState('o', gs) == true || IsDraw(gs) == true;
    }

    void free(StateNode root)
    {
        count--;
        if (root.children.Count == 0)
        {
            root = null;
            return;
        }

        foreach (var node in root.children)
            free(node);

        root.children.Clear();
        root.children.TrimExcess();
        root.children = null;
        root = null;
    }

    // Main AI function that generates possible tic-tac-toe game outcomes (with alpha betta prunning) and stores it in ram for the AI to use when playing.
    public void GenerateStates(StateNode root, char startingPlayer)
    {

        count++;
        // start when X moves first (X is the human player and o is the computer)
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (root.gs.state[i, j] == 0 && IsTerminalState(root.gs) == false)
                {
                    var copiedGameState = root.gs.Copy();
                    copiedGameState.state[i, j] = startingPlayer;

                    var newNode = new StateNode();
                    newNode.gs = copiedGameState;
                    newNode.parent = root;
                    newNode.alpha = root.alpha;
                    newNode.betta = root.betta;
                    newNode.depth = root.depth + 1;
                    root.children.Add(newNode);

                    newNode.action = Tuple.Create(i, j);

                    if (startingPlayer == 'x')
                    {
                        newNode.player = 'o';
                        GenerateStates(newNode, 'o');
                    }
                    else if (startingPlayer == 'o')
                    {
                        newNode.player = 'x';
                        GenerateStates(newNode, 'x');
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        // If we reach a terminal node then it's minimax value is simply the outcome of the game since there are no more moves to play
        if (root.children.Count == 0)
        {
            root.minimaxValue = Utility(root);
            if (root.player == 'x')
            {
                root.alpha = root.minimaxValue;

                root.parent.betta = root.alpha;
            }
            else
            {
                root.betta = root.minimaxValue;

                root.parent.alpha = root.betta;
            }
        }
        else
        {
            int i = 0;
            if (root.player == 'x')
            {

                // the max player
                var max = int.MinValue;

                for (i = 0; i < root.children.Count; i++)
                {
                    StateNode child = root.children[i];
                    max = Math.Max(max, child.minimaxValue);

                    if (root.parent != null)
                        root.parent.betta = Math.Min(root.alpha, root.parent.betta);


                    root.alpha = Math.Max(max, root.alpha);

                    if (max >= root.betta)
                    {
                        int t = 0;
                        for (int g = i + 1; g < root.children.Count; g++)
                            if (root.children[g].minimaxValue > max)
                            {
                                t = 1;
                                break;
                            }

                        if (t == 0)
                            break;
                    }

                }



                if (i < root.children.Count - 1)
                    for (int j = i + 1; j < root.children.Count; j++)
                    {
                        free(root.children[j]);
                        root.children.Remove(root.children[j]);
                        j--;

                    }

                root.minimaxValue = max;
            }
            else
            {
                // min player
                var min = int.MaxValue;

                for (i = 0; i < root.children.Count; i++)
                {
                    StateNode child = root.children[i];
                    min = Math.Min(min, child.minimaxValue);

                    if (root.parent != null)
                        root.parent.alpha = Math.Max(root.betta, root.parent.alpha);



                    root.betta = Math.Min(root.betta, min);

                    if (min <= root.alpha)
                    {
                        int t = 0;
                        for (int g = i + 1; g < root.children.Count; g++)
                            if (root.children[g].minimaxValue < min)
                            {
                                t = 1;
                                break;
                            }

                        if (t == 0)
                            break;
                    }
                }



                if (i < root.children.Count - 1)
                    for (int j = i + 1; j < root.children.Count; j++)
                    {
                        free(root.children[j]);
                        root.children.Remove(root.children[j]);
                        j--;

                    }

                root.minimaxValue = min;
            }


        }

    }
    static int firstTurn=0;

    public void AIMove()
    {
        int row = (boardNumber - 1) / 3;
        int col = (boardNumber - 1) % 3;
        List<StateNode> listOfMin = new List<StateNode>();
        List<int> dangerNode;
        if (gameRunning == true)
        {
            // choose the min minimax value
            StateNode minNode = null;
            var min = int.MaxValue;

            if (isEmptyBoard(currentNode.gs) == false)
            {
                dangerNode = testIfHeWillWinAdvance(GameController.mainBoard.currentNode.gs);
            foreach (var n in currentNode.children)
            {
                    
                        GameState copiedGameState5 = currentNode.gs.Copy();

                        copiedGameState5.state[n.action.Item1, n.action.Item2] = 'o';


                        GameState copiedGameState7 = GameController.mainBoard.currentNode.gs.Copy();

                    

                    copiedGameState7.state[row, col] = 'o';

                        if (testIfWinInTheBoard(copiedGameState5, 'o') == true && testIfWinInTheBoard(copiedGameState7, 'o'))
                        {
                            min = n.minimaxValue;
                            minNode = n;
                            break;
                        }


                    

                    if (n.minimaxValue <= min && nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].gameRunning == true && (IndexToButtonNumber(n.action.Item1, n.action.Item2) == this.boardNumber || GameController.registery.Contains(nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].boardNumber) == false))
                {
                            if (IndexToButtonNumber(n.action.Item1, n.action.Item2) == this.boardNumber)
                            {
                                GameState copiedGameState1 = this.currentNode.gs.Copy();

                                copiedGameState1.state[n.action.Item1, n.action.Item2] = 'o';
                                if (testIfHeWillWin(copiedGameState1) == true) continue;
                            }
                        //if (previousBoard == -1 || previousBoard != nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].boardNumber)
                        //{

                        if (dangerNode.Contains(IndexToButtonNumber(n.action.Item1, n.action.Item2))) continue;//Danger !!!!

                        if (n.minimaxValue == min)
                        {
                            listOfMin.Add(n);
                        }
                        else
                        {
                            listOfMin.Clear();
                            min = n.minimaxValue;
                            minNode = n;
                        }
                    //}
                }
                else if (n.minimaxValue <= min && nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].gameRunning == true && GameController.registery.Contains(nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].boardNumber) == true)
                {
                    if (GameController.registery.Contains(this.boardNumber))
                    {
                        GameState copiedGameState2 = this.currentNode.gs.Copy();

                        copiedGameState2.state[n.action.Item1, n.action.Item2] = 'o';
                        if (testIfHeWillWin(copiedGameState2) == true) {

                            continue;
                        }
                        else
                        {
                            GameState copiedGameState4 = GameController.mainBoard.currentNode.gs.Copy();

                            copiedGameState4.state[n.action.Item1, n.action.Item2] = 'x';
                            if (testIfWinInTheBoard(copiedGameState4, 'x') == true) continue;

                                if (dangerNode.Contains(IndexToButtonNumber(n.action.Item1, n.action.Item2))) continue;//Danger !!!!
                                if (n.minimaxValue == min)
                                {
                                    listOfMin.Add(n);
                                }
                                else
                                {
                                    listOfMin.Clear();
                                    min = n.minimaxValue;
                                    minNode = n;
                                }
                            }
                    }
                }
            }

            /*if(min == int.MaxValue)
            {
                Debug.Log("we don't find any node then go it without previous check!");
                foreach (var n in currentNode.children)
                {

                    if (n.minimaxValue < min && nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].gameRunning == true && (IndexToButtonNumber(n.action.Item1, n.action.Item2) == this.boardNumber || GameController.registery.Contains(nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].boardNumber) == false))
                    {

                        if (IndexToButtonNumber(n.action.Item1, n.action.Item2) == this.boardNumber)
                        {
                            GameState copiedGameState1 = this.currentNode.gs.Copy();

                            copiedGameState1.state[n.action.Item1, n.action.Item2] = 'x';
                            if (testIfHeWillWin(copiedGameState1) == true) continue;
                        }

                        min = n.minimaxValue;
                            minNode = n;
                        
                    }
                }
            }*/

            if (min == int.MaxValue)
            {
                Debug.Log("we don't find any node then go it and check if i win!");
                foreach (var n in currentNode.children)
                {

                        if (n.minimaxValue <= min && nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].gameRunning == true)
                    {

                        if (GameController.registery.Contains(nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].boardNumber) == true)
                        {
                            GameState copiedGameState = GameController.mainBoard.currentNode.gs.Copy();

                            copiedGameState.state[n.action.Item1, n.action.Item2] = 'x';
                            if (testIfWinInTheBoard(copiedGameState, 'x') == true) continue;

                            copiedGameState = currentNode.gs.Copy();
                            copiedGameState.state[n.action.Item1, n.action.Item2] = 'o';
                            if (testIfWinInTheBoard(copiedGameState, 'o') == false) continue;
                        }
                            if (dangerNode.Contains(IndexToButtonNumber(n.action.Item1, n.action.Item2))) continue;//Danger !!!!
                            Debug.Log("Iwin and he win, the it is tie ! :)");

                        if (n.minimaxValue == min)
                        {
                            listOfMin.Add(n);
                        }
                        else
                        {
                            listOfMin.Clear();
                            min = n.minimaxValue;
                            minNode = n;
                        }

                    }
                }
            }

            if (min == int.MaxValue)
            {
                Debug.Log("we don't find any node !");
                foreach (var n in currentNode.children)
                {

                        if (n.minimaxValue <= min && nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].gameRunning == true)
                        {
                        if (GameController.registery.Contains(nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].boardNumber) == true)
                        {
                            GameState copiedGameState = GameController.mainBoard.currentNode.gs.Copy();

                            copiedGameState.state[n.action.Item1, n.action.Item2] = 'x';
                            if (testIfWinInTheBoard(copiedGameState, 'x') == true) continue;
                        }
                        GameController.registery.Remove(nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].boardNumber);

                            if (dangerNode.Contains(IndexToButtonNumber(n.action.Item1, n.action.Item2))) continue;//Danger !!!!

                            Debug.Log("He will be not a win, then go it!");

                            if (n.minimaxValue == min)
                            {
                                listOfMin.Add(n);
                            }
                            else
                            {
                                listOfMin.Clear();
                                min = n.minimaxValue;
                                minNode = n;
                            }
                        }
                }
            }

            if (min == int.MaxValue)
            {
                Debug.Log("we don't find any node again!");
                foreach (var n in currentNode.children)
                {

                    if (n.minimaxValue <= min /*&& nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].gameRunning == true*/)
                    {

                            if (nineBoard[IndexToButtonNumber(n.action.Item1, n.action.Item2) - 1].gameRunning == false)
                            {
                                GameState copiedGameState5 = currentNode.gs.Copy();
                                int c = 0;

                                if (testIfHeWillWin(copiedGameState5)) c = 1;
                                
                                copiedGameState5.state[n.action.Item1, n.action.Item2] = 'o';



                                if ((testIfWinInTheBoard(copiedGameState5, 'o') == true && testIfHeWillWin(GameController.mainBoard.currentNode.gs) == false) || (c == 1 && testIfHeWillWin(copiedGameState5) == false)) ;
                                else continue;

 
                            }

                            if (dangerNode.Contains(IndexToButtonNumber(n.action.Item1, n.action.Item2))) continue;//Danger !!!!

                            if (n.minimaxValue == min)
                            {
                                listOfMin.Add(n);
                            }
                            else
                            {
                                listOfMin.Clear();
                                min = n.minimaxValue;
                                minNode = n;
                            }
                        }
                }
            }

            if (min == int.MaxValue)
            {
                Debug.Log("we don't find any node again again !");
                foreach (var n in currentNode.children)
                {

                    if (n.minimaxValue <= min)
                    {
                            if (dangerNode.Contains(IndexToButtonNumber(n.action.Item1, n.action.Item2))) continue;//Danger !!!!
                            if (n.minimaxValue == min)
                            {
                                listOfMin.Add(n);
                            }
                            else
                            {
                                listOfMin.Clear();
                                min = n.minimaxValue;
                                minNode = n;
                            }
                        }
                }
            }
                if (min == int.MaxValue)
                {
                    Debug.Log("we don't find any node again again again!");
                    foreach (var n in currentNode.children)
                    {

                        if (n.minimaxValue <= min)
                        {
                            if (n.minimaxValue == min)
                            {
                                listOfMin.Add(n);
                            }
                            else
                            {
                                listOfMin.Clear();
                                min = n.minimaxValue;
                                minNode = n;
                            }
                        }
                    }
                }
            }
        else if (isEmptyBoard(currentNode.gs))
            {
            /* // min = currentNode.children[4].minimaxValue;
             StateNode minNodeTest = currentNode.children[r.Next(5)];
             int test = 0;
             while (nineBoard[IndexToButtonNumber(minNodeTest.action.Item1, minNodeTest.action.Item2) - 1].gameRunning == false)
             {
                 test++;
                 if (test == 5) break;
                 minNodeTest = currentNode.children[r.Next(5)];

             }

             if(test < 5)
             {
                 minNode = minNodeTest;
             }*/
            int minimumNumberOfNode = int.MaxValue;
            int boardN = -1;
            for (int p = 0; p < nineBoard.Length; p++)
            {
                if (minimumNumberOfNode > coutNumberOfX(nineBoard[p].currentNode.gs) && nineBoard[p].gameRunning == true)
                {
                    minimumNumberOfNode = coutNumberOfX(nineBoard[p].currentNode.gs);
                    boardN = p;
                }
            }

            if (minimumNumberOfNode == int.MaxValue)
            {
                for (int p = 0; p < nineBoard.Length; p++)
                {
                    if (minimumNumberOfNode > coutNumberOfX(nineBoard[p].currentNode.gs))
                    {
                        minimumNumberOfNode = coutNumberOfX(nineBoard[p].currentNode.gs);
                        boardN = p;
                    }
                }
            }

            if (boardN != -1)
                minNode = currentNode.children[boardN];


        }

            if(listOfMin.Count > 0)
            {
                int maxWinPoss = evaluationFunction(minNode.gs);

                foreach( StateNode z in listOfMin)
                {
                    int temp5 = evaluationFunction(z.gs);
                    if (maxWinPoss < temp5)
                    {
                        maxWinPoss = temp5;
                        minNode = z;
                    }
                }
            }

            if (minNode != null)
            {
                // perform the ai move now
                
                FillWithO(IndexToButtonNumber(minNode.action.Item1, minNode.action.Item2));
            }
            else
            {
                return;
            }


        }
        else
        {
            GameController.mainBoard.currentNode.player = 'o';
            GameController.mainBoard.currentNode.alpha = int.MinValue;
            GameController.mainBoard.currentNode.betta = int.MaxValue;
            GameController.mainBoard.currentNode.depth = 0;
            GameController.mainBoard.currentNode.children.Clear();

            GameController.mainBoard.GenerateStates(GameController.mainBoard.currentNode, 'o');

            int move=GameController.mainBoard.getMove();

            if (move < 9 && move >= 0)
            {
                nineBoard[move].previousBoard = this.previousBoard;


                nineBoard[move].currentNode.player = 'o';
                nineBoard[move].currentNode.alpha = int.MinValue;
                nineBoard[move].currentNode.betta = int.MaxValue;
                nineBoard[move].currentNode.depth = 0;
                nineBoard[move].currentNode.children.Clear();

                nineBoard[move].GenerateStates(nineBoard[move].currentNode, 'o');

                nineBoard[move].AIMove();
            }
            else
            {
                Debug.Log("Move out of range!!! "+move  );
            }

            /*GameController.mainBoard.currentNode.player = 'o';
            GameController.mainBoard.currentNode.alpha = int.MinValue;
            GameController.mainBoard.currentNode.betta = int.MaxValue;
            GameController.mainBoard.currentNode.depth = 0;
            GameController.mainBoard.currentNode.children.Clear();

            GameController.mainBoard.GenerateStates(GameController.mainBoard.currentNode, 'o');

            int max = int.MaxValue,temp,index=-1;
            int l = 0;
            StateNode node;
            for(int i=0;i<nineBoard.Length;i++)
            {
                if (nineBoard[i].gameRunning == true)
                {
                    temp = evaluationFunction(nineBoard[i].currentNode.gs);
                    if (max > temp)
                    {
                        node = nineBoard[i].currentNode;
                        index = i;
                        max = temp;
                    }
                    if(max == temp)
                    {
                        
                        if(i==4)
                        {
                            l = 1;
                            node = nineBoard[i].currentNode;
                            index = i;
                            max = temp;
                        }
                        if(l!=1 && isEmptyBoard(nineBoard[index].currentNode.gs) && !isEmptyBoard(nineBoard[i].currentNode.gs))
                        {
                            node = nineBoard[i].currentNode;
                            index = i;
                            max = temp;
                        }
                    }
                }
            }

            if (index != -1)
            {
                nineBoard[index].currentNode.player = 'o';
                nineBoard[index].currentNode.alpha = int.MinValue;
                nineBoard[index].currentNode.betta = int.MaxValue;
                nineBoard[index].currentNode.depth = 0;
                nineBoard[index].currentNode.children.Clear();
                nineBoard[index].previousBoard = this.previousBoard;


                nineBoard[index].GenerateStates(nineBoard[index].currentNode, 'o');

                nineBoard[index].AIMove();
            }
            else
            {
                Debug.Log("La jeux est termine !");
            }
            */

        }
    }

    public int evaluationFunction(GameState root)
    {
        int nbrLineForComputerx = 0;
        int nbrLineForComputero = 0;

        nbrLineForComputerx = checkNbrOfLine(root, 'x');
        nbrLineForComputero = checkNbrOfLine(root, 'o');




        return nbrLineForComputero- nbrLineForComputerx;
    }

    public int checkNbrOfLine(GameState game, char player)
    {
        int t = 0;
        int nbrLine = 0;

        for (var i = 0; i < 3; i++)
        {
            var horizontalCount = 0;
           // t = 0;
            for (var j = 0; j < 3; j++)
            {
                if (game.state[i, j] == 0 || game.state[i, j] == player)
                {
                    /*if (game.state[i, j] ==0 && horizontalCount ==2)
                    {
                        t = 1;
                    }*/
                    horizontalCount++;
                }
            }

            if (horizontalCount == 3)
            {
                /*if (t == 1)
                    nbrLine++;*/
                nbrLine++;
            }
        }
        // check veritcally
        for (var i = 0; i < 3; i++)
        {
            var verticalCount = 0;
            //t = 0;
            for (var j = 0; j < 3; j++)
            {
                if (game.state[j, i] == 0 || game.state[j, i] == player)
                {

                    /*if (game.state[i, j] == 0 && verticalCount == 2)
                    {
                        t = 1;
                    }*/


                    verticalCount++;
                }
            }

            if (verticalCount == 3)
            {
                /*if (t == 1)
                    nbrLine++;*/

                    nbrLine++;
            }
        }

        // check both diagonals

        if ((game.state[0, 0] == player || game.state[0, 0] == 0) && (game.state[1, 1] == player || game.state[1, 1] == 0) && (game.state[2, 2] == player || game.state[2, 2] == 0))
        {
            nbrLine++;
        }
        if ((game.state[2, 0] == player || game.state[2, 0] == 0) && (game.state[1, 1] == player || game.state[1, 1] == 0) && (game.state[0, 2] == player || game.state[0, 2] == 0))
        {
            nbrLine++;
        }


        return nbrLine;
    }

    public Boolean isEmptyBoard(GameState game)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (game.state[i,j] != 0) return false;

        return true;
    }    

    /// This function returns how good a particular move is for the AI to make.
    private int Utility(StateNode node)
    {
        var result = 0;

        if (IsWinningState('x', node.gs) == true)
        {
            result = 1000 - node.depth * 100;
            //result = 1;
        }
        else if (IsWinningState('o', node.gs) == true)
        {
            result = -1000 + node.depth * 100;
            //result = -1;
        }
        else
        {
            result = 0;
        }

        return result;
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
            GameController.mainBoard.currentNode.player = 'o';
            GameController.mainBoard.currentNode.alpha = int.MinValue;
            GameController.mainBoard.currentNode.betta = int.MaxValue;
            GameController.mainBoard.currentNode.depth = 0;
            GameController.mainBoard.currentNode.children.Clear();

            GameController.mainBoard.GenerateStates(GameController.mainBoard.currentNode, 'o');

            GameController.mainBoard.FillWithO(this.boardNumber);

            return 1;
            
        }
        else if (IsWinningState('x', currentNode.gs) == true)
        {

            gameRunning = false;

            GameOver();

            GameController.mainBoard.currentNode.player = 'x';
            GameController.mainBoard.currentNode.alpha = int.MinValue;
            GameController.mainBoard.currentNode.betta = int.MaxValue;
            GameController.mainBoard.currentNode.depth = 0;
            GameController.mainBoard.currentNode.children.Clear();

            GameController.mainBoard.GenerateStates(GameController.mainBoard.currentNode, 'x');

            GameController.mainBoard.FillWithX(this.boardNumber);
            return 2;

        }
        else if (IsDraw(currentNode.gs) == true)
        {

            gameRunning = false;
            GameOver();

            GameController.mainBoard.currentNode.player = 't';
            GameController.mainBoard.currentNode.alpha = int.MinValue;
            GameController.mainBoard.currentNode.betta = int.MaxValue;
            GameController.mainBoard.currentNode.depth = 0;
            GameController.mainBoard.currentNode.children.Clear();

            //GameController.mainBoard.GenerateStates(GameController.mainBoard.currentNode, 't');

            GameController.mainBoard.FillWithT(this.boardNumber);
            return 3;

        }

        return 0;
    }

    //function that is used by players to make a move in the game and move down the decision tree.
    private void MoveThroughTree(Tuple<int, int> action)
    {
        foreach (var n in currentNode.children)
        {
            if (n.action.Item1 == action.Item1 && n.action.Item2 == action.Item2)
            {
                currentNode = n;
                return;
            }
        }

        Debug.Log("we don't find this state !!!!!!!!!!!!!!!!");
    }

    static Boolean allGameRunning = true;
    public void FillWithX(int buttonNumber)
    {
        if (gameRunning == true && allGameRunning == true)
        {
            

            var row = (buttonNumber - 1) / 3;
            var col = (buttonNumber - 1) % 3;

            MoveThroughTree(Tuple.Create(row, col));
            int number = IndexToButtonNumber(row, col);

            nineBoard[number-1].previousBoard = this.boardNumber;


            buttonList[buttonNumber - 1].text = "X";
            currentNode.gs.state[row, col] = 'x';


            buttonList[buttonNumber - 1].GetComponentInParent<Button>().interactable = false;

            int check =CheckState();

            if (boardNumber != 10 && allGameRunning == true)
            {

                for (int i = 0; i < nineBoard.Length; i++)
                {
                    //if (i == (buttonNumber - 1)) continue;
                    if (nineBoard[i].gameRunning == true)
                        nineBoard[i].enableAllButton();
                }
                if (check == 0 && testIfHeWillWin(currentNode.gs) == true && GameController.registery.Count < 5)
                {

                    GameController.registery.Add(boardNumber);
                }
                if (check != 0 && GameController.registery.Contains(boardNumber))
                {
                    GameController.registery.Remove(boardNumber);
                }
            }

            if (this.boardNumber == 10 && check != 0)
            {
                allGameRunning = false;
                for (int i = 0; i < nineBoard.Length; i++)
                {


                    nineBoard[i].disbleAllButton();
                }
            }
        }


        
    }
    
    public void FillWithT(int buttonNumber)
    {
        if (gameRunning == true && allGameRunning == true)
        {
            

            var row = (buttonNumber - 1) / 3;
            var col = (buttonNumber - 1) % 3;

            MoveThroughTree(Tuple.Create(row, col));
            int number = IndexToButtonNumber(row, col);

            nineBoard[number-1].previousBoard = this.boardNumber;


            buttonList[buttonNumber - 1].text = "T";
            currentNode.gs.state[row, col] = 't';


            buttonList[buttonNumber - 1].GetComponentInParent<Button>().interactable = false;

            int check =CheckState();

            if (this.boardNumber == 10 && check != 0)
            {
                allGameRunning = false;
                for (int i = 0; i < nineBoard.Length; i++)
                {


                    nineBoard[i].disbleAllButton();
                }
            }


        }
        
    }

    private void FillWithO(int buttonNumber)
    {
        if (gameRunning == true && allGameRunning == true)
        {
            var row = (buttonNumber - 1) / 3;
            var col = (buttonNumber - 1) % 3;
            MoveThroughTree(Tuple.Create(row, col));
            currentNode.gs.state[row, col] = 'o';

            buttonList[buttonNumber - 1].text = "O";
            buttonList[buttonNumber - 1].GetComponentInParent<Button>().interactable = false;

            int check = CheckState();

            if (boardNumber != 10 && allGameRunning == true)
            {
                if (nineBoard[buttonNumber - 1].gameRunning == true)
                {
                    for (int i = 0; i < nineBoard.Length; i++)
                    {
                        if (i == (buttonNumber - 1)) continue;

                        nineBoard[i].disbleAllButton();
                    }
                }
                else
                {
                    for (int i = 0; i < nineBoard.Length; i++)
                    {
                        if (i == (buttonNumber - 1) || nineBoard[i].gameRunning == false) continue;

                        nineBoard[i].enableAllButton();
                    }
                }

                if (check != 0 && GameController.registery.Contains(boardNumber))
                {
                    GameController.registery.Remove(boardNumber);
                }
                if (check == 0 && testIfHeWillWin(currentNode.gs) == false)
                {
                    GameController.registery.Remove(boardNumber);
                }
            }

            if (this.boardNumber == 10 && check != 0)
            {
                allGameRunning = false;
                for (int i = 0; i < nineBoard.Length; i++)
                {


                    nineBoard[i].disbleAllButton();
                }
            }

        }

        /*int gameOver = GameController.mainBoard.CheckState();

        if (gameOver == 1)
        {
            Debug.Log("La jeux est termine , tu as choue");
        }if (gameOver == 2)
        {
            Debug.Log("La jeux est termine , tu as gagner");
        }if (gameOver == 3)
        {
            Debug.Log("Personne a gagner");
        }*/
    }

    public void disbleAllButton()
    {
        for(int i=0;i<buttonList.Length;i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = false;
        }
    }
    
    public void enableAllButton()
    {
        for(int i=0;i<buttonList.Length;i++)
        {
            if(buttonList[i].text.Equals(""))
            buttonList[i].GetComponentInParent<Button>().interactable = true;
        }
    }

    public int getMove()
    {
        if (gameRunning == true)
        {
            // choose the min minimax value
            StateNode minNode = null;
            var min = int.MaxValue;

            foreach (var n in currentNode.children)
            {

                if (n.minimaxValue < min )
                {
                    min = n.minimaxValue;
                    minNode = n;
                }
            }

            if (minNode != null)
            {
                return IndexToButtonNumber(minNode.action.Item1, minNode.action.Item2) - 1;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return -1;
        }
    }

    public bool testIfHeWillWin(GameState game)
    {
        int y = 0,i1=0,i2=0;
        int nbrLine = 0;

        for (var i = 0; i < 3; i++)
        {
            i1 = 0;i2 = 0;
            for (var j = 0; j < 3; j++)
            {
                if (game.state[i, j] == 0 )
                {
                    i1++;
                }
                if (game.state[i, j] == 'x' )
                {
                    i2++;
                }
            }

            if (i2 == 2 && i1== 1)
            {
                return true;
            }
        }
        // check veritcally
        for (var i = 0; i < 3; i++)
        {
            i1 = 0; i2 = 0;
            for (var j = 0; j < 3; j++)
            {
                if (game.state[j, i] == 0)
                {
                    i1++;
                }
                if (game.state[j, i] == 'x')
                {
                    i2++;
                }
            }

            if (i2 == 2 && i1 == 1)
            {
                return true;
            }
        }
        // check both diagonals
        i1 = 0;i2 = 0;

        if (game.state[0, 0] == 'x') i2++;
        if (game.state[1, 1] == 'x') i2++;
        if (game.state[2, 2] == 'x') i2++;
        
        if (game.state[0, 0] == 0) i1++;
        if (game.state[1, 1] == 0) i1++;
        if (game.state[2, 2] == 0) i1++;

        if (i2 == 2 && i1 == 1)
        {
            return true;
        }
        
        
        
        
        i1 = 0;i2 = 0;

        if (game.state[2, 0] == 'x') i2++;
        if (game.state[1, 1] == 'x') i2++;
        if (game.state[0, 2] == 'x') i2++;
        
        if (game.state[2, 0] == 0) i1++;
        if (game.state[1, 1] == 0) i1++;
        if (game.state[0, 2] == 0) i1++;

        if (i2 == 2 && i1 == 1)
        {
            return true;
        }
        


        return false;
    }

    public bool testIfWinInTheBoard(GameState game,char player)
    {
        if (IsWinningState(player, game) == true)
        {
            return true;

        }
        else if (IsWinningState(player, game) == true)
        {

            return true;

        }
        else if (IsDraw(game) == true)
        {
            return false;

        }

        return false;
    }

    public int coutNumberOfX(GameState game)
    {
        var filledInSpots = 0;

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (game.state[i, j] == 'x')
                {
                    filledInSpots++;
                }
            }
        }

        return filledInSpots;
    }


    public List<int> testIfHeWillWinAdvance(GameState game)
    {
        int y = 0, i1 = 0, i2 = 0;
        int nbrLine = 0;
        int temp=-1;
        List<int> dangerNode = new List<int>();

        for (var i = 0; i < 3; i++)
        {
            i1 = 0; i2 = 0;
            temp = -1;
            for (var j = 0; j < 3; j++)
            {
                if (game.state[i, j] == 0)
                {
                    temp = IndexToButtonNumber(i,j);
                    i1++;
                }
                if (game.state[i, j] == 'x')
                {
                    i2++;
                }
            }

            if (i2 == 2 && i1 == 1)
            {
                dangerNode.Add(temp);
            }
        }
        // check veritcally
        for (var i = 0; i < 3; i++)
        {
            i1 = 0; i2 = 0;
            temp = -1;
            for (var j = 0; j < 3; j++)
            {
                if (game.state[j, i] == 0)
                {
                    temp = IndexToButtonNumber(j, i);
                    i1++;
                }
                if (game.state[j, i] == 'x')
                {
                    i2++;
                }
            }

            if (i2 == 2 && i1 == 1)
            {
                dangerNode.Add(temp);
            }
        }
        // check both diagonals
        i1 = 0; i2 = 0;
        temp = -1;

        if (game.state[0, 0] == 'x') i2++;
        if (game.state[1, 1] == 'x') i2++;
        if (game.state[2, 2] == 'x') i2++;

        if (game.state[0, 0] == 0)
        {
            i1++;
            temp = IndexToButtonNumber(0, 0);
        }
        if (game.state[1, 1] == 0)
        {
            i1++;
            temp = IndexToButtonNumber(1, 1);
        }

        if (game.state[2, 2] == 0) { 
            i1++;
            temp = IndexToButtonNumber(2, 2);
        }

        if (i2 == 2 && i1 == 1)
        {
            dangerNode.Add(temp);
        }




        i1 = 0; i2 = 0;
        temp = -1;

        if (game.state[2, 0] == 'x') i2++;
        if (game.state[1, 1] == 'x') i2++;
        if (game.state[0, 2] == 'x') i2++;

        if (game.state[2, 0] == 0) { 
            i1++;
            temp = IndexToButtonNumber(2, 0);
        }
        if (game.state[1, 1] == 0) { 
            i1++;
            temp = IndexToButtonNumber(1, 1);
        }
        if (game.state[0, 2] == 0) { 
            i1++;
            temp = IndexToButtonNumber(0, 2);
        }

        if (i2 == 2 && i1 == 1)
        {
            dangerNode.Add(temp);
        }


        for(int i=0; i< dangerNode.Count;i++)
        {
           int test = dangerNode[i];
            if (testIfHeWillWin(nineBoard[test-1].currentNode.gs) == false)
            {
                dangerNode.Remove(test);
                i--;
            }
        }


        return dangerNode;
    }
}
