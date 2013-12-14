using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayersManager : MonoBehaviour {

	// ======================================================== //

	public List<PlayerData> players = new List<PlayerData>();
	public NetworkPlayer networkPlayer;

	public bool named = false;
	public string playerName;

	public bool scored = false;
	public int playerScore;

	// ======================================================== //

	// ======================================================== //

	void Awake()
	{
		this.networkView.observed = this;
	}

	// ======================================================== //

	void Update ()
	{
	
		if (named)
		{
			networkView.RPC ("RPC_ChangePlayersWithName", RPCMode.AllBuffered, Network.player, playerName);
			named = false;
		}

		if (scored)
		{
			networkView.RPC("RPC_ChangePlayersListWithScore", RPCMode.AllBuffered, Network.player, playerScore);
			scored = false;
		}

	}

	// ======================================================== //

	public int getPlayerScore(NetworkPlayer pl)
	{
		for(int i = 0; i < players.Count; i++)
		{
			if (players[i].networkPlayer == int.Parse(pl.ToString()))
			{
				return players[i].playerScore;
			}
		}
		return 0;
	}

	// ======================================================== //

	void OnPlayerConnected(NetworkPlayer pl)
	{
		networkView.RPC("RPC_AddPlayer", RPCMode.AllBuffered, pl);
	}

	// ======================================================== //

	void OnPlayerDisconnected(NetworkPlayer pl)
	{
		networkView.RPC("RPC_RemovePlayer", RPCMode.AllBuffered, pl);
	}

	// ============================ RPC ============================ //

	[RPC] void RPC_AddPlayer(NetworkPlayer pl)
	{
		Debug.Log("Player added to manager");
		PlayerData player = new PlayerData();
		player.networkPlayer = int.Parse(pl.ToString());
		players.Add(player);
	}

	// ============================ RPC ============================ //

	[RPC] void RPC_RemovePlayer(NetworkPlayer pl)
	{
		for(int i = 0; i < players.Count; i++)
		{
			if (players[i].networkPlayer == int.Parse(pl.ToString()))
			{
				Debug.Log("Player removed from manager");
				players.RemoveAt(i);
			}
		}
	}

	// ============================ RPC ============================ //

	[RPC] void RPC_ChangePlayersWithName(NetworkPlayer pl, string pName)
	{
		for(int i = 0; i < players.Count; i++)
		{
			if (players[i].networkPlayer == int.Parse(pl.ToString()))
			{
				players[i].playerName = pName;
			}
		}
	}

	// ============================ RPC ============================ //

	[RPC] void RPC_ChangePlayersListWithScore(NetworkPlayer pl, int pScore)
	{
		for(int i = 0; i < players.Count; i++)
		{
			if (players[i].networkPlayer == int.Parse(pl.ToString()))
			{
				players[i].playerScore = pScore;
			}
		}
	}

	// ============================ RPC ============================ //
}
