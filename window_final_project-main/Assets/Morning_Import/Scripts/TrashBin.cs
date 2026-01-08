using UnityEngine;

public class TrashBin : MonoBehaviour
{
    private void OnMouseDown()
    {
        
        if (CookingStation.Instance != null)
        {
            CookingStation.Instance.ClearStation();
        }

      
        if (MouseFollower.Instance != null)
        {
            MouseFollower.Instance.ClearHeldItem();
        }

       
        transform.localScale = Vector3.one * 1.1f;
        Invoke("ResetScale", 0.1f);
    }

    void ResetScale() => transform.localScale = Vector3.one;
}