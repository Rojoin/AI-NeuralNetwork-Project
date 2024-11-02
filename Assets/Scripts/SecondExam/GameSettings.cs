using RojoinSaveSystem;
using RojoinSaveSystem.Attributes;

[System.Serializable]
public class GameSettings : ISaveObject
{
    SaveObjectData saveObject = new SaveObjectData();
    [SaveValue(0)]public int gridSizeX = 10;
    [SaveValue(1)]public int gridSizeY = 10;
    [SaveValue(2)]public int turnCount = 100;
    [SaveValue(3)]public int PopulationCount = 40;
    [SaveValue(4)]public float GenerationDuration = 20.0f;
    [SaveValue(5)]public int EliteCount = 4;
    [SaveValue(6)]public float MutationChance = 0.10f;
    [SaveValue(7)]public float MutationRate = 0.01f;
    [SaveValue(8)]public int[] TESTVARIABLEARRAY;

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