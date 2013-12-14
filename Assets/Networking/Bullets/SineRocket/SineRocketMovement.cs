using UnityEngine;
using System.Collections;

public class SineRocketMovement : MonoBehaviour {

	// ======================================================== //

	private Vector3 basePosition;

	private float speed = 15.0f;

	private float amplitude = 1.0f;

	private float omega = 15.0f;

	// ======================================================== //

	void Awake () 
	{
		this.networkView.observed = this;
		Invoke("DestroyBullet", 5.0f);
	}

	// ======================================================== //

	void Start()
	{
		basePosition = transform.position;
	}

	// ======================================================== //

	void Update () 
	{
		basePosition += transform.forward * speed * Time.deltaTime;
		transform.position = basePosition + transform.right * amplitude * Mathf.Sin(omega * Time.time);
	}

	// ======================================================== //

	void DestroyBullet()
	{
		DestroyImmediate(this.gameObject);
	}

	// ======================================================== //

	void OnTriggerEnter (Collider other) 
	{
		if (other.transform.gameObject.tag == "Player") 
		{
			GameObject scoreManager = GameObject.FindWithTag("ScoreManager");
			scoreManager.networkView.RPC("RPC_ChangePlayersListWithScore", RPCMode.AllBuffered, networkView.owner, scoreManager.GetComponent<PlayersManager>().getPlayerScore(networkView.owner)+1);
			other.transform.networkView.RPC("RPC_NeedRespawn", other.transform.networkView.owner);
		}

		Network.Destroy(this.gameObject);
	}

	// ======================================================== //
}
