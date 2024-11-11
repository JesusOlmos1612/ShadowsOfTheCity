using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LVLNetwork : MonoBehaviourPunCallbacks
{
    public static LVLNetwork instance;
     PhotonView m_PV;

    private bool canMove;

    public float temp = 3.0f;

    [SerializeField]
    TextMeshProUGUI victoryWarning;
    [SerializeField]
    GameObject victoryPanel;
    public bool CanMove { get => canMove; set => canMove = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        m_PV = GetComponent<PhotonView>();
        CanMove = true;

        victoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            print("Comienza el timer antes de que se pueda mover el jugador");
            temp -= Time.deltaTime;
            if (temp <= 0)
            {
                m_PV.RPC("CanMoveTrueFunction", RpcTarget.AllBuffered);
            }
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {

                Player lastPlayer = PhotonNetwork.PlayerList[0];
                victoryWarning.text = "!!!!!!!!!!" + lastPlayer.NickName + " wins!!!!!!!!";
                victoryPanel.SetActive(true);
                m_PV.RPC("OnPlayerLeftRoom", RpcTarget.AllBuffered);
            }
        }

        
    }
    [PunRPC]
    void CanMoveTrueFunction()
    {
        CanMove = true;
    }
    public void disconnectFromCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("Entró nuevo player: " + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print("Salió el player: " + otherPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Player lastPlayer = PhotonNetwork.PlayerList[0];
            victoryWarning.text = lastPlayer.NickName + " wins!";
            victoryPanel.SetActive(true);
        }
    }
   

}
