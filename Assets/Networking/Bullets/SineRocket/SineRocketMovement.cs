﻿using UnityEngine;
using System.Collections;

public class SineRocketMovement : MonoBehaviour {

	// ======================================================== //

	private Vector3 basePosition;

	public float speed = 15.0f;

	public float amplitude = 1.0f;

	public float omega = 15.0f;

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
		if (other.transform.GetComponent<NetworkView>() == null) {
			Network.Destroy(this.gameObject);
			return;
		}

		if (other.transform.gameObject.tag == "Player" && other.transform.networkView.owner != this.networkView.owner) 
		{
			GameObject scoreManager = GameObject.FindWithTag("ScoreManager");
			scoreManager.networkView.RPC("RPC_ChangePlayersListWithScore", RPCMode.AllBuffered, networkView.owner, scoreManager.GetComponent<PlayersManager>().getPlayerScore(networkView.owner)+1);
			other.transform.networkView.RPC("RPC_NeedRespawn", RPCMode.AllBuffered, other.transform.networkView.owner);
		} 
		else if (other.transform.networkView.owner == this.networkView.owner)
		{
			return;
		}

		Network.Destroy(this.gameObject);
	}

	// ======================================================== //

	void OnCollisionEnter(Collision other)
	{
		Debug.Log(other.transform.gameObject.name);
		Network.Destroy(this.gameObject);
	}

	// ======================================================== //
}
