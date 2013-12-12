using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

	public GameObject PlayerPrefab;
	public GameObject scoreManager;

	public string ip = "127.0.0.1";
	public string port = "27010";
	public bool connected = false;

	public GameObject spawn;

	private GameObject _ref;

	private bool hideMenu = false;
	private bool showLists = false;

	public GUISkin menuSkin;

	private const string typeName = "WWWAAARRRSSS";

	// game name for register on master server
	public string gameName = "RoomName";

	private bool isRefreshingHostList = false;
	private HostData[] hostList;

	private bool isServer = false;

	private bool isScore = false;

	// Use this for initialization
	void Start () 
	{
		this.networkView.observed = this;
	}

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape)) 
		{
			hideMenu = !hideMenu;
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			isScore = !isScore;
		} 

		if (isRefreshingHostList && MasterServer.PollHostList().Length > 0)
		{
			isRefreshingHostList = false;
			hostList = MasterServer.PollHostList();
		}
	}


	void OnGUI()
	{
		if (connected) 
		{

			ConnectedMenu();
		}
		else if (!showLists)
		{
			StartMenu();
		} 
		else 
		{
			HostsList();
		}

		if (isScore && connected)
		{
			ScoreTab();
		}

	}


	void ConnectedMenu() 
	{
		if (!hideMenu)
		{
			return;
		}

		GUI.Label(new Rect((Screen.width - 120)/2, Screen.height/2 - 35, 120, 30), "Connected: " + Network.connections.Length);

		if(GUI.Button(new Rect((Screen.width - 100)/2, Screen.height/2, 100, 30), "Disconnect")) 
		{
			isServer = false;
			Network.Disconnect(200);
		}

		if(GUI.Button(new Rect((Screen.width - 100)/2, Screen.height/2 + 35, 100, 30), "Exit"))
			Application.Quit();

	}


	void StartMenu()
	{
		GUILayout.BeginHorizontal();
		GUI.Label(new Rect((Screen.width - 100)/2, Screen.height/2-60, 100, 20), "Ip");
		GUI.Label(new Rect((Screen.width - 100)/2, Screen.height/2-30, 100, 20), "Port");
		ip = GUI.TextField(new Rect((Screen.width - 100)/2+35, Screen.height/2-60, 100, 20), ip);
		port = GUI.TextField(new Rect((Screen.width - 100)/2+35, Screen.height/2-30, 50, 20), port);
		
		if(GUI.Button(new Rect((Screen.width - 110)/2, Screen.height/2, 110, 30), "Connect")) 
		{
			isServer = false;
			Network.Connect(ip, Convert.ToInt32(port));
		}

		if(GUI.Button(new Rect((Screen.width - 110)/2, Screen.height/2 + 35, 110, 30), "Create Game")) 
		{
			Network.InitializeServer(10, Convert.ToInt32(port), false);
			isServer = true;
			MasterServer.RegisterHost(typeName, gameName);
		}

		if(GUI.Button(new Rect((Screen.width - 110)/2, Screen.height/2 + 70, 110, 30), "Find"))
			showLists = true;

		if(GUI.Button(new Rect((Screen.width - 110)/2, Screen.height/2 + 105, 110, 30), "Exit"))
			Application.Quit();

		GUILayout.EndHorizontal();
	}


	void ScoreTab()
	{

		GUILayout.BeginHorizontal();

		int i = 0;
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			i++;
			Client client = player.GetComponent<Client>();

			if (client == null)
				continue;

			GUI.Label(new Rect((Screen.width - 100)/2, Screen.height/2 - 200 + 30 * i, 100, 20), player.name + " " + client.currentScore );
		}

		GUILayout.EndHorizontal();
	}


	void HostsList() 
	{
		GUILayout.BeginHorizontal();
		
		if (hostList != null)
		{
			for (int i = 0; i < hostList.Length; i++)
			{
				if (GUI.Button(new Rect((Screen.width - 110)/2, Screen.height/2 + 35 * (i + 1), 110, 30), hostList[i].gameName))
					JoinServer(hostList[i]);
			}
		}

		if (GUI.Button(new Rect((Screen.width - 110)/2, Screen.height/2, 110, 30), "Refresh Hosts"))
			RefreshHostList();

		if(GUI.Button(new Rect((Screen.width - 110)/2, Screen.height/2 - 35, 110, 30), "Back"))
			showLists = false;

		GUILayout.EndHorizontal();
	}


	private void RefreshHostList()
	{
		if (!isRefreshingHostList)
		{
			isRefreshingHostList = true;
			MasterServer.RequestHostList(typeName);
		}
	}


	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}


	void OnConnectedToServer () 
	{
		CreatePlayer();
	}


	void OnServerInitialized () 
	{
		CreatePlayer();
		OnPlayerConnected(Network.player);
	}


	void OnDisconnectedFromServer (NetworkDisconnection info) 
	{
		connected = false;
		camera.enabled = true;
		camera.gameObject.GetComponent<AudioListener>().enabled = true;
		hideMenu = false;
		showLists = false;
		Application.LoadLevel("main");
	}


	void OnPlayerDisconnected (NetworkPlayer pl) 
	{
		Network.RemoveRPCs(pl);
		Network.DestroyPlayerObjects(pl);
	}


	void OnPlayerConnected(NetworkPlayer pl) 
	{

	}


	void CreatePlayer() 
	{
		connected = true;
		camera.enabled = false;
		camera.gameObject.GetComponent<AudioListener>().enabled = false;
		_ref = (GameObject)Network.Instantiate(PlayerPrefab, spawn.transform.position, spawn.transform.rotation, 1);
		_ref.transform.GetComponentInChildren<Camera>().camera.enabled = true;
		_ref.transform.GetComponentInChildren<AudioListener>().enabled = true;
	}
	
}
