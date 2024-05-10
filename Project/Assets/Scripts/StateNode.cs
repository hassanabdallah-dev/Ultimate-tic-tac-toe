using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StateNode
{
    public GameState gs;
    public StateNode parent;
    public List<StateNode> children;
    public int minimaxValue;
    public int alpha;
    public int betta;
    public char player;
    public int depth;
    public Tuple<int, int> action = new Tuple<int, int>(-1, -1);
    public StateNode()
    {
        gs = new GameState();
        children = new List<StateNode>();
    }
}
