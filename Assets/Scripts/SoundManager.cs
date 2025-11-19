using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource audioSource;
    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip mismatchClip;
    public AudioClip gameOverClip;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayFlip()
    {
        PlayOneShot(flipClip);
    }
    public void PlayMatch()
    {
        PlayOneShot(matchClip);
    }
    public void PlayMismatch()
    {
        PlayOneShot(mismatchClip);
    }
    public void PlayGameOver()
    {
        PlayOneShot(gameOverClip);
    }

    void PlayOneShot(AudioClip clip)
    {
        if (clip == null)
            return;
        audioSource.PlayOneShot(clip);
    }
}
