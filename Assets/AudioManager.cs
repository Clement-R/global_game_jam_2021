using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public AudioClip[] grabSounds;
    public AudioClip[] dropSounds;
    
    // Start is called before the first frame update
    void Awake()
    {
        // foreach(Sound s in sounds)
        // {
        //     s.source = gameObject.AddComponent<AudioSource>();
        //     s.source.clip = s.clip;
            
        //     s.source.volume = s.volume;
        //     s.source.pitch = s.pitch;
        // }
    }

    // Update is called once per frame
    public void Play(string name)
    {
        AudioClip clip = null;
        if (name == "drop") {
            clip = dropSounds[Random.Range(0, dropSounds.Length)];
        }
        if (name == "grab") {
            clip = dropSounds[Random.Range(0, dropSounds.Length)];
        }
        if (clip == null) {
            return;
        }        
        var obj = new GameObject();
        var audioSource = obj.AddComponent<AudioSource>();
        
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(obj, clip.length + 0.1f);
    }
}
