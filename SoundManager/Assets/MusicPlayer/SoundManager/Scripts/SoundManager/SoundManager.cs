using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AudioListener))]
public sealed class SoundManager : TSingleton<SoundManager>
{
    #region Variables
	
	/// <summary>
	/// The volume variable. Call SetVolume to change the volume of all audio clips
	/// </summary>
    public float volume = 1.0f;
    public int audioSourcesAtStart = 10;

    private List<EnhancedAudioSource> audioSources;

    #endregion

    #region Init
	
	protected override void Awake()
	{
		base.Awake();
		
		audioSources = new List<EnhancedAudioSource>();

        //start out with a certain amount of audio sources
        for (int x = 0; x < audioSourcesAtStart; x++)
        {
            EnhancedAudioSource temp = gameObject.AddComponent(typeof(EnhancedAudioSource)) as EnhancedAudioSource;
            temp.audio.playOnAwake = false;
            audioSources.Add(temp);
        }

        SetVolume(volume);
	}

    #endregion

    #region Methods
	
	/// <summary>
	/// Use this to set the volume
	/// </summary>
	/// <param name='newVolume'>
	/// New volume.
	/// </param>
    public static void SetVolume(float newVolume)
    {
        I.volume = Mathf.Clamp(newVolume, 0.0f, 1.0f);

        I.SetGlobalVolume();
    }

    /// <summary>
    /// Sets Volume to currently set volume variable for each audio source we have
    /// </summary>
    void SetGlobalVolume()
    {
        foreach (EnhancedAudioSource aS in audioSources)
            if (aS != null)
                aS.volume = volume;
    }
	
	/// <summary>
	/// Stops the sound. WARNING this will stop all sounds that are equal to this clip
	/// </summary>
	/// <param name='clipToStop'>
	/// Clip to stop.
	/// </param>
	public static void StopSound(AudioClip clipToStop)
	{
		var sounds = from s in I.audioSources
					where s.clip.Equals(clipToStop)
					select s;
		
		foreach(var sound in sounds)
			sound.Stop();
	}

    /// <summary>
    /// Will play an AudioClip once, attaches it to the supplied GameObject, and call the CompletionDelegate when its done playing
    /// </summary>
    /// <param name="clip">
    /// A <see cref="AudioClip"/>
    /// </param>
    /// <param name="completionDelegate">
    /// A <see cref="AudioCompleteDelegate"/>
    /// </param>
    /// <returns>
    /// A <see cref="AudioSource"/>
    /// </returns>
    public static EnhancedAudioSource PlaySoundOnce(AudioClip clip, GameObject objectToAttachTo = null)
    {
        if (clip == null)
        {
            Debug.LogWarning("Warning! null AudioClip. Returning");
            return null;
        }

        EnhancedAudioSource aS = I.GetEmptyAudioSource(objectToAttachTo);

        //just incase we recivew null from above function
        if (aS != null)
        {
            aS.clip = clip;
            aS.loop = false;
            aS.Play();
        }

        return aS;
    }

    /// <summary>
    /// Will play a looping sound, attached to the supplied GameObject. Must be stopped manually
    /// </summary>
    /// <param name="clip">
    /// A <see cref="AudioClip"/>
    /// </param>
    /// <param name="completionDelegate">
    /// A <see cref="AudioCompleteDelegate"/>
    /// </param>
    /// <returns>
    /// A <see cref="AudioSource"/>
    /// </returns>
    public static EnhancedAudioSource PlaySoundLooping(AudioClip clip, GameObject objectToAttachTo = null)
    {
        if (clip == null)
        {
            Debug.LogWarning("Warning! null AudioClip. Returning");
            return null;
        }

        EnhancedAudioSource aS = I.GetEmptyAudioSource(objectToAttachTo);

        //just incase we recivew null from above function
        if (aS != null)
        {
            aS.clip = clip;
            aS.loop = true;
            aS.Play();
        }

        return aS;
    }

    /// <summary>
    /// Gets first empty audio source. If none available will create a new one. Automatically sets volume to current global volume set in the SoundManager
    /// </summary>
    /// <param name="objectToAddTo">
    /// A <see cref="GameObject"/>
    /// </param>
    /// <returns>
    /// A <see cref="AudioSource"/>
    /// </returns>
    EnhancedAudioSource GetEmptyAudioSource(GameObject objectToAttachTo)
    {
        EnhancedAudioSource tempAudioSource = null;

        foreach (EnhancedAudioSource aS in audioSources)
        {
            if (aS.clip == null)
            {
                if (objectToAttachTo == null)
                    tempAudioSource = aS;
                else
                {
                    //if objectToAttachTo is supplied and not null, search that object for open AudioSources, Otherwise make a new one on that object
                    if (aS.gameObject.Equals(objectToAttachTo) == true)
                        tempAudioSource = aS;
                }

                if (tempAudioSource != null)
                    goto Return;
            }
        }

        //add either to gameObject passed in or to the SouneManager
        //will only hit this if we have no available audio sources
        if (objectToAttachTo == null)
            tempAudioSource = gameObject.AddComponent(typeof(EnhancedAudioSource)) as EnhancedAudioSource;
        else
            tempAudioSource = objectToAttachTo.AddComponent(typeof(EnhancedAudioSource)) as EnhancedAudioSource;

    	Return:
        {
            if (tempAudioSource != null)
            {
                bool existsInList = false;

                tempAudioSource.playOnAwake = false;

                foreach (EnhancedAudioSource aS in audioSources)
                    if (aS.Equals(tempAudioSource))
                        existsInList = true;

                //add the newly created audiosource to our list of sources only if its not already in there. This will only get called if we crate a new one, so we dont have to worry about adding duplicates
                if (existsInList == false)
                    audioSources.Add(tempAudioSource);
                tempAudioSource.volume = volume;
            }

            return tempAudioSource;
        }
    }

    #endregion
}