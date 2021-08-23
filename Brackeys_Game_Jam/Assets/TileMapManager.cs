using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class TileMapManager : MonoBehaviour
{

    [SerializeField] private Camera mainCam;
    private PixelPerfectCamera ppCam;

    [SerializeField] private GameObject grid;
    [SerializeField] private Tilemap tileMapPrefab;
    [SerializeField] private TileBase[] tiles;
    [SerializeField] private Tilemap tileMap;

    // Start is called before the first frame update
    void Start()
    {
        tileMap = GameObject.FindObjectOfType<Tilemap>();
        if (tileMap == null)
            Debug.LogError("No TileMap found!");
        if (mainCam != null)
            ppCam = mainCam.GetComponent<PixelPerfectCamera>();

        Vector2Int tileMapDims = GetCameraPixelSize();
        ResetTileMap(new Vector3Int(-tileMapDims.x/2, -tileMapDims.y/2, 0), new Vector3Int(tileMapDims.x / 2 - 1, tileMapDims.y / 2 - 1, 0));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int tilemapPos = tileMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetButtonDown("Fire1"))
        {
            if (tileMap.GetTile<Tile>(tilemapPos).name == "black")
                ColorSelectedTile(tilemapPos);
        }

        if (Input.GetButtonDown("Jump"))
        {
            int[,] newMap = CalculateStep();
            SetTileMap(newMap);
        }


    }

    public void ColorSelectedTile(Vector3Int selectedTilePos)
    {
        tileMap.SetTile(selectedTilePos, tiles[1]);
    }

    public void ResetTileMap(Vector3Int minPos, Vector3Int maxPos)
    {
        tileMap.ClearAllTiles();

        tileMap.SetTile(minPos, tiles[2]);
        tileMap.SetTile(maxPos, tiles[2]);

        // Red colored bounds
        tileMap.BoxFill(Vector3Int.zero, tiles[2], tileMap.cellBounds.min.x, tileMap.cellBounds.min.y, tileMap.cellBounds.max.x, tileMap.cellBounds.max.y);
        // Black colored interiour Tiles
        tileMap.BoxFill(Vector3Int.zero, tiles[0], tileMap.cellBounds.min.x + 1, tileMap.cellBounds.min.y + 1, tileMap.cellBounds.max.x - 2, tileMap.cellBounds.max.y - 2);
    }

    public Vector2Int GetCameraPixelSize()
    {
        int camSizeX = ppCam.refResolutionX;
        int camSizeY = ppCam.refResolutionY;
        Vector2Int camSize = new Vector2Int(camSizeX, camSizeY);
        return camSize;
    }

    public void SetCameraPixelSize(Vector2Int newCamSize)
    {
        ppCam.refResolutionX = newCamSize.x;
        ppCam.refResolutionY = newCamSize.y;
    }

    public int[,] CalculateStep()
    {

        int[,] newTileMap = new int[ppCam.refResolutionX, ppCam.refResolutionY];

        for (int x = tileMap.cellBounds.min.x; x < tileMap.cellBounds.max.x; x++)
        {
            for (int y = tileMap.cellBounds.min.y; y < tileMap.cellBounds.max.y; y++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                Debug.Log(currentPos);
                Tile currentTile = tileMap.GetTile<Tile>(currentPos);
                if (currentTile.name == "red")
                {
                    newTileMap[x + ppCam.refResolutionX / 2, y + ppCam.refResolutionY / 2] = 2;
                    continue;
                }
                else
                {
                    int aliveCells = 0;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            Vector3Int searchPos = new Vector3Int(currentPos.x + i, currentPos.y + j, 0);
                            if (searchPos != currentPos)
                            {
                                try
                                {
                                    Tile searchTile = tileMap.GetTile<Tile>(searchPos);
                                    if (searchTile.name == "white")
                                        aliveCells++;
                                }
                                catch
                                {
                                    Debug.LogError("No Tile exists here!");
                                }
                            }
                        }
                    }

                    if (aliveCells < 2 || aliveCells > 3)
                    {
                        newTileMap[x + ppCam.refResolutionX / 2, y + ppCam.refResolutionY / 2] = 0;
                    }
                    else if (aliveCells > 1 && aliveCells < 4)
                    {
                        newTileMap[x + ppCam.refResolutionX / 2, y + ppCam.refResolutionY / 2] = 1;
                    }
                }
            }
        }
        return newTileMap;
    }

    public void SetTileMap(int[,] newTileMap)
    {
        for (int a = 0; a < newTileMap.GetLength(0); a++)
        {
            for (int b = 0; b < newTileMap.GetLength(1); b++)
            {
                Vector3Int tilePos = new Vector3Int(-ppCam.refResolutionX / 2 + a, -ppCam.refResolutionY / 2 + b, 0);
                tileMap.SetTile(tilePos, tiles[newTileMap[a, b]]);
            }
        }
    }
}
