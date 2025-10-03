using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    public string towerName = "Basic Tower";
    public int cost = 50;
    public float damage = 10f;
    public float range = 3f;
    public float fireRate = 1f; // shots per second
    public float rotationSpeed = 90f; // degrees per second
    
    [Header("Visual")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform turretPivot; // The part that rotates
    public SpriteRenderer towerSprite;
    
    [Header("Targeting")]
    public bool autoTarget = true; // Should tower automatically target enemies?
    public string targetTag = "Enemy"; // Tag of enemies to target
    public LayerMask targetLayerMask = -1; // Layer mask for targeting
    
    [Header("Terrain Bonuses")]
    public bool receivesTerrainBonuses = true;
    
    // Runtime variables
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Enemy currentTarget;
    private float lastFireTime;
    private TerrainData currentTerrainData;
    private Vector3Int gridPosition;
    
    private void Start()
    {
        gridPosition = TerrainManager.Instance.WorldToGrid(transform.position);
        UpdateTerrainBonuses();
    }
    
    private void Update()
    {
        if (autoTarget)
        {
            UpdateTarget();
            
            if (currentTarget != null)
            {
                RotateTowardsTarget();
                TryFire();
            }
        }
    }
    
    // Hàm chọn mục tiêu: Tìm enemy gần nhất trong tầm bắn
    private void UpdateTarget()
    {
        // Loại bỏ các enemy đã chết
        enemiesInRange.RemoveAll(enemy => enemy == null);
        
        // Tìm enemy gần nhất trong danh sách
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;
        
        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                // Tính khoảng cách từ tháp đến enemy
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        
        currentTarget = closestEnemy; // Đặt enemy gần nhất làm mục tiêu
    }
    
    private void RotateTowardsTarget()
    {
        if (currentTarget == null || turretPivot == null) return;
        
        Vector3 direction = (currentTarget.transform.position - turretPivot.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        turretPivot.rotation = Quaternion.RotateTowards(turretPivot.rotation, targetRotation, 90f * Time.deltaTime);
    }
    
    private void TryFire()
    {
        if (Time.time - lastFireTime >= 1f / GetEffectiveFireRate())
        {
            Fire();
            lastFireTime = Time.time;
        }
    }
    
    // Hàm bắn đạn: Tạo projectile và nhắm vào mục tiêu
    private void Fire()
    {
        if (projectilePrefab == null || firePoint == null) return;
        
        // Tạo đạn từ prefab tại vị trí firePoint
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        // Thiết lập thuộc tính cho đạn
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetTarget(currentTarget); // Đặt mục tiêu cho đạn
            projectileScript.SetDamage(GetEffectiveDamage()); // Đặt damage (có thể được tăng bởi địa hình)
        }
        
        // Hiệu ứng flash khi bắn
        StartCoroutine(MuzzleFlash());
    }
    
    private IEnumerator MuzzleFlash()
    {
        if (towerSprite != null)
        {
            Color originalColor = towerSprite.color;
            towerSprite.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
            towerSprite.color = originalColor;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider has the target tag and layer
        if (other.CompareTag(targetTag) && IsInTargetLayer(other.gameObject))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag) && IsInTargetLayer(other.gameObject))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }
    
    private bool IsInTargetLayer(GameObject obj)
    {
        return (targetLayerMask.value & (1 << obj.layer)) != 0;
    }
    
    private void UpdateTerrainBonuses()
    {
        if (TerrainManager.Instance != null && receivesTerrainBonuses)
        {
            currentTerrainData = TerrainManager.Instance.GetTerrainData(gridPosition);
        }
    }
    
    // Tính damage thực tế: Base damage * bonus từ địa hình (ví dụ: đặt trên Ground = x1.0, Water = x0.8)
    public float GetEffectiveDamage()
    {
        float baseDamage = damage;
        if (currentTerrainData != null && receivesTerrainBonuses)
        {
            baseDamage *= currentTerrainData.towerDamageMultiplier; // Nhân với hệ số bonus từ địa hình
        }
        return baseDamage;
    }
    
    public float GetEffectiveRange()
    {
        float baseRange = range;
        if (currentTerrainData != null && receivesTerrainBonuses)
        {
            baseRange *= currentTerrainData.towerRangeMultiplier;
        }
        return baseRange;
    }
    
    public float GetEffectiveFireRate()
    {
        float baseFireRate = fireRate;
        if (currentTerrainData != null && receivesTerrainBonuses)
        {
            baseFireRate *= currentTerrainData.towerFireRateMultiplier;
        }
        return baseFireRate;
    }
    
    // Called when terrain changes
    public void OnTerrainChanged()
    {
        UpdateTerrainBonuses();
    }
}
