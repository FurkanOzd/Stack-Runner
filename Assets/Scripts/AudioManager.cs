using Signals;
using UnityEngine;
using Zenject;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _audioClips;

    [SerializeField]
    private AudioSource _audioSource;

    [Inject]
    private SignalBus _signalBus;

    private int _playIndex;
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
            _playIndex++;
            _playIndex = Mathf.Clamp(_playIndex, 0, _audioClipCount -1);
        }
        else
        {
            _playIndex = 0;
        }
        
        Play();
    }

    private void ListenEvents()
    {
        _signalBus.Subscribe<BlockFitSignal>(OnBlockFit);
    }
    
    private void Play()
    {
        _audioSource.clip = _audioClips[_playIndex];
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
