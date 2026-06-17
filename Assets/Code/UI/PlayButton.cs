using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private string sceneName = "Gameplay";

    public void Play()
    {
        if (LevelTransition.Instance != null)
            LevelTransition.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
