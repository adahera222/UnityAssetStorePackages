using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class Launcher : MonoBehaviour
{
	#region Variables
	
	public Transform projectileSpawn;
	
	public Projectile projetilePrefab;

    private Projectile currentProjectile;
	public Projectile CurrentProjectile
	{
		get{ return currentProjectile; }
		set{ currentProjectile = value; }
	}
		
	#region ThrowStuff

   	public float respawnIntervalMilliseconds = 200;
	public float clampAngle = 45;
    private Vector2 touchStartPosition;
    private float startTouchTime;
    public float minSwipeTime = 0.13f;
    public float maxSwipeTime = 0.8f;
    private float minSwipeDistance;
    private float maxSwipeDistance;
    private float maxPossibleSpeed;
    public int maxVerticalLaunchForce = 35000;
    public Vector2 throwVelocity = new Vector2(45000, 21);
    private float throwPower;
    public float swipeWidthAsPercent = 0.3f;
    private float swipeMinX = 6f;
    private float swipeMaxX = 12f;
	
	#endregion
	
	#endregion
	
	#region Init
	
	void Awake()
	{
		Application.targetFrameRate = 60;
		Physics.gravity = new Vector3(0, -50, 0);
	}

    void Start()
    {
        swipeMinX = (Screen.width / 2) - (Screen.width * swipeWidthAsPercent / 2);
        swipeMaxX = (Screen.width / 2) + (Screen.width * swipeWidthAsPercent / 2);
		
        minSwipeDistance = Screen.height / 6;
        maxSwipeDistance = Screen.height / 1.3f;
        maxPossibleSpeed = maxSwipeDistance / minSwipeTime;
		
		//this is where we create the pool of projectiles
		RecycleBin.CreateNewRecylable<Projectile>(projetilePrefab, 5);
		GetNewProjectile(0.0f);
    }
	
	void GetNewProjectile(float elapsedTime)
	{
		//this is how to get a new object from the pool. If they are all in use, a new one will be created from the supplied prefab
		currentProjectile = RecycleBin.GetFreeObject<Projectile>();
		
		if (currentProjectile != null)
		{
			currentProjectile.MyTransform.parent = projectileSpawn;
			currentProjectile.MyTransform.localPosition = Vector3.zero;
		}
	}
	
	#endregion
	
	#region Methods
	
	void OnGUI()
	{
		var center = GUI.skin.GetStyle("Label");
		center.alignment = TextAnchor.UpperCenter;
		GUI.Label(new Rect(0, Screen.height / 2, Screen.width, 20), "Swipe with mouse or touch to throw!", center);
	}
	
	void Update()
	{
		DoTouch();
		DoMouse();
	}
	
	void DoTouch()
	{
		if (Input.touches.Length <= 0)
			return;
		
		switch(Input.GetTouch(0).phase)
		{
			case TouchPhase.Began:
			
				DragStart(Input.GetTouch(0).position);
			
			break;
			
			case TouchPhase.Ended:
			
				DragEnd(Input.GetTouch(0).position);
			
			break;
		}	
	}
	
	void DoMouse()
	{
	 	if( Input.GetMouseButton(0))
            if( Input.GetMouseButtonDown(0))
				DragStart(Input.mousePosition); //start

        if( Input.GetMouseButtonUp(0))
			DragEnd(Input.mousePosition); //end
	}
	
	void DragStart(Vector2 pos)
	{
		touchStartPosition = pos;
    	startTouchTime = Time.time;
	}
	
	void DragEnd(Vector2 pos)
	{
		Vector2 touchEndPosition = pos;

        float currentSwipeTime = Time.time - startTouchTime;
        float clampedSwipeTime = Mathf.Clamp(currentSwipeTime, minSwipeTime, maxSwipeTime);
		
        if (touchStartPosition.x > swipeMinX &&
			touchStartPosition.x < swipeMaxX &&
            touchStartPosition.y < minSwipeDistance &&
			touchStartPosition.y < touchEndPosition.y &&
			currentSwipeTime < maxSwipeTime)
        {
            float currentThrowAngle = ThrowAngle(touchStartPosition, touchEndPosition);

            float currentSwipeDistance = Vector2.Distance(touchStartPosition, touchEndPosition);

            float overallSwipe = currentSwipeDistance / clampedSwipeTime;

            if (overallSwipe > maxPossibleSpeed)
                overallSwipe = maxPossibleSpeed;
			
			if (currentSwipeDistance > minSwipeDistance)
                Launch(currentThrowAngle, overallSwipe, throwVelocity);
        }
	}

    void Launch(float angle, float swipePower, Vector2 swipeVelocity)
    {
        if (currentProjectile != null)
        {
			currentProjectile.MyTransform.position = projectileSpawn.position;
			currentProjectile.constantForce.enabled = false;
			
			projectileSpawn.localRotation = Quaternion.Euler(new Vector3(0, angle, 0));

            float forceY = Mathf.Clamp((swipeVelocity.y * swipePower * 0.6f), 0, maxVerticalLaunchForce);

            currentProjectile.Launch();
			
			//adds forward force
			currentProjectile.rigidbody.AddForce(projectileSpawn.forward.x * swipeVelocity.x, projectileSpawn.forward.y + forceY, projectileSpawn.forward.z * swipeVelocity.x, ForceMode.Impulse);
			
			currentProjectile.constantForce.torque = new Vector3(0, 0, angle * 300); //makes it look like its rotating
			Timer.CreateMillisecondTimer<Projectile, float>(250, ResetProjectileForceAfterTime, currentProjectile, angle);	
			
			currentProjectile.MyTransform.parent = null;
          	currentProjectile = null;
			
			Timer.CreateMillisecondTimer(respawnIntervalMilliseconds, GetNewProjectile);
        }
    }
	
	/// <summary>
	/// Resets the "curve" we put on the projectile
	/// </summary>
	/// <param name='elapsedTime'>
	/// Elapsed time.
	/// </param>
	/// <param name='affectedProjectile'>
	/// Affected projectile.
	/// </param>
	/// <param name='angle'>
	/// Angle.
	/// </param>
	void ResetProjectileForceAfterTime(float elapsedTime, Projectile affectedProjectile, float angle)
	{
		if (affectedProjectile.gameObject.active == true)
			affectedProjectile.constantForce.force = new Vector3(angle * -600, affectedProjectile.constantForce.force.y, 0); //angle * -val will make it curve the oposite way!
	}

    float ThrowAngle(Vector2 pointOne, Vector2 pointTwo)
    {
       float x1 =  (pointOne.x < pointTwo.x) ? Mathf.Abs(pointOne.x - pointTwo.x) :  Mathf.Abs(pointTwo.x - pointOne.x);

        float y1 = Mathf.Abs(pointOne.y - pointTwo.y);
        float angle = Mathf.Atan(x1 / y1);
        angle *= Mathf.Rad2Deg;

        if (pointOne.x > pointTwo.x)
            angle *= -1;

        return Mathf.Clamp(angle, -clampAngle, clampAngle);
    }
	
	#endregion
}