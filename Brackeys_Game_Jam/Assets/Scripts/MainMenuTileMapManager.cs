using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainMenuTileMapManager : MonoBehaviour
{

    [SerializeField] private TileBase[] tiles;

    [SerializeField] private Tilemap tileMap;

    private bool hasStarted = false;
    [SerializeField] private float roundTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (tileMap == null)
            tileMap = GameObject.FindObjectOfType<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(AutoCalc());
        }
    }

    private IEnumerator AutoCalc()
    {
        while (true)
        {
            yield return new WaitForSeconds(roundTime);
            int[,] newMap = CalculateStep();
            SetTileMap(newMap);
        }
    }

    public int[,] CalculateStep()
    {
        int[,] newTileMap = new int[42, 42];
        // Iterate over every Tile in the TileMap
        for (int x = -21; x < 21; x++)
        {
            for (int y = -21; y < 21; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                Tile currentTile = tileMap.GetTile<Tile>(currentPos);

                newTileMap[x + 21, y + 21] = CheckWinCondition(currentPos, currentTile, CountLiveCells(currentPos));
            }
        }
        return newTileMap;
    }

    private int CountLiveCells(Vector3Int currentPos)
    {
        int aliveCells = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector3Int searchPos = new Vector3Int(currentPos.x + i, currentPos.y + j, 0);
                if (searchPos != currentPos)
                {
                    // if a Tile exists at position [i,j] add 1 for each alive cell
                    try
                    {
                        Tile searchTile = tileMap.GetTile<Tile>(searchPos);
                        if (searchTile == tiles[1])
                            aliveCells++;
                    }
                    catch
                    {
                        Debug.LogError("No Tile exists here!");
                    }
                }
            }
        }
        return aliveCells;
    }

    private int CheckWinCondition(Vector3Int currPos, Tile currTile, int aliveCells)
    {

            if (aliveCells < 2 || aliveCells > 3)
            {
                return 0;
            }
            else if (aliveCells == 3 && currTile == tiles[0])
            {
                return 1;
            }
            else if (aliveCells > 1 && aliveCells < 4 && currTile == tiles[1])
            {
                return 1;
            }
        return 0;
    }

    public void SetTileMap(int[,] newTileMap)
    {
        for (int a = 0; a < newTileMap.GetLength(0); a++)
        {
            for (int b = 0; b < newTileMap.GetLength(1); b++)
            {
                Vector3Int tilePos = new Vector3Int(-21 + a, -21 + b, 0);
                tileMap.SetTile(tilePos, tiles[newTileMap[a, b]]);
            }
        }
    }
}
