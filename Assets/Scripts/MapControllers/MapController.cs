using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController
{
    // Constant
    public const int MAP_TYPE_NUMBER = 4;
    public const int ONE_MAP_WIDTH = 60;
    public const int ONE_MAP_HEIGHT = 40;
    public const int MAP_WIDTH_NUMBER = 3;
    public const int MAP_HEIGHT_NUMBER = 3;
    public const int MAP_WIDTH = ONE_MAP_WIDTH * MAP_WIDTH_NUMBER;
    public const int MAP_HEIGHT = ONE_MAP_HEIGHT * MAP_HEIGHT_NUMBER;
    public const int LEFT_BORDER = -(MAP_WIDTH_NUMBER* ONE_MAP_WIDTH / 2);
    public const int RIGHT_BORDER = MAP_WIDTH_NUMBER* ONE_MAP_WIDTH / 2;
    public const int TOP_BORDER = MAP_HEIGHT_NUMBER* ONE_MAP_HEIGHT / 2;
    public const int BOTTOM_BORDER = -(MAP_HEIGHT_NUMBER* ONE_MAP_HEIGHT / 2);
    public const float BOSS_REGION_RATIO_X = 1f / 3f;
    public const float BOSS_REGION_RATIO_Y = 1f / 2f;

    // Map Border

    // Map Grid
    public GameObject mapGrid;

    // Map Info
    public enum MapType
    {
        Fire = 0,
        FireLight = 1,
        FireDark = 2,
        Soil = 3,
        SoilLight = 4,
        SoilDark = 5,
        Wood = 6,
        WoodLight = 7,
        WoodDark = 8,
        Wind = 9,
        WindLight = 10,
        WindDark = 11,
    }

    public string mapPrefabPath;
    public string mapBorderPrefabPath;
    public string[] mapPrefabName;

    public Dictionary<MapType, Transform> mapPrefabs;
    public List<List<MapType>> mapTypes;
    public List<List<Transform>> maps;
    public int lightX;
    public int lightY;
    public int darkX;
    public int darkY;
    public Vector2 leftTopLight;
    public Vector2 leftTopDark;

    // Start is called before the first frame update
    public MapController()
    {
        // Prefab Name
        mapPrefabPath = "Prefabs/Game/Maps/";
        mapPrefabName = new string[MAP_TYPE_NUMBER * 3];
        mapPrefabName[0] = "fire_map_normal";
        mapPrefabName[1] = "fire_map_light";
        mapPrefabName[2] = "fire_map_dark";
        mapPrefabName[3] = "soil_map_normal";
        mapPrefabName[4] = "soil_map_light";
        mapPrefabName[5] = "soil_map_dark";
        mapPrefabName[6] = "wood_map_normal";
        mapPrefabName[7] = "wood_map_light";
        mapPrefabName[8] = "wood_map_dark";
        mapPrefabName[9] = "wind_map_normal";
        mapPrefabName[10] = "wind_map_light";
        mapPrefabName[11] = "wind_map_dark";
        mapBorderPrefabPath = "MapBorder";
        // Set Map Grid
        mapGrid = GameObject.Find("MapGrid");
        // Initial map
        maps = new List<List<Transform>>();
    }

    /************************* Initialize *************************/

    public void Initialize()
    {
        // Initialize FireRegionController
        FireRegionController.Initialize();
        // Initialize WindMapController
        WindMapController.Initialize();
        // Initialize map types
        InitializeMapTypes();
        // Initialize Maps
        InitializeMaps();
        // Initialize Border
        InitializeMapBorders();
        // Initialize FistBase
        InitializeFistBase();
    }

    private void InitializeMapTypes()
    {
        mapTypes = new List<List<MapType>>();
        mapPrefabs = new Dictionary<MapType, Transform>();
        lightX = (int)(Random.value * MAP_WIDTH_NUMBER);
        lightY = (int)(Random.value * MAP_HEIGHT_NUMBER);
        while (true)
        {
            darkX = (int)(Random.value * MAP_WIDTH_NUMBER);
            darkY = (int)(Random.value * MAP_HEIGHT_NUMBER);
            if (darkX == lightX && darkY == lightY)
            {
                continue;
            }
            break;
        }
        leftTopLight = new Vector2(LEFT_BORDER + lightX * ONE_MAP_WIDTH, TOP_BORDER - lightY * ONE_MAP_HEIGHT);
        leftTopDark = new Vector2(LEFT_BORDER + darkX * ONE_MAP_WIDTH, TOP_BORDER - darkY * ONE_MAP_HEIGHT);
        for (int i = 0; i < MAP_HEIGHT_NUMBER; i++)
        {
            List<MapType> mapTypeList = new List<MapType>();
            for (int j = 0; j < MAP_WIDTH_NUMBER; j++)
            {
                // Random a type
                int mapType = (int)(Random.value * MAP_TYPE_NUMBER) * 3;
                // Check if boss map
                if (j == lightX && i == lightY)
                {
                    mapType += 1;
                }
                if (j == darkX && i == darkY)
                {
                    mapType += 2;
                }
                // Add to list
                mapTypeList.Add((MapType)mapType);
                // if no prefab, add
                if (!mapPrefabs.ContainsKey((MapType)mapType))
                {
                    mapPrefabs[(MapType)mapType] = Resources.Load<Transform>(mapPrefabPath + mapPrefabName[mapType]);
                }
            }
            mapTypes.Add(mapTypeList);
        }
    }

    private void InitializeMaps()
    {
        for (int i = 0; i < MAP_HEIGHT_NUMBER; i++)
        {
            List<Transform> mapList = new List<Transform>();
            for (int j = 0; j < MAP_WIDTH_NUMBER; j++)
            {
                Transform tmpMap = Object.Instantiate(mapPrefabs[mapTypes[i][j]], new Vector3(LEFT_BORDER + (j + 0.5f) * ONE_MAP_WIDTH, TOP_BORDER - (i + 0.5f) * ONE_MAP_HEIGHT, 0), Quaternion.identity, mapGrid.transform);
                // Set Poison Invisible
                tmpMap.transform.Find("PoisonRegionMask").gameObject.SetActive(false);
                mapList.Add(tmpMap);
            }
            maps.Add(mapList);
        }
    }

    private void InitializeMapBorders()
    {
        GameObject mapBorderPrefab = (GameObject)Resources.Load(mapPrefabPath + mapBorderPrefabPath);
        GameObject mapBorderTop = Object.Instantiate(mapBorderPrefab, new Vector3(0, TOP_BORDER + 1, 0), Quaternion.identity);
        mapBorderTop.transform.localScale = new Vector3(ONE_MAP_WIDTH * MAP_WIDTH_NUMBER + 10, 1, 1);
        GameObject mapBorderBottom = Object.Instantiate(mapBorderPrefab, new Vector3(0, BOTTOM_BORDER, 0), Quaternion.identity);
        mapBorderBottom.transform.localScale = new Vector3(ONE_MAP_WIDTH * MAP_WIDTH_NUMBER + 10, 1, 1);
        GameObject mapBorderLeft = Object.Instantiate(mapBorderPrefab, new Vector3(LEFT_BORDER, 0, 0), Quaternion.identity);
        mapBorderLeft.transform.localScale = new Vector3(1, ONE_MAP_HEIGHT * MAP_HEIGHT_NUMBER + 10, 1);
        GameObject mapBorderRight = Object.Instantiate(mapBorderPrefab, new Vector3(RIGHT_BORDER + 1, 0, 0), Quaternion.identity);
        mapBorderRight.transform.localScale = new Vector3(1, ONE_MAP_HEIGHT * MAP_HEIGHT_NUMBER + 10, 1);
    }

    private void InitializeFistBase()
    {
        // Register for Fire Region
        GameObject[] fireRegionList = GameObject.FindGameObjectsWithTag("FireRegion");
        foreach(var fireRegion in fireRegionList)
        {
            if(fireRegion.GetComponent<FistBase>() != null)
            {
                GlobalController.Instance.gameController.fistManager.Register(fireRegion.GetComponent<FistBase>());
            }
        }
        // Register for Wind Map
        GameObject[] windMapList = GameObject.FindGameObjectsWithTag("WindMap");
        foreach (var windMap in windMapList)
        {
            if (windMap.GetComponent<FistBase>() != null)
            {
                GlobalController.Instance.gameController.fistManager.Register(windMap.GetComponent<FistBase>());
            }
        }
    }








    public bool InBossMap(Vector2 position)
    {
        // Check in Light
        if(position.x > leftTopLight.x && position.x < leftTopLight.x + ONE_MAP_WIDTH && position.y < leftTopLight.y && position.y > leftTopLight.y - ONE_MAP_HEIGHT)
        {
            return true;
        }
        // Check in Dark
        if (position.x > leftTopDark.x && position.x < leftTopDark.x + ONE_MAP_WIDTH && position.y < leftTopDark.y && position.y > leftTopDark.y - ONE_MAP_HEIGHT)
        {
            return true;
        }
        return false;
    }

    public Vector2 InWhichMap(Vector2 position)
    {
        Vector2 mapIndex = new Vector2();
        mapIndex.x = (int)((position.x - LEFT_BORDER) / ONE_MAP_WIDTH);
        mapIndex.y = (int)((TOP_BORDER - position.y) / ONE_MAP_HEIGHT);
        return mapIndex;
    }

    public bool InBossRegion(Vector2 position)
    {
        Vector2 leftTopBorder = new Vector2(LEFT_BORDER + (lightX + BOSS_REGION_RATIO_X / 2) * ONE_MAP_WIDTH, TOP_BORDER - (lightY + BOSS_REGION_RATIO_Y / 2) * ONE_MAP_HEIGHT);
        Vector2 rightBottomBorder = new Vector2(LEFT_BORDER + (lightX + 1 - BOSS_REGION_RATIO_X / 2) * ONE_MAP_WIDTH, TOP_BORDER - (lightY + 1 - BOSS_REGION_RATIO_Y / 2) * ONE_MAP_HEIGHT);
        // Check in Light
        if (position.x > leftTopBorder.x && position.x < rightBottomBorder.x && position.y < leftTopBorder.y && position.y > rightBottomBorder.y)
        {
            return true;
        }
        leftTopBorder = new Vector2(LEFT_BORDER + (darkX + BOSS_REGION_RATIO_X / 2) * ONE_MAP_WIDTH, TOP_BORDER - (darkY + BOSS_REGION_RATIO_Y / 2) * ONE_MAP_HEIGHT);
        rightBottomBorder = new Vector2(LEFT_BORDER + (darkX + 1 - BOSS_REGION_RATIO_X / 2) * ONE_MAP_WIDTH, TOP_BORDER - (darkY + 1 - BOSS_REGION_RATIO_Y / 2) * ONE_MAP_HEIGHT);
        // Check in Dark
        if (position.x > leftTopBorder.x && position.x < rightBottomBorder.x && position.y < leftTopBorder.y && position.y > rightBottomBorder.y)
        {
            return true;
        }
        return false;
    }

}
