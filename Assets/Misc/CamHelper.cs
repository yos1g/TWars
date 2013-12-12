using UnityEngine;
using System.Collections;

public class CamHelper : MonoBehaviour {

	void Update () {
		camera.transform.LookAt(this.transform.parent.transform.position);
	}
}
