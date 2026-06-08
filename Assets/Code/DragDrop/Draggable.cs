using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int draggingSortOrder = 100;
    [SerializeField] private float followSpeed = 15f;
    [SerializeField] private float maxTilt = 25f;
    [SerializeField] private float tiltSensitivity = 0.5f;
    [SerializeField] private float tiltSmoothing = 10f;

    private int _originalSortOrder;
    private bool _isDragging;
    private Vector3 _dragTarget;
    private Vector3 _previousPosition;
    private float _currentTilt;

    private void Reset() => spriteRenderer = GetComponent<SpriteRenderer>();

    private void LateUpdate()
    {
        if (!_isDragging) return;

        Vector3 next = Vector3.Lerp(transform.position, _dragTarget, followSpeed * Time.deltaTime);

        float velocityX = (next.x - _previousPosition.x) / Time.deltaTime;
        float targetTilt = Mathf.Clamp(-velocityX * tiltSensitivity, -maxTilt, maxTilt);
        _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, tiltSmoothing * Time.deltaTime);

        _previousPosition = next;
        transform.SetPositionAndRotation(next, Quaternion.Euler(0f, 0f, _currentTilt));
    }

    public void BeginDrag()
    {
        _isDragging = true;
        _dragTarget = transform.position;
        _previousPosition = transform.position;
        _currentTilt = 0f;

        _originalSortOrder = spriteRenderer.sortingOrder;
        spriteRenderer.sortingOrder = draggingSortOrder;

        transform.DOKill();
        transform.DOScale(1.1f, 0.12f).SetEase(Ease.OutBack);
    }

    public void Drag(Vector3 worldPosition) => _dragTarget = worldPosition;

    public void EndDrag()
    {
        _isDragging = false;
        spriteRenderer.sortingOrder = _originalSortOrder;

        transform.DOKill();
        transform.DOScale(1f, 0.1f);
        transform.DORotate(Vector3.zero, 0.25f).SetEase(Ease.OutQuad);
    }
}
