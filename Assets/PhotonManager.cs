using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    
    public Player[] players = new Player[]{};

    private List<System.Action<Player[]>> playersUpdateListeners = new List<System.Action<Player[]>>();

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4, PublishUserId = true }, TypedLobby.Default);
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-10f, 10f), 5, 0), transform.rotation);
        players = PhotonNetwork.PlayerList;
        Debug.Log(players.ToStringFull());
        playersUpdateListeners.ForEach(listener => listener(players));
    }

    public void RegisterPlayerListUpdateListener(System.Action<Player[]> listener) {
        playersUpdateListeners.Add(listener);
    }

}
