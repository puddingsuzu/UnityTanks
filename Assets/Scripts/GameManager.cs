using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tanks
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager instance;
        public static GameObject localPlayer;
        private GameObject defaultSpawnPoint;

        private const string MAP_PROP_KEY = "map";
        private const string GAME_MODE_PROP_KEY = "gm";
        private const string AI_PROP_KEY = "ai";

        private TypedLobby customLobby = new TypedLobby("race", LobbyType.Default);
        private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

        string gameVersion = "1";
        void Awake()
        {
            defaultSpawnPoint = new GameObject("Default SpawnPoint");
            defaultSpawnPoint.transform.position = new Vector3(0, 0, 0);
            defaultSpawnPoint.transform.SetParent(transform, false);

            if (instance != null)
            {
                Debug.LogErrorFormat(gameObject,
                "Multiple instances of {0} is not allow", GetType().Name);
                DestroyImmediate(gameObject);
                return;
            }
            PhotonNetwork.AutomaticallySyncScene = true;
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby(customLobby);
        }
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo info = roomList[i];
                if (info.RemovedFromList)
                    cachedRoomList.Remove(info.Name);
                else
                    cachedRoomList[info.Name] = info;
            }
        }
        
        public void CreateGame(int map, int gameMode)
        {
            var roomOptions = new RoomOptions();
            roomOptions.CustomRoomPropertiesForLobby = new[] { MAP_PROP_KEY, GAME_MODE_PROP_KEY, AI_PROP_KEY };
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
            { MAP_PROP_KEY, map },
            { GAME_MODE_PROP_KEY, gameMode }
            };
            roomOptions.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(null, roomOptions, null);
        }
        public void JoinRandomGame(int map, int gameMode)
        {
            byte expectedMaxPlayers = 0;
            var expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
            { MAP_PROP_KEY, map },
            { GAME_MODE_PROP_KEY, gameMode }
            };
            PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers);
        }
        // Start is called before the first frame update
        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            PhotonNetwork.GameVersion = gameVersion;
        }
        public bool ConnectToServer(string account)
        {
            PhotonNetwork.NickName = account;
            return PhotonNetwork.ConnectUsingSettings();
        }
        public override void OnConnected()
        {
            Debug.Log("PUN Connected");
        }
        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Connected to Master");
        }
        public override void OnJoinedLobby()
        {
            cachedRoomList.Clear();
        }
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList(roomList);
        }
        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            cachedRoomList.Clear();
        }

        public void JoinGameRoom()
        {
            var options = new RoomOptions
            {
                MaxPlayers = 6
            };
            PhotonNetwork.JoinOrCreateRoom("Kingdom", options, null);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Created room!!");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}{ PhotonNetwork.CurrentRoom.CustomProperties}");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameScene");
            }
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Random Room Failed: ({returnCode}) {message}");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!PhotonNetwork.InRoom)
            {
                return;
            }
            var spawnPoint = GetRandomSpawnPoint();
            localPlayer = PhotonNetwork.Instantiate("TankPlayer", spawnPoint.position, spawnPoint.rotation, 0);
            Debug.Log("Player Instance ID: " + localPlayer.GetInstanceID());
        }

        public static List<GameObject> GetAllObjectsOfTypeInScene<T>()
        {
            var objectsInScene = new List<GameObject>();
            foreach (var go in (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                //Unity內建的一些底層物件，遇到不處理。
                if (go.hideFlags == HideFlags.NotEditable ||
                go.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                if (go.GetComponent<T>() != null)
                    objectsInScene.Add(go);
            }
            return objectsInScene;
        }

        private Transform GetRandomSpawnPoint()
        {
            var spawnPoints = GetAllObjectsOfTypeInScene<SpawnPoint>();
            return spawnPoints.Count == 0
            ? defaultSpawnPoint.transform
            : spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
        }
    }
}