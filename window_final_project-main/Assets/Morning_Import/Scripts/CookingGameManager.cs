using UnityEngine;


public enum ItemType
{
    None,
    Beef, Bowl, WaterPot,     
    CoffeeBean,                
    Beans, Curry, PhoneLine,   
    BeefSoup, Coffee, DormMeal, Bento
}

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