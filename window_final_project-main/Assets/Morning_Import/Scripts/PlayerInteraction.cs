using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
  
    public ItemType currentHeldFood = ItemType.None;

    void Update()
    {
     
        if (Input.GetMouseButtonDown(0))
        {
           
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
               
                if (hit.collider.CompareTag("Food"))
                {
                   
                    string objName = hit.collider.gameObject.name;

                    if (objName.Contains("Coffee")) currentHeldFood = ItemType.Coffee;
                    else if (objName.Contains("Bento")) currentHeldFood = ItemType.Bento;
                    else if (objName.Contains("BeefSoup")) currentHeldFood = ItemType.BeefSoup;
                    else if (objName.Contains("DormMeal")) currentHeldFood = ItemType.DormMeal;

                    Debug.Log("你拿起了：" + currentHeldFood);
                }

              
                LakeAnimal animal = hit.collider.GetComponent<LakeAnimal>();
                if (animal != null && currentHeldFood != ItemType.None)
                {
                    Debug.Log("把食物餵給動物！");
                    animal.ReceiveFood(currentHeldFood);
                    currentHeldFood = ItemType.None; 
                }
            }
        }
    }
}