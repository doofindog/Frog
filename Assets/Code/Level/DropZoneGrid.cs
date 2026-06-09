using System.Collections.Generic;
using UnityEngine;

public class DropZoneGrid : MonoBehaviour
{
    private Dictionary<Vector2Int, DropZone> _grid;

    private void Awake() => Build();

    public void Build()
    {
        _grid = new Dictionary<Vector2Int, DropZone>();
        foreach (var zone in GetComponentsInChildren<DropZone>())
            _grid[zone.gridCoord] = zone;
    }

    public DropZone GetZone(Vector2Int coord) =>
        _grid.TryGetValue(coord, out var z) ? z : null;

    public IEnumerable<DropZone> GetNeighbours(Vector2Int coord)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var d in dirs)
            if (_grid.TryGetValue(coord + d, out var z))
                yield return z;
    }

    public Vector2Int? FindFrog(string frogName)
    {
        foreach (var (coord, zone) in _grid)
            if (zone.OccupantFrogName == frogName) return coord;
        return null;
    }

    public string GetOccupantNames()
    {
        var names = new System.Collections.Generic.List<string>();
        foreach (var (coord, zone) in _grid)
            if (zone.OccupantFrogName != null)
                names.Add($"'{zone.OccupantFrogName}' at {coord}");
        return names.Count > 0 ? string.Join(", ", names) : "none";
    }

    public bool AreFrogsAdjacent(string a, string b)
    {
        var posA = FindFrog(a);
        var posB = FindFrog(b);
        if (posA == null || posB == null) return false;
        return (posA.Value - posB.Value).sqrMagnitude == 1;
    }
}
