using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    private GameObject selectedTowerPrefab;
    private PlacementGhost currentGhost;
    public GameObject ghostPrefab;

    public bool HasSelectedTower => selectedTowerPrefab != null;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Cancel with right-click
        if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
        }
    }

    public void SelectTower(GameObject towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;
        CreateGhost(towerPrefab);
    }

    void CreateGhost(GameObject towerPrefab)
    {
        if (ghostPrefab == null) return;
        if (currentGhost != null) Destroy(currentGhost.gameObject);

        GameObject ghostObj = Instantiate(ghostPrefab);
        currentGhost = ghostObj.GetComponent<PlacementGhost>();

        SpriteRenderer towerSprite = towerPrefab.GetComponent<SpriteRenderer>();
        if (towerSprite)
            ghostObj.GetComponent<SpriteRenderer>().sprite = towerSprite.sprite;
    }

    public void ConfirmPlacement(Vector3 position)
    {
        if (selectedTowerPrefab == null) return;

        Instantiate(selectedTowerPrefab, position, Quaternion.identity);
        ClearSelection();
    }

    public void ClearSelection()
    {
        selectedTowerPrefab = null;
        if (currentGhost != null) currentGhost.DestroyGhost();
    }
}
