using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomerQueue : MonoBehaviour
{
    [SerializeField] private Vector2 slotSpacing = new Vector2(1.5f, 0f);

    private readonly List<Draggable> _customers = new List<Draggable>();

    public void Populate(IEnumerable<Draggable> customers)
    {
        _customers.Clear();
        foreach (var c in customers)
            _customers.Add(c);

        for (int i = 0; i < _customers.Count; i++)
            _customers[i].transform.position = GetSlotPosition(i);
    }

    public void TryRemove(Draggable customer)
    {
        if (!_customers.Remove(customer)) return;
        RefreshPositions();
    }

    public void AddToEnd(Draggable customer)
    {
        _customers.Add(customer);
        customer.transform.DOMove(GetSlotPosition(_customers.Count - 1), 0.35f).SetEase(Ease.OutBack);
    }

    private void RefreshPositions()
    {
        for (int i = 0; i < _customers.Count; i++)
            _customers[i].transform.DOMove(GetSlotPosition(i), 0.3f).SetEase(Ease.OutQuad);
    }

    private Vector3 GetSlotPosition(int index)
    {
        return transform.position + (Vector3)(slotSpacing * index);
    }
}
