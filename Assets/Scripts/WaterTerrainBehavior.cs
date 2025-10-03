using UnityEngine;

[CreateAssetMenu(fileName = "WaterTerrainBehavior", menuName = "Tower Defense/Water Terrain Behavior")]
public class WaterTerrainBehavior : TerrainBehavior
{
    [Header("Water Effects")]
    public float waterSlowdown = 0.5f;
    public bool causesSplash = true;
    
    protected override void ApplySpecialEffects(Enemy enemy)
    {
        base.ApplySpecialEffects(enemy);
        
        // Có thể thêm hiệu ứng splash, trạng thái ướt, etc.
        if (causesSplash)
        {
            // Kích hoạt hiệu ứng splash
            enemy.TriggerSplashEffect();
        }
    }
}
