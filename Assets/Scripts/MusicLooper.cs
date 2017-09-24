using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLooper : MonoBehaviour {

    public AudioSource source;
    public List<AudioClip> clips;
    public bool randomiseFirstClip;

    private int clipIdx; 

	// Use this for initialization
	void Start () {
		if (randomiseFirstClip)
        {
            clipIdx = Mathf.RoundToInt(Random.Range(0, clips.Count));
        }
         else
        {
            clipIdx = 0;
        }
        PlayClip(clipIdx);
	}

    private void PlayNext()
    {
        clipIdx++;
        if (clipIdx >= clips.Count)
            clipIdx = 0;
        PlayClip(clipIdx);
    }

    private void PlayClip(int clipIdx)
    {
        source.clip = clips[clipIdx];
        source.Play(0);
        Invoke("PlayNext", source.clip.length);
    }



	

}
