using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    MainMenu,
    PauseMenu,
    InstructionsMenu,
    LoadLevelMenu,
    Game
}

public class NetworkManager : Photon.MonoBehaviour
{

    // Scene GameObjects
    public GameObject level;
    public GameObject standbyCamera;
    SpawnSpot[] spawnSpots;

    // Network Variables
    public bool offlineMode = false;
    public bool connecting = false;
    public bool singlePlayer = false;
    List<string> chatMessages;
    int maxChatMessages = 5;
    int randPlayerColor;
    string gameVersion = "FPST 1.0";

    // GameState Enum
    public GameState gameState = GameState.MainMenu;

    // Level Scene Name String
    static string[] levelNames = {	/*"FPSTeleport",
									"The Confines",
                                    "The Domain",*/
                                    "Drone" };
    string levelSelected = levelNames[0];



    //Use this for initialization
    void Start()
    {
        spawnSpots = FindObjectsOfType<SpawnSpot>();
        level = GameObject.Find("Level");
        standbyCamera = GameObject.Find("standbyCamera");
        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "Gladiatus");
        chatMessages = new List<string>();

        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.SwitchToProtocol(ExitGames.Client.Photon.ConnectionProtocol.Udp);
    }

    void OnDestroy()
    {
        PlayerPrefs.SetString("Username", PhotonNetwork.player.name);
    }

    void Connect()
    {
        PhotonNetwork.ConnectToRegion(CloudRegionCode.us, gameVersion);
        //PhotonNetwork.ConnectUsingSettings(gameVersion);
    }

    public void AddChatMessage(string msg)
    {
        while (chatMessages.Count >= maxChatMessages)
        {
            chatMessages.RemoveAt(0);
        }
        chatMessages.Add(msg);
    }

    void OnGUI()
    {
        // Print out connection status (Upper Left)
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        // On first connect, prevent screen clipping while connecting
        if (PhotonNetwork.connected == false && connecting == false)
        {
            if (gameState != GameState.MainMenu && gameState != GameState.InstructionsMenu) { gameState = GameState.LoadLevelMenu; }
        }

        // Player Join Info (Upper Right)
        if (PhotonNetwork.connected == true && connecting == false)
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            foreach (string msg in chatMessages)
            {
                GUILayout.Label(msg);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        // Menu States
        if (gameState == GameState.MainMenu) { MainMenuGUI(); }
        else if (gameState == GameState.LoadLevelMenu) { LoadLevelMenuGUI(); }
        else if (gameState == GameState.InstructionsMenu) { InstructionsMenuGUI(); }
        else if (gameState == GameState.PauseMenu) { PauseMenuGUI(); }
        else if (gameState == GameState.Game) { /* No Menu */ }
    }

    void Update()
    {
        if (gameState != GameState.Game) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Lock or Unlock the Cursor via L or ESC
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //Debug.Log("Lock");
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //Debug.Log("Unlock");
        }
        /*
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.L))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                //Debug.Log("Unlock");
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                //Debug.Log("Lock");
            }
        }*/
    }

    public void MainMenuGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        // Title
        GUI.DrawTexture(
            new Rect(
            Screen.width / 4, Screen.height / 5,
            Screen.width / 2, Screen.height / 5),
            AssetManager.titleTexture,
            ScaleMode.ScaleToFit);

        // Player Username Input
        GUILayout.BeginHorizontal();
        GUILayout.Label("Username: ");
        PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name);
        GUILayout.EndHorizontal();

        // Buttons
        if (GUILayout.Button("Single Player"))
        {
            singlePlayer = true;
            gameState = GameState.LoadLevelMenu;
        }
        else if (GUILayout.Button("Multiplayer Player"))
        {
            singlePlayer = false;
            gameState = GameState.LoadLevelMenu;
        }
        else if (GUILayout.Button("Instructions"))
        {
            gameState = GameState.InstructionsMenu;
        }
        else if (GUILayout.Button("Quit"))
        {
            Application.Quit();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void InstructionsMenuGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUI.DrawTexture(
            new Rect(
            ((Screen.width / 2) - (AssetManager.instructionsTexture.width / 2)),
            ((Screen.height / 2) - (AssetManager.instructionsTexture.height / 2) - 10),
            AssetManager.instructionsTexture.width,
            AssetManager.instructionsTexture.height),
            AssetManager.instructionsTexture,
            ScaleMode.ScaleToFit);

        if (GUILayout.Button("Return to Main Menu"))
        {
            gameState = GameState.MainMenu;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void PauseMenuGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUI.DrawTexture(
            new Rect(
            ((Screen.width / 2) - (AssetManager.instructionsTexture.width / 2)),
            ((Screen.height / 2) - (AssetManager.instructionsTexture.height / 2) - 10),
            AssetManager.instructionsTexture.width,
            AssetManager.instructionsTexture.height),
            AssetManager.instructionsTexture,
            ScaleMode.ScaleToFit);

        if (GUILayout.Button("Resume Game"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            gameState = GameState.Game;
        }
        if (GUILayout.Button("Return to Main Menu"))
        {
            LeaveGame();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void LoadLevelMenuGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Select Map");
        GUILayout.EndHorizontal();

        // Create a button for each Level
        for (int i = 0; i < levelNames.Length; i++)
        {
            // Select and Load Scene
            if (GUILayout.Button(levelNames[i]))
            {
                levelSelected = levelNames[i];
                ChangeLevel(levelNames[i]);
                gameState = GameState.Game;
                BeginGame();
            }
        }

        if (GUILayout.Button("Return to Main Menu"))
        {
            gameState = GameState.MainMenu;
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void BeginGame()
    {
        // Single Player Mode
        if (singlePlayer)
        {
            connecting = true;
            PhotonNetwork.offlineMode = true;
            OnJoinedLobby();
        }
        // Multiplayer Mode
        else
        {
            connecting = true;
            Connect();
        }
    }

    void LeaveGame()
    {
        gameState = GameState.MainMenu;
        standbyCamera.SetActive(true);
        FindObjectOfType<PlayerController>().gameObject.GetComponent<PlayerAttributes>().Die();
        PhotonNetwork.Disconnect();
        chatMessages.Clear();
    }

    void ChangeLevel(string levelName)
    {
        if (level.transform.childCount != 0)
        {
            Destroy(level.transform.GetChild(0).gameObject);
        }
        GameObject newLevel = Instantiate(AssetManager.levels[levelName]);
        newLevel.transform.parent = level.transform;
    }

    void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby()");
        PhotonNetwork.JoinRoom(levelSelected);
    }

    void OnPhotonJoinRoomFailed()
    {
        Debug.Log("OnPhotonRandomJoinedFailed()");

        // Each level gets its own Room! ***
        PhotonNetwork.CreateRoom(levelSelected);
    }

    void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() " + PhotonNetwork.room.name);

        connecting = false;
        SpawnMyPlayer();
    }

    public void SpawnMyPlayer()
    {
        spawnSpots = FindObjectsOfType<SpawnSpot>();

        if (spawnSpots == null)
        {
            Debug.LogError("No Spawns On Map");
            return;
        }

        AddChatMessage(PhotonNetwork.player.name + " has joined the battle!");

        // Create Player GameObject and Assign Color

        SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        GameObject myPlayerGO = PhotonNetwork.Instantiate("_Player", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        myPlayerGO.GetComponent<PlayerController>().enabled = true;
        myPlayerGO.GetComponent<PlayerAttributes>().guiHUD = Instantiate(AssetManager.HUDObj);
        myPlayerGO.GetComponent<PhotonView>().RPC("SetColor", PhotonTargets.AllBuffered, Random.Range(0, AssetManager.colors.Length));

        // Set Camera to Player
         
        //myPlayerGO.transform.FindChild("Main Camera").gameObject.SetActive(true);
        standbyCamera.SetActive(false);

        // Create Health Bar UI

        myPlayerGO.GetComponent<PlayerAttributes>().guiHealthBar = Instantiate(AssetManager.healthBar);

        // Spawn Particle Effect
        PhotonNetwork.Instantiate("Spawn Particles", myPlayerGO.transform.position, Quaternion.identity, 0);
    }
}