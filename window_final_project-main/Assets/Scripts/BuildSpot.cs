using UnityEngine;
using UnityEngine.EventSystems; 

// 2. 在這裡加上 ", IPointerClickHandler" (讓這個物件能接收點擊事件)
public class BuildSpot : MonoBehaviour, IPointerClickHandler
{
    public GameObject currentTower;
    public TowerBlueprint towerBlueprint;
    private bool isUpgraded = false;

    // 3. 我們不再用 OnMouseDown，改用這個更強大的函式
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("【成功】終於點到了！(透過 EventSystem)");

        // 呼叫工頭
        if (BuildManager.instance != null)
        {
            BuildManager.instance.SelectSpot(this);
        }
    }

    // --- 以下保持不變 ---
    public void BuildTower(TowerBlueprint blueprint)
    {
        if (GameManager.instance != null)
        {
            currentTower = Instantiate(blueprint.prefab, transform.position, Quaternion.identity);
            towerBlueprint = blueprint;
            isUpgraded = false;
        }
    }

    public void UpgradeTower()
    {
        if (isUpgraded) return;
        Destroy(currentTower);
        currentTower = Instantiate(towerBlueprint.upgradedPrefab, transform.position, Quaternion.identity);
        isUpgraded = true;
        Debug.Log("塔升級完成！");
    }
}