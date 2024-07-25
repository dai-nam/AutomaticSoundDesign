using UnityEngine;
using EasyButtons;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.Timeline;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadFile : MonoBehaviour
{

    List<AudioSource> audioSources;
 

    void Start()
    {
        AudioSource[] existingAudiosources = GetComponents<AudioSource>();
        foreach (AudioSource audioSource in existingAudiosources)
        {
            Destroy(audioSource);
        }

        audioSources = new List<AudioSource>();
    }


    [Button]
    public void Load()
    {
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        AudioClip[] loadedClips = Resources.LoadAll<AudioClip>("");

        if (loadedClips.Length == 0)
        {
            Debug.LogError("No audio files found in Resources");
            return;
        }

        foreach (AudioClip clip in loadedClips)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSources.Add(audioSource);
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
        }

    }

    [Button]
    public void PlayAll()
    {
        foreach(AudioSource audioSource in audioSources)
        {            
            audioSource.Play();
        }        
    }
    
}
