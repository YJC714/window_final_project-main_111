using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    [Header("子彈屬性")]
    public float speed = 70f;
    public int damage = 50;  // 子彈造成的傷害
    public float explosionRadius = 0f; // 爆炸範圍 (0代表單體攻擊)
    public GameObject impactEffect; // 命中特效 (可選)

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);


        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //朝上=減 90 度
        //transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void HitTarget()
    {
        // 播放命中特效 (如果有設定的話)
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }

        // 判斷是單體還是範圍
        if (explosionRadius > 0f)
        {
            Explode(); // 範圍攻擊
        }
        else
        {
            Damage(target); // 單體攻擊
        }

        Destroy(gameObject); // 銷毀子彈
    }

    // --- 範圍攻擊邏輯 ---
    void Explode()
    {
        // 畫出一個圓形，抓取裡面所有的敵人 (Layer 記得要設對，假設敵人是 Default 或是 Enemy layer)
        // 這裡我們抓取所有有 Collider 的東西
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.tag == "Enemy") // 確保只炸到敵人
            {
                Damage(collider.transform);
            }
        }
    }

    // --- 造成傷害 ---
    void Damage(Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>(); // 取得敵人的腳本
        if (e != null)
        {
            e.TakeDamage(damage); // 呼叫敵人的扣血函式
        }
    }

    // 在編輯器裡畫出爆炸範圍，方便調整 (只在 Scene 視窗顯示)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

