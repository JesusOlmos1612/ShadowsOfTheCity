using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;


public class PhotonConnection : MonoBehaviourPunCallbacks
{

    
     TextMeshProUGUI m_newNickname;


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();


    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Se ha conectado al master");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {

        Debug.Log("Se ha conectado al lobby abstracto");
        RoomOptions roomOptions = new RoomOptions();
        //PhotonNetwork.JoinOrCreateRoom("x", newRoom(), null);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Se entro al room");
        PhotonNetwork.LoadLevel("Level 1");
    }
    public void EntrarAlCuarto()
    {
        PhotonNetwork.JoinOrCreateRoom("Game", newRoom(), null);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Hubo un error al crear un room" + message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Hubo un error al entrar al room" + message);
    }
    RoomOptions newRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        return roomOptions;

    }

    public void joinRoom()
    {
        if (m_newNickname.text == null)
        {
            print("Necesita un nombre.");
            return;
        }
        PhotonNetwork.NickName = m_newNickname.text;



    }
    public void OnPlayButtonPressed()
    {
        if (string.IsNullOrEmpty(m_newNickname.text))
        {
            Debug.Log("Introduce un Nickname antes de jugar.");
            return;
        }

    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }

  

   

}
