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
                _currentId = PlayerPrefs.GetInt("LastLevel", -1);

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

    public static void LoadLevel(int id)
    {
        _currentConfig = Config.Levels[id];
        LastId = id;
        LastDialogNodeId = _currentConfig.DialogNodeId;
        foreach (var cell in _currentConfig.WireCells)
        {
            var cellObj = Pool<WireCell>.Get(Gameplay.Transform);
            cellObj.Set(cell);
            Gameplay.Started += cellObj.Init;
        }
    }

    public static void UnloadLevel()
    {


        // unload \ wirecell \ метод очищает ячейки \ вызвать \ не забывать о таймере \ обнулить вайрсистем
    }

    public static void ShowLevel()
    {
        Gameplay.Play();
    }

}
