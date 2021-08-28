using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelEditor", order = 1)]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public string levelName;
    public Vector2Int levelSize;
    public int imageSize;

    public Tilemap level;

    public int numberOfAvailableBlocks;
    public Vector3Int targetPosition;
}
