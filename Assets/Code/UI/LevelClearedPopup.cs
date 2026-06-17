using Febucci.TextAnimatorForUnity;
using UnityEngine;
using DG.Tweening;

public class LevelClearedPopup : MonoBehaviour
{
    [SerializeField] private TypewriterComponent typewriter;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private float punch = 0.3f;
    [SerializeField] private float punchDuration = 0.4f;
    [SerializeField] private int punchVibrato = 6;

    private Vector3 _baseScale;

    private void Awake() => _baseScale = typewriter.transform.localScale;

    private void Start() => typewriter.gameObject.SetActive(false);

    [ContextMenu("Show")]
    public void Show()
    {
        typewriter.gameObject.SetActive(true);

        typewriter.onTextShowed.RemoveListener(HandleTextShowed);
        typewriter.onTextShowed.AddListener(HandleTextShowed);

        typewriter.StartShowingText(restart: true);
    }

    private void HandleTextShowed()
    {
        typewriter.onTextShowed.RemoveListener(HandleTextShowed);

        var t = typewriter.transform;
        t.DOKill();
        t.localScale = _baseScale;
        t.DOPunchScale(Vector3.one * punch, punchDuration, punchVibrato)
            .OnComplete(GoToNextLevel);
    }

    private void GoToNextLevel()
    {
        typewriter.gameObject.SetActive(false);

        if (levelLoader != null)
            levelLoader.LoadNextLevel();
    }
}
