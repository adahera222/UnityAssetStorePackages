using UnityEngine;
using System.Collections;

public class Projectile : RecyclableGameObject
{
	public AudioClip launchSound;
	public int timeoutInMilliseconds = 6000;
	
	protected override void Awake()
	{
		base.Awake();
		
		if (constantForce == null)
			gameObject.AddComponent(typeof(ConstantForce));
		if (rigidbody == null)
			gameObject.AddComponent(typeof(Rigidbody));
		if (collider == null)
			gameObject.AddComponent(typeof(SphereCollider));
	}
	
	public void Launch()
	{
		rigidbody.useGravity = true;
		
		constantForce.enabled = true;
		
		Timer.CreateMillisecondTimer(timeoutInMilliseconds, OnTimeOut);
	}
	
	void OnTimeOut(float elapsedTime)
	{
		Recycle();
	}
	
	public override void Recycle()
	{
		base.Recycle();
		
		constantForce.enabled = false;
		
		rigidbody.useGravity = false;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
	}
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		if (constantForce.force.y != -10000)
			constantForce.force = new Vector3(0, -10000, 0);
	
		constantForce.enabled = false;
		
		rigidbody.useGravity = false;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		constantForce.force = Vector3.zero;
		constantForce.torque = Vector3.zero;
	}
}