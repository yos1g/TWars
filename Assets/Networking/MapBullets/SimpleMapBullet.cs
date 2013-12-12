using UnityEngine;
using System.Collections;

public class SimpleMapBullet : MonoBehaviour {

	public GameObject bullet;

	void Awake () {
		this.networkView.observed = this;
	}
	
	void OnTriggerEnter (Collider other) 
	{
		if (other.transform.gameObject.tag != "Player") 
			return;

		Client client = other.transform.gameObject.GetComponent<Client>();
		client.bullet = this.bullet;
		client.currentScore = client.getScore() + 1; 
		Destroy(this.gameObject);
	}
}
