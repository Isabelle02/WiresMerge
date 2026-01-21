using UnityEngine;

public static class LevelManager
{
    private static LevelsConfig _config;
    private static LevelConfig _currentConfig;

    private static LevelsConfig Config 
    {
        get 
        { 
            if (_config == null)
                _config = Resources.Load<LevelsConfig>("Levels/LevelsConfig"); 

            return _config;
        }
    }

    public static int LastId
    {
        get
        {
            return PlayerPrefs.GetInt("LastLevel", 0);
        }
        private set
        {
            PlayerPrefs.SetInt("LastLevel", value);
        }
    }

    public static bool IsNextExist => LastId < Config.Levels.Count;

    public static int LastDialogNodeId
    {
        get 
        {
            return PlayerPrefs.GetInt($"LastDialogNodeId{LastId}", -1);
        }
        set
        {
            PlayerPrefs.SetInt($"LastDialogNodeId{LastId}", value);
        }
    }

    public static float LastTimerDuration { get; private set; }

    public static void LoadLevel(int id)
    {
        LastId = id;
        if (Config.Levels.Count > id)
        {
            _currentConfig = Config.Levels[id];
            LastDialogNodeId = _currentConfig.DialogNodeId;
            LastTimerDuration = _currentConfig.TimerDuration;
        }
        else
        {
            // GameObject finish dialog
            LastDialogNodeId = 127;
        }
    }

    public static void UnloadLevel()
    {
        

        // unload \ wirecell \ метод очищает ячейки \ вызвать \ не забывать о таймере \ обнулить вайрсистем
    }

    public static void ShowLevel()
    {
        foreach (var cell in _currentConfig.WireCells)
        {
            var cellObj = Pool.Get<WireCell>(Gameplay.Transform);
            cellObj.Set(cell);
            Gameplay.Started += cellObj.Init;
        }
        Gameplay.Play();
    }

}
