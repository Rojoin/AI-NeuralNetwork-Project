using System;
using RojoinSaveSystem;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public RojoinSaveSystem.SaveSystem _saveSystem;

    public static SaveSystem instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    public string pathToSave = "/Saves/Genomes";
    public string extension = "genome";

    private void OnEnable()
    {
        _saveSystem = new RojoinSaveSystem.SaveSystem();
        var currentPath = Application.dataPath + pathToSave + "." + extension;
        _saveSystem.savePath = currentPath;
        _saveSystem.StartSaveSystem(DebugLogger);

    }

    public void AddObjectToSave(ISaveObject objectToSave)
    {
        _saveSystem.AddObjectToSave(objectToSave);
    }
    public void DebugLogger(string text) => Debug.Log(text);

    [ContextMenu("Save Game")]
    public void Save()
    {
        _saveSystem.CreateSaveFile();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        _saveSystem.LoadSaveFile();
     _saveSystem.ExecuteLoadAction();
    }
}