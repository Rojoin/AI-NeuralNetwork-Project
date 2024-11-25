using System;
using System.Collections.Generic;
using RojoinSaveSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveSystemUnity : MonoBehaviour
{
    public SaveSystem _saveSystem;


    public string pathToSave = "/Saves/Genomes";
    private string currentPath;
    public GeneticAlgorithmData geneticAlgorithmData;
    public GeneticAlgorithmData geneticAlgorithmData2;
    public RojoinNeuralNetwork.Save.SaveSystem.GeneticAlgorithmDataManager dataManager = new RojoinNeuralNetwork.Save.SaveSystem.GeneticAlgorithmDataManager();

    private void OnEnable()
    {

        currentPath = Application.dataPath + pathToSave + ".genome";
        var mainHerb = new BrainData(11, new int[] { 7, 5, 3 }, 3, 0.5f, 0.5f);
        geneticAlgorithmData = new GeneticAlgorithmData(10, 0.10f, 10f, mainHerb.ToBrain());
        dataManager.AddDataset(geneticAlgorithmData);
        dataManager.AddDataset(geneticAlgorithmData2);
    }

    public void AddObjectToSave(ISaveObject objectToSave)
    {
        _saveSystem.AddObjectToSave(objectToSave);
    }

    public void DebugLogger(string text) => Debug.Log(text);

    [ContextMenu("Save Game")]
    public void Save()
    {
        dataManager.SaveAll(currentPath);
        // _saveSystem.CreateSaveFile();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        Debug.Log("ExecuteAction");
        dataManager.LoadAll(currentPath);
        List<GeneticAlgorithmData> algorithmData = dataManager.GetAllDatasets();
        geneticAlgorithmData = algorithmData[0];
        geneticAlgorithmData2 = algorithmData[1];
        // _saveSystem.LoadSaveFile();
        // geneticAlgorithmData.Load(currentPath);
    }
}