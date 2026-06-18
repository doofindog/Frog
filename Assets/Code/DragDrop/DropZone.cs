using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour
{
    public Vector2Int gridCoord;
    public TileType tileType = TileType.None;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hoverTint = new Color(1f, 1f, 0.4f, 1f);
    [SerializeField] private Color occupiedTint = new Color(0.6f, 1f, 0.6f, 1f);

    private Color _defaultColor;
    private Draggable _occupant;

    public bool IsOccupied => _occupant != null;
    public string OccupantFrogName => _occupant?.frogName;
    public Draggable Occupant => _occupant;
    [SerializeField] private string frog;

    private void Awake()
    {
        if (spriteRenderer != null)
            _defaultColor = spriteRenderer.color;
    }

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnDragEnter(Draggable draggable)
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

        frog = draggable.frogName;
        return true;
    }

    // Registers a frog already parented under this zone in the level prefab as its occupant,
    // without the repositioning/visual setup that drag-and-drop placement does.
    public void SetStaticOccupant(Draggable occupant)
    {
        if (occupant == null) return;

        _occupant = occupant;
        if (spriteRenderer != null)
            spriteRenderer.color = occupiedTint;

        frog = occupant.frogName;
    }

    public void Vacate()
    {
        _occupant = null;
        if (spriteRenderer != null)
            spriteRenderer.color = _defaultColor;
    }
}
