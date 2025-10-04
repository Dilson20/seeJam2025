using UnityEngine;

public class TileSlot : MonoBehaviour
{
    public bool isOccupied = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseEnter()
    {
        if (!isOccupied && TowerManager.Instance.HasSelectedTower)
            sr.color = Color.green;
    }

    void OnMouseExit()
    {
        sr.color = Color.white;
    }

    void OnMouseDown()
    {
        if (isOccupied || !TowerManager.Instance.HasSelectedTower)
            return;

        TowerManager.Instance.ConfirmPlacement(transform.position);
        isOccupied = true;
        sr.color = Color.white;
    }
}
