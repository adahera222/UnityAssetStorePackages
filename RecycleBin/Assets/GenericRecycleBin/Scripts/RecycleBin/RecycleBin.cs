using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RecycleBin : TSingleton<RecycleBin>
{
	#region Variables
	
	private List<IRecyclable> recyclables;
	public int defaultNumberOfObjects = 10;
	
	#endregion
	
	#region Init
	
	protected override void Awake()
	{
		base.Awake();
		
		I.recyclables = new List<IRecyclable>();
		transform.position = new Vector3(5000, 5000, 5000); //hide
	}
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Creates a new Recyclable of type T. Can be passed in a prefab, if passed in nothing, it will create a new gameobject with component of type T. T must inherit from RecyclableGameObject
	/// </summary>
	/// <param name='objectPrefab'>
	/// Object prefab.
	/// </param>
	/// <typeparam name='T'>
	/// The 1st type parameter.
	/// </typeparam>
	public static void CreateNewRecylable<T>(T objectPrefab = null, int overrideNumberOfObjects = 0) where T : RecyclableGameObject
	{
		IRecyclable r = null;
		
		if (objectPrefab == null)
			r = new Recyclable<T>((T)new GameObject(typeof(T).ToString()).AddComponent(typeof(T)), overrideNumberOfObjects) as IRecyclable; //create new gameobject with T
		else
			r = new Recyclable<T>(objectPrefab, overrideNumberOfObjects); //create one based off the prefab
		
		I.recyclables.Add(r);
	}
	
	/// <summary>
	/// Returns a free object of type T. Make sure you have called CreateNewRecyclable before calling this or it will return null. If there are no free objects it will create a new one
	/// </summary>
	/// <returns>
	/// The free object.
	/// </returns>
	/// <typeparam name='T'>
	/// The 1st type parameter.
	/// </typeparam>
	public static T GetFreeObject<T>() where T : RecyclableGameObject
	{
		IRecyclable foundRecyclable = I.recyclables.Find(item => (item as Recyclable<T> != null)); //find the correct recylable
		
		if (foundRecyclable == null)
		{
			Debug.Log("Recyclable " + typeof(T) + " does not exist. Create it first!");
			return default(T);
		}
		
		T foundRecylableGameObject = ((Recyclable<T>)foundRecyclable).RecyclableObjects.Find(item => item.gameObject.active == false);
		
		if (foundRecylableGameObject != null) //we found a free one
		{
			foundRecylableGameObject.gameObject.SetActiveRecursively(true);
			return foundRecylableGameObject;
		}
		else //we didnt find a free one so make a new one!
		{
			var createdObject =  ((Recyclable<T>)foundRecyclable).AddNewObject();
			createdObject.gameObject.SetActiveRecursively(true);
			return createdObject;
		}
	}
	
	#endregion
}