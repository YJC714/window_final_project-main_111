using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("屬性設定")]
    public float range = 3f;      // 攻擊半徑
    public float fireRate = 1f;   // 攻速 (每秒幾發)

    [Header("連結設定")]
    public GameObject bulletPrefab; // 這裡要放你的子彈 Prefab
    public Transform firePoint;     // (選用) 子彈發射的位置，如果沒設就從塔中心發射

    private Transform target;       // 目前鎖定的敵人
    private float fireCountdown = 0f; // 射擊冷卻計時器

    void Start()
    {
        // 優化：不需要每幀都找敵人，每 0.5 秒找一次就好，節省效能
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        // 1. 找出場景上所有的敵人
        // Unity 6 建議使用 FindObjectsByType (舊版是用 FindObjectsOfType)
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        float shortestDistance = Mathf.Infinity; // 預設最近距離是無限大
        GameObject nearestEnemy = null;

        // 2. 遍歷所有敵人，找出哪一隻離我最近
        foreach (Enemy enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.gameObject;
            }
        }

        // 3. 如果有找到最近的，且距離在攻擊範圍內 -> 鎖定它
        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null; // 敵人跑出範圍或死光了，取消鎖定
        }
    }

    void Update()
    {
        // 如果沒有目標，就休息
        if (target == null) return;

        // (選用) 讓塔旋轉看向敵人
        // RotateTower(); 

        // 處理射擊冷卻
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate; // 重置冷卻時間
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        // 決定生成位置 (如果有設 firePoint 就用它的位置，不然就用塔自己中心)
        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;

        // 生成子彈
        GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        // 取得子彈腳本
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            // 告訴子彈：「去追這個目標！」
            bullet.Seek(target);
        }
    }

    // 【輔助功能】在編輯器畫出紅圈，讓你看見攻擊範圍
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}