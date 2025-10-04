using UnityEngine;

public class GroundTile : MonoBehaviour
{
    [Header("Slot Settings")]
    public GameObject slotPrefab;
    public float verticalOffset = 0.5f;

    private GameObject spawnedSlot;

    void Start()
    {
        if (slotPrefab == null)
        {
            Debug.LogWarning("GroundTile: Slot Prefab not assigned!");
            return;
        }

        Vector3 slotPos = transform.position + new Vector3(0, verticalOffset, -0.1f);
        spawnedSlot = Instantiate(slotPrefab, slotPos, Quaternion.identity, transform);
    }
}
