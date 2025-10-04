using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    public GameObject towerPrefab;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            TowerManager.Instance.SelectTower(towerPrefab);
        });
    }
}
