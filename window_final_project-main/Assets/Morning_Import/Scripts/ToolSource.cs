using UnityEngine;

public class ToolSource : MonoBehaviour
{
   
    public ItemType itemToGive = ItemType.None;

    private void OnMouseDown()
    {
        
        transform.localScale = Vector3.one * 1.2f;
        Invoke("ResetScale", 0.1f);

        

        
        if (CookingStation.Instance != null)
        {
            CookingStation.Instance.AddIngredient(itemToGive);
            
           
        }
        else
        {
            
            Debug.LogError("找不到 CookingStation！請確保場景中有 CookingStation 並啟動了 Instance。");
        }
    }

    void ResetScale() => transform.localScale = Vector3.one;
}