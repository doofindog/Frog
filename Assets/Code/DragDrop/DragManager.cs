using UnityEngine;
using UnityEngine.InputSystem;

public class DragManager : MonoBehaviour
{
    public static event System.Action OnFrogPlaced;
    public static event System.Action OnBoardChanged;

    [SerializeField] private LayerMask draggableLayer;
    [SerializeField] private LayerMask dropZoneLayer;
    [SerializeField] private CustomerQueue queue;
    [SerializeField] private RuleBook ruleBook;

    private Camera _camera;
    private Draggable _dragging;
    private DropZone _hoveredDropZone;

    private void Awake() => _camera = Camera.main;

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector3 worldPos = ScreenToWorld(mouse.position.ReadValue());

        if (mouse.leftButton.wasPressedThisFrame)
            TryBeginDrag(worldPos);

        if (_dragging != null)
        {
            _dragging.Drag(worldPos);
            UpdateHoveredZone(worldPos);
        }

        if (mouse.leftButton.wasReleasedThisFrame && _dragging != null)
            EndDrag();
    }

    private void TryBeginDrag(Vector3 worldPos)
    {
        var hit = Physics2D.OverlapPoint(worldPos, draggableLayer);
        if (hit == null) return;

        _dragging = hit.GetComponent<Draggable>();
        if (_dragging == null) return;

        Physics2D.OverlapPoint(worldPos, dropZoneLayer)?.GetComponent<DropZone>()?.Vacate();
        queue.TryRemove(_dragging);

        _dragging.BeginDrag();
        OnBoardChanged?.Invoke();
    }

    private void UpdateHoveredZone(Vector3 worldPos)
    {
        var hit = Physics2D.OverlapPoint(worldPos, dropZoneLayer);
        var zone = hit != null ? hit.GetComponent<DropZone>() : null;

        if (zone == _hoveredDropZone) return;

        _hoveredDropZone?.OnDragExit();
        _hoveredDropZone = zone;
        _hoveredDropZone?.OnDragEnter(_dragging, ruleBook);
    }

    private void EndDrag()
    {
        bool accepted = _hoveredDropZone != null && _hoveredDropZone.TryAccept(_dragging, ruleBook);

        _hoveredDropZone?.OnDragExit();
        _hoveredDropZone = null;

        _dragging.EndDrag();

        if (!accepted)
            queue.AddToEnd(_dragging);
        else
            OnFrogPlaced?.Invoke();

        _dragging = null;
        OnBoardChanged?.Invoke();
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        var pos = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -_camera.transform.position.z));
        pos.z = 0f;
        return pos;
    }
}
