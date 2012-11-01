using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Basically a list of the object we are creating. H
/// </summary>
public class Recyclable<T> : IRecyclable where T : RecyclableGameObject
{
	private T suppliedObjectPrefab;
	
	private List<T> recyclableObjects;
	public List<T> RecyclableObjects{ get{ return recyclableObjects; } }
	
	public Recyclable(T objectPrefab, int overrideNumberOfObjects = 0)
	{
		suppliedObjectPrefab = objectPrefab;
		int number = (overrideNumberOfObjects <= 0) ? RecycleBin.I.defaultNumberOfObjects : overrideNumberOfObjects;
		
		recyclableObjects = new List<T>();
		
		for(int x = 0; x < number; x++)
			AddNewObject();
	}
	
	public T AddNewObject()
	{
		var temp = UnityEngine.Object.Instantiate(suppliedObjectPrefab, RecycleBin.I.transform.position, RecycleBin.I.transform.rotation) as T;
		temp.Init();
		temp.MyTransform.parent = RecycleBin.I.transform;
		temp.gameObject.SetActiveRecursively(false);
		recyclableObjects.Add(temp);
		return temp;
	}
}