using UnityEngine;
using System.Collections;

public class SimpleRocketMovement : MonoBehaviour {

	private NetworkPlayer owner;

	void Awake () {
		this.networkView.observed = this;
		Invoke("DestroyBullet", 4.0f);
	}

	void Update () {
		transform.position = transform.position + transform.forward * 10.0f * Time.deltaTime;
	}

	void DestroyBullet()
	{
		DestroyImmediate(this.gameObject);
	}

}
