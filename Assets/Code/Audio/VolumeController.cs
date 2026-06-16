using UnityEngine;

public class VolumeController : MonoBehaviour
{
    private const string PrefKey = "MasterVolume";

    public static float Volume => PlayerPrefs.GetFloat(PrefKey, 1f);

    private void Awake() => AudioListener.volume = Volume;

    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(PrefKey, value);
    }
}
