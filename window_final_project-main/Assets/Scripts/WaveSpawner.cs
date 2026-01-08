
using UnityEngine;
using System.Collections;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string name;           // 波次名稱
        public GameObject enemyPrefab;// 這波怪物的種類
        public int count;             // 這波數量
        public float rate;            // 生成速度 (每秒幾隻)
    }

    public Wave[] waves;
    public Transform[] routes; // 多路徑支援

    public float timeBetweenWaves = 10f; // 波與波之間的休息時間 (預設10秒)
    private float countdown = 2f;        // 第一波開始前的準備時間

    public TextMeshProUGUI waveCountdownText;
    private int waveIndex = 0;

    private bool isSpawning = false; // 【新增】是否正在生成中
    // 【新增】記錄是否所有波次都結束了，避免重複呼叫勝利
    private bool allWavesFinished = false;

    void Update()
    {
        // 1. 如果正在生怪，或者遊戲已經結束，就跳過
        if (isSpawning || GameManager.instance.gameIsOver || allWavesFinished) return;

        // 2. 檢查：所有波次是否都生完了？
        if (waveIndex >= waves.Length)
        {
            // 3. 【關鍵判斷】檢查場上還有沒有活著的敵人
            // 這裡尋找所有 Tag 是 "Enemy" 的物件
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                // 如果找不到敵人，代表贏了！
                allWavesFinished = true;
                GameManager.instance.WinLevel();
                // this.enabled = false; // 可以關閉腳本，也可以不關
            }

            // 不管有沒有贏，只要波次結束了，就不要再執行下面的倒數邏輯
            return;
        }

        // 4. 倒數計時 (保持不變)
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave(waves[waveIndex]));
            return;
        }

        countdown -= Time.deltaTime;
        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

        if (waveCountdownText != null)
            waveCountdownText.text = string.Format("{0:00.00}", countdown);
    }

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("開始生成波次: " + wave.name);
        isSpawning = true; // 【鎖定】開始生怪，停止倒數

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1f / wave.rate);
        }

        isSpawning = false; // 【解鎖】生完了

        waveIndex++; // 準備下一波索引

        // 【關鍵】生完之後，才把倒數時間重置為 10 秒
        // 這樣玩家就有完整的 10 秒休息時間
        if (waveIndex < waves.Length) // 如果還有下一波
        {
            countdown = timeBetweenWaves;
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        // 隨機選路徑
        int randomRouteIndex = Random.Range(0, routes.Length);
        Transform selectedRoute = routes[randomRouteIndex];

        Transform[] waypoints = new Transform[selectedRoute.childCount];
        for (int i = 0; i < waypoints.Length; i++) waypoints[i] = selectedRoute.GetChild(i);

        if (waypoints.Length > 0)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, waypoints[0].position, Quaternion.identity);
            enemyGO.GetComponent<EnemyMovement>().SetPath(waypoints);
        }
    }
}

