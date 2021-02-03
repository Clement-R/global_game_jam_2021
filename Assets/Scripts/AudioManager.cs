using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] grabSounds;
    public AudioClip[] dropSounds;

    public void Play(string name)
    {
        AudioClip clip = null;
        if (name == "drop")
        {
            clip = dropSounds[Random.Range(0, dropSounds.Length)];
        }
        if (name == "grab")
        {
            clip = dropSounds[Random.Range(0, dropSounds.Length)];
        }
        if (clip == null)
        {
            return;
        }
        var obj = new GameObject();
        var audioSource = obj.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.Play();
        Destroy(obj, clip.length + 0.1f);
    }
}