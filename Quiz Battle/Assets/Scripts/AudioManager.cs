using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource soundEffectSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip buttonNavigateClip;
    [SerializeField] private AudioClip buttonSelectClip;

    private void Start()
    {
        // Start playing background music
        if (backgroundMusicClip != null)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.loop = true; // Loop background music
            backgroundMusicSource.Play();
        }
    }

    // Method to play navigation sound
    public void PlayNavigateSound()
    {
        if (buttonNavigateClip != null)
        {
            soundEffectSource.PlayOneShot(buttonNavigateClip);
        }
    }

    // Method to play button select sound
    public void PlaySelectSound()
    {
        if (buttonSelectClip != null)
        {
            soundEffectSource.PlayOneShot(buttonSelectClip);
        }
    }
}