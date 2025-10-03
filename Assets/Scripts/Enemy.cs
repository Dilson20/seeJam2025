using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Configuration")]
    public EnemyType enemyType;
    
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    
    // Runtime variables
    private float currentHealth;
    private bool isDead = false;
    private bool isFalling = false;
    private Vector3Int lastGridPosition;
    private TerrainBehavior currentTerrainBehavior;
    private Vector3 startPosition;
    private float baseSpeed;
    private float baseHealth;
    
    // Events
    public System.Action<Enemy> OnEnemyDied;
    public System.Action<Enemy> OnEnemyReachedEnd;
    
    private void Start()
    {
        InitializeFromEnemyType();
        
        startPosition = transform.position;
        
        // Initialize terrain tracking
        lastGridPosition = TerrainManager.Instance.WorldToGrid(transform.position);
        UpdateTerrainEffects();
    }
    
    private void InitializeFromEnemyType()
    {
        if (enemyType == null) return;
        
        // Đặt giá trị cơ bản từ EnemyType
        baseHealth = enemyType.maxHealth;
        baseSpeed = enemyType.speed;
        currentHealth = baseHealth;
        
        // Áp dụng cài đặt hình ảnh
        if (spriteRenderer != null && enemyType.enemySprite != null)
        {
            spriteRenderer.sprite = enemyType.enemySprite;
            spriteRenderer.color = enemyType.enemyColor;
            spriteRenderer.transform.localScale = Vector3.one * enemyType.spriteScale;
        }
    }
    
    private void Update()
    {
        if (isDead) return;
        
        if (isFalling)
        {
            HandleFalling();
            return;
        }
        
        // Update terrain effects
        UpdateTerrainTracking();
        
        // Check if enemy should fall
        CheckForFall();
        
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
        if (!enemyType.affectedByTerrain || TerrainManager.Instance == null) return;
        
        // Lấy behavior địa hình tại vị trí hiện tại
        currentTerrainBehavior = TerrainManager.Instance.GetTerrainBehavior(lastGridPosition);
        
        // Áp dụng behavior nếu có
        if (currentTerrainBehavior != null)
        {
            currentTerrainBehavior.ApplyToEnemy(this);
        }
    }
    
    // Kiểm tra xem enemy có nên rơi không (Tiny Defense mechanic)
    private void CheckForFall()
    {
        if (enemyType.isFlying || !enemyType.canFall || isFalling) return;
        
        // Kiểm tra vị trí hiện tại và vị trí tiếp theo
        Vector3Int currentGridPos = TerrainManager.Instance.WorldToGrid(transform.position);
        Vector3 nextPosition = transform.position + enemyType.moveDirection * GetEffectiveSpeed() * Time.deltaTime;
        Vector3Int nextGridPos = TerrainManager.Instance.WorldToGrid(nextPosition);
        
        // Nếu vị trí tiếp theo khác với vị trí hiện tại
        if (nextGridPos != currentGridPos)
        {
            TerrainBehavior nextTerrainBehavior = TerrainManager.Instance.GetTerrainBehavior(nextGridPos);
            TerrainBehavior currentTerrainBehavior = TerrainManager.Instance.GetTerrainBehavior(currentGridPos);
            
            // Kiểm tra nếu enemy đang ở trên solid ground và bước tiếp theo không phải solid ground
            if (currentTerrainBehavior != null && currentTerrainBehavior.isSolidGround &&
                nextTerrainBehavior != null && !nextTerrainBehavior.isSolidGround)
            {
                // Enemy sẽ rơi!
                StartFalling();
            }
        }
    }
    
    // Bắt đầu rơi
    private void StartFalling()
    {
        isFalling = true;
        // Có thể thêm hiệu ứng âm thanh hoặc animation ở đây
    }
    
    // Xử lý khi enemy đang rơi
    private void HandleFalling()
    {
        // Rơi xuống dưới
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        
        // Kiểm tra xem có chạm đất không
        Vector3Int gridPos = TerrainManager.Instance.WorldToGrid(transform.position);
        TerrainData terrainData = TerrainManager.Instance.GetTerrainData(gridPos);
        
        if (terrainData != null && terrainData.isSolidGround)
        {
            // Chạm đất - dừng rơi
            isFalling = false;
        }
        else if (terrainData != null && terrainData.killsEnemiesOnFall)
        {
            // Rơi vào void - chết
            Die();
        }
        else if (transform.position.y < fallDeathHeight) // Rơi quá xa - chết
        {
            Die();
        }
    }
    
    private void MoveTowardsTarget()
    {
        // Di chuyển theo hướng đã cấu hình
        Vector3 direction = enemyType.moveDirection.normalized;
        float moveSpeed = GetEffectiveSpeed();
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Kiểm tra xem đã đến cuối chưa dựa trên khoảng cách từ vị trí bắt đầu
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= enemyType.moveDistance)
        {
            ReachedEnd();
        }
    }
    
    // Tính tốc độ thực tế: Base speed * hệ số từ địa hình (ví dụ: Water = x0.5, Ground = x1.0)
    private float GetEffectiveSpeed()
    {
        float effectiveSpeed = baseSpeed;
        
        // Áp dụng multiplier từ terrain behavior hiện tại
        if (currentTerrainBehavior != null)
        {
            effectiveSpeed *= currentTerrainBehavior.movementSpeedMultiplier;
        }
        
        return effectiveSpeed;
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
        return currentHealth / baseHealth;
    }
    
    // Public methods for TerrainBehavior to call
    public void ApplySpeedMultiplier(float multiplier)
    {
        baseSpeed = enemyType.speed * multiplier;
    }
    
    public void ApplyHealthMultiplier(float multiplier)
    {
        baseHealth = enemyType.maxHealth * multiplier;
        currentHealth = baseHealth;
    }
    
    public void EnableFallingDetection(float threshold)
    {
        // Triển khai phát hiện rơi
    }
    
    public void TriggerSplashEffect()
    {
        // Triển khai hiệu ứng splash
    }
    
    public int GetReward()
    {
        return enemyType.reward;
    }
}
