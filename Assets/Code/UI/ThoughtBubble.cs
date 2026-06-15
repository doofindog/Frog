using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Draggable))]
public class ThoughtBubble : MonoBehaviour
{
    [SerializeField] private Transform bubble;
    [SerializeField] private float showDuration = 0.25f;
    [SerializeField] private float hideDuration = 0.15f;
    [SerializeField] private float punchAngle = 15f;
    [SerializeField] private float punchDuration = 0.4f;
    [SerializeField] private int punchVibrato = 5;

    private Draggable _draggable;
    private bool _isDragging;

    private void Awake()
    {
        _draggable = GetComponent<Draggable>();

        bubble.localScale = Vector3.zero;
        bubble.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_draggable == null) return;
        _draggable.DragStarted += HandleDragStarted;
        _draggable.DragEnded += HandleDragEnded;
    }

    private void OnDisable()
    {
        if (_draggable == null) return;
        _draggable.DragStarted -= HandleDragStarted;
        _draggable.DragEnded -= HandleDragEnded;
    }

    private void OnMouseEnter()
    {
        if (_isDragging) return;
        Show();
    }

    private void OnMouseExit() => Hide();

    private void HandleDragStarted()
    {
        _isDragging = true;
        Hide();
    }

    private void HandleDragEnded() => _isDragging = false;

    private void Show()
    {
        bubble.DOKill();
        bubble.gameObject.SetActive(true);
        bubble.localRotation = Quaternion.identity;

        bubble.DOScale(1f, showDuration).SetEase(Ease.OutBack);
        bubble.DOPunchRotation(new Vector3(0f, 0f, punchAngle), punchDuration, punchVibrato);
    }

    private void Hide()
    {
        bubble.DOKill();
        bubble.DOScale(0f, hideDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => bubble.gameObject.SetActive(false));
    }
}
