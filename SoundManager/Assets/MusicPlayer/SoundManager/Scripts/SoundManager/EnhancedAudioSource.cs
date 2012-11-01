using UnityEngine;
using System.Collections;

public sealed class EnhancedAudioSource : MonoBehaviour
{
	#region Variables
	
	public delegate void AudioCompletionDelegate(EnhancedAudioSource source);
	public delegate void AudioPauseDelegate(EnhancedAudioSource source, bool isPaused);
	
	private event AudioPauseDelegate onAudioPause;
	private event AudioCompletionDelegate onAudioComplete;
	
	private AudioSource myAudio;
	public AudioSource MyAudio{ get{ return myAudio; } }
	
	private bool isPaused;
	public bool IsPaused{ get{ return isPaused; } }
	
	private bool soundCompletionRoutineRunning = false; //make sure we only start one coroutine to check on our playing status
	
	#endregion
	
	#region Methods
	
	#region OverrideAudioSource
	
	/// <summary>
	/// Plays our clip
	/// </summary>
	public void Play()
	{
		isPaused = false;
		myAudio.Play();
		
		if (soundCompletionRoutineRunning == false)
			StartCoroutine(SoundDonePlaying());
	}
	
	/// <summary>
	/// Stops our clip
	/// </summary>
	public void Stop()
	{
		isPaused = false;
		myAudio.Stop();
	}
	
	/// <summary>
	/// Pauses our clip
	/// </summary>
	public void Pause()
	{
		if (isPaused == true) //we are already paused
			return;
		
		isPaused = true;
		
		if (onAudioPause != null)
			onAudioPause(this, isPaused);
		
		myAudio.Pause();
	}
	
	/// <summary>
	/// Unpauses our clip
	/// </summary>
	public void UnPause()
	{
		if (isPaused == false) //we are already unpaused
			return;
		
		isPaused = false;
		
		if (onAudioPause != null)
			onAudioPause(this, isPaused);
		
		Play();
	}
	
	public float volume
	{
		get{ return myAudio.volume; }
		set{ myAudio.volume = value; }
	}
	
	public float minDistance
	{
		get{ return myAudio.minDistance; }
		set{ myAudio.minDistance = value; }
	}
	
	public float maxDistance
	{
		get{ return myAudio.maxDistance; }
		set{ myAudio.maxDistance = value; }
	}
	
	public float pan
	{
		get{ return myAudio.pan; }
		set{ myAudio.pan = value; }
	}
	
	public float panLevel
	{
		get{ return myAudio.panLevel; }
		set{ myAudio.panLevel = value; }
	}
	
	public float pitch
	{
		get{ return myAudio.pitch; }
		set{ myAudio.pitch = value; }
	}
	
	public AudioClip clip
	{
		get{ return myAudio.clip; }
		set{ myAudio.clip = value; }
	}
	
	public bool isPlaying
	{
		get{ return myAudio.isPlaying; }
	}
	
	public bool playOnAwake
	{
		get{ return myAudio.playOnAwake; }
		set{ myAudio.playOnAwake = value; }
	}
	
	public AudioRolloffMode rolloffMode
	{
		get{ return myAudio.rolloffMode; }
		set{ myAudio.rolloffMode = value; }
	}
	
	public bool mute
	{
		get{ return myAudio.mute; }
		set{ myAudio.mute = value; }
	}
	
	public bool loop
	{
		get{ return myAudio.loop; }
		set{ myAudio.loop = value; }
	}
	
	public bool bypassEffects
	{
		get{ return myAudio.bypassEffects; }
		set{ myAudio.bypassEffects = value; }
	}
	
	public float dopplerLevel
	{
		get{ return myAudio.dopplerLevel; }
		set{ myAudio.dopplerLevel = value; }
	}
	
	public float spread
	{
		get{ return myAudio.spread; }
		set{ myAudio.spread = value; }
	}
	
	public float time
	{
		get{ return myAudio.time; }
		set{ myAudio.time = value; }
	}
	
	public AudioVelocityUpdateMode velocityUpdateMode
	{
		get{ return myAudio.velocityUpdateMode; }
		set{ myAudio.velocityUpdateMode = value; }
	}
	
	public int timeSamples
	{
		get{ return myAudio.timeSamples; }
		set{ myAudio.timeSamples = value; }
	}
	
	void OnEnable()
	{
		if (myAudio == null)
			myAudio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		
		myAudio.enabled = true;
	}
	
	void OnDisable()
	{
		if (myAudio != null)
			myAudio.enabled = false;
	}
	
	#endregion
	
	public void RegisterAudioPauseDelegate(AudioPauseDelegate del)
	{
		onAudioPause += del;
	}
	
	public void RemoveAudioPauseDelegate(AudioPauseDelegate del)
	{
		onAudioPause -= del;
	}
	
	public void RegisterAudioCompleteDelegate(AudioCompletionDelegate del)
	{
		onAudioComplete += del;
	}
	
	public void RemoveAudioCompleteDelegate(AudioCompletionDelegate del)
	{
		onAudioComplete -= del;
	}
	
	/// <summary>
	/// Fast forwards our audio track
	/// </summary>
	public void Forward()
	{
		time += 0.35f;
	}
	
	/// <summary>
	/// Rewinds our audio track
	/// </summary>
	public void Rewind()
	{
		time -= 0.35f;
	}
	
	/// <summary>
	/// Returns the total length of our current track
	/// </summary>
	/// <returns>
	/// The track total length.
	/// </returns>
	public float CurrentTrackTotalLength()
	{
		if (myAudio.clip == null)
			return -100.0f; //error
		else
			return clip.length;
	}
	
	/// <summary>
	/// Returns the current position of out track
	/// </summary>
	/// <returns>
	/// The track time position.
	/// </returns>
	public float CurrentTrackTimePosition()
	{
		if (myAudio.clip == null)
			return -100.0f; //error
		else
			return time;
	}
	
	/// <summary>
	/// Handles the delegates and cleanup for when the sound is finished playing
	/// </summary>
	/// <returns>
	/// The done playing.
	/// </returns>
	IEnumerator SoundDonePlaying()
    {
		soundCompletionRoutineRunning = true;
		bool playingOrPaused = true;
		
        while (playingOrPaused == true)
		{
			playingOrPaused = (isPlaying == false && isPaused == false) ? false : true; //we may just be paused here
			
            yield return null;
		}

        if (onAudioComplete != null)
            onAudioComplete(this);

        clip = null;
		
		soundCompletionRoutineRunning = false;
    }
	
	#endregion
}