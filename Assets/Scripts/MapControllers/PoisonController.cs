using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonController
{
    // Initial
    private static float POISON_DAMAGE = 10;
    private static float POISON_GENERATION_INTERVAL = 60;

    // Controller Info
    public bool poisonChanged;

    // Map Controller
    private MapController mapController;

    // Poison Region
    private List<Vector2> poisonRegion;

    // Poison Attack
    private float poisonAttackInterval;
    private float poisonAttackTimer;

    // Poison Region Generate
    private float poisonGenerationInterval;
    private float poisonGenerationTimer;

    // Poison Attribute
    private float poisonDamage;
    private int poisonRegionNumber;

    public PoisonController(MapController _mapController)
    {
        // Set Map Controller
        mapController = _mapController;

        // Controller Info
        poisonChanged = false;

        // Set Time
        poisonAttackInterval = 0.5f;
        poisonAttackTimer = 0;
        poisonGenerationInterval = POISON_GENERATION_INTERVAL;
        poisonGenerationTimer = 0;

        // Poison Attribute
        poisonDamage = POISON_DAMAGE;
        poisonRegionNumber = 0;

        // Poison Region
        poisonRegion = new List<Vector2>();
    }

    public void Update(List<GamePlayer> players, float dt)
    {
        // Set Attack Timer
        if(poisonAttackTimer > poisonAttackInterval)
        {
            // Attack
            foreach(var player in players)
            {
                if (InPoison(player.champion.transform.position))
                {
                    player.champion.Damage(poisonDamage, 1);
                }
            }
            // Set Timer
            poisonAttackTimer = 0;
        }
        // Set Generation Timer
        if(poisonGenerationTimer > poisonGenerationInterval)
        {
            // Generate
            Vector2 newPoisonRegion = new Vector2((int)(Random.value * MapController.MAP_WIDTH_NUMBER), (int)(Random.value * MapController.MAP_HEIGHT_NUMBER));
            poisonRegion.Add(newPoisonRegion);
            poisonRegionNumber++;
            // Set Poison Image
            mapController.maps[(int)newPoisonRegion.y][(int)newPoisonRegion.x].transform.Find("PoisonRegionMask").gameObject.SetActive(true);
            // Set Damage
            poisonDamage = POISON_DAMAGE * (1 + 0.2f * (poisonRegionNumber - 1));
            // Set Timer
            poisonGenerationTimer = 0;
            // Set Changed
            poisonChanged = true;
        }
        // Time Add
        poisonAttackTimer += dt;
        poisonGenerationTimer += dt;
    }

    public bool InPoison(Vector2 position)
    {
        Vector2 mapIndex = mapController.InWhichMap(position);
        if (poisonRegion.Contains(mapIndex))
        {
            return true;
        }
        return false;
    }



}
