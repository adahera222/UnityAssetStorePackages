using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class MusicPlayer : TSingleton<MusicPlayer>
{
	#region Variables
	
	public List<Playlist> playlists;
	
	private Playlist currentPlaylist;
	public Playlist CurrentPlaylist{ get{ return currentPlaylist; } }
	
	#endregion
	
	#region Init
	
	protected override void Awake()
	{
		base.Awake();
		
		if (playlists == null)
			playlists = new List<Playlist>();
	}
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Will search through our playlists by name and play the one specified if found
	/// </summary>
	/// <param name='searchName'>
	/// Search name.
	/// </param>
	public void PlayPlaylist(string searchName)
	{
		var foundPlaylist = playlists.Find(p => p.name == searchName);
		
		currentPlaylist = foundPlaylist;
		
		if (currentPlaylist != null)
		{
			currentPlaylist.Play();
			currentPlaylist.RegisterCompletionDelegate(OnPlaylistComplete); //for cleanup
		}
		else
			Debug.LogWarning("Warning! Could not play playlist titled " + searchName + " Reason - Could Not Find");
	}
	
	/// <summary>
	/// Called on playlist completion. Used for cleanup
	/// </summary>
	/// <param name='p'>
	/// P.
	/// </param>
	void OnPlaylistComplete(Playlist p)
	{
		if (p.Equals(currentPlaylist) == false)
			return;
		currentPlaylist = null;
	}
	
	#endregion
}