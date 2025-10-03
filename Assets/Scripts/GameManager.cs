using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int startingMoney = 100;
    public int startingLives = 10;
    public float gameSpeed = 1f; // Game speed multiplier
    
    [Header("Enemy Spawning")]
    public float spawnInterval = 2f; // Time between enemy spawns
    public int maxEnemiesOnScreen = 20; // Maximum enemies at once
    public GameObject[] enemyPrefabs; // Available enemy types
    
    [Header("Tower Settings")]
    public GameObject[] towerPrefabs; // Available tower types
    
    [Header("Game State")]
    public bool gameStarted = false;
    public bool gamePaused = false;
    
    // Runtime variables
    private int currentMoney;
    private int currentLives;
    private int currentWave = 1;
    private float lastSpawnTime;
    private List<Enemy> activeEnemies = new List<Enemy>();
    
    // Singleton
    public static GameManager Instance { get; private set; }
    
    // Events
    public System.Action<int> OnMoneyChanged;
    public System.Action<int> OnLivesChanged;
    public System.Action<int> OnWaveChanged;
    public System.Action OnGameOver;
    public System.Action OnGameWin;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Subscribe to enemy events
        Enemy.OnEnemyDied += OnEnemyDied;
        Enemy.OnEnemyReachedEnd += OnEnemyReachedEnd;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        Enemy.OnEnemyDied -= OnEnemyDied;
        Enemy.OnEnemyReachedEnd -= OnEnemyReachedEnd;
    }
    
    private void Update()
    {
        if (!gameStarted || gamePaused) return;
        
        // Handle enemy spawning
        if (Time.time - lastSpawnTime >= spawnInterval && activeEnemies.Count < maxEnemiesOnScreen)
        {
            SpawnRandomEnemy();
        }
    }
    
    private void InitializeGame()
    {
        currentMoney = startingMoney;
        currentLives = startingLives;
        currentWave = 1;
        
        // Notify UI of initial values
        OnMoneyChanged?.Invoke(currentMoney);
        OnLivesChanged?.Invoke(currentLives);
        OnWaveChanged?.Invoke(currentWave);
    }
    
    public void StartGame()
    {
        gameStarted = true;
        gamePaused = false;
        Time.timeScale = gameSpeed;
    }
    
    public void PauseGame()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : gameSpeed;
    }
    
    public void SpawnRandomEnemy()
    {
        if (enemyPrefabs.Length == 0) return;
        
        // Get random spawn position from TerrainManager
        Vector3Int spawnPos = TerrainManager.Instance.GetRandomSpawnPosition();
        Vector3 worldPos = TerrainManager.Instance.GridToWorld(spawnPos);
        
        // Select random enemy prefab
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        
        // Spawn enemy
        GameObject enemyObj = Instantiate(enemyPrefab, worldPos, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        
        if (enemy != null)
        {
            activeEnemies.Add(enemy);
        }
        
        lastSpawnTime = Time.time;
    }
    
    public void SpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        if (activeEnemies.Count >= maxEnemiesOnScreen) return;
        
        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        
        if (enemy != null)
        {
            activeEnemies.Add(enemy);
        }
    }
    
    private void OnEnemyDied(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            AddMoney(enemy.reward);
        }
    }
    
    private void OnEnemyReachedEnd(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            TakeDamage(1);
        }
    }
    
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    public void SpendMoney(int amount)
    {
        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }
    
    public bool CanAfford(int cost)
    {
        return currentMoney >= cost;
    }
    
    public void TakeDamage(int damage)
    {
        currentLives -= damage;
        OnLivesChanged?.Invoke(currentLives);
        
        if (currentLives <= 0)
        {
            GameOver();
        }
    }
    
    public void NextWave()
    {
        currentWave++;
        OnWaveChanged?.Invoke(currentWave);
        
        // Increase difficulty (you can customize this)
        spawnInterval *= 0.9f; // Spawn faster
        maxEnemiesOnScreen += 2; // More enemies on screen
    }
    
    private void GameOver()
    {
        gameStarted = false;
        OnGameOver?.Invoke();
    }
    
    private void GameWin()
    {
        gameStarted = false;
        OnGameWin?.Invoke();
    }
    
    // Public getters
    public int GetCurrentMoney() => currentMoney;
    public int GetCurrentLives() => currentLives;
    public int GetCurrentWave() => currentWave;
    public List<Enemy> GetActiveEnemies() => activeEnemies;
    public bool IsGameStarted() => gameStarted;
    public bool IsGamePaused() => gamePaused;
}
