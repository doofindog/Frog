using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour
{
    public TileType tileType = TileType.None;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hoverTint = new Color(1f, 1f, 0.4f, 1f);
    [SerializeField] private Color occupiedTint = new Color(0.6f, 1f, 0.6f, 1f);

    private Color _defaultColor;
    private Draggable _occupant;

    public bool IsOccupied => _occupant != null;
    public string OccupantFrogName => _occupant?.frogName;

    private void Awake()
    {
        if (spriteRenderer != null)
            _defaultColor = spriteRenderer.color;
    }

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnDragEnter()
    {
        if (spriteRenderer == null || IsOccupied) return;
        spriteRenderer.color = hoverTint;
    }

    public void OnDragExit()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = IsOccupied ? occupiedTint : _defaultColor;
    }

    public bool TryAccept(Draggable draggable)
    {
        if (IsOccupied) return false;

        _occupant = draggable;
        draggable.transform.position = transform.position;

        if (spriteRenderer != null)
            spriteRenderer.color = occupiedTint;

        return true;
    }

    public void Vacate()
    {
        _occupant = null;
        if (spriteRenderer != null)
            spriteRenderer.color = _defaultColor;
    }
}
