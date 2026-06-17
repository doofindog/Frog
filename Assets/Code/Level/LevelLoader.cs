using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelSequence sequence;
    [SerializeField] private CustomerQueue queue;
    [SerializeField] private RuleBook rules;
    [SerializeField] private Transform customerParent;
    [SerializeField] private Transform dropZoneParent;
    [SerializeField] private CustomerMoodController moodController;
    [SerializeField] private float spawnStagger = 0.1f;

    private int _currentIndex = -1;
    private readonly List<Draggable> _spawnedCustomers = new();
    private DropZoneGrid _spawnedLayout;

    public int CurrentIndex => _currentIndex;
    public int LevelCount => sequence != null ? sequence.levels.Length : 0;
    public bool HasNextLevel => _currentIndex < LevelCount - 1;
    public LevelData CurrentLevel => _currentIndex >= 0 ? sequence.levels[_currentIndex] : null;

    private void Start() => LoadLevel(0);

    public void LoadNextLevel()
    {
        if (HasNextLevel)
            GoToLevel(_currentIndex + 1);
    }

    public void ReloadCurrentLevel() => GoToLevel(_currentIndex);

    private void GoToLevel(int index)
    {
        if (LevelTransition.Instance != null)
            LevelTransition.Instance.Play(() => LoadLevel(index));
        else
            LoadLevel(index);
    }

    public void LoadLevel(int index)
    {
        ClearLevel();
        _currentIndex = index;

        var data = sequence.levels[index];
        var dropZoneGrid = SpawnLayout(data);
        dropZoneGrid.Build();
        SpawnCustomers(data);
        LoadRules(data);
        
        moodController.Initialize(dropZoneGrid);
    }

    private DropZoneGrid SpawnLayout(LevelData data)
    {
        var parent = dropZoneParent != null ? dropZoneParent : transform;
        _spawnedLayout = Instantiate(data.dropZoneLayoutPrefab, parent);
        return _spawnedLayout;
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

        for (int i = 0; i < _spawnedCustomers.Count; i++)
            if (_spawnedCustomers[i].TryGetComponent<CustomerSpawnScale>(out var spawnScale))
                spawnScale.Play(i * spawnStagger);
    }

    private void LoadRules(LevelData data)
    {
        rules ??= FindAnyObjectByType<RuleBook>();
        if (!rules)
        {
            Debug.LogError("No RuleBook found in scene!");
            return;
        }
        
        rules.LoadRules(data.sentences, data.wordBank);
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
