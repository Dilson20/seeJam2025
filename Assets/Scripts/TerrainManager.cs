using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    
    [Header("Terrain Database")]
    public TerrainDatabase terrainDatabase;
    
    [Header("Terrain Tile Mapping")]
    public TerrainTileMapping[] terrainTileMappings;
    
    // Cache for terrain data lookup
    private Dictionary<Vector3Int, TerrainData> terrainCache = new Dictionary<Vector3Int, TerrainData>();
    private Dictionary<TileBase, TerrainType> tileToTerrainType = new Dictionary<TileBase, TerrainType>();
    
    [System.Serializable]
    public class TerrainTileMapping
    {
        public TileBase tile;
        public TerrainType terrainType;
    }
    
    public static TerrainManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeTerrainMapping();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeTerrainMapping()
    {
        // Build the mapping from tiles to terrain types
        foreach (var mapping in terrainTileMappings)
        {
            if (mapping.tile != null)
            {
                tileToTerrainType[mapping.tile] = mapping.terrainType;
            }
        }
    }
    
    // Hàm quan trọng nhất: Lấy thông tin địa hình tại vị trí grid
    // Trả về TerrainData chứa tất cả thuộc tính của địa hình (tốc độ di chuyển, có thể đặt tháp không, bonus damage, etc.)
    public TerrainData GetTerrainData(Vector3Int gridPosition)
    {
        // Kiểm tra cache trước (để tăng hiệu suất)
        if (terrainCache.ContainsKey(gridPosition))
        {
            return terrainCache[gridPosition];
        }
        
        // Lấy tile tại vị trí này từ tilemap
        TileBase tile = groundTilemap.GetTile(gridPosition);
        
        if (tile != null && tileToTerrainType.ContainsKey(tile))
        {
            // Chuyển đổi tile thành loại địa hình (Ground/Air/Water)
            TerrainType terrainType = tileToTerrainType[tile];
            // Lấy dữ liệu chi tiết của địa hình này
            TerrainData terrainData = terrainDatabase.GetTerrainData(terrainType);
            
            // Lưu vào cache để lần sau truy cập nhanh hơn
            terrainCache[gridPosition] = terrainData;
            return terrainData;
        }
        
        // Mặc định trả về Ground nếu không tìm thấy tile hoặc mapping
        TerrainData defaultTerrain = terrainDatabase.GetTerrainData(TerrainType.Ground);
        terrainCache[gridPosition] = defaultTerrain;
        return defaultTerrain;
    }
    
    public bool CanPlaceTower(Vector3Int gridPosition)
    {
        TerrainData terrainData = GetTerrainData(gridPosition);
        return terrainData != null && terrainData.allowsTowerPlacement;
    }
    
    public bool CanMoveThrough(Vector3Int gridPosition)
    {
        TerrainData terrainData = GetTerrainData(gridPosition);
        return terrainData != null && terrainData.allowsMovement;
    }
    
    public float GetMovementSpeedMultiplier(Vector3Int gridPosition)
    {
        TerrainData terrainData = GetTerrainData(gridPosition);
        return terrainData != null ? terrainData.movementSpeedMultiplier : 1f;
    }
    
    public Vector3Int WorldToGrid(Vector3 worldPosition)
    {
        return groundTilemap.WorldToCell(worldPosition);
    }
    
    public Vector3 GridToWorld(Vector3Int gridPosition)
    {
        return groundTilemap.CellToWorld(gridPosition) + groundTilemap.tileAnchor;
    }
    
    // Clear cache when tiles are modified
    public void RefreshTerrainCache()
    {
        terrainCache.Clear();
    }
    
    // Get all positions that can spawn enemies
    public List<Vector3Int> GetValidSpawnPositions()
    {
        List<Vector3Int> spawnPositions = new List<Vector3Int>();
        
        BoundsInt bounds = groundTilemap.cellBounds;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TerrainData terrainData = GetTerrainData(pos);
                
                if (terrainData != null && terrainData.allowsMovement && terrainData.enemySpawnWeight > 0)
                {
                    spawnPositions.Add(pos);
                }
            }
        }
        
        return spawnPositions;
    }
}
