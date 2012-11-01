using UnityEngine;
using System.Collections;

/// <summary>
/// Inherit from this class to use your object as a recyclable game object
/// </summary>
public abstract class RecyclableGameObject : MonoBehaviour
{
	private Transform myTransform;
	public Transform MyTransform{ get{ return myTransform; } }
	
	public virtual void Init()
	{
		myTransform = transform;	
	}
	
	protected virtual void Awake()
	{
		Init();
	}
	
	protected virtual void Start()
	{
		
	}
	
	protected virtual void OnEnable()
	{
		
	}
	
	public virtual void Recycle()
	{
		myTransform.parent = RecycleBin.I.transform;
		myTransform.localPosition = Vector3.zero;
		myTransform.localRotation = RecycleBin.I.transform.rotation;
		
		gameObject.SetActiveRecursively(false);
	}
}