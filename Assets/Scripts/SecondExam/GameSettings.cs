using RojoinSaveSystem;
using RojoinSaveSystem.Attributes;

[System.Serializable]
public class GameSettings : ISaveObject
{
    SaveObjectData saveObject = new SaveObjectData();
     public int gridSizeX = 10;
     public int gridSizeY = 10;
     public int turnCount = 100;
     public int PopulationCount = 40;
     public float GenerationDuration = 20.0f;
     public int EliteCount = 4;
     public float MutationChance = 0.10f;
     public float MutationRate = 0.01f;
    [SaveValue(8)] public int[] TESTVARIABLEARRAY;

    public GameSettings()
    {
    }
    public int GetID()
    {
        return saveObject.id;
    }

    public ISaveObject GetObject()
    {
        return this;
    }

    public void Save()
    {
    }
}