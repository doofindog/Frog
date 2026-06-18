using System;
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

    // All board coordinates occupied by a customer with this name. Multiple customers
    // (e.g. a whole species like "Pink Frogs") can share the same name.
    public IEnumerable<Vector2Int> FindFrogs(string frogName)
    {
        foreach (var (coord, zone) in _grid)
            if (string.Equals(zone.OccupantFrogName, frogName, StringComparison.OrdinalIgnoreCase))
                yield return coord;
    }

    // The board coordinate of this exact customer instance, regardless of name collisions.
    public Vector2Int? FindCoord(Draggable occupant)
    {
        foreach (var (coord, zone) in _grid)
            if (zone.Occupant == occupant) return coord;
        return null;
    }

    public string GetOccupantNames()
    {
        var names = new List<string>();
        foreach (var (coord, zone) in _grid)
            if (zone.OccupantFrogName != null)
                names.Add($"'{zone.OccupantFrogName}' at {coord}");
        return names.Count > 0 ? string.Join(", ", names) : "none";
    }

    public static bool AreAdjacent(Vector2Int a, Vector2Int b) => (a - b).sqrMagnitude == 1;
}
