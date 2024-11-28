using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Xml.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    [Header("SFX Settings")]
    public GameObject audioSourcePrefab;
    public int maxAudioSources = 10;
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> activeAudioSources = new List<AudioSource>(); // Para rastrear los SFX activos

    [SerializeField] private AudioClip buttonClickSFX;
    public bool isMuted = true;



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
        if (audioSourcePrefab == null)
        {
            //Debug.LogError("AudioSource prefab no asignado en el Inspector.");
            return;
        }

        //Debug.Log($"Inicializando pool de AudioSources con tamaño {maxAudioSources}.");

        for (int i = 0; i < maxAudioSources; i++)
        {
            GameObject obj = Instantiate(audioSourcePrefab, transform);
            AudioSource source = obj.GetComponent<AudioSource>();

            if (source == null)
            {
                //Debug.LogError($"El prefab {audioSourcePrefab.name} no contiene un AudioSource.");
                continue;
            }

            source.playOnAwake = false;
            obj.SetActive(false);
            audioSourcePool.Enqueue(source); // Agregar al pool
        }
        //Debug.Log($"Pool inicializado con {audioSourcePool.Count} AudioSources.");
    }

    public void PlayMusic(AudioClip clip, bool loopeable)
    {
        if (IsServer()) return;

        musicSource.clip = clip;
        musicSource.loop = loopeable;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (IsServer()) return;
        musicSource.Stop();
    }

    public AudioSource PlaySFX(AudioClip clip)
    {
        if (IsServer()) return null;

        //Debug.Log($"AudioSource pool size antes de reproducir: {audioSourcePool.Count}");

        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            if (source == null)
            {
                //Debug.LogError("El AudioSource sacado del pool es nulo.");
                return null;
            }

            source.gameObject.SetActive(true);
            source.clip = clip;
            source.Play();
            activeAudioSources.Add(source);

            //Debug.Log($"Reproduciendo SFX: {clip.name}, quedan {audioSourcePool.Count} en el pool.");

            StartCoroutine(ReturnToPool(source, clip.length));
            return source;
        }

        //Debug.LogWarning("No hay AudioSources disponibles en el pool.");
        return null;
    }

    public AudioSource PlaySFX(AudioClip clip, Vector3 position)
    {
        if (IsServer() || clip == null) return null;

        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            if (source == null)
            {
                Debug.LogError("El AudioSource sacado del pool es nulo.");
                return null;
            }

            // Mover el AudioSource a la posición deseada
            source.transform.position = position;

            source.gameObject.SetActive(true);
            source.clip = clip;
            source.Play();
            activeAudioSources.Add(source);

            StartCoroutine(ReturnToPool(source, clip.length));
            return source;
        }

        Debug.LogWarning("No hay AudioSources disponibles en el pool.");
        return null;
    }

    public bool IsSFXPlaying(AudioClip clip)
    {
        foreach (var source in activeAudioSources)
        {
            if (source.clip == clip && source.isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    public void StopSFX(AudioSource source)
    {
        if (source == null) return;

        if (activeAudioSources.Contains(source))
        {
            source.Stop();
            source.gameObject.SetActive(false);
            activeAudioSources.Remove(source);
            audioSourcePool.Enqueue(source);
        }
        else
        {
            // Si es un AudioSource externo, simplemente lo detiene
            source.Stop();
        }
    }

    public void StopSFX(AudioClip clip)
    {
        for (int i = activeAudioSources.Count - 1; i >= 0; i--)
        {
            AudioSource source = activeAudioSources[i];
            if (source.clip == clip && source.isPlaying)
            {
                StopSFX(source);
            }
        }
    }

    public void PlaySFX(AudioSource externalSource, AudioClip clip)
    {
        if (IsServer() || externalSource == null) return;

        externalSource.clip = clip;
        externalSource.Play();
        //Debug.Log("SFX PLAYED: " + clip.name);
    }

    private IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        StopSFX(source);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic, true);
    }

    public void PlayGameMusic()
    {
        PlayMusic(gameMusic, false);
    }

    public void PlayButtonSFX()
    {
        PlaySFX(buttonClickSFX);
    }

    private bool IsServer()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;
    }

    public void ToggleAudio()
    {
        if (IsServer()) return;
        isMuted = !isMuted;

        // Activar o desactivar música
        if (musicSource != null)
        {
            Debug.Log("Hola?");
            musicSource.mute = isMuted;
        }

        // Activar o desactivar todos los SFX
        foreach (AudioSource sfx in audioSourcePool)
        {
            if (sfx != null)
            {
                sfx.mute = isMuted;
            }
        }
    }

}
