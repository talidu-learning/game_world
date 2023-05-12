using System;
using Enumerations;
using UnityEngine;
using UnityEngine.Events;

public class SoundEvent : UnityEvent<SoundType>
{
}

public class GameAudio : MonoBehaviour
{
    [SerializeField] private AudioClip buy;
    [SerializeField] private AudioClip place;
    [SerializeField] private AudioClip placeSocket;
    [SerializeField] private AudioClip delete;
    [SerializeField] private AudioClip select;
    [SerializeField] private AudioClip selectSocket;
    [SerializeField] private AudioClip OpenShop;
    [SerializeField] private AudioClip openInventory;
    [SerializeField] private AudioClip closeShop;
    [SerializeField] private AudioClip closeInventory;
    [SerializeField] private AudioClip invalidPlacement;
    [SerializeField] private AudioClip filter;

    public static SoundEvent PlaySoundEvent = new SoundEvent();

    public static UnityEvent StopPLaying = new UnityEvent();

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        PlaySoundEvent.AddListener(PlaySound);

        StopPLaying.AddListener(OnStopPLaying);
    }

    private void OnStopPLaying()
    {
        _audioSource.Stop();
    }

    private void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Buy:
                _audioSource.PlayOneShot(buy);
                break;
            case SoundType.Place:
                _audioSource.PlayOneShot(place);
                break;
            case SoundType.Delete:
                _audioSource.PlayOneShot(delete);
                break;
            case SoundType.PlaceSocket:
                _audioSource.PlayOneShot(placeSocket);
                break;
            case SoundType.OpenInventory:
                _audioSource.PlayOneShot(openInventory);
                break;
            case SoundType.OpenShop:
                _audioSource.PlayOneShot(OpenShop);
                break;
            case SoundType.InvalidPlacemnet:
                _audioSource.PlayOneShot(invalidPlacement);
                break;
            case SoundType.Select:
                _audioSource.PlayOneShot(select);
                break;
            case SoundType.SelectSocket:
                _audioSource.PlayOneShot(selectSocket);
                break;
            case SoundType.CloseInventory:
                _audioSource.PlayOneShot(closeInventory);
                break;
            case SoundType.CloseShop:
                _audioSource.PlayOneShot(closeShop);
                break;
            case SoundType.Filter:
                _audioSource.PlayOneShot(filter);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
        }
    }
}