using UnityEngine;
using System.Collections;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string name;// 波次名稱
        public GameObject enemyPrefab;// 這波怪物的種類
        public int count;// 這波數量
        public float rate;// 生成速度 (每秒幾隻)
    }

    public Wave[] waves;
    public Transform[] routes;

    public float timeBetweenWaves = 10f;
    private float countdown = 2f;

    public TextMeshProUGUI waveCountdownText;
    private int waveIndex = 0;

    private bool isSpawning = false;
    private bool allWavesFinished = false;

    void Update()
    {
        if (isSpawning || GameManager.instance.gameIsOver || allWavesFinished) return;

        if (waveIndex >= waves.Length)
        {
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                allWavesFinished = true;
                GameManager.instance.WinLevel();
            }
            return;
        }

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
        isSpawning = true;

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(1f / wave.rate);
        }

        isSpawning = false;

        waveIndex++;
        if (waveIndex < waves.Length)
        {
            countdown = timeBetweenWaves;
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
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

