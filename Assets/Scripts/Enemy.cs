using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName = "Basic Enemy";
    public float maxHealth = 100f;
    public float speed = 2f;
    public int reward = 10;
    
    [Header("Terrain Interaction")]
    public bool affectedByTerrain = true;
    
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    
    // Runtime variables
    private float currentHealth;
    private bool isDead = false;
    private Vector3Int lastGridPosition;
    private TerrainData currentTerrainData;
    
    // Events
    public System.Action<Enemy> OnEnemyDied;
    public System.Action<Enemy> OnEnemyReachedEnd;
    
    private void Start()
    {
        currentHealth = maxHealth;
        
        // Initialize terrain tracking
        lastGridPosition = TerrainManager.Instance.WorldToGrid(transform.position);
        UpdateTerrainEffects();
    }
    
    private void Update()
    {
        if (isDead) return;
        
        // Update terrain effects
        UpdateTerrainTracking();
        
        // Move towards target
        MoveTowardsTarget();
    }
    
    private void UpdateTerrainTracking()
    {
        Vector3Int currentGridPosition = TerrainManager.Instance.WorldToGrid(transform.position);
        
        if (currentGridPosition != lastGridPosition)
        {
            lastGridPosition = currentGridPosition;
            UpdateTerrainEffects();
        }
    }
    
    // Cập nhật hiệu ứng địa hình: Lấy thông tin địa hình hiện tại để áp dụng bonus/penalty
    private void UpdateTerrainEffects()
    {
        if (!affectedByTerrain || TerrainManager.Instance == null) return;
        
        // Lấy dữ liệu địa hình tại vị trí hiện tại (Ground/Air/Water)
        currentTerrainData = TerrainManager.Instance.GetTerrainData(lastGridPosition);
    }
    
    private void MoveTowardsTarget()
    {
        // Simple movement towards the right side of the screen
        Vector3 targetPosition = transform.position + Vector3.right * 10f;
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        float moveSpeed = GetEffectiveSpeed();
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Check if reached the end
        if (transform.position.x > 15f)
        {
            ReachedEnd();
        }
    }
    
    // Tính tốc độ thực tế: Base speed * hệ số từ địa hình (ví dụ: Water = x0.5, Ground = x1.0)
    private float GetEffectiveSpeed()
    {
        float baseSpeed = speed;
        
        if (affectedByTerrain && currentTerrainData != null)
        {
            // Nhân với hệ số tốc độ từ địa hình (Tiny Defense mechanic)
            baseSpeed *= currentTerrainData.movementSpeedMultiplier;
        }
        
        return baseSpeed;
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        // Visual feedback
        StartCoroutine(DamageFlash());
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Notify listeners
        OnEnemyDied?.Invoke(this);
        
        // Give reward to player (you'll implement GameManager later)
        // GameManager.Instance?.AddMoney(reward);
        
        // Destroy enemy
        Destroy(gameObject);
    }
    
    public void ReachedEnd()
    {
        if (isDead) return;
        
        // Deal damage to player (you'll implement GameManager later)
        // GameManager.Instance?.TakeDamage(1);
        
        // Notify listeners
        OnEnemyReachedEnd?.Invoke(this);
        
        // Destroy enemy
        Destroy(gameObject);
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}
