using UnityEngine;

[System.Serializable] 
public class TowerBlueprint
{
    public string name;       // 塔的名字
    public int cost;          // 建造價格
    public GameObject prefab; // Level 1 的塔 (Prefab)

    [Header("升級設定")]
    public GameObject upgradedPrefab; // Level 2 的塔 (Prefab)
    public int upgradeCost;           // 升級價格
}