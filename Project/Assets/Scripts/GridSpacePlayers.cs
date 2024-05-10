using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

public class GridSpacePlayers : MonoBehaviour
{

    public Button button;
    public Text buttonText;

    public GameMiniBoardPlayers gameController;

    public void SetGameControllerReference(GameMiniBoardPlayers controller)
    {
        gameController = controller;
    }


    public void button1_Click()
    {

        gameController.Fill(1);
    }

    public void button2_Click()
    {

        gameController.Fill(2);

    }

    public void button3_Click()
    {
        gameController.Fill(3);
    }

    public void button4_Click()
    {
        gameController.Fill(4);
    }

    public void button5_Click()
    {
        gameController.Fill(5);
    }

    public void button6_Click()
    {
        gameController.Fill(6);
    }

    public void button7_Click()
    {
        gameController.Fill(7);
    }

    public void button8_Click()
    {
        gameController.Fill(8);
    }

    public void button9_Click()
    {
        gameController.Fill(9);
    }
}