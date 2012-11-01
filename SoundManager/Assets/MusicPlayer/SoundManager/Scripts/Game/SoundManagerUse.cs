using UnityEngine;
using System.Collections;

public class SoundManagerUse : MonoBehaviour
{
	public AudioClip playOnceSound;
	public AudioClip loopingSound;
	
	public GameObject front;
	public GameObject right;
	public GameObject rear;
	public GameObject left;
	
	private EnhancedAudioSource loopingAudioSource;
	
	void Awake()
	{
		Application.runInBackground = true;
	}
	
	void OnGUI()
	{
		GUI.backgroundColor = new Color(0, 0, 0, 0.7f);
		GUI.skin.button.normal.background = new Texture2D(1, 1);
		GUI.skin.box.normal.background = new Texture2D(1, 1);
		
		GUI.Box(new Rect(0, 0, Screen.width, 50), "You can attach the sound to either the sound manager or the game object of your choice. The benefit in this is if you are using 3D sounds." +
					"Keey the SoundManager and an AudioListener on the players point of view, and add sounds to gameobjects around you to hear them correctly in 3D");
		
		if (loopingAudioSource == null)
		{
			if (GUI.Button(new Rect(0, 50, Screen.width, 30), "Play Sound Once, Add To Sound Manager"))
				SoundManager.PlaySoundOnce(playOnceSound);
			
			if (GUI.Button(new Rect(0, 80, Screen.width, 30), "Play Sound Once, Front"))
				SoundManager.PlaySoundOnce(playOnceSound, front);
			
			if (GUI.Button(new Rect(0, 110, Screen.width, 30), "Play Sound Once, Right"))
				SoundManager.PlaySoundOnce(playOnceSound, right);
			
			if (GUI.Button(new Rect(0, 140, Screen.width, 30), "Play Sound Once, Rear"))
				SoundManager.PlaySoundOnce(playOnceSound, rear);
			
			if (GUI.Button(new Rect(0, 170, Screen.width, 30), "Play Sound Once, Left"))
				SoundManager.PlaySoundOnce(playOnceSound, left);
			
			if (GUI.Button(new Rect(0, 200, Screen.width, 30), "Play Sound Looping"))
				loopingAudioSource = SoundManager.PlaySoundLooping(loopingSound);
		}
		else
		{
			if (GUI.Button(new Rect(0, 50, Screen.width, 30), "Stop Sound Looping"))
			{
				loopingAudioSource.Stop();
				loopingAudioSource = null;
			}
		}
		
		SoundManager.I.volume = GUI.VerticalSlider(new Rect(Screen.width / 2 - 15, Screen.height - 300, 30, 300), SoundManager.I.volume, 1.0f, 0.0f);
		SoundManager.SetVolume(SoundManager.I.volume);
		float yPos = Screen.height - (300 * SoundManager.I.volume) - 15;
		GUI.Label(new Rect(Screen.width / 2, yPos, 300, 20), SoundManager.I.volume + " VOLUME");
	}
	
	void OnComplete(AudioSource s)
	{
		Debug.Log(s + " finished");
	}
}