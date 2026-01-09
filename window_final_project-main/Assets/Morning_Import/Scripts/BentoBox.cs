using UnityEngine;

public class BentoBox : MonoBehaviour
{
    public ItemType bentoType = ItemType.Bento;
    public Sprite bentoSprite;

    void Start()
    {
        
        transform.localScale = new Vector3(0.35f, 0.35f, 1f);

       
        Vector3 pos = transform.position;
        pos.z = -5f;
        transform.position = pos;
    }

    private void OnMouseDown()
    {
        if (MouseFollower.Instance != null && !MouseFollower.Instance.isHolding)
        {
            MouseFollower.Instance.PickUpFood(bentoSprite, bentoType);
            
        }
    }
}