using UnityEngine;
using System.Collections;


public class Client : MonoBehaviour {

	// ======================================================== //

	public Camera cam;				// ref camera

	private Vector3 moveDirection;	// 
	
	private float speed = 6.0F;		// speed input

	public float speedStep = 6.0f;	// speed walk

	public float speedShift = 9.0f;	// speed run

	public float gravity = 20.0F;	// 

	private float speedRotate = 120.0f;	// 

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

	public bool needRespawn = false;

	private bool isFired = false;

	// ======================================================== //

	void Start()
	{
		if (networkView.isMine)
		{
			GameObject scoreManager = GameObject.FindWithTag("ScoreManager");
			PlayersManager nManager = scoreManager.gameObject.GetComponent<PlayersManager>();

			for(int i = 0; i < nManager.players.Count; i++)
			{
				if (nManager.players[i].networkPlayer == int.Parse(Network.player.ToString()))
				{
					currentScore = nManager.players[i].playerScore;
					UpdateScore(currentScore);
				}
			}

		} else {
			//enabled = false;
		}
	}

	// ======================================================== //

	void Awake () 
	{
		cam = transform.GetComponentInChildren<Camera>().camera;
		controller = GetComponent<CharacterController>();
		this.networkView.observed = this;
	}

	// ======================================================== //

	void Update () 
	{
		if(networkView.isMine) 
		{
			if (needRespawn)
			{
				GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");
				transform.position = spawns[UnityEngine.Random.Range(0, spawns.Length)].transform.position;
				needRespawn = false;
			}

			if (controller.isGrounded) 
			{
				moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= speed;
				
				if (Input.GetKey(KeyCode.LeftShift))
					speed = speedShift;
				else speed = speedStep;

				if (Input.GetButtonDown("Jump") && bullet != null && isFired == false) {
					isFired = true;
					GameObject _refBullet = (GameObject)Network.Instantiate(bullet, transform.position + transform.forward + Vector3.up * 0.25f, transform.rotation, networkView.group);
					Invoke("Recharge", _refBullet.GetComponent<SineRocketMovement>().rechargeTime);
				}

			}
			
			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move(moveDirection * Time.deltaTime);
			transform.Rotate(Vector3.down * (speedRotate + speed * 2) * Input.GetAxis("Horizontal") * -1 *  Time.deltaTime, Space.World);
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

	// ======================================================== //

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

	// ======================================================== //

	private void Recharge()
	{
		isFired = false;
	}

	// ======================================================== //

	// Interpolation
	private void SyncedMovement() 
	{
		syncTime += Time.deltaTime;
		transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
		transform.rotation = Quaternion.Slerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
	}

	// ======================================================== //
	/// <summary>
	/// Updates the score.
	/// </summary>
	/// <param name="score">Score.</param>
	void UpdateScore(int score)
	{
		GameObject scoreManager = GameObject.FindWithTag("ScoreManager");
		PlayersManager nManager = scoreManager.GetComponent<PlayersManager>();
		nManager.scored = true;
		nManager.playerScore = score;
	}

	// ============================ RPC ============================ //

	[RPC] void RPC_Scored()
	{
		GameObject scoreManager = GameObject.FindWithTag("ScoreManager");
		PlayersManager nManager = scoreManager.GetComponent<PlayersManager>();
		
		for(int i = 0; i < nManager.players.Count; i++)
		{
			if (nManager.players[i].networkPlayer == int.Parse(Network.player.ToString()))
			{
				currentScore = nManager.players[i].playerScore + 1;
				UpdateScore(currentScore);
			}
		}
	}

	// ============================ RPC ============================ //

	[RPC] void RPC_NeedRespawn(NetworkPlayer pl)
	{
		if (pl != Network.player)
			return;

		needRespawn = true;
	}

	// ======================================================== //
}
