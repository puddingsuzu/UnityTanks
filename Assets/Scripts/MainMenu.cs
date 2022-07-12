using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;

namespace Tanks
{
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        public static MainMenu instance;
        private GameObject m_loginUI;
        private TMP_InputField m_accountInput;
        private Button m_loginButton;
        private GameObject m_lobbyUI;
        private InputField m_lobbyInput;
        private Button m_joinLobbyButton;
        private Button m_leaveLobbyButton;
        private Dropdown m_mapSelector;
        private Dropdown m_gameModeSelector;
        private Button m_createGameButton;
        private Button m_joinGameButton;
        private GameObject m_roomUI;

        private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
        void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            instance = this;

            m_loginUI = transform.FindAnyChild<Transform>("LoginUI").gameObject;
            m_accountInput = transform.FindAnyChild<TMP_InputField>("AccountInput");
            m_loginButton = transform.FindAnyChild<Button>("LoginButton");
            m_lobbyUI = transform.FindAnyChild<Transform>("LobbyUI").gameObject;
            m_lobbyInput = transform.FindAnyChild<InputField>("LobbyInput");
            m_joinLobbyButton = transform.FindAnyChild<Button>("JoinLobbyButton");
            m_leaveLobbyButton = transform.FindAnyChild<Button>("LeaveLobbyButton");
            m_mapSelector = transform.FindAnyChild<Dropdown>("MapSelector");
            m_gameModeSelector = transform.FindAnyChild<Dropdown>("GameModeSelector");
            m_createGameButton = transform.FindAnyChild<Button>("CreateGameButton");
            m_joinGameButton = transform.FindAnyChild<Button>("JoinGameButton");
            m_roomUI = transform.FindAnyChild<Transform>("RoomUI").gameObject;

            ResetUI();
        }

        private void ResetUI()
        {
            m_loginUI.SetActive(true);
            m_accountInput.interactable = true;
            m_loginButton.interactable = true;
            m_lobbyUI.SetActive(false);
            m_lobbyInput.interactable = true;
            m_joinLobbyButton.interactable = true;
            m_leaveLobbyButton.interactable = false;
            m_mapSelector.interactable = true;
            m_gameModeSelector.interactable = true;
            m_createGameButton.interactable = true;
            m_joinGameButton.interactable = true;
            m_roomUI.SetActive(false);

            cachedRoomList.Clear();
        }

        public void JoinLobby()
        {
            cachedRoomList.Clear();

            var typedLobby = new TypedLobby(m_lobbyInput.text, LobbyType.Default);
            PhotonNetwork.JoinLobby(typedLobby);
        }
        public void LeaveLobby()
        {
            PhotonNetwork.LeaveLobby();
        }
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList(roomList);
            printRoomList();
        }
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo info = roomList[i];
                if (info.RemovedFromList) // 不紀錄已關閉、滿了、或是隱藏的房間
                    cachedRoomList.Remove(info.Name);
                else
                    cachedRoomList[info.Name] = info;
            }
        }
        private void printRoomList()
        {
            var message = $"Room List: {cachedRoomList.Count} rooms\n";
            foreach (var roomInfo in cachedRoomList)
            {
                message += $" {roomInfo.Key}, {roomInfo.Value.IsOpen}, " +
                $"{roomInfo.Value.PlayerCount}/{roomInfo.Value.MaxPlayers}\n";
            }

            Debug.Log(message);
        }
        public override void OnJoinedLobby()
        {
            Debug.Log($"Joined Lobby: {PhotonNetwork.CurrentLobby.Name} {PhotonNetwork.CurrentLobby.Type}");
            m_leaveLobbyButton.interactable = true;
            cachedRoomList.Clear();
        }
        public override void OnLeftLobby()
        {
            // 離開 Lobby 的時候,會加回 Default Lobby
            Debug.Log($"Left Lobby: {PhotonNetwork.CurrentLobby.Name} {PhotonNetwork.CurrentLobby.Type}");
            m_leaveLobbyButton.interactable = false;
            cachedRoomList.Clear();
        }
        public void CreateGame()
        {
            GameManager.instance.CreateGame(m_mapSelector.value + 1, m_gameModeSelector.value + 1);
        }
        public void JoinRandomGame()
        {
            GameManager.instance.JoinRandomGame(m_mapSelector.value + 1, m_gameModeSelector.value + 1);
        }

        public void Login() // 處理 登入伺服器流程
        {
            if (string.IsNullOrEmpty(m_accountInput.text))
            {
                Debug.Log("Please input your account!!");
                return;
            }
            m_accountInput.interactable = false;
            m_loginButton.interactable = false;
            if (!GameManager.instance.ConnectToServer(m_accountInput.text))
            {
                Debug.Log("Connect to PUN Failed!!");
                m_accountInput.interactable = true;
                m_loginButton.interactable = true;
            }
        }
        public override void OnConnectedToMaster()
        {
            m_loginUI.SetActive(false);
            m_lobbyUI.SetActive(true);
        }
        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!PhotonNetwork.InRoom)
            {
                ResetUI();
            }
            else
            {
                m_lobbyUI.SetActive(false);
                m_roomUI.SetActive(false);
            }
        }
    }
}
