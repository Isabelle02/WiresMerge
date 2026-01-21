using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AudioClipConfig", menuName = "Configs/AudioClipConfig")]
public class AudioClipConfig : ScriptableObject
{
    [SerializeField] private SerializedDictionary<Sound, AudioClip> _sounds;

    private static AudioClipConfig _instance;

    private static AudioClipConfig Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<AudioClipConfig>("AudioClipConfig");

            return _instance;
        }
    }

    public static AudioClip GetClip(Sound sound)
    {
        return Instance._sounds[sound];
    }
}

public enum Sound
{
    Main,
    ButtonClick,
    WireClick,
    WinPopup,
    LosePopup
}