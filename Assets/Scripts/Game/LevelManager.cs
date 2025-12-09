using UnityEngine;

public static class LevelManager
{
    private static LevelsConfig _config;
    private static int _currentId = -1;
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
            if (_currentId == -1)
                _currentId = PlayerPrefs.GetInt("LastLevel", 0);

            return _currentId;
        }
        private set
        {
            if (_currentId == value)
                return;

            _currentId = value;
            PlayerPrefs.SetInt("LastLevel", value);
        }
    }

    public static bool IsNextExist => LastId < Config.Levels.Count;

    public static int LastDialogNodeId
    {
        get 
        {
            return PlayerPrefs.GetInt($"LastDialogNodeId{_currentId}", -1);
        }
        set
        {
            PlayerPrefs.SetInt($"LastDialogNodeId{_currentId}", value);
        }
    }

    public static float LastTimerDuration { get; private set; }

    public static void LoadLevel(int id)
    {
        if (Config.Levels.Count > id)
        {
            _currentConfig = Config.Levels[id];
            LastId = id;
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
