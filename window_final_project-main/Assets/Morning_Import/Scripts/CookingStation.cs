using UnityEngine;
using System.Collections.Generic; 

public class CookingStation : MonoBehaviour
{
    // --- 【單例模式】 讓 ToolSource 找得到它 ---
    public static CookingStation Instance;
    private bool isCooking = false;
    [System.Serializable]
    public struct ItemVisual
    {
        public ItemType type;
        public GameObject prefab;

    }

    public List<ItemVisual> visualDatabase; 
    public List<ItemType> itemsOnTable = new List<ItemType>();
    
    private List<GameObject> spawnedVisuals = new List<GameObject>();
    public Transform spawnPoint; 
    
    public ItemType finishedProduct = ItemType.None;

    private void Awake()
    {
        Instance = this; 
    }
    
    public void AddIngredient(ItemType type)
    {
       
        if (finishedProduct != ItemType.None)
        {
            Debug.Log("檯面上還有成品，請先拿走！");
            return;
        }

        itemsOnTable.Add(type);
        SpawnItemVisual(type);
        CheckRecipes();
    }
    
    public void ClearStation()
    {
        
        StopAllCoroutines();
        
        isCooking = false; 

        
        foreach (GameObject obj in spawnedVisuals)
        {
            if (obj != null) Destroy(obj);
        }

        
        spawnedVisuals.Clear();
        itemsOnTable.Clear();

        
        finishedProduct = ItemType.None;

      
    }
    private void OnMouseDown()
    {
        
        if (finishedProduct != ItemType.None)
        {
            PickUpFinishedProduct();
            
        }
        
    }
    void SpawnItemVisual(ItemType type)
    {
        foreach (var item in visualDatabase)
        {
            if (item.type == type)
            {
                GameObject instance = Instantiate(item.prefab);
                instance.transform.SetParent(transform);

               
                if (type == ItemType.Bento)
                {
                    instance.transform.localScale = new Vector3(2f, 2f, 1f);
                }
                else
                {
                    instance.transform.localScale = Vector3.one;
                }

                instance.transform.position = spawnPoint.position;
                instance.transform.localPosition = new Vector3(instance.transform.localPosition.x, instance.transform.localPosition.y, -0.1f);

                if (instance.GetComponent<Collider2D>()) instance.GetComponent<Collider2D>().enabled = false;

                spawnedVisuals.Add(instance);
                break;
            }
        }
    }
    void CheckRecipes()
    {
        // 牛肉湯公式：牛肉 + 碗 + 水
        if (itemsOnTable.Contains(ItemType.Beef) && itemsOnTable.Contains(ItemType.Bowl) && itemsOnTable.Contains(ItemType.WaterPot))
        {
            CompleteCooking(ItemType.BeefSoup);
        }

        // 咖啡公式：磨豆機 + 咖啡豆 + 水
        else if ( itemsOnTable.Contains(ItemType.CoffeeBean) && itemsOnTable.Contains(ItemType.WaterPot))
        {
            
            CompleteCooking(ItemType.Coffee);
        }
        // 光餐公式：三色豆 + 螢光咖哩 + 電話線
        else if (itemsOnTable.Contains(ItemType.Beans) && itemsOnTable.Contains(ItemType.Curry) && itemsOnTable.Contains(ItemType.PhoneLine))
        {
           
            CompleteCooking(ItemType.DormMeal);
        }
        else if (itemsOnTable.Contains(ItemType.Bento))
        {
           
            CompleteCooking(ItemType.Bento);
        }
    }

    void CompleteCooking(ItemType result)
    {
        // 清除食材邏輯與視覺
        itemsOnTable.Clear();
        foreach (GameObject obj in spawnedVisuals) { Destroy(obj); }
        spawnedVisuals.Clear();
        // 設定成品
        finishedProduct = result;
      
        // 在檯面上顯示成品圖案
        SpawnItemVisual(result);
    }
    private Sprite GetFinishedProductSprite()
    {
        // 從你現有的 visualDatabase 找尋對應成品的 Prefab
        foreach (var item in visualDatabase)
        {
            if (item.type == finishedProduct)
            {
                
                return item.prefab.GetComponent<SpriteRenderer>().sprite;
            }
        }
        return null;
    }
    private void PickUpFinishedProduct()
    {
        
        Sprite foodSprite = GetFinishedProductSprite();

        // 交給滑鼠 
        if (MouseFollower.Instance != null && foodSprite != null)
        {
            MouseFollower.Instance.PickUpFood(foodSprite, finishedProduct);
           

            
            foreach (GameObject obj in spawnedVisuals)
            {
                if (obj != null) Destroy(obj);
            }
            spawnedVisuals.Clear();
            itemsOnTable.Clear();

            // 重設狀態
            finishedProduct = ItemType.None;
        }
        else
        {
            Debug.LogWarning("拿取失敗：找不到圖片或 MouseFollower 實體");
        }
    }

}