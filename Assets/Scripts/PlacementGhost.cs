using UnityEngine;

public class PlacementGhost : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color validColor = new Color(0, 1, 0, 0.5f);
    private Color invalidColor = new Color(1, 0, 0, 0.5f);
    private Vector3 snapPos;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        FollowMouse();
        UpdateValidityColor();
    }

    void FollowMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit && hit.GetComponent<TileSlot>())
            snapPos = hit.transform.position;
        else
            snapPos = mousePos;

        transform.position = snapPos;
    }

    void UpdateValidityColor()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit && hit.GetComponent<TileSlot>() && !hit.GetComponent<TileSlot>().isOccupied)
            sr.color = validColor;
        else
            sr.color = invalidColor;
    }

    public void DestroyGhost()
    {
        Destroy(gameObject);
    }
}
