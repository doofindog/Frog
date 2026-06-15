using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone : MonoBehaviour
{
    public Vector2Int gridCoord;
    public TileType tileType = TileType.None;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hoverTint = new Color(1f, 1f, 0.4f, 1f);
    [SerializeField] private Color occupiedTint = new Color(0.6f, 1f, 0.6f, 1f);
    [SerializeField] private Color blockedTint = new Color(1f, 0.4f, 0.4f, 1f);

    private Color _defaultColor;
    private Draggable _occupant;

    public bool IsOccupied => _occupant != null;
    public string OccupantFrogName => _occupant?.frogName;
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

    public void OnDragEnter(Draggable draggable, RuleBook ruleBook)
    {
        if (spriteRenderer == null || IsOccupied) return;

        bool blocked = IsBlocked(draggable, ruleBook);
        spriteRenderer.color = blocked ? blockedTint : hoverTint;
    }

    public void OnDragExit()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = IsOccupied ? occupiedTint : _defaultColor;
    }

    public bool TryAccept(Draggable draggable, RuleBook ruleBook)
    {
        if (IsOccupied) return false;
        if (IsBlocked(draggable, ruleBook)) return false;

        _occupant = draggable;
        draggable.transform.position = transform.position;

        if (spriteRenderer != null)
            spriteRenderer.color = occupiedTint;

        frog = draggable.frogName;
        return true;
    }

    public void Vacate()
    {
        _occupant = null;
        if (spriteRenderer != null)
            spriteRenderer.color = _defaultColor;
    }

    private bool IsBlocked(Draggable draggable, RuleBook ruleBook)
    {
        if (ruleBook == null || draggable == null) return false;
        var constraint = ruleBook.GetConstraintForFrog(draggable.frogName);
        if (constraint == null || constraint.objectType != RuleObjectType.Tile) return false;
        return !constraint.IsSatisfiedBy(tileType);
    }
}
