using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class LevelTransition : MonoBehaviour
{
    public static LevelTransition Instance { get; private set; }

    [SerializeField] private Transform maskGraphic;
    [SerializeField] private float revealedScale = 15f;
    [SerializeField] private float coveredScale = 0f;
    [SerializeField] private float settleScale = 1f;
    [SerializeField] private float settleDuration = 0.35f;
    [SerializeField] private float settleHold = 1f;
    [SerializeField] private float coverDuration = 0.35f;
    [SerializeField] private float holdDuration = 0.1f;
    [SerializeField] private float revealDuration = 0.45f;
    [SerializeField] private Ease settleEase = Ease.InBack;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _canvasGroup = GetComponent<CanvasGroup>();
        SetRevealed();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void LoadScene(string sceneName) => Play(() => SceneManager.LoadScene(sceneName));

    [ContextMenu("Test Transition")]
    public void TestTransition() => Play(null);

    public void Play(Action onCovered)
    {
        maskGraphic.DOKill();
        _canvasGroup.blocksRaycasts = true;

        var seq = DOTween.Sequence().SetLink(gameObject);

        seq.Append(maskGraphic.DOScale(settleScale, settleDuration).SetEase(settleEase));
        seq.AppendInterval(settleHold);
        seq.Append(maskGraphic.DOScale(coveredScale, coverDuration).SetEase(Ease.InBack));
        seq.AppendCallback(() => onCovered?.Invoke());
        seq.AppendInterval(holdDuration);
        seq.Append(maskGraphic.DOScale(revealedScale, revealDuration).SetEase(Ease.OutBack));
        seq.OnComplete(() => _canvasGroup.blocksRaycasts = false);
    }

    private void SetRevealed()
    {
        _canvasGroup.blocksRaycasts = false;
        maskGraphic.localScale = Vector3.one * revealedScale;
    }
}
