using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class TileMapManager : MonoBehaviour
{

    [SerializeField] private TileBase[] tiles;

    [SerializeField] private Tilemap tileMap;

    [SerializeField]  private Vector2Int mapSize;

    [SerializeField]  private int maxNumOfBlocks = 10;
    [SerializeField]  private int currentNumOfSetBlocks = 0;
    [SerializeField] private Vector3Int targetBlock;

    private bool hasStarted = false;
    private bool boardSet = false;
    private bool stop = false;

    [SerializeField] private bool autoStart = false;
    [SerializeField] private float roundTime = 0.5f;
    private bool looping = false;

    [SerializeField] private UIController uiController;

    // Start is called before the first frame update
    void Start()
    {
        if (uiController == null)
        {
            uiController = GameObject.FindObjectOfType<UIController>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (tileMap != null)
        {
            if (!stop)
                CheckForInputs();
            else
                StopAllCoroutines();
        }
    }

    private void CheckForInputs()
    {
        if (Input.GetButtonDown("Fire1") && !hasStarted)
        {
            ToggleSelectedTile();
        }

        if (Input.GetButtonDown("Jump"))
        {
            hasStarted = true;
            if (hasStarted && !boardSet)
            {
                boardSet = true;
                SetBoardInStone();
            }
            if (autoStart)
            {
                if (!looping)
                {
                    looping = true;
                    StartCoroutine(AutoCalc());
                }
            }
            else
            {
                int[,] newMap = CalculateStep();
                SetTileMap(newMap);
            }
        }     
        
        if (Input.GetButtonDown("Cancel"))
        {
            looping = false;
            autoStart = false;
        }
    }

    private IEnumerator AutoCalc()
    {
        while(looping)
        {
            yield return new WaitForSeconds(roundTime);
            int[,] newMap = CalculateStep();
            SetTileMap(newMap);
        }
    }

    private void SetBoardInStone()
    {
        for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                Tile tile = tileMap.GetTile<Tile>(pos);
                if (tile != tiles[0] && tile != tiles[1] && tile != tiles[2] && tile != tiles[3])
                    tileMap.SetTile(pos, tiles[0]);
            }
        }
    }

    public void SetValues(Vector2Int levelSize, int numBlocks, Vector3Int targetPos)
    {
        mapSize = levelSize;
        maxNumOfBlocks = numBlocks;
        currentNumOfSetBlocks = 0;
        targetBlock = targetPos;

        autoStart = true;
        hasStarted = false;
        boardSet = false;
        stop = false;
        looping = true;
    }

    public void SetTileMap(Tilemap activeMap)
    {
        tileMap = activeMap;
    }

    public void ToggleSelectedTile()
    {
        Vector3Int tilemapPos = tileMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));         
            if (currentNumOfSetBlocks < maxNumOfBlocks && tileMap.GetTile<Tile>(tilemapPos) == tiles[0])
            {
                tileMap.SetTile(tilemapPos, tiles[1]);
                currentNumOfSetBlocks++;
            }
            else if (tileMap.GetTile<Tile>(tilemapPos) == tiles[1])
            {
                tileMap.SetTile(tilemapPos, tiles[0]);
                currentNumOfSetBlocks--;
            }
    }

    public int[,] CalculateStep()
    {
        int numberOfLiveCells = 0;
        int[,] newTileMap = new int[mapSize.x, mapSize.y];
        // Iterate over every Tile in the TileMap
        for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                Tile currentTile = tileMap.GetTile<Tile>(currentPos);
                // Ignore Red Tiles
                if (currentTile == tiles[2])
                {
                    newTileMap[x + mapSize.x / 2, y + mapSize.y / 2] = 2;
                    continue;
                }
                else
                {
                    if (currentTile == tiles[1])
                        numberOfLiveCells++;
                    // Given the number of alive cells around the position [x,y] calculate new state
                    newTileMap[x + mapSize.x / 2, y + mapSize.y / 2] = CheckWinCondition(currentPos, currentTile, CountLiveCells(currentPos));
                }
            }
        }
        if (numberOfLiveCells == 0)
        {
            stop = true;
            uiController.SetUIState(true, "Game Over");
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
        if (currPos == targetBlock)
        {
            if (aliveCells == 3)
            {
                stop = true;
                Debug.Log("You won!");
                uiController.SetUIState(true, "You won!");
                return 1;
            }
            else
            {
                return 3;
            }
        }
        else
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
        }
        return 0;
    }

    private int CompareColors(Tile currentTile)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (currentTile == tiles[i])
            {
                return i;
            }
        }
        return 0;
    }

    public void SetTileMap(int[,] newTileMap)
    {
        for (int a = 0; a < newTileMap.GetLength(0); a++)
        {
            for (int b = 0; b < newTileMap.GetLength(1); b++)
            {
                Vector3Int tilePos = new Vector3Int(-mapSize.x / 2 + a, -mapSize.y / 2 + b, 0);
                tileMap.SetTile(tilePos, tiles[newTileMap[a, b]]);
            }
        }
    }
}
