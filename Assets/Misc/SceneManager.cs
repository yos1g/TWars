using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

	public GameObject explosionPrefab;

	public GameObject enemyExplosionPrefab;

	/// <summary>
	/// Gets the prefab.
	/// </summary>
	/// <returns>The prefab.</returns>
	/// <param name="type">Type.</param>
	public GameObject GetPrefab(DestroyType type)
	{
		switch(type) {
			case DestroyType.OnlyMe:
				return explosionPrefab;
			case DestroyType.MeAndEnemy:
				return enemyExplosionPrefab;
			default:
				return explosionPrefab;
		}
	}
}
