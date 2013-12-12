using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour {
	public Camera cam;				// ref camera
	private Vector3 moveDirection;	// 
	
	private float speed = 6.0F;		// speed input
	public float speedStep = 6.0f;	// speed walk
	public float speedShift = 9.0f;	// speed run
	public float gravity = 20.0F;	// 
	public float speedRotate = 4;	// 
	
	private CharacterController controller;	// ref controller
	
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;

	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;

	private Quaternion syncStartRotation = Quaternion.identity;
	private Quaternion syncEndRotation = Quaternion.identity;

	public GameObject bullet;

	public int currentScore;

	public int getScore()
	{
		return currentScore;
	}

	void Awake () 
	{
		cam = transform.GetComponentInChildren<Camera>().camera;
		controller = GetComponent<CharacterController>();
		this.networkView.observed = this;
	}


	void Update () 
	{
		if(networkView.isMine) 
		{
			if (controller.isGrounded) 
			{
				moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= speed;
				
				if (Input.GetKey(KeyCode.LeftShift))
					speed = speedShift;
				else speed = speedStep;

				if (Input.GetButtonDown("Jump") && bullet != null)
					networkView.RPC ("RPC_Fire", RPCMode.All, transform.position + transform.forward, transform.rotation);
			}
			
			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move(moveDirection * Time.deltaTime);
			transform.Rotate(Vector3.down * speedRotate * Input.GetAxis("Horizontal") * -1, Space.World);
		}
		else 
		{
			if(cam.enabled)
			{ 
				cam.enabled = false; 
				cam.gameObject.GetComponent<AudioListener>().enabled = false;
			}

			SyncedMovement();
		}
	}


	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Quaternion syncRotation = Quaternion.identity;
		int score = 0;

		if (stream.isWriting)
		{
			syncPosition = transform.position;
			syncRotation = transform.rotation;
			score = currentScore;

			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref score);
		} 
		else 
		{
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncRotation);
			stream.Serialize(ref score);


			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			syncStartPosition = transform.position;
			syncStartRotation = transform.rotation;
			syncEndPosition = syncPosition;
			syncEndRotation = syncRotation;
			currentScore = score;
		}

	}


	// Interpolation
	private void SyncedMovement() 
	{
		syncTime += Time.deltaTime;
		transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
		transform.rotation = Quaternion.Slerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
	}

	[RPC]
	void RPC_Fire(Vector3 startPosition, Quaternion velocity)
	{
		Instantiate(bullet, startPosition, velocity);
	}
	
}
