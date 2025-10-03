using UnityEngine;

[CreateAssetMenu(fileName = "PlatformTerrainBehavior", menuName = "Tower Defense/Platform Terrain Behavior")]
public class PlatformTerrainBehavior : TerrainBehavior
{
    [Header("Platform Effects")]
    public bool canEnemiesFallFrom = true;
    public float fallThreshold = 0.1f; // How close to edge before falling
    
    protected override void ApplySpecialEffects(Enemy enemy)
    {
        base.ApplySpecialEffects(enemy);
        
        if (canEnemiesFallFrom)
        {
            // Kích hoạt phát hiện rơi cho enemy này
            enemy.EnableFallingDetection(fallThreshold);
        }
    }
}
