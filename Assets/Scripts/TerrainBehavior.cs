using UnityEngine;

[CreateAssetMenu(fileName = "TerrainBehavior", menuName = "Tower Defense/Terrain Behavior")]
public class TerrainBehavior : ScriptableObject
{
    [Header("Movement Effects")]
    public float movementSpeedMultiplier = 1f;
    public bool allowsMovement = true;
    
    [Header("Entity Placement Rules")]
    public bool allowsTowerPlacement = true;
    public bool allowsEnemySpawn = true;
    public float enemySpawnWeight = 1f;
    public bool isEnemyPreferred = false;
    
    [Header("Tower Bonuses")]
    public float towerDamageMultiplier = 1f;
    public float towerRangeMultiplier = 1f;
    public float towerFireRateMultiplier = 1f;
    
    [Header("Enemy Bonuses")]
    public float enemyDamageMultiplier = 1f;
    public float enemyHealthMultiplier = 1f;
    public float enemySpeedMultiplier = 1f;
    
    [Header("Platform Physics")]
    public bool canEnemiesFall = false;
    public bool isSolidGround = true;
    public bool killsEnemiesOnFall = false;
    
    [Header("Visual")]
    public Color terrainColor = Color.white;
    
    // Áp dụng behavior này lên entity
    public virtual void ApplyToEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        
        // Áp dụng bonus máu
        enemy.ApplyHealthMultiplier(enemyHealthMultiplier);
        
        // Kiểm tra hiệu ứng đặc biệt
        ApplySpecialEffects(enemy);
    }
    
    public virtual void ApplyToTower(Tower tower)
    {
        if (tower == null) return;
        
        // Áp dụng bonus cho tháp
        tower.ApplyDamageMultiplier(towerDamageMultiplier);
        tower.ApplyRangeMultiplier(towerRangeMultiplier);
        tower.ApplyFireRateMultiplier(towerFireRateMultiplier);
    }
    
    protected virtual void ApplySpecialEffects(Enemy enemy)
    {
        // Ghi đè trong các terrain behavior cụ thể để có hiệu ứng đặc biệt
    }
}
