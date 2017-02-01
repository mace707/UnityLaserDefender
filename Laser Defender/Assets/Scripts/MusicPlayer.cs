using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


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

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if(Music)
		{
			Music.Stop();

			switch(scene.buildIndex)
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

}
