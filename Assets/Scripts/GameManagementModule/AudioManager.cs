using Signals;
using UnityEngine;
using Zenject;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _audioClips;

    [SerializeField]
    private AudioClip _breakAudioClip;
    private AudioClip _selectedAudioClip;

    [SerializeField]
    private AudioSource _audioSource;

    [Inject]
    private SignalBus _signalBus;

    private int _stackIndex;
    private int _audioClipCount;
    
    private void Start()
    {
        _audioClipCount = _audioClips.Length;
        ListenEvents();
    }

    private void OnBlockFit(BlockFitSignal blockFitSignal)
    {
        if (blockFitSignal.IsSuccessFul)
        {
            _selectedAudioClip = _audioClips[_stackIndex];
            _stackIndex++;
            _stackIndex = Mathf.Clamp(_stackIndex, 0, _audioClipCount - 1);
        }
        else
        {
            _stackIndex = 0;
            _selectedAudioClip = _breakAudioClip;
        }
        
        Play();
    }

    private void ListenEvents()
    {
        _signalBus.Subscribe<BlockFitSignal>(OnBlockFit);
    }
    
    private void Play()
    {
        _audioSource.clip = _selectedAudioClip;
        _audioSource.Play();
    }

    private void UnsubscribeFromEvents()
    {
        _signalBus.TryUnsubscribe<BlockFitSignal>(OnBlockFit);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
}
