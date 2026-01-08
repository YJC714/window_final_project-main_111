using UnityEngine;

[RequireComponent(typeof(Enemy))] // 強制要求要有 Enemy 腳本，方便讀取速度
public class EnemyMovement : MonoBehaviour
{
    private Transform target;
    private int wavepointIndex = 0;
    private Transform[] waypoints; // 儲存這隻怪要走的路徑點

    private Enemy enemy; // 引用自己的數值腳本

    void Start()
    {
        enemy = GetComponent<Enemy>();
        //等待 WaveSpawner 告訴我們要走哪條路
    }

    // 這個函式給生成器呼叫，用來設定路徑
    public void SetPath(Transform[] newPath)
    {
        waypoints = newPath;
        if (waypoints.Length > 0)
        {
            target = waypoints[0];
        }
    }

    void Update()
    {
        // 如果沒有設路徑，就不動
        if (waypoints == null || target == null) return;

        // 移動邏輯
        Vector3 dir = target.position - transform.position;
        float currentSpeed = enemy.speed; // 從 Enemy 腳本讀取速度

        transform.Translate(dir.normalized * currentSpeed * Time.deltaTime, Space.World);

        // 檢查是否抵達目標點
        if (Vector3.Distance(transform.position, target.position) <= 0.2f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        if (wavepointIndex >= waypoints.Length - 1)
        {
            EndPath();
            return;
        }

        wavepointIndex++;
        target = waypoints[wavepointIndex];
    }

    void EndPath()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.TakeDamage(1);
        }
        Destroy(gameObject);
    }
}
