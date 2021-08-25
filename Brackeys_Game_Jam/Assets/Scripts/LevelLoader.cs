using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class LevelLoader : MonoBehaviour
{

    [SerializeField] PixelPerfectCamera ppCam;
    [SerializeField] GameObject grid;
    [SerializeField] TileMapManager mapManager;

    [SerializeField] LevelData level;

    private int levelNum;
    private string levelName;

    private Vector2Int levelSize;
    private int spriteSize;

    private Tilemap levelMap;

    private int numOfBlocks;

    // Start is called before the first frame update
    void Start()
    {
        if (ppCam == null)
        {
            ppCam = GameObject.FindObjectOfType<PixelPerfectCamera>();
        }

        if (level != null)
            LoadLevel(level);
    }

    public void LoadLevel(LevelData newLevel)
    {
        LoadLevelData(newLevel);
        SetCamera(levelSize);
        BuildLevel(levelMap);
        //SetCamera(levelSize * 2);
    }

    void LoadLevelData(LevelData level)
    {
        levelNum = level.levelNumber;
        levelName = level.levelName;
        levelSize = level.levelSize;
        spriteSize = level.imageSize;
        levelMap = level.level;
        numOfBlocks = level.numberOfAvailableBlocks;

        mapManager.SetValues(levelSize, numOfBlocks);
    }

    void SetCamera(Vector2Int camSize)
    {
        ppCam.assetsPPU = spriteSize;
        ppCam.refResolutionX = camSize.x * spriteSize;
        ppCam.refResolutionY = camSize.y * spriteSize;
    }

    void BuildLevel(Tilemap map)
    {
        if (grid != null)
        {
            if (grid.transform.childCount != 0)
            {
                foreach (Transform child in grid.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            Tilemap newMap = Instantiate(levelMap, grid.transform);
            mapManager.SetTileMap(newMap);
        }
    }
}
