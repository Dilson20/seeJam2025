using UnityEngine;

[System.Serializable]
public enum TerrainType
{
    Ground,     // Normal terrain - can place towers, normal movement
    Air,        // Air/empty space - no towers, normal movement  
    Water       // Water terrain - no towers, slower movement
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
    
    [Header("Tower Properties")]
    public bool allowsTowerPlacement = true;
    public float towerDamageMultiplier = 1f;
    public float towerRangeMultiplier = 1f;
    public float towerFireRateMultiplier = 1f;
    
    [Header("Enemy Preferences")]
    public float enemySpawnWeight = 1f;
    public bool isEnemyPreferred = false;
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
