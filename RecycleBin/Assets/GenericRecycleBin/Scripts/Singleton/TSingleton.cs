using UnityEngine;
using System.Collections;

public abstract class TSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T myInstance = null;
	public static bool isQuitting = false;
 
    public static T I
	{
        get
		{
			if (myInstance != null)
			{
				if (isQuitting == true)
					return default(T);
				else
					return myInstance;
			}
			else if (myInstance == null)
			{
                myInstance = FindObjectOfType(typeof(T)) as T;
 
	            if (myInstance == null)
				{
	                GameObject obj = new GameObject();
	                myInstance = obj.AddComponent(typeof(T)) as T;
	            }
			}
 
            return myInstance;
        }
    }
	
	protected virtual void Awake()
	{
		DontDestroyOnLoad(gameObject);
		RenameToType();
	}
	
	protected virtual void Reset()
	{
		RenameToType();
	}
	
	void RenameToType()
	{
		gameObject.name = this.GetType().ToString();
	}
 
    void OnApplicationQuit()
	{
		isQuitting = true;
        myInstance = null;
    }
}