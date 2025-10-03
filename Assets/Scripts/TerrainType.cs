using UnityEngine;

[System.Serializable]
public enum TerrainType
{
    Ground,     // Normal terrain - can place towers, normal movement
    Air,        // Air/empty space - no towers, normal movement  
    Water,      // Water terrain - no towers, slower movement
    Platform,   // Platform terrain - enemies can fall off if they go out of bounds
    Void        // Void/empty space - enemies die if they fall here
}

[System.Serializable]
public class TerrainData
{
    public TerrainType terrainType;
    public string displayName;
    public Color terrainColor = Color.white;
    
    [Header("Movement Properties")]
    public float movementSpeedMultiplier = 1f;
    public bool allowsMovement = true;
    
    [Header("Entity Placement Rules")]
    [Tooltip("Can towers be placed on this terrain?")]
    public bool allowsTowerPlacement = true;
    [Tooltip("Can enemies spawn on this terrain?")]
    public bool allowsEnemySpawn = true;
    [Tooltip("Weight for enemy spawning (higher = more likely to spawn here)")]
    public float enemySpawnWeight = 1f;
    [Tooltip("Is this terrain preferred by enemies?")]
    public bool isEnemyPreferred = false;
    
    [Header("Tower Bonuses (when placed here)")]
    public float towerDamageMultiplier = 1f;
    public float towerRangeMultiplier = 1f;
    public float towerFireRateMultiplier = 1f;
    
    [Header("Enemy Bonuses (when spawned here)")]
    [Tooltip("Damage multiplier for enemies on this terrain")]
    public float enemyDamageMultiplier = 1f;
    [Tooltip("Health multiplier for enemies on this terrain")]
    public float enemyHealthMultiplier = 1f;
    [Tooltip("Speed multiplier for enemies on this terrain")]
    public float enemySpeedMultiplier = 1f;
    
    [Header("Platform Physics")]
    [Tooltip("Can enemies fall off this terrain?")]
    public bool canEnemiesFall = false;
    [Tooltip("Is this terrain solid ground that enemies walk on?")]
    public bool isSolidGround = true;
    [Tooltip("Does this terrain kill enemies when they fall into it?")]
    public bool killsEnemiesOnFall = false;
}

[CreateAssetMenu(fileName = "TerrainDatabase", menuName = "Tower Defense/Terrain Database")]
public class TerrainDatabase : ScriptableObject
{
    public TerrainData[] terrainTypes;
    
    public TerrainData GetTerrainData(TerrainType terrainType)
    {
        foreach (var terrain in terrainTypes)
        {
            if (terrain.terrainType == terrainType)
                return terrain;
        }
        return null;
    }
}
