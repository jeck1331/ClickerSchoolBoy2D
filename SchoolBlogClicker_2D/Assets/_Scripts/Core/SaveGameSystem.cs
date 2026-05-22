using System.IO;
using System;
using _Scripts.Models.Save;
using Newtonsoft.Json;
using UnityEngine;

public class SaveGameSystem
{
    private bool _isInitialized = false;
    private const string FileName = "wasilisk_data.json";
    private string _pathFileSave;

    public void Save(SavedGameData savedGameData)
    {
        CheckPath();
        
        string json = JsonConvert.SerializeObject(savedGameData);
        File.WriteAllText(_pathFileSave, json);
        Debug.Log("save");
    }

    private SavedGameData LoadFromFile()
    {
        CheckPath();
        if (!File.Exists(_pathFileSave)) return FirstLaunchData();

        try
        {
            string json = File.ReadAllText(_pathFileSave);
            if (string.IsNullOrWhiteSpace(json)) return FirstLaunchData();
            var gameData = JsonConvert.DeserializeObject<SavedGameData?>(json);
            return gameData ?? FirstLaunchData();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Save file is broken. Using defaults. Reason: {ex.Message}");
            return FirstLaunchData();
        }

        //// Оффлайн-прогресс
        //double secondsOffline = (DateTime.Now.ToBinary() - dto.lastSaveTime) / 10000000d;
        //OfflineEarnings.Calculate(secondsOffline);
    }

    public RuntimeSavedGameData Load()
    {
        Debug.Log("load");
        return new RuntimeSavedGameData(LoadFromFile());
    }

    private void CheckPath()
    {
        if (_isInitialized) return;


        _pathFileSave = Path.Combine(Application.persistentDataPath, FileName);
        _isInitialized = true;
    }

    private SavedGameData FirstLaunchData() => new SavedGameData
    {
        Coins = 0,
        IsFirstLaunch = false,
    };
}