using UnityEngine;

[CreateAssetMenu(fileName = "TowerType", menuName = "Tower Defense/Tower Type")]
public class TowerType : ScriptableObject
{
    [Header("Basic Stats")]
    public string towerName = "Basic Tower";
    public int cost = 50;
    public float damage = 10f;
    public float range = 3f;
    public float fireRate = 1f;
    public float rotationSpeed = 90f;
    
    [Header("Targeting")]
    public bool autoTarget = true;
    public string targetTag = "Enemy";
    public LayerMask targetLayerMask = -1;
    
    [Header("Terrain Interaction")]
    public bool receivesTerrainBonuses = true;
    
    [Header("Visual")]
    public Sprite towerSprite;
    public Color towerColor = Color.white;
    public float spriteScale = 1f;
    
    [Header("Prefab")]
    public GameObject towerPrefab;
    public GameObject projectilePrefab;
}
