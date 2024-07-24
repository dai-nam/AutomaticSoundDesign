using UnityEngine;
using EasyButtons;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.Timeline;

public class LoadFile : MonoBehaviour
{

    public List<AudioClip> audioClips  = new List<AudioClip>();
    AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
 
    [Button]
    public void Load()
    {
        audioSource.clip = null;
        audioClips.Clear();

        AudioClip[] loadedClips = Resources.LoadAll<AudioClip>("");

        if (loadedClips.Length == 0)
        {
            Debug.LogError("No audio files found in Resources");
            return;
        }

        foreach (AudioClip clip in loadedClips)
        {
            audioClips.Add(clip);
            Debug.Log("Loaded audio clip: " + clip.name);
        }

    }

    [Button]
    public void PlayAll()
    {
        StartCoroutine(PlayClipsSequentially());
    }

    
    IEnumerator PlayClipsSequentially()
    {

        foreach (AudioClip clip in audioClips)
        {
            print("Playing: "+clip.name);
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
    }
}
