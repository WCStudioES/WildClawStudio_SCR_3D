using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;

    [Header("SFX Settings")]
    public GameObject audioSourcePrefab;
    public int maxAudioSources = 10;
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSourcePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSourcePool()
    {
        for (int i = 0; i < maxAudioSources; i++)
        {
            GameObject obj = Instantiate(audioSourcePrefab, transform);
            AudioSource source = obj.GetComponent<AudioSource>();
            source.playOnAwake = false;
            obj.SetActive(false);
            audioSourcePool.Enqueue(source);
        }
    }

    public void PlayMusic(AudioClip clip, bool loopeable)
    {
        musicSource.clip = clip;
        musicSource.loop = loopeable;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            source.gameObject.SetActive(true);
            source.clip = clip;
            source.Play();
            StartCoroutine(ReturnToPool(source, clip.length));
        }
    }

    private IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic, true);
    }

    public void PlayGameMusic()
    {
        PlayMusic(gameMusic, false);
    }
}
