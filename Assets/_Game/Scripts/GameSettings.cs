using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    public int BoardSizeX = 5;
    public int BoardSizeY = 5;
    public int BottomSize = 5;
    public int DuplicateMin = 3;
    public float LevelTime = 30f;
    public float TimeForHint = 5f;
}
