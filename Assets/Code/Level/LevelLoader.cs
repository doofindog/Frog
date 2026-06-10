using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelSequence sequence;
    [SerializeField] private CustomerQueue queue;
    [SerializeField] private RuleBook rules;
    [SerializeField] private Transform customerParent;
    [SerializeField] private Transform dropZoneParent;

    private int _currentIndex = -1;
    private readonly List<Draggable> _spawnedCustomers = new();
    private GameObject _spawnedLayout;

    public int CurrentIndex => _currentIndex;
    public int LevelCount => sequence != null ? sequence.levels.Length : 0;
    public bool HasNextLevel => _currentIndex < LevelCount - 1;

    private void Start() => LoadLevel(0);

    public void LoadNextLevel()
    {
        if (HasNextLevel)
            LoadLevel(_currentIndex + 1);
    }

    public void ReloadCurrentLevel() => LoadLevel(_currentIndex);

    public void LoadLevel(int index)
    {
        ClearLevel();
        _currentIndex = index;

        var data = sequence.levels[index];
        SpawnLayout(data);
        FindAnyObjectByType<DropZoneGrid>()?.Build();
        SpawnCustomers(data);
        LoadRules(data);
    }

    private void SpawnLayout(LevelData data)
    {
        var parent = dropZoneParent != null ? dropZoneParent : transform;
        _spawnedLayout = Instantiate(data.dropZoneLayoutPrefab, parent);
    }

    private void SpawnCustomers(LevelData data)
    {
        foreach (var prefab in data.customerPrefabs)
        {
            var go = Instantiate(prefab, customerParent);
            if (go.TryGetComponent<Draggable>(out var draggable))
                _spawnedCustomers.Add(draggable);
        }

        queue.Populate(_spawnedCustomers);
    }

    private void LoadRules(LevelData data)
    {
        rules ??= FindAnyObjectByType<RuleBook>();
        if (!rules)
        {
            Debug.LogError("No RuleBook found in scene!");
            return;
        }
        
        rules.LoadRules(data.rules);
    }

    private void ClearLevel()
    {
        foreach (var customer in _spawnedCustomers)
        {
            Destroy(customer.gameObject);
        }
        _spawnedCustomers.Clear();

        if (_spawnedLayout != null)
        {
            Destroy(_spawnedLayout);
        }
        _spawnedLayout = null;
    }
}
