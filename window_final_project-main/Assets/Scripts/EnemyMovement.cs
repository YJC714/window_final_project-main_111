using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    private Transform target;
    private int wavepointIndex = 0;
    private Transform[] waypoints;

    private Enemy enemy;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

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
        if (waypoints == null || target == null) return;

        Vector3 dir = target.position - transform.position;
        float currentSpeed = enemy.speed;

        transform.Translate(dir.normalized * currentSpeed * Time.deltaTime, Space.World);

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
