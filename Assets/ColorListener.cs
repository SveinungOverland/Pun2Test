using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ColorListener : TurnCallbackMonoBehaviour
{

    public SpriteRenderer sRenderer;

    public PhotonView photonView;

    void Start() {
        photonView = GetComponent<PhotonView>();
    }

    override public void NewTurn(string newPlayer, bool isMe) {
        if (photonView.Owner.UserId == newPlayer) {
            sRenderer.color = Color.green;
        } else {
            sRenderer.color = Color.grey;
        }
    }
}
