using UnityEngine;
using System.Collections;

public class SineRocketMovement : MonoBehaviour {

	private Vector3 basePosition;

	private float speed = 15.0f;
	private float amplitude = 1.0f;
	private float omega = 15.0f;

	void Awake () 
	{
		this.networkView.observed = this;
		Invoke("DestroyBullet", 5.0f);
	}

	void Start()
	{
		basePosition = transform.position;
	}
	
	void Update () 
	{
		basePosition += transform.forward * speed * Time.deltaTime;
		transform.position = basePosition + transform.right * amplitude * Mathf.Sin(omega * Time.time);
	}
	
	void DestroyBullet()
	{
		DestroyImmediate(this.gameObject);
	}
}
