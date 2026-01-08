using UnityEngine;
using System.Collections.Generic;

public class AnimalSpawner : MonoBehaviour
{
    [Header("基礎配置")]
    public GameObject[] animalPrefabs;
    public Transform[] lilyPads;

    [Header("特殊機率")]
    [Range(0, 1)] public float highAchieverChance = 0.2f;

    private bool[] isOccupied = new bool[4];

    void Start()
    {
        
        Invoke("SpawnSequence", 1f);
    }

    void SpawnSequence()
    {
        TrySpawnAnimal();

        
        float nextSpawnTime = 5f; // 預設值
        if (MorningLevelManager.Instance != null)
        {
            nextSpawnTime = MorningLevelManager.Instance.GetCurrentLevelData().spawnInterval;
        }

        // 根據關卡設定的時間，決定下一隻動物什麼時候出現
        Invoke("SpawnSequence", nextSpawnTime);
    }

    void TrySpawnAnimal()
    {

        if (MorningLevelManager.Instance == null)
        {
            Debug.LogError("找不到 LevelManager！請確保場景中有這個物件。");
            return;
        }

        //  獲取當前關卡資訊
        GameLevelData currentLevel = MorningLevelManager.Instance.GetCurrentLevelData();

        // 隨機選取該關卡准許的動物 Prefab
        List<GameObject> pool = currentLevel.allowedPrefabs;
        if (pool == null || pool.Count == 0) return;

        int randomIdx = Random.Range(0, pool.Count);


        // --- 防呆檢查 ---
        if (animalPrefabs == null || animalPrefabs.Length == 0)
        {
            Debug.LogWarning("Spawner 沒放動物 Prefab！");
            return;
        }
        if (currentLevel.possibleFoods == null || currentLevel.possibleFoods.Length == 0)
        {
            Debug.LogWarning("這一關忘了設定食物清單！");
            return;
        }

        // 檢查空位
        List<int> freeSlots = new List<int>();
        for (int i = 0; i < isOccupied.Length; i++)
        {
            if (!isOccupied[i]) freeSlots.Add(i);
        }

        if (freeSlots.Count == 0) return;

        //  隨機選位子與 Prefab
        int randomSlotIndex = freeSlots[Random.Range(0, freeSlots.Count)];

        GameObject prefabToSpawn = pool[randomIdx];


        Transform targetPad = lilyPads[randomSlotIndex];
        GameObject animal = Instantiate(prefabToSpawn, targetPad.position, Quaternion.identity);


        animal.transform.SetParent(targetPad);
        animal.transform.localPosition = Vector3.zero;
        animal.transform.localRotation = Quaternion.identity;

        isOccupied[randomSlotIndex] = true; 

        LakeAnimal animalScript = animal.GetComponent<LakeAnimal>();
        if (animalScript != null)
        {
            bool isSpecial = Random.value < highAchieverChance;

            // 從這一關「允許的食物清單」中隨機選一個想吃的
            int foodIdx = Random.Range(0, currentLevel.possibleFoods.Length);
            animalScript.favoriteFood = currentLevel.possibleFoods[foodIdx];

            
            animalScript.SetupAnimal(randomSlotIndex, isSpecial, this, currentLevel.animalWaitTime);
            
        }
    }

    public void ReleaseSlot(int index)
    {
        if (index >= 0 && index < isOccupied.Length)
        {
            isOccupied[index] = false;
            
        }
    }
}
