using System.IO;
using UnityEngine;

public class SaveGameSystem
{
    private bool _isInitialized = false;
    private const string FileName = "wasilisk_data.json";
    private string _pathFileSave;

    public void Save(SaveGameData saveGameData)
    {
        CheckPath();

        string json = JsonUtility.ToJson(saveGameData, true);
        File.WriteAllText(_pathFileSave, json);
        Debug.Log("save");
    }

    private SaveGameData LoadFromFile()
    {
        CheckPath();
        if (!File.Exists(_pathFileSave)) return null;

        string json = File.ReadAllText(_pathFileSave);
        var gameData = JsonUtility.FromJson<SaveGameData>(json);

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
}