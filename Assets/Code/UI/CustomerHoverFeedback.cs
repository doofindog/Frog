using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Draggable))]
public class CustomerHoverFeedback : MonoBehaviour
{
    [SerializeField] private Transform graphic;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private float punchAngle = 12f;
    [SerializeField] private float punchDuration = 0.3f;
    [SerializeField] private int punchVibrato = 6;

    private Draggable _draggable;
    private Vector3 _baseScale;
    private bool _isDragging;

    private void Awake()
    {
        _draggable = GetComponent<Draggable>();
        _baseScale = graphic.localScale;
    }

    private void OnEnable()
    {
        _draggable.DragStarted += HandleDragStarted;
        _draggable.DragEnded += HandleDragEnded;
    }

    private void OnDisable()
    {
        _draggable.DragStarted -= HandleDragStarted;
        _draggable.DragEnded -= HandleDragEnded;
    }

    private void OnMouseEnter()
    {
        if (_isDragging) return;

        graphic.DOKill();
        graphic.localScale = _baseScale;
        graphic.localRotation = Quaternion.identity;

        graphic.DOScale(_baseScale * hoverScale, scaleDuration).SetEase(Ease.OutBack);
        graphic.DOPunchRotation(new Vector3(0f, 0f, punchAngle), punchDuration, punchVibrato);
    }

    private void OnMouseExit()
    {
        if (_isDragging) return;
        ResetGraphic();
    }

    private void HandleDragStarted()
    {
        _isDragging = true;
        ResetGraphic();
    }

    private void HandleDragEnded() => _isDragging = false;

    private void ResetGraphic()
    {
        graphic.DOKill();
        graphic.DOScale(_baseScale, scaleDuration).SetEase(Ease.OutQuad);
        graphic.DOLocalRotate(Vector3.zero, scaleDuration).SetEase(Ease.OutQuad);
    }
}
