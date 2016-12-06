using UnityEngine;
using System.Collections;

public class SoundManager : Photon.MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource1;
    public AudioSource musicSource2;
    public static SoundManager instance = null;

    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

	// Use this for initialization
	void Awake () {
		
	    	if(instance == null)
        	{
            	instance = this;
        	}
        	else if (instance != this)
       	 	{
            	Destroy(gameObject);
        	}
        DontDestroyOnLoad(gameObject);
	}

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void StartSecondBGM()
    {
        musicSource1.Stop();
        musicSource2.Play();
    }

    public void StartFirstBGM()
    {
        musicSource2.Stop();
        musicSource1.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
