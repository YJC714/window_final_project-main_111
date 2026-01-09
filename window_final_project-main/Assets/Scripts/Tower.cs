using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("屬性設定")]
    public float range = 3f;// 攻擊半徑
    public float fireRate = 1f;// 攻速 (每秒幾發)

    [Header("連結設定")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Transform target;
    private float fireCountdown = 0f;

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Enemy enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.gameObject;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (target == null) return;
        //RotateTower(); 

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;

        GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}