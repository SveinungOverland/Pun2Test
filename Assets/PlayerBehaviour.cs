using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBehaviour : MonoBehaviourPun
{
    private TurnManager turnManager;

    // Start is called before the first frame update
    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        Debug.Log(turnManager);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            SendChatMessage("Hello world");
        } else if (Input.GetKeyDown(KeyCode.Return)) {
            Debug.Log("Trying to end turn");
            turnManager.EndTurn();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            Debug.Log("User pressed start turn based button");
            turnManager.StartTurnBased();
        }
    }

    public void SendChatMessage(string message) {
        Debug.Log("Sending chat message");
        this.photonView.RPC("HandleChatMessage", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.UserId, message);
    }

    [PunRPC]
    void HandleChatMessage(string identifier, string message) {
        Debug.Log(string.Format("ChatMessage {0}. {1} : {2}", turnManager.SendingMessage(), identifier, message));
    }

}
