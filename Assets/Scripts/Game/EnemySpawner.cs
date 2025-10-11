using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySquad
{
    public string squadName = "Squad";          // Tên squad
    public List<GameObject> enemies = new();    // Danh sách enemy trong squad
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    void Awake() { instance = this; }

    public float spawnInterval = 2f;                      // Thời gian giữa các lượt spawn

    public List<Transform> spawnPoints = new();           // Danh sách điểm spawn

    public List<EnemySquad> spawnSquads = new();          // Danh sách đội hình enemy

    private Coroutine spawnRoutine;                       // Dùng để quản lý coroutine

    void Start()
    {
        // ✅ Gọi hàm bắt đầu spawn
        StartSpawning();
    }

    public void StartSpawning()
    {
        // Ngăn chạy trùng coroutine
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnEnemySquad();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemySquad()
    {
        if (spawnSquads.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogWarning("⚠️ Chưa có squad hoặc spawn point nào được gán!");
            return;
        }

        // Random 1 squad
        int randomSquadID = Random.Range(0, spawnSquads.Count);
        EnemySquad selectedSquad = spawnSquads[randomSquadID];

        // Random 1 vị trí spawn
        int randomSpawnPointID = Random.Range(0, spawnPoints.Count);
        Vector3 spawnPosition = spawnPoints[randomSpawnPointID].position;

        // Spawn tất cả enemy trong squad tại cùng vị trí
        foreach (GameObject enemyPrefab in selectedSquad.enemies)
        {
            if (enemyPrefab != null)
            {
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }

        Debug.Log($"🌀 Spawned {selectedSquad.squadName} tại {spawnPoints[randomSpawnPointID].name}");
    }
}
