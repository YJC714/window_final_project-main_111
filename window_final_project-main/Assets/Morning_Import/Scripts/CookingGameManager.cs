using UnityEngine;

// Enum 保持原樣
public enum ItemType
{
    None,
    Beef, Bowl, WaterPot,      // 牛肉湯組
    CoffeeBean,                // 咖啡組
    Beans, Curry, PhoneLine,   // 光餐組
    BeefSoup, Coffee, DormMeal, Bento // 成品
}

// 類別名稱改為 CookingGameManager
public class CookingGameManager : MonoBehaviour
{
    public static CookingGameManager Instance;
    public ItemType currentItem = ItemType.None;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        Debug.Log("失敗大學(早晨)：啟動！");
    }

    public void PickUpItem(ItemType newItem)
    {
        currentItem = newItem;
    }
}