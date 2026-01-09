using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LakeAnimal : MonoBehaviour
{
    [Header("身分設定")]
    public AnimalType myType;
    [Header("食物設定")]
    public ItemType favoriteFood;

    [System.Serializable]
    public struct FoodSpritePair
    {
        public ItemType type;
        public Sprite image;
        public Color foodColor;
    }
    public FoodSpritePair[] foodDatabase;

    [Header("UI 連結")]
    public SpriteRenderer foodIconDisplay; 
    public Slider patienceSlider;
    public GameObject testPaperVisual;
    public SpriteRenderer bodyRenderer;
   
    public Sprite happySprite; 
    public Sprite angrySprite;
    public Sprite[] happyAnimationSprites;
    public float animationSpeed = 0.15f;
    
    
    public SpriteRenderer emotionRenderer;
    
    [Header("數值設定")]
    public float waitTime = 10f;
    public bool hasTestPaper = false;

    private int slotIndex;
    private AnimalSpawner spawner;
    private bool isSatiated = false;

   
    public void SetupAnimal(int index, bool isElite, AnimalSpawner manager, float duration)
    {
        this.slotIndex = index;
        this.spawner = manager;
        this.hasTestPaper = isElite;
        this.waitTime = duration;

        
        if (patienceSlider != null)
        {
            patienceSlider.minValue = 0;
            patienceSlider.maxValue = waitTime;
            patienceSlider.value = waitTime;
        }

        if (testPaperVisual != null)
            testPaperVisual.SetActive(hasTestPaper);

        UpdateOrderVisual();
    }

    void Start()
    {
        
        if (bodyRenderer == null)
            bodyRenderer = GetComponentInChildren<SpriteRenderer>();

        if (bodyRenderer != null) bodyRenderer.color = Color.white;
        if (emotionRenderer != null) emotionRenderer.gameObject.SetActive(false);/////0108


        StartCoroutine(JumpInAnimation());
        DetermineIfCarryingItem();
    }


    void DetermineIfCarryingItem()
    {
        if (specialItemVisual == null)
        {
            Debug.LogError($"[道具錯誤] {gameObject.name} 沒有掛載 specialItemVisual！");
            return;
        }

        isCarryingSpecialItem = false;
        specialItemVisual.SetActive(false);

        if (MorningLevelManager.Instance == null) return;

        int currentLevel = MorningLevelManager.Instance.currentLevelIndex;
        bool hasPermission = false;

       
        if (currentLevel == 0 && myType == AnimalType.Duck) hasPermission = true;
        else if (currentLevel == 1 && (myType == AnimalType.Turtle || myType == AnimalType.Salamander)) hasPermission = true;
        else if (currentLevel >= 2) hasPermission = true;

       

        if (hasPermission)
        {
           
             if (Random.value < 0.6f) 
            {
                isCarryingSpecialItem = true;
                specialItemVisual.SetActive(true);
                Debug.Log($"<color=yellow>[道具成功] ★ {gameObject.name} 已顯示道具圖示！</color>");
            }
        }
    }
  
    IEnumerator JumpInAnimation()
    {
        Vector3 targetPos = transform.localPosition;
        Vector3 startPos = targetPos + Vector3.down * 2f;
        transform.localPosition = startPos;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2.5f;
            transform.localPosition = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

       
        StartCoroutine(WaitTimer());
        StartCoroutine(IdleBobbing());
    }

    IEnumerator WaitTimer()
    {
        float timer = waitTime;
        while (timer > 0 && !isSatiated)
        {
            timer -= Time.deltaTime;
            if (patienceSlider != null)
                patienceSlider.value = timer;
            yield return null;
        }

        if (!isSatiated) Fail();
    }

   
    public void ReceiveFood(ItemType food)
    {
        if (isSatiated) return;

        if (food == favoriteFood)
        {
            Success();
        }
        else
        {
           
            if (MorningLevelManager.Instance != null)
            {
                MorningLevelManager.Instance.ResetCombo();
            }
        }
    }


   
    IEnumerator PlayHappyAnimation()
    {
        Debug.Log("<color=cyan>【動畫】協程開始跑了！</color>");

        emotionRenderer.gameObject.SetActive(true);
        emotionRenderer.color = Color.white;

        int frame = 0;
        float elapsed = 0f;
        float duration = 1.0f;

        while (elapsed < duration)
        {
            if (happyAnimationSprites[frame] != null)
            {
                emotionRenderer.sprite = happyAnimationSprites[frame];
                Debug.Log($"【動畫】正在更換為第 {frame} 張圖片：" + happyAnimationSprites[frame].name);
            }

            frame = (frame + 1) % happyAnimationSprites.Length;
            yield return new WaitForSeconds(animationSpeed);
            elapsed += animationSpeed;
        }

        Debug.Log("<color=cyan>【動畫】播放完畢，關閉物件。</color>");
        emotionRenderer.gameObject.SetActive(false);
    }
   
    void Success()
    {
        if (isSatiated) return;
        isSatiated = true;
       
        bodyRenderer.sprite = happySprite;
        bodyRenderer.color = Color.white;
        if (emotionRenderer != null)
        {
            
            StartCoroutine(PlayHappyAnimation());
        }
        
       
        if (MorningLevelManager.Instance != null)
        {
            MorningLevelManager.Instance.AddAnimalCount(myType);
            if (isCarryingSpecialItem)
            {
                MorningLevelManager.Instance.MarkAnimalAsUpgradeable(this.myType);
            }
        }

     
       
        if (MouseFollower.Instance != null)
        {
            MouseFollower.Instance.ClearHeldItem();
        }

        StartCoroutine(JumpOut());

        
    }
   
    private void OnMouseDown()
    {
      
        if (MouseFollower.Instance != null && MouseFollower.Instance.heldFoodType != ItemType.None)
        {
           
            ReceiveFood(MouseFollower.Instance.heldFoodType);
        }
    }

    void Fail()
    {
        if (isSatiated) return;
        isSatiated = true;

      
        if (MorningLevelManager.Instance != null)
        {
            MorningLevelManager.Instance.ResetCombo();
        }

        if (bodyRenderer != null)
        {
            bodyRenderer.sprite = angrySprite;
           
            bodyRenderer.color = Color.white;
         
        }
        StartCoroutine(JumpOut());
    }

    IEnumerator JumpOut()
    {
        yield return new WaitForSeconds(0.8f);
        float t = 0;
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + Vector3.down * 3f;

        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator IdleBobbing()
    {
        Vector3 startPos = transform.localPosition;
        while (!isSatiated)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * 4f) * 0.1f;
            transform.localPosition = new Vector3(startPos.x, newY, transform.localPosition.z);
            yield return null;
        }
    }

    public void UpdateOrderVisual()
    {
        if (foodIconDisplay == null)
        {
            Debug.LogError($"{gameObject.name} 找不到 Food Icon Display 欄位！");
            return;
        }

        foreach (var pair in foodDatabase)
        {
            if (pair.type == favoriteFood)
            {
                foodIconDisplay.sprite = pair.image;
               
                foodIconDisplay.color = Color.white;
                foodIconDisplay.gameObject.SetActive(true);
                foodIconDisplay.sortingOrder = 999;

               
                Debug.Log($"<color=cyan>成功！{gameObject.name} 想要吃 {favoriteFood}，已更換圖片。</color>");
                return;
            }
        }

      
    }
   
    [Header("道具設定")]
    public bool isCarryingSpecialItem = false;
    public GameObject specialItemVisual; 

    
    void OnDestroy()
    {
       
        if (spawner != null) spawner.ReleaseSlot(slotIndex);
    }
}