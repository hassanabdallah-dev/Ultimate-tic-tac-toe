using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class GridSpace : MonoBehaviour
{

    public Button button;
    public Text buttonText;

    public GameMiniBoard gameController;

    public void SetGameControllerReference(GameMiniBoard controller)
    {
        gameController = controller;
    }

    public void initializeTree(GameMiniBoard game)
    {
        game.currentNode.player = 'o';
        game.currentNode.alpha = int.MinValue;
        game.currentNode.betta = int.MaxValue;
        game.currentNode.depth = 0;
        game.currentNode.children.Clear();

    }
    
    public void initializeTreeX(GameMiniBoard game)
    {
        game.currentNode.player = 'x';
        game.currentNode.alpha = int.MinValue;
        game.currentNode.betta = int.MaxValue;
        game.currentNode.depth = 0;
        game.currentNode.children.Clear();

    }

    public void button1_Click()
    {

        

        initializeTreeX(gameController);


        gameController.GenerateStates(gameController.currentNode, 'x');

        gameController.FillWithX(1);

        initializeTree(gameController.nineBoard[0]);

        gameController.nineBoard[0].GenerateStates(gameController.nineBoard[0].currentNode, 'o');

        gameController.nineBoard[0].AIMove();
    }

    public void button2_Click()
    {
        initializeTreeX(gameController);

        gameController.GenerateStates(gameController.currentNode, 'x');

        gameController.FillWithX(2);

        initializeTree(gameController.nineBoard[1]);

        gameController.nineBoard[1].GenerateStates(gameController.nineBoard[1].currentNode, 'o');

        gameController.nineBoard[1].AIMove();
    }

    public void button3_Click()
    {
        initializeTreeX(gameController);
        gameController.GenerateStates(gameController.currentNode, 'x');
        gameController.FillWithX(3);
        initializeTree(gameController.nineBoard[2]);

        gameController.nineBoard[2].GenerateStates(gameController.nineBoard[2].currentNode, 'o');

        gameController.nineBoard[2].AIMove();
    }

    public void button4_Click()
    {
        initializeTreeX(gameController);
        gameController.GenerateStates(gameController.currentNode, 'x');
        gameController.FillWithX(4);
        initializeTree(gameController.nineBoard[3]);

        gameController.nineBoard[3].GenerateStates(gameController.nineBoard[3].currentNode, 'o');

        gameController.nineBoard[3].AIMove();
    }

    public void button5_Click()
    {
        initializeTreeX(gameController);
        gameController.GenerateStates(gameController.currentNode, 'x');
        gameController.FillWithX(5);
        initializeTree(gameController.nineBoard[4]);

        gameController.nineBoard[4].GenerateStates(gameController.nineBoard[4].currentNode, 'o');

        gameController.nineBoard[4].AIMove();
    }

    public void button6_Click()
    {
        initializeTreeX(gameController);
        gameController.GenerateStates(gameController.currentNode, 'x');
        gameController.FillWithX(6);
        initializeTree(gameController.nineBoard[5]);

        gameController.nineBoard[5].GenerateStates(gameController.nineBoard[5].currentNode, 'o');

        gameController.nineBoard[5].AIMove();
    }

    public void button7_Click()
    {
        initializeTreeX(gameController);
        gameController.GenerateStates(gameController.currentNode, 'x');
        gameController.FillWithX(7);
        initializeTree(gameController.nineBoard[6]);

        gameController.nineBoard[6].GenerateStates(gameController.nineBoard[6].currentNode, 'o');

        gameController.nineBoard[6].AIMove();
    }

    public void button8_Click()
    {
        initializeTreeX(gameController);
        gameController.GenerateStates(gameController.currentNode, 'x');
        gameController.FillWithX(8);
        initializeTree(gameController.nineBoard[7]);

        gameController.nineBoard[7].GenerateStates(gameController.nineBoard[7].currentNode, 'o');

        gameController.nineBoard[7].AIMove();
    }

    public void button9_Click()
    {
        initializeTreeX(gameController);
        gameController.GenerateStates(gameController.currentNode, 'x');
        gameController.FillWithX(9);
        initializeTree(gameController.nineBoard[8]);

        gameController.nineBoard[8].GenerateStates(gameController.nineBoard[8].currentNode, 'o');

        gameController.nineBoard[8].AIMove();
    }
}