using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Timer
{
	#region Variables
	
	public delegate void OnTimerComplete (float elapsedTime);
	public delegate void OnTimerComplete<T> (float elapsedTime, T itme);
	public delegate void OnTimerComplete<T, U> (float elapsedTime, T itemOne, U itemTwo);
	public delegate void OnSecondTimerUpdate (float elapsedTime, float timeLeft, float totalTimeInseconds);
	public delegate void OnSecondTimerUpdate<T> (float elapsedTime, float timeLeft, float totalTimeInseconds, T item);
	public delegate void OnSecondTimerUpdate<T, U> (float elapsedTime, float timeLeft, float totalTimeInseconds, T itemOne, U itemTwo);
	
	#endregion
	
	#region Constructors
	
	public static Routine CreateSecondIntervalTimer (float timeInSeconds, Timer.OnSecondTimerUpdate updateDelegate, Timer.OnTimerComplete completionDelegate)
	{
		return CoroutineManager.CreateRoutine(Timer.SecondIntervalTimer(timeInSeconds, updateDelegate, completionDelegate));
	}
	
	public static Routine CreateSecondIntervalTimer<T> (float timeInSeconds, Timer.OnSecondTimerUpdate<T> updateDelegate, Timer.OnTimerComplete<T> completionDelegate, T item)
	{
		return CoroutineManager.CreateRoutine(Timer.SecondIntervalTimer<T>(timeInSeconds, updateDelegate, completionDelegate, item));
	}
	
	public static Routine CreateSecondIntervalTimer<T, U> (float timeInSeconds, Timer.OnSecondTimerUpdate<T, U> updateDelegate, Timer.OnTimerComplete<T, U> completionDelegate, T itemOne, U itemTwo)
	{
		return CoroutineManager.CreateRoutine(Timer.SecondIntervalTimer<T, U>(timeInSeconds, updateDelegate, completionDelegate, itemOne, itemTwo));
	}
	
	public static Routine CreateMillisecondTimer (float timeInMilliseconds, Timer.OnTimerComplete completionDelegate)
	{
		return CoroutineManager.CreateRoutine(Timer.MillisecondTimer(timeInMilliseconds, completionDelegate));
	}
	
	public static Routine CreateMillisecondTimer<T> (float timeInMilliseconds, Timer.OnTimerComplete<T> completionDelegate, T item)
	{
		return CoroutineManager.CreateRoutine(Timer.MillisecondTimer<T>(timeInMilliseconds, completionDelegate, item));
	}
	
	public static Routine CreateMillisecondTimer<T, U> (float timeInMilliseconds, Timer.OnTimerComplete<T, U> completionDelegate, T itemOne, U itemTwo)
	{
		return CoroutineManager.CreateRoutine(Timer.MillisecondTimer<T, U>(timeInMilliseconds, completionDelegate, itemOne, itemTwo));
	}
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Creates a timer which will call back every second until its complete. Perfect for a countdown clock
	/// </summary>
	/// <returns>
	/// The interval timer.
	/// </returns>
	/// <param name='timeInSeconds'>
	/// Time in seconds.
	/// </param>
	/// <param name='updateDelegate'>
	/// Update delegate.
	/// </param>
	/// <param name='completionDelegate'>
	/// Completion delegate.
	/// </param>
	private static IEnumerator SecondIntervalTimer(float timeInSeconds, OnSecondTimerUpdate updateDelegate, OnTimerComplete completionDelegate)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + timeInSeconds;
		
		float lastUpdateTime = startTime;
		
		while (Time.realtimeSinceStartup < endTime)
		{
			if (Time.realtimeSinceStartup >= lastUpdateTime)
			{
				lastUpdateTime += 1.0f;
				
				if (updateDelegate != null)
					updateDelegate(Time.realtimeSinceStartup - startTime, endTime - Time.realtimeSinceStartup, timeInSeconds);
			}
			
			yield return null;
		}
		
		if (completionDelegate != null)
			completionDelegate(endTime - startTime);
	}
	
	/// <summary>
	/// Creates a timer which will call back every second until its complete. Perfect for a countdown clock. Can pass one argument
	/// </summary>
	/// <returns>
	/// The interval timer.
	/// </returns>
	/// <param name='timeInSeconds'>
	/// Time in seconds.
	/// </param>
	/// <param name='updateDelegate'>
	/// Update delegate.
	/// </param>
	/// <param name='completionDelegate'>
	/// Completion delegate.
	/// </param>
	/// <param name='item'>
	/// Item.
	/// </param>
	/// <typeparam name='T'>
	/// The 1st type parameter.
	/// </typeparam>
	private static IEnumerator SecondIntervalTimer<T>(float timeInSeconds, OnSecondTimerUpdate<T> updateDelegate, OnTimerComplete<T> completionDelegate, T item)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + timeInSeconds;
		
		float lastUpdateTime = startTime;
		
		while (Time.realtimeSinceStartup < endTime)
		{
			if (Time.realtimeSinceStartup >= lastUpdateTime)
			{
				lastUpdateTime += 1.0f;
				
				if (updateDelegate != null)
					updateDelegate(Time.realtimeSinceStartup - startTime, endTime - Time.realtimeSinceStartup, timeInSeconds, item);
			}
			
			yield return null;
		}
		
		if (completionDelegate != null)
			completionDelegate(endTime - startTime, item);
	}
	
	/// <summary>
	/// Creates a timer which will call back every second until its complete. Perfect for a countdown clock. Can pass two arguments
	/// </summary>
	/// <returns>
	/// The interval timer.
	/// </returns>
	/// <param name='timeInSeconds'>
	/// Time in seconds.
	/// </param>
	/// <param name='updateDelegate'>
	/// Update delegate.
	/// </param>
	/// <param name='completionDelegate'>
	/// Completion delegate.
	/// </param>
	/// <param name='itemOne'>
	/// Item one.
	/// </param>
	/// <param name='itemTwo'>
	/// Item two.
	/// </param>
	/// <typeparam name='T'>
	/// The 1st type parameter.
	/// </typeparam>
	/// <typeparam name='U'>
	/// The 2nd type parameter.
	/// </typeparam>
	private static IEnumerator SecondIntervalTimer<T, U>(float timeInSeconds, OnSecondTimerUpdate<T, U> updateDelegate, OnTimerComplete<T, U> completionDelegate, T itemOne, U itemTwo)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + timeInSeconds;
		
		float lastUpdateTime = startTime;
		
		while (Time.realtimeSinceStartup < endTime)
		{
			if (Time.realtimeSinceStartup >= lastUpdateTime)
			{
				lastUpdateTime += 1.0f;
				
				if (updateDelegate != null)
					updateDelegate(Time.realtimeSinceStartup - startTime, endTime - Time.realtimeSinceStartup, timeInSeconds, itemOne, itemTwo);
			}
			
			yield return null;
		}
		
		if (completionDelegate != null)
			completionDelegate(endTime - startTime, itemOne, itemTwo);
	}
	
	/// <summary>
	/// Creates a millisecond timer
	/// </summary>
	/// <returns>
	/// The timer.
	/// </returns>
	/// <param name='timeInMilliseconds'>
	/// Time in milliseconds.
	/// </param>
	/// <param name='completionDelegate'>
	/// Completion delegate.
	/// </param>
	private static IEnumerator MillisecondTimer(float timeInMilliseconds, OnTimerComplete completionDelegate)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + ConvertMillisecondsToSeconds(timeInMilliseconds);
		
		while (Time.realtimeSinceStartup < endTime)
			yield return null;
		
		if (completionDelegate != null)
			completionDelegate(endTime - startTime);
	}
	
	/// <summary>
	/// Creates a millisecond timer. Can pass one argument
	/// </summary>
	/// <returns>
	/// The timer.
	/// </returns>
	/// <param name='timeInMilliseconds'>
	/// Time in milliseconds.
	/// </param>
	/// <param name='completionDelegate'>
	/// Completion delegate.
	/// </param>
	/// <param name='item'>
	/// Item.
	/// </param>
	/// <typeparam name='T'>
	/// The 1st type parameter.
	/// </typeparam>
	private static IEnumerator MillisecondTimer<T>(float timeInMilliseconds, OnTimerComplete<T> completionDelegate, T item)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + ConvertMillisecondsToSeconds(timeInMilliseconds);
		
		while (Time.realtimeSinceStartup < endTime)
			yield return null;
		
		if (completionDelegate != null)
			completionDelegate(endTime - startTime, item);
	}
	
	/// <summary>
	/// Creates a millisecond timer. Can pass one argument
	/// </summary>
	/// <returns>
	/// The timer.
	/// </returns>
	/// <param name='timeInMilliseconds'>
	/// Time in milliseconds.
	/// </param>
	/// <param name='completionDelegate'>
	/// Completion delegate.
	/// </param>
	/// <param name='itemOne'>
	/// Item one.
	/// </param>
	/// <param name='itemTwo'>
	/// Item two.
	/// </param>
	/// <typeparam name='T'>
	/// The 1st type parameter.
	/// </typeparam>
	/// <typeparam name='U'>
	/// The 2nd type parameter.
	/// </typeparam>
	private static IEnumerator MillisecondTimer<T, U>(float timeInMilliseconds, OnTimerComplete<T, U> completionDelegate, T itemOne, U itemTwo)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + ConvertMillisecondsToSeconds(timeInMilliseconds);
		
		while (Time.realtimeSinceStartup < endTime)
			yield return null;
		
		if (completionDelegate != null)
			completionDelegate(endTime - startTime, itemOne, itemTwo);
	}
	
	#endregion
	
	#region Helper
	
	public static float ConvertSecondsToMilliseconds (float seconds)
	{
		return seconds * 1000f;
	}
	
	public static double ConvertSecondsToMilliseconds (double seconds)
	{
		return seconds * 1000.0;
	}
	
	public static double ConvertMillisecondsToSeconds (double milliseconds)
	{
		return milliseconds / 1000.0;
	}
	
	public static float ConvertMillisecondsToSeconds (float milliseconds)
	{
		return milliseconds / 1000f;
	}
	
	#endregion
}