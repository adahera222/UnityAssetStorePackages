using UnityEngine;
using System.Collections;

public class MusicPlayerUse : MonoBehaviour
{
	public Texture2D playButton;
	public Texture2D stopButton;
	public Texture2D pauseButton;
	public Texture2D nextButton;
	public Texture2D previousButton;
	public Texture2D forwardButton;
	public Texture2D rewindButton;
	
	public Texture2D shuffleOnButton;
	public Texture2D shuffleOffButton;
	
	public Texture2D repeatOffButton;
	public Texture2D repeatAllButton;
	public Texture2D repeatSongButton;
	
	public Light[] lights;
	
	void Awake()
	{
		Application.runInBackground = true;
		InvokeRepeating("Animate", 0.0f, 0.2f);
	}
	
	void Animate()
	{
		if (MusicPlayer.I.CurrentPlaylist != null && MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.IsPaused == false)
			foreach(Light l in lights)
				l.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
	}
	
	void OnGUI()
	{
		DrawControls();
	}
	
	void DrawControls()
	{
		GUI.backgroundColor = new Color(0, 0, 0, 0);
		Rect controlRect = new Rect(0, 0, playButton.width, playButton.height);
		
		if (MusicPlayer.I.CurrentPlaylist == null) //show list of current playlists
		{
			foreach(Playlist p in MusicPlayer.I.playlists)
			{
				GUI.Label(new Rect(controlRect.x + playButton.width, controlRect.y + (playButton.height / 2) - 10, 400, 20),  "Play " + p.name);
				
				if (GUI.Button(controlRect, playButton))
					MusicPlayer.I.PlayPlaylist(p.name);
				
				controlRect.y += playButton.height;
			}
		}
		else if (MusicPlayer.I.CurrentPlaylist != null && MusicPlayer.I.CurrentPlaylist.CurrentAudioClip != null) //we are playing a playlist. draw music controls
		{
			#region BasicControls
			
			if (GUI.Button(new Rect(0, 0, 50, 50), new GUIContent(stopButton, "Press To Stop")))
				MusicPlayer.I.CurrentPlaylist.Stop();
			
			Texture2D playPauseTex = (MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.IsPaused == true) ? playButton : pauseButton; //are we paused or playing
			
			if (GUI.Button(new Rect(50, 0, 50, 50), new GUIContent(playPauseTex, "Press To Play/Pause")))
			{
				if (MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.IsPaused == true)
					MusicPlayer.I.CurrentPlaylist.UnPause();
				else
					MusicPlayer.I.CurrentPlaylist.Pause();
			}
			
			if (GUI.Button(new Rect(100, 0, 50, 50), new GUIContent(previousButton, "Press To Go Back A Track")))	
				MusicPlayer.I.CurrentPlaylist.PreviousTrack();
			
			if (GUI.Button(new Rect(150, 0, 50, 50), new GUIContent(nextButton, "Press To Skip Track")))
				MusicPlayer.I.CurrentPlaylist.NextTrack();
			
			if (GUI.RepeatButton(new Rect(200, 0, 50, 50), new GUIContent(rewindButton, "Hold To Rewind")))
				MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.Rewind();
			
			if (GUI.RepeatButton(new Rect(250, 0, 50, 50), new GUIContent(forwardButton, "Hold To Fast Forward")))
				MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.Forward();
			
			#endregion
			
			#region SuffleAndRepeat
			
			#region SelectRepeatTexture
			
			Texture2D useRepeatButton = null;
			
			switch(MusicPlayer.I.CurrentPlaylist.repeat)
			{
				case Playlist.RepeatOptions.Off:
					useRepeatButton = repeatOffButton;
				break;
				
				case Playlist.RepeatOptions.All:
					useRepeatButton = repeatAllButton;
				break;
				
				case Playlist.RepeatOptions.Song:
					useRepeatButton = repeatSongButton;
				break;
			}
			
			#endregion
			
			if (GUI.Button(new Rect(300, 0, 50, 50), new GUIContent(useRepeatButton, "Press To Toggle Repeat Mode")))
				MusicPlayer.I.CurrentPlaylist.ToggleRepeat();
			
			Texture2D useShuffleButton = (MusicPlayer.I.CurrentPlaylist.shuffle == true) ? shuffleOnButton : shuffleOffButton;
			
			//Debug.Log(MusicPlayer.I.CurrentPlaylist.shuffle);
			
			if (GUI.Button(new Rect(350, 0, 50, 50), new GUIContent(useShuffleButton, "Press To Toggle Shuffle")))
				MusicPlayer.I.CurrentPlaylist.ToggleShuffle();
			
			#endregion
			
			//TOOLTIP
			GUI.Label(new Rect(215, 55, 300, 20), GUI.tooltip);
			
			#region SongInfoAndTime
			
			if (MusicPlayer.I.CurrentPlaylist.CurrentAudioClip != null)
			{
				Rect temp = new Rect(0, 100, 400, 20);
				
				//current song info
				if (MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.clip != null)
					GUI.Label(temp, "Playing song " + MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.clip.name);
				
				GUI.backgroundColor = new Color(1, 1, 1, 1);
				temp.y += 20;
				//audio time bar
				GUI.HorizontalSlider(temp, MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.CurrentTrackTimePosition(), 0.0f, MusicPlayer.I.CurrentPlaylist.CurrentAudioClip.CurrentTrackTotalLength());
				GUI.backgroundColor = new Color(0, 0, 0, 0);
			}
			
			#endregion
			
			#region Volume
			
			GUI.backgroundColor = new Color(1, 1, 1, 1);
			
			SoundManager.I.volume = GUI.VerticalSlider(new Rect(450, 15, 30, 100), SoundManager.I.volume, 1.0f, 0.0f);
			SoundManager.SetVolume(SoundManager.I.volume);
			float yPos = 115 - (100 * SoundManager.I.volume) - 15; //calculate y position for label to follow slider
			GUI.Label(new Rect(460, yPos, 500, 20), SoundManager.I.volume + " VOLUME");
			
			#endregion
		}
	}
}