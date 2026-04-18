using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveGameSystem
{
    private bool _isInitialized = false;
    private const string FileName = "wasilisk_data.json";
    private string _pathFileSave;

    public void Save(SaveGameData saveGameData)
    {
        CheckPath();
        
        string json = JsonConvert.SerializeObject(saveGameData);
        File.WriteAllText(_pathFileSave, json);
        Debug.Log("save");
    }

    private SaveGameData LoadFromFile()
    {
        CheckPath();
        if (!File.Exists(_pathFileSave)) return FirstLaunchData();

        string json = File.ReadAllText(_pathFileSave);
        var gameData = JsonConvert.DeserializeObject<SaveGameData>(json);

        //// Оффлайн-прогресс
        //double secondsOffline = (DateTime.Now.ToBinary() - dto.lastSaveTime) / 10000000d;
        //OfflineEarnings.Calculate(secondsOffline);

        return gameData;
    }

    public RuntimeSaveGameData Load()
    {
        Debug.Log("load");
        return new RuntimeSaveGameData(LoadFromFile());
    }

    private void CheckPath()
    {
        if (_isInitialized) return;


        _pathFileSave = Path.Combine(Application.persistentDataPath, FileName);
        _isInitialized = true;
    }

    private SaveGameData FirstLaunchData() => new SaveGameData
    {
        Coins = "0",
        IsFirstLaunch = false,
    };
}