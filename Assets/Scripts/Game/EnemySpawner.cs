using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    void Awake() { instance = this; }

    // Enemy prefabs
    public List<GameObject> prefabs;
    // Enemy spawn root points
    public List<Transform> spawnPoints;
    // Enemy spawn interval
    public float spawnInterval = 2f;

    // ✅ Danh sách lưu vị trí gốc
    private List<Vector3> originalSpawnPositions = new List<Vector3>();

    void Start()
    {
        // Lưu lại vị trí gốc của các spawn point
        foreach (Transform point in spawnPoints)
        {
            originalSpawnPositions.Add(point.position);
        }

        StartSpawning();
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        SpawnEnemy();
        yield return new WaitForSeconds(spawnInterval);
        StartCoroutine(SpawnDelay());
    }

    void SpawnEnemy()
    {
        int randomPrefabID = Random.Range(0, prefabs.Count);
        int randomSpawnPointID = Random.Range(0, originalSpawnPositions.Count);

        // ✅ Lấy đúng vị trí gốc ban đầu
        Vector3 spawnPosition = originalSpawnPositions[randomSpawnPointID];

        Instantiate(prefabs[randomPrefabID], spawnPosition, Quaternion.identity);
    }
}
