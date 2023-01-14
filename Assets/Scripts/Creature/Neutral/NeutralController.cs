using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralController
{
    // Neutral Config
    private static int LOW_NEUTRAL_NUM_IN_A_MAP = 10;
    private static int MIDDLE_NEUTRAL_NUM_IN_A_MAP = 4;

    


    // Variables for Utility Functions
    private List<GamePlayer> gamePlayers;
    private MapController mapController;

    // Prefab
    private Dictionary<MapController.MapType, Dictionary<NeutralObject.NeutralType, string>> neutralPrefabNames;
    private Dictionary<MapController.MapType, Dictionary<NeutralObject.NeutralType, Transform>> neutralPrefab;

    


    public NeutralController(List<GamePlayer> _gamePlayers, MapController _mapController)
    {
        // Set Basic Info
        gamePlayers = _gamePlayers;
        mapController = _mapController;
        // Set Prefab
        neutralPrefabNames = new Dictionary<MapController.MapType, Dictionary<NeutralObject.NeutralType, string>>();
        neutralPrefab = new Dictionary<MapController.MapType, Dictionary<NeutralObject.NeutralType, Transform>>();

        Dictionary<NeutralObject.NeutralType, string> soilPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        soilPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Soil/SoilLow/SoilLow";
        soilPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Soil/SoilMiddle/SoilMiddle";
        neutralPrefabNames[MapController.MapType.Soil] = soilPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> soilLightPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        soilLightPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Soil/SoilLow/SoilLow";
        soilLightPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Soil/SoilMiddle/SoilMiddle";
        soilLightPrefabNames[NeutralObject.NeutralType.High] = "Fist/Neutral/Light/LightHigh/LightHigh";
        neutralPrefabNames[MapController.MapType.SoilLight] = soilLightPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> soilDarkPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        soilDarkPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Soil/SoilLow/SoilLow";
        soilDarkPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Soil/SoilMiddle/SoilMiddle";
        soilDarkPrefabNames[NeutralObject.NeutralType.High] = "Fist/Neutral/Dark/DarkHigh/DarkHigh";
        neutralPrefabNames[MapController.MapType.SoilDark] = soilDarkPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> firePrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        firePrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Fire/FireLow/FireLow";
        firePrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Fire/FireMiddle/FireMiddle";
        neutralPrefabNames[MapController.MapType.Fire] = firePrefabNames;

        Dictionary<NeutralObject.NeutralType, string> fireLightPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        fireLightPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Fire/FireLow/FireLow";
        fireLightPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Fire/FireMiddle/FireMiddle";
        fireLightPrefabNames[NeutralObject.NeutralType.High] = "Fist/Neutral/Light/LightHigh/LightHigh";
        neutralPrefabNames[MapController.MapType.FireLight] = fireLightPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> fireDarkPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        fireDarkPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Fire/FireLow/FireLow";
        fireDarkPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Fire/FireMiddle/FireMiddle";
        fireDarkPrefabNames[NeutralObject.NeutralType.High] = "Fist/Neutral/Dark/DarkHigh/DarkHigh";
        neutralPrefabNames[MapController.MapType.FireDark] = fireDarkPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> windPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        windPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Wind/WindLow/WindLow";
        windPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Wind/WindMiddle/WindMiddle";
        neutralPrefabNames[MapController.MapType.Wind] = windPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> windLightPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        windLightPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Wind/WindLow/WindLow";
        windLightPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Wind/WindMiddle/WindMiddle";
        windLightPrefabNames[NeutralObject.NeutralType.High] = "Fist/Neutral/Light/LightHigh/LightHigh";
        neutralPrefabNames[MapController.MapType.WindLight] = windLightPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> windDarkPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        windDarkPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Wind/WindLow/WindLow";
        windDarkPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Wind/WindMiddle/WindMiddle";
        windDarkPrefabNames[NeutralObject.NeutralType.High] = "Fist/Neutral/Dark/DarkHigh/DarkHigh";
        neutralPrefabNames[MapController.MapType.WindDark] = windDarkPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> woodPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        woodPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Wood/WoodLow/WoodLow";
        woodPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Wood/WoodMiddle/WoodMiddle";
        neutralPrefabNames[MapController.MapType.Wood] = woodPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> woodLightPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        woodLightPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Wood/WoodLow/WoodLow";
        woodLightPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Wood/WoodMiddle/WoodMiddle";
        woodLightPrefabNames[NeutralObject.NeutralType.High] = "Fist/Neutral/Light/LightHigh/LightHigh";
        neutralPrefabNames[MapController.MapType.WoodLight] = woodLightPrefabNames;

        Dictionary<NeutralObject.NeutralType, string> woodDarkPrefabNames = new Dictionary<NeutralObject.NeutralType, string>();
        woodDarkPrefabNames[NeutralObject.NeutralType.Low] = "Fist/Neutral/Wood/WoodLow/WoodLow";
        woodDarkPrefabNames[NeutralObject.NeutralType.Middle] = "Fist/Neutral/Wood/WoodMiddle/WoodMiddle";
        woodDarkPrefabNames[NeutralObject.NeutralType.High] = "Fist/Neutral/Dark/DarkHigh/DarkHigh";
        neutralPrefabNames[MapController.MapType.WoodDark] = woodDarkPrefabNames;
    }

    /************************* Initialize *************************/
    public void Initialize()
    {
        // Initialize Prefab
        InitializePrefab();

        // Initialize Neutral Object
        InitializeNeutralObject();
    }

    private void InitializePrefab()
    {
        foreach(var neutralTypePrefabNameDict in neutralPrefabNames)
        {
            Dictionary<NeutralObject.NeutralType, Transform> neutralPrefabDict = new Dictionary<NeutralObject.NeutralType, Transform>();
            foreach (var neutralPrefabName in neutralTypePrefabNameDict.Value)
            {
                neutralPrefabDict[neutralPrefabName.Key] = Resources.Load<Transform>(neutralPrefabName.Value);
            }
            neutralPrefab[neutralTypePrefabNameDict.Key] = neutralPrefabDict;
        }
    }

    private void InitializeNeutralObject()
    {
        //InitializeTest();
        // Initialize Neutral for Normal Map
        InitializeNeutralForNormalMap();
        // Initialize Neutral for Boss Map
        InitializeNeutralForBossMap();
    }

    private void InitializeNeutralForNormalMap()
    {
        for (int i = 0; i < mapController.mapTypes.Count; i++)
        {
            for (int j = 0; j < mapController.mapTypes[i].Count; j++)
            {
                bool bossMap = false;
                // if boss map, continue
                if (i == mapController.lightY && j == mapController.lightX || i == mapController.darkY && j == mapController.darkX)
                {
                    bossMap = true;
                }
                // Initialize Low Neutral Object
                for (int k = 0; k < LOW_NEUTRAL_NUM_IN_A_MAP; k++)
                {
                    GenerateNeutralObject(mapController.mapTypes[i][j], NeutralObject.NeutralType.Low, new Vector2(j, i), bossMap);
                }
                // Initialize Middle Neutral Object
                for (int k = 0; k < MIDDLE_NEUTRAL_NUM_IN_A_MAP; k++)
                {
                    GenerateNeutralObject(mapController.mapTypes[i][j], NeutralObject.NeutralType.Middle, new Vector2(j, i), bossMap);
                }
            }
        }
    }

    private void InitializeNeutralForBossMap()
    {
        /* Initialize Light Boss Map */
        Vector2 mapIndex = new Vector2(mapController.lightX, mapController.lightY);
        Vector2 position = new Vector2(MapController.LEFT_BORDER + (mapIndex.x + 0.5f) * MapController.ONE_MAP_WIDTH, MapController.TOP_BORDER - (mapIndex.y + 0.5f) * MapController.ONE_MAP_HEIGHT);
        GenerateNeutralObject(mapController.mapTypes[mapController.lightY][mapController.lightX], NeutralObject.NeutralType.High, mapIndex, position, true);
        /* Initialize Dark Boss Map */
        mapIndex = new Vector2(mapController.darkX, mapController.darkY);
        position = new Vector2(MapController.LEFT_BORDER + (mapIndex.x + 0.5f) * MapController.ONE_MAP_WIDTH, MapController.TOP_BORDER - (mapIndex.y + 0.5f) * MapController.ONE_MAP_HEIGHT);
        GenerateNeutralObject(mapController.mapTypes[mapController.darkY][mapController.darkX], NeutralObject.NeutralType.High, mapIndex, position, true);
    }

    private void InitializeTest()
    {
        GenerateNeutralObject(MapController.MapType.Soil, NeutralObject.NeutralType.Low, new Vector2(0, 0));
    }

    /************************* Generate Neutral Object *************************/

    private void GenerateNeutralObject(MapController.MapType mapType, NeutralObject.NeutralType neutralType, Vector2 mapIndex, Vector2 position, bool boss=false)
    {
        Transform prefab = GlobalController.Instance.gameController.fistManager.Instantiate(neutralPrefab[mapType][neutralType], position, Quaternion.identity);
        if (boss)
        {
            Vector2 leftTopBorder = new Vector2(MapController.LEFT_BORDER + (mapIndex.x + MapController.BOSS_REGION_RATIO_X / 2) * MapController.ONE_MAP_WIDTH, MapController.TOP_BORDER - (mapIndex.y + MapController.BOSS_REGION_RATIO_Y / 2) * MapController.ONE_MAP_HEIGHT);
            Vector2 rightBottomBorder = new Vector2(MapController.LEFT_BORDER + (mapIndex.x + 1 - MapController.BOSS_REGION_RATIO_X / 2) * MapController.ONE_MAP_WIDTH, MapController.TOP_BORDER - (mapIndex.y + 1 - MapController.BOSS_REGION_RATIO_Y / 2) * MapController.ONE_MAP_HEIGHT);
            prefab.GetComponent<NeutralObject>().SetBasicInfo(leftTopBorder, rightBottomBorder);
        }
        else
        {
            Vector2 leftTopBorder = new Vector2(MapController.LEFT_BORDER + mapIndex.x * MapController.ONE_MAP_WIDTH, MapController.TOP_BORDER - mapIndex.y * MapController.ONE_MAP_HEIGHT);
            Vector2 rightBottomBorder = new Vector2(MapController.LEFT_BORDER + (mapIndex.x + 1) * MapController.ONE_MAP_WIDTH, MapController.TOP_BORDER - (mapIndex.y + 1) * MapController.ONE_MAP_HEIGHT);
            prefab.GetComponent<NeutralObject>().SetBasicInfo(leftTopBorder, rightBottomBorder);
        }
    }

    private void GenerateNeutralObject(MapController.MapType mapType, NeutralObject.NeutralType neutralType, Vector2 mapIndex, bool bossMap=false)
    {
        Vector2 position;
        if (bossMap)
        {
            while (true)
            {
                float x = MapController.LEFT_BORDER + mapIndex.x * MapController.ONE_MAP_WIDTH + Random.value * MapController.ONE_MAP_WIDTH;
                float y = MapController.TOP_BORDER - mapIndex.y * MapController.ONE_MAP_HEIGHT - Random.value * MapController.ONE_MAP_HEIGHT; 
                position = new Vector2(x, y);
                if (!mapController.InBossRegion(position))
                {
                    break;
                }
            }
        }
        else{
            float x = MapController.LEFT_BORDER + mapIndex.x * MapController.ONE_MAP_WIDTH + Random.value * MapController.ONE_MAP_WIDTH;
            float y = MapController.TOP_BORDER - mapIndex.y * MapController.ONE_MAP_HEIGHT - Random.value * MapController.ONE_MAP_HEIGHT;
            position = new Vector2(x, y);
        }
        Transform prefab = GlobalController.Instance.gameController.fistManager.Instantiate(neutralPrefab[mapType][neutralType], position, Quaternion.identity);
        Vector2 leftTopBorder = new Vector2(MapController.LEFT_BORDER + mapIndex.x * MapController.ONE_MAP_WIDTH, MapController.TOP_BORDER - mapIndex.y * MapController.ONE_MAP_HEIGHT);
        Vector2 rightBottomBorder = new Vector2(MapController.LEFT_BORDER + (mapIndex.x + 1) * MapController.ONE_MAP_WIDTH, MapController.TOP_BORDER - (mapIndex.y + 1) * MapController.ONE_MAP_HEIGHT);
        prefab.GetComponent<NeutralObject>().SetBasicInfo(leftTopBorder, rightBottomBorder);
    }

    /************************* Utility *************************/
    /// <summary>
    /// Find the nearest champion who is within watchRadius and in the same region
    /// </summary>
    /// <param name="currentPosition"></param>
    /// <param name="watchRadius"></param>
    /// <returns></returns>
    public ChampionObject FindNearestChampion(Vector2 currentPosition, Vector2 leftTopBorder, Vector2 rightBottomBorder, float watchRadius)
    {
        ChampionObject nearestChampion = null;
        float nearestDistance = int.MaxValue;
        foreach (GamePlayer player in gamePlayers)
        {
            if (player.dead)
            {
                continue;
            }
            // Check within watchRadius
            Vector2 championPosition = player.champion.transform.position;
            float distance = (championPosition - currentPosition).magnitude;
            if(distance > watchRadius)
            {
                continue;
            }
            if(!UtilityTool.InRegion(championPosition, leftTopBorder, rightBottomBorder))
            {
                continue;
            }
            if(distance < nearestDistance)
            {
                nearestChampion = player.champion;
                nearestDistance = distance;
            }
        }
        return nearestChampion;
    }




}
