using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public sealed class Routine : IDisposable
{
	#region Variables
	
	public delegate void OnRoutinePause(bool didPause, Routine self);
	public delegate void OnRoutineComplete(bool wasKilledEarly, Routine self);
	
	private OnRoutinePause pauseEvent;
	private OnRoutineComplete completionEvent;
	
	private bool isRunning;
	public bool IsRunning { get { return isRunning; } }
	
	private bool isPaused;
	public bool IsPaused { get { return isPaused; } }
	
	private IEnumerator coroutine;
	private bool routineWasKilled;
	
	#endregion
	
	#region Methods
	
	public Routine (IEnumerator iE, bool startPaused)
	{
		coroutine = iE;
		isPaused = startPaused;
		
		if (startPaused == true)
			isRunning = false;
		else
			StartRoutine();
	}
	
	public void AddPauseDelegate(params OnRoutinePause []pauseDelegates)
	{
		foreach(OnRoutinePause p in pauseDelegates)
			if (p != null)
				pauseEvent += p;
	}
	
	public void AddCompletionDelegates(params OnRoutineComplete[] completionDelegates)
	{
		foreach(OnRoutineComplete c in completionDelegates)
			if (c != null)
				completionEvent += c;
	}
	
	public void StartRoutine()
	{
        if (isRunning == true)
		{
			Debug.Log("Routine already runnning");
			return;
		}

		isRunning = true;
		isPaused = false;
		CoroutineManager.I.StartCoroutine(Run());
	}
	
	public void Pause()
	{
		isPaused = true;
		
		FirePausedEvent();
	}
			
	public void UnPause()
	{
		isPaused = false;
		
		FirePausedEvent();
	}
	
	private void FirePausedEvent()
	{
		if (pauseEvent != null)
			pauseEvent(isPaused, this);
	}
	
	public void Kill()
	{
		routineWasKilled = true;
		isRunning = false;
		isPaused = false;
	}
	
	private IEnumerator Run()
	{
        while(isRunning == true)
		{
           if (isPaused == true)
				yield return null;
		    else
            {
                //run next iteration and stop if we are done
                if (coroutine.MoveNext())
                    yield return coroutine.Current;
                else
                    isRunning = false;
            }
		}

		if (completionEvent != null)
		{
			completionEvent(routineWasKilled, this);
			Dispose();
		}
	}
	
	#endregion
	
	#region IDisposable
	
	public void Dispose()
	{
		coroutine = null;
		pauseEvent = null;
		completionEvent = null;
	}
	
	#endregion
}