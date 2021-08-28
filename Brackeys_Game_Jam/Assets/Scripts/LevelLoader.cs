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
    [SerializeField] UIController uiController;

    [SerializeField] LevelData level;

    private int levelNum;
    private string levelName;

    private Vector2Int levelSize;
    private int spriteSize;

    private Tilemap levelMap;

    private int numOfBlocks;
    private Vector3Int targetBlock;

    // Start is called before the first frame update
    void Start()
    {
        if (ppCam == null)
        {
            try
            {
                ppCam = GameObject.FindObjectOfType<PixelPerfectCamera>();
            }
            catch
            {
                Debug.LogError("No PixelPerfectCamera Object found!");
            }
        }

        if (level != null)
            LoadLevel(level);
        else
            Debug.LogError("No valid Level found!");

        if (uiController == null)
        {
            try
            {
                uiController = GameObject.FindObjectOfType<UIController>();
            }
            catch
            {
                Debug.LogError("No UIController in Scene found!");
            }
        }
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
        targetBlock = level.targetPosition;

        mapManager.SetValues(levelSize, numOfBlocks, targetBlock);
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

    public void RestartLevel()
    {
        LoadLevelData(level);
        SetCamera(levelSize);
        BuildLevel(levelMap);
        uiController.SetUIState(false, "");
    }
}
