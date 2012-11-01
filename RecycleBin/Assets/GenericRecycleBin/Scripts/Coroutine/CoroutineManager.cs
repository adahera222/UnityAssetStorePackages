using System.Collections.Generic;
using System.Collections;

public sealed class CoroutineManager : TSingleton<CoroutineManager>
{
	#region Variables
	
	private List<Routine> allRoutines;
	public List<Routine> AllRoutines{ get{ return allRoutines; } }
	
	#endregion
	
	#region Init
	
	protected override void Awake()
	{
		base.Awake();
		allRoutines = new List<Routine>();
	}
	
	#endregion
	
	#region Methods
	
	public static Routine CreateRoutine(IEnumerator iE, bool startPaused = false)
	{
		Routine temp = new Routine(iE, startPaused);
		temp.AddCompletionDelegates(I.RoutineFinished); //add completion del for each routine so we can remove it on completion
		I.allRoutines.Add(temp);
		return temp;
	}
	
	void RoutineFinished(bool wasKilled, Routine r)
	{
		allRoutines.Remove(r);
		r = null;
	}
	
	#endregion
}