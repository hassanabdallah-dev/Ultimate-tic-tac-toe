using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameControllerPlayers : MonoBehaviour
{

    GameMiniBoardPlayers[] nineBoard;

    public static GameMiniBoardPlayers mainBoard;

    // Buttons used in unity UI
    public Text[] buttonList;
    public Text[] buttonList1;
    public Text[] buttonList2;
    public Text[] buttonList3;
    public Text[] buttonList4;
    public Text[] buttonList5;
    public Text[] buttonList6;
    public Text[] buttonList7;
    public Text[] buttonList8;
    public Text[] buttonMainList;


    public void Back()
    {
        StartCoroutine(loadAsynchronously());
    }

    IEnumerator loadAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);


        while (!operation.isDone)
        {
            //Debug.Log(""+operation.progress);
            float progress = Mathf.Clamp01(operation.progress / .9f);

            yield return null;
        }
    }


    void Awake()
    {

        nineBoard = new GameMiniBoardPlayers[9];
        Debug.Log("Game started !");

        nineBoard[0] = new GameMiniBoardPlayers(buttonList, 1, nineBoard);
        nineBoard[1] = new GameMiniBoardPlayers(buttonList1, 2, nineBoard);
        nineBoard[2] = new GameMiniBoardPlayers(buttonList2, 3, nineBoard);
        nineBoard[3] = new GameMiniBoardPlayers(buttonList3, 4, nineBoard);
        nineBoard[4] = new GameMiniBoardPlayers(buttonList4, 5, nineBoard);
        nineBoard[5] = new GameMiniBoardPlayers(buttonList5, 6, nineBoard);
        nineBoard[6] = new GameMiniBoardPlayers(buttonList6, 7, nineBoard);
        nineBoard[7] = new GameMiniBoardPlayers(buttonList7, 8, nineBoard);
        nineBoard[8] = new GameMiniBoardPlayers(buttonList8, 9, nineBoard);

        mainBoard = new GameMiniBoardPlayers(buttonMainList, 10, nineBoard);

        for (int i = 0; i < buttonMainList.Length; i++)
        {
            buttonMainList[i].GetComponentInParent<Button>().enabled = false;
        }
    }

}