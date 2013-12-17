using UnityEngine;

public class PlayerData {

	// ======================================================== //

	public int networkPlayer;

	public string playerName;

	public int playerScore;

	public Color playerColor;

	// ======================================================== //

	public PlayerData Constructor ()
	{
		PlayerData player = new PlayerData();

		player.networkPlayer = networkPlayer;
		player.playerName = playerName;
		player.playerScore = playerScore;
		player.playerColor = playerColor;

		return player;
	}

	// ======================================================== //
}
