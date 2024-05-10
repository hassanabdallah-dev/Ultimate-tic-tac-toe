using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    GameMiniBoard[] nineBoard;

    public static List<int> registery;

    public static GameMiniBoard mainBoard;
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
        
        nineBoard = new GameMiniBoard[9];
        Debug.Log("Game started !");

            nineBoard[0] = new GameMiniBoard(buttonList,1,  nineBoard);
            nineBoard[1] = new GameMiniBoard(buttonList1,2, nineBoard);
            nineBoard[2] = new GameMiniBoard(buttonList2,3, nineBoard);
            nineBoard[3] = new GameMiniBoard(buttonList3,4, nineBoard);
            nineBoard[4] = new GameMiniBoard(buttonList4,5, nineBoard);
            nineBoard[5] = new GameMiniBoard(buttonList5,6, nineBoard);
            nineBoard[6] = new GameMiniBoard(buttonList6,7, nineBoard);
            nineBoard[7] = new GameMiniBoard(buttonList7,8, nineBoard);
            nineBoard[8] = new GameMiniBoard(buttonList8,9, nineBoard);

            mainBoard = new GameMiniBoard(buttonMainList,10,nineBoard);

        registery = new List<int>();

        nineBoard[4].firstMove();

        for (int i = 0; i < buttonMainList.Length; i++)
        {
            buttonMainList[i].GetComponentInParent<Button>().enabled=false;
        }

    }

}