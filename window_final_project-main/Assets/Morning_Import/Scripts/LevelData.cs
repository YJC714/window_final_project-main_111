[System.Serializable]
public class LevelSettings
{
    public string levelName;      // 關卡名稱
    public float spawnInterval;   // 生成速度
    public float animalWaitTime;  // 動物耐性
    public int targetScore;      
    public ItemType[] possibleFoods; // 這一關會出現的食物種類
    public float levelDuration = 60f; // 設定這關玩多久（秒）
}