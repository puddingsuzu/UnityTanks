using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsUI : MonoBehaviourPunCallbacks
{

    private int CountOfPlayers = -1;
    public Text CountOfPlayersText;
    private int CountOfRooms = -1;
    public Text CountOfRoomsText;
    private int CountOfPlayersInRooms = -1;
    public Text CountOfPlayersInRoomsText;
    private int CountOfPlayersOnMaster = -1;
    public Text CountOfPlayersOnMasterText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLobbyInfo();
    }
    private void UpdateLobbyInfo()
    {
        if (PhotonNetwork.NetworkingClient.Server == ServerConnection.MasterServer)
        {
            RefreshLobbyInfo();
        }
        else
        {
            ResetLobbyInfo();
        }
    }
    private void ResetLobbyInfo()
    {
        if (CountOfPlayers != -1)
        {
            CountOfPlayers = -1;
            CountOfPlayersText.text = "n/a";
        }
        if (CountOfRooms != -1)
        {
            CountOfRooms = -1;
            CountOfRoomsText.text = "n/a";
        }
        if (CountOfPlayersInRooms != -1)
        {
            CountOfPlayersInRooms = -1;
            CountOfPlayersInRoomsText.text = "n/a";
        }
        if (CountOfPlayersOnMaster != -1)
        {
            CountOfPlayersOnMaster = -1;
            CountOfPlayersOnMasterText.text = "n/a";
        }
    }

    private void RefreshLobbyInfo()
    {
        if (!PhotonNetwork.IsConnected)
        {
            ResetLobbyInfo();
            return;
        }
        if (CountOfPlayers != PhotonNetwork.CountOfPlayers)
        {
            CountOfPlayers = PhotonNetwork.CountOfPlayers;
            CountOfPlayersText.text = CountOfPlayers.ToString();
        }
        if (CountOfRooms != PhotonNetwork.CountOfRooms)
        {
            CountOfRooms = PhotonNetwork.CountOfRooms;
            CountOfRoomsText.text = CountOfRooms.ToString();
        }
        if (CountOfPlayersInRooms != PhotonNetwork.CountOfPlayersInRooms)
        {
            CountOfPlayersInRooms = PhotonNetwork.CountOfPlayersInRooms;
            CountOfPlayersInRoomsText.text = CountOfPlayersInRooms.ToString();
        }
        if (CountOfPlayersOnMaster != PhotonNetwork.CountOfPlayersOnMaster)
        {
            CountOfPlayersOnMaster = PhotonNetwork.CountOfPlayersOnMaster;
            CountOfPlayersOnMasterText.text = CountOfPlayersOnMaster.ToString();
        }
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        var message = "Lobby Count: " + lobbyStatistics.Count + "\r\n";
        foreach (var lobby in lobbyStatistics)
        {
            message += " " + lobby;
        }
        Debug.Log(message);
    }

}
