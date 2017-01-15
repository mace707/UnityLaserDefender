using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {
	static MusicPlayer instance = null;

	public AudioClip StartClip;
	public AudioClip GameClip;
	public AudioClip EndClip;

	private AudioSource Music;

	void Start () 
	{
		if (instance != null && instance != this) 
		{
			Destroy (gameObject);
			print ("Duplicate music player self-destructing!");
		} 
		else 
		{
			instance = this;
			GameObject.DontDestroyOnLoad (gameObject);
			Music = GetComponent<AudioSource> ();
			Music.clip = StartClip;
			Music.loop = true;
			Music.Play();
		}
	}

	void OnLevelWasLoaded(int level)
	{
		Debug.Log("Music Player Loaded Level " + level);
		Music.Stop();

		switch (level) 
		{
		case 0:
			Music.clip = StartClip;
			break;
		case 1:
			Music.clip = GameClip;
			break;
		case 2:
			Music.clip = EndClip;
			break;
		}

		Music.loop = true;

		Music.Play();
	}

}
