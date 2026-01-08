using UnityEngine;
using System.Collections; 


public class MouseFollower : MonoBehaviour
{
    public static MouseFollower Instance;

    [Header("ª¬ºA")]
    public ItemType heldFoodType = ItemType.None;
    public bool isHolding = false;

    private SpriteRenderer sr;

    private void Awake()
    {
        Instance = this;
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        isHolding = false;
    }



    public void PickUpFood(Sprite foodSprite, ItemType type)
    {
        heldFoodType = type;
        isHolding = true;
        sr.sprite = foodSprite;
        sr.enabled = true;

      
        if (type == ItemType.Bento)
        {
            transform.localScale = new Vector3(2f, 2f, 1f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private IEnumerator BentoPickupAnimation()
{
   
    transform.localScale = new Vector3(0.42f, 0.42f, 1f);

    
    yield return new WaitForSeconds(0.1f);

   
    transform.localScale = new Vector3(0.35f, 0.35f, 1f);
}

public bool IsHoldingItem()
    {
        return isHolding;
    }

   
    public void ClearHeldItem()
    {
        ResetHand();
    }

    
    public void ResetHand()
    {
        heldFoodType = ItemType.None;
        isHolding = false;
        sr.sprite = null;
        sr.enabled = false;
     
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }
}