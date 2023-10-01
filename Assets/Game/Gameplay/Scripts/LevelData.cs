using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class LevelData
{
    public int level = 0;
    public List<Tile> tileList = new List<Tile>();
}

[Serializable]
public class Tile
{
    public ELEMENT_TYPE element = ELEMENT_TYPE.EMPTY;
    public Vector2Int gridPos = Vector2Int.zero;
    public bool isAvailable = true;
}