using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;


public abstract class TurnCallbackMonoBehaviour : MonoBehaviour {
    abstract public void NewTurn(string newPlayer, bool isMe);
}

public class TurnManager : MonoBehaviour
{


    private PhotonManager photonManager;

    private PhotonView photonView;

    private List<string> players = new List<string>();

    private string currentPlayerTurn = "";
    private int messageCount = 0;

    void Start() {
        photonManager = GetComponent<PhotonManager>();
        photonView = GetComponent<PhotonView>();
    }

    public int SendingMessage() {
        return ++messageCount;
    }
    public void StartTurnBased() {
        if (currentPlayerTurn != "") {
            Debug.Log("Game already started");
        } else {
            Debug.Log("Starting TurnBased");
            this.photonView.RPC("RPCTurnInit", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPCTurnInit() {
        Debug.Log("Registering " + PhotonNetwork.LocalPlayer.UserId + " with master client");
        this.photonView.RPC("RPCMasterTurnInit", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId);
    }

    [PunRPC]
    void RPCMasterTurnInit(string id) {
        if (PhotonNetwork.IsMasterClient) {
            this.players.Add(id);
            if (this.players.Count == PhotonNetwork.PlayerList.Length) {
                Debug.Log(this.players.Count + " players registered at start of game");
                var firstPlayer = this.players[Random.Range(0, this.players.Count)];
                Debug.Log("First player" + firstPlayer);
                this.photonView.RPC("RPCAssignTurnToPlayer", RpcTarget.All, firstPlayer);
            }
        }
    }

    [PunRPC]
    void RPCAssignTurnToPlayer(string playerID) {
        this.currentPlayerTurn = playerID;
        Debug.Log("Rpc input " + playerID);
        Debug.Log("Turn was assigned to: " + this.currentPlayerTurn + " you are " + PhotonNetwork.LocalPlayer.UserId);
        var isMe = playerID == PhotonNetwork.LocalPlayer.UserId;
        foreach (var listener in FindObjectsOfType<TurnCallbackMonoBehaviour>()) {
            listener.NewTurn(playerID, isMe);
        }
    }

    public void EndTurn() {
        if (currentPlayerTurn == PhotonNetwork.LocalPlayer.UserId) {
            this.photonView.RPC("RPCMasterCallEndTurn", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId);
        } else {
            Debug.Log("Not your turn: " + PhotonNetwork.LocalPlayer.UserId + " its " + this.currentPlayerTurn + " 's turn");
        }
    }

    [PunRPC]
    void RPCMasterCallEndTurn(string playerID) {
        if (PhotonNetwork.IsMasterClient) {
            if (playerID == currentPlayerTurn) {
                Debug.Log("Players to chose from: " + this.players.ToStringFull());
                var nextPlayerIndex = this.players.FindIndex(it => it == playerID) + 1;
                if (nextPlayerIndex == this.players.Count) nextPlayerIndex = 0;
                this.photonView.RPC("RPCAssignTurnToPlayer", RpcTarget.All, this.players[nextPlayerIndex]);
            }
        }
    }

    // public void EndTurn() {
    //     // currentPlayerTurn = currentPlayerTurn.GetNext();
    //     // SetTurn(currentPlayerTurn.UserId);
    //     if (currentPlayerTurn == PhotonNetwork.LocalPlayer.UserId) {
    //         this.photonView.RPC("RPCEndTurnHandler", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId);
    //     } else {
    //         Debug.Log("Turn has already been ended, can't do it again");
    //     }
    // }
    // [PunRPC]
    // void RPCEndTurnHandler(string playerEndingTurnId) {
    //     if (PhotonNetwork.IsMasterClient) {
    //         if (playerEndingTurnId == currentPlayerTurn) {
    //             var playerlist = PhotonNetwork.PlayerList;
    //             Debug.Log("Playerlist length:" + playerlist.Length);
    //             Player nextPlayer = null;
    //             for (var i = 0; i < playerlist.Length; i++) {
    //                 Debug.Log("Player ID for " + i + " " + playerlist[i].UserId);
    //                 if (playerlist[i].UserId == currentPlayerTurn) {
    //                     if (i+1 == playerlist.Length) {
    //                         nextPlayer = playerlist[0];
    //                     } else {
    //                         nextPlayer = playerlist[i +1];
    //                     }
    //                 }
    //             }
    //             if (nextPlayer == null) {
    //                 Debug.LogError("Next player could not be determined");
    //                 return;
    //             }
    //             Debug.Log("Next player Id: " + nextPlayer.UserId);
    //             // End turn is accepted and next player starts their turn
    //             this.photonView.RPC("RPCNewTurnHandler", RpcTarget.All, nextPlayer.UserId);
    //         }
    //     }
    // }
    // [PunRPC]
    // void RPCNewTurnHandler(string newPlayerId) {
    //     currentPlayerTurn = newPlayerId;
    //     Debug.Log("New playerId: " + newPlayerId);
    //     var isMe = newPlayerId == PhotonNetwork.LocalPlayer.UserId;
    //     foreach (var listener in listeners) listener.NewTurn(newPlayerId, isMe);
    // }

    // public void DumpToConsole(object obj) {
    //     var output = JsonUtility.ToJson(obj, true);
    //     Debug.Log(output);
    // }
}
