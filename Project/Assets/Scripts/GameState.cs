using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameState
{
    public char[,] state;

    public GameState()
    {
        state = new char[3, 3];
    }

    // Checks if this game state is equal to another game state
    public bool Equals(GameState gs)
    {

        for (var i = 0; i < state.GetLength(0); i++)
        {
            for (var j = 0; j < state.GetLength(1); j++)
            {
                if (state[i, j] != gs.state[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Returns a fresh copy of this game state
    public GameState Copy()
    {
        var copy = new GameState();
        copy.state = new char[3, 3];

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                copy.state[i, j] = state[i, j];
            }
        }
        return copy;
    }
}
