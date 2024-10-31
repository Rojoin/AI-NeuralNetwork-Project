[System.Serializable]
public class GameSettings
{
    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public int turnCount = 100;
    public int PopulationCount = 40;
    public float GenerationDuration = 20.0f;
    public int EliteCount = 4;
    public float MutationChance = 0.10f;
    public float MutationRate = 0.01f;
}