using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip dragStartClip;
    [SerializeField] private AudioClip frogPlacedClip;
    [SerializeField] private AudioClip levelClearedClip;
    [SerializeField] private AudioClip ruleDragClip;
    [SerializeField] private AudioClip ruleDropClip;
    [SerializeField] private AudioClip frogDroppedClip;

    private AudioSource _source;

    private void Awake() => _source = GetComponent<AudioSource>();

    private void OnEnable()
    {
        DragManager.OnFrogPickedUp += PlayDragStart;
        DragManager.OnFrogPlaced += PlayFrogPlaced;
        DragManager.OnFrogDropped += PlayFrogDropped;
        WinLoseEvaluator.OnLevelCleared += PlayLevelCleared;
        WordToken.OnDragStarted += PlayRuleDrag;
        WordToken.OnDropped += PlayRuleDrop;
    }

    private void OnDisable()
    {
        DragManager.OnFrogPickedUp -= PlayDragStart;
        DragManager.OnFrogPlaced -= PlayFrogPlaced;
        DragManager.OnFrogDropped -= PlayFrogDropped;
        WinLoseEvaluator.OnLevelCleared -= PlayLevelCleared;
        WordToken.OnDragStarted -= PlayRuleDrag;
        WordToken.OnDropped -= PlayRuleDrop;
    }

    private void PlayDragStart() => Play(dragStartClip);

    private void PlayFrogPlaced() => Play(frogPlacedClip);

    private void PlayFrogDropped() => Play(frogDroppedClip);

    private void PlayLevelCleared() => Play(levelClearedClip);

    private void PlayRuleDrag() => Play(ruleDragClip);

    private void PlayRuleDrop() => Play(ruleDropClip);

    private void Play(AudioClip clip)
    {
        _source.pitch = Random.Range(0.9f, 1.1f);

        if (clip != null)
            _source.PlayOneShot(clip);
    }
}
