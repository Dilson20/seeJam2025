using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "Tower Defense/Enemy Type")]
public class EnemyType : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName = "Basic Enemy";
    public float maxHealth = 100f;
    public float speed = 2f;
    public int reward = 10;
    
    [Header("Enemy Properties")]
    public bool isFlying = false;
    public bool canFall = true;
    public bool affectedByTerrain = true;
    
    [Header("Movement")]
    public Vector3 moveDirection = Vector3.right;
    public float moveDistance = 15f;
    public float fallSpeed = 5f;
    public float fallDeathHeight = -10f;
    
    [Header("Visual")]
    public Sprite enemySprite;
    public Color enemyColor = Color.white;
    public float spriteScale = 1f;
    
    [Header("Prefab")]
    public GameObject enemyPrefab;
}
