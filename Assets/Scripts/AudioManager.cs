using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundSource;

    private const string SoundKey = "Sound";
    private const string MusicKey = "Music";

    private static AudioManager _instance;

    public static float SoundVolumeValue
    {
        get => PlayerPrefs.GetFloat(SoundKey, 1);
        set
        {
            PlayerPrefs.SetFloat(SoundKey, value);
            _instance._soundSource.volume = value;
        }
    }

    public static float MusicVolumeValue
    {
        get => PlayerPrefs.GetFloat(MusicKey, 1);
        set
        {
            PlayerPrefs.SetFloat(MusicKey, value);
            _instance._musicSource.volume = value;
        }
    }

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);

        _soundSource.volume = SoundVolumeValue;
        _musicSource.volume = MusicVolumeValue;

        Play(Sound.Main, true);
    }

    public static float GetClipLength(Sound sound)
    {
        return AudioClipConfig.GetClip(sound).length;
    }

    public static void Play(Sound sound, bool loop)
    {
        if (_instance._musicSource.clip == AudioClipConfig.GetClip(sound))
            return;

        _instance._musicSource.loop = loop;
        _instance._musicSource.clip = AudioClipConfig.GetClip(sound);
        _instance._musicSource.Play();
    }

    public static void PlayOneShot(Sound sound)
    {
        _instance._soundSource.PlayOneShot(AudioClipConfig.GetClip(sound));
    }
}
