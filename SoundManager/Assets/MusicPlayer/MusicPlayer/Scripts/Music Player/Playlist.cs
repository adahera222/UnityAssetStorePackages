using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public sealed class Playlist
{
	#region Variables
	
	public delegate void OnPlaylistComplete(Playlist completedPlaylist);
	private event OnPlaylistComplete onPlaylistComplete;
	
	public string name;
	public List<AudioClip> audioClips;
	
	public enum RepeatOptions
	{
		Off,
		All,
		Song
	}
	public RepeatOptions repeat;
	/// <summary>
	/// Read only. Public for inspector use
	/// </summary>
	public bool shuffle;
	
	private EnhancedAudioSource currentAudioClip;
	public EnhancedAudioSource CurrentAudioClip{ get{ return currentAudioClip; } }
	private int currentAudioClipIndex = 0;
	
	#endregion
	
	#region Init
	
	public Playlist(params AudioClip[] clips)
	{
		if (audioClips == null)
			audioClips = new List<AudioClip>();
		
		foreach(AudioClip clip in clips)
			audioClips.Add(clip);
	}
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Registers a completion delegate.
	/// </summary>
	/// <param name='completionDel'>
	/// Completion del.
	/// </param>
	public void RegisterCompletionDelegate(OnPlaylistComplete completionDel)
	{
		onPlaylistComplete += completionDel;
	}
	
	/// <summary>
	/// Removes a completion delegate.
	/// </summary>
	/// <param name='completionDel'>
	/// Completion del.
	/// </param>
	public void RemoveCompletionDelegate(OnPlaylistComplete completionDel)
	{
		onPlaylistComplete -= completionDel;
	}
	
	/// <summary>
	/// Play the playlist from the beginning
	/// </summary>
	public void Play()
	{
		if (audioClips.Count <= 0)
			return;
		
		if (shuffle == true)
			audioClips.ShuffleContents();
		
		currentAudioClipIndex = 0;
		currentAudioClip = SoundManager.PlaySoundOnce(audioClips[0]);
		currentAudioClip.RegisterAudioCompleteDelegate(CurrentTrackComplete);
	}
	
	/// <summary>
	/// Pause this playlist.
	/// </summary>
	public void Pause()
	{
		if (currentAudioClip == null)
			return;
		
		currentAudioClip.Pause();
	}
	
	/// <summary>
	/// Unpause this playlist
	/// </summary>
	public void UnPause()
	{
		if (currentAudioClip == null)
			return;
		
		currentAudioClip.UnPause();
	}
	
	/// <summary>
	/// Stop this playlist
	/// </summary>
	public void Stop()
	{
		if (currentAudioClip == null)
			return;
		
		currentAudioClip.RemoveAudioCompleteDelegate(CurrentTrackComplete); //make sure we done start a new song
		currentAudioClip.Stop();
		
		if (onPlaylistComplete != null)
			onPlaylistComplete(this);
	}
	
	/// <summary>
	/// This will be called on the current track ending. All handling of next track and playlist ending occurs here
	/// </summary>
	/// <param name='s'>
	/// S.
	/// </param>
	void CurrentTrackComplete(EnhancedAudioSource s)
	{
		if (currentAudioClip == null)
			return;
		
		currentAudioClip.RemoveAudioCompleteDelegate(CurrentTrackComplete); //remove our old deleagte
		
		if (repeat != RepeatOptions.Song) //this will make sure we play same song if we have it on repeate song
			currentAudioClipIndex++;
		
		if (currentAudioClipIndex >= audioClips.Count) //we are at the end of our playlist
		{
			currentAudioClipIndex = 0; //reset
			
			if (repeat == RepeatOptions.All) //if we are repeating, then play again!
				Play();
			else
				if (onPlaylistComplete != null) //call the playlist completion delegate
					onPlaylistComplete(this);
			
			return;
		}
		
		currentAudioClip = SoundManager.PlaySoundOnce(audioClips[currentAudioClipIndex]); //if we hit here we need to play the next song in the list
		currentAudioClip.RegisterAudioCompleteDelegate(CurrentTrackComplete);
	}
	
	/// <summary>
	/// Skips forward a track.
	/// </summary>
	public void NextTrack()
	{
		if (currentAudioClip == null)
			return;
		
		currentAudioClip.Stop();
	}
	
	/// <summary>
	/// Skips back a track.
	/// </summary>
	public void PreviousTrack()
	{
		if (currentAudioClip == null)
			return;
		
		if (repeat != RepeatOptions.Song) //this will make sure we play same song if we have it on repeate song
		{
			currentAudioClipIndex -= 2;
			
			if (currentAudioClipIndex < -1) //if we are at the first song go back to the last one
				currentAudioClipIndex = audioClips.Count - 2;
		}
		
		currentAudioClip.Stop();
	}
	
	/// <summary>
	/// Toggles shuffle mode. If you are not in suffle mode and call this, it will shuffle the playlist and restart from the beginning of a newly shuffled list
	/// </summary>
	public void ToggleShuffle()
	{
		if (shuffle == false)
			Shuffle();
		else
			shuffle = false;
	}
	
	/// <summary>
	/// Force a shuffle
	/// </summary>
	public void Shuffle()
	{
		shuffle = true;
		
		currentAudioClip.RemoveAudioCompleteDelegate(CurrentTrackComplete);
		currentAudioClip.Stop();
		
		audioClips.ShuffleContents();
		
		Play();
	}
	
	/// <summary>
	/// Toggles repeat mode
	/// </summary>
	public void ToggleRepeat()
	{
		int current = (int)repeat;
		current++;
		
		if (current > 2)
			current = 0;
		
		repeat = (RepeatOptions)current;
	}
	
	#endregion
}