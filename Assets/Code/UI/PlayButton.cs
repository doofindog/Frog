using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private string sceneName = "Gameplay";

    public void Play() => SceneManager.LoadScene(sceneName);
}
