using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [SerializeField]
    private AudioClip bgm;
    [SerializeField]
    private AudioClip[] audioClips;
    [SerializeField]
    private string[] clipNames;

    private AudioSource _audioSource;
    private Dictionary<string, AudioClip> _clipPairs = new Dictionary<string, AudioClip>();

    #region Unity Method
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for(int i = 0; i < audioClips.Length; ++i)
        {
            _clipPairs.Add(clipNames[i], audioClips[i]);
        }
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = bgm;
        _audioSource.Play();
    }
    #endregion

    #region Utils
    public void PlaySoundAtPosition(string name, Vector3 position)
    {
        AudioClip clip = _clipPairs[name];
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, position);
    }

    public void StopBGM()
    {
        _audioSource.Stop();
    }

    public void PlayBGM()
    {
        _audioSource.Play();
    }

    #endregion
}
