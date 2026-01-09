using UnityEngine;
using UnityEngine.EventSystems; 

public class BuildSpot : MonoBehaviour, IPointerClickHandler
{
    public GameObject currentTower;
    public TowerBlueprint towerBlueprint;
    public bool isUpgraded = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("【成功】終於點到了！(透過 EventSystem)");

        if (BuildManager.instance != null)
        {
            BuildManager.instance.SelectSpot(this);
        }
    }

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