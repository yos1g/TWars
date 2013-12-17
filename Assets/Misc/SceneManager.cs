using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

	public GameObject explosionPrefab;

	public GameObject enemyExplosionPrefab;

	public GameObject GetPrefab(int prefab)
	{
		switch(prefab) {
			case 1:
				return explosionPrefab;
			case 2:
				return enemyExplosionPrefab;
			default:
				return explosionPrefab;
		}
	}
}
