[System.Serializable]
public class LevelSettings
{
    public string levelName;      
    public float spawnInterval;   
    public float animalWaitTime;  
    public int targetScore;      
    public ItemType[] possibleFoods; 
    public float levelDuration = 60f; 
}