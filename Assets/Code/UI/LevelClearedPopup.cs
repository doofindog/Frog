using Febucci.TextAnimatorForUnity;
using UnityEngine;

public class LevelClearedPopup : MonoBehaviour
{
    [SerializeField] private TypewriterComponent typewriter;

    private void Start() => typewriter.gameObject.SetActive(false);

    [ContextMenu("Show")]
    public void Show()
    {
        typewriter.gameObject.SetActive(true);
        typewriter.StartShowingText(restart: true);
    }
}
