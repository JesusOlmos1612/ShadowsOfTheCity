using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    Transform pos1, pos2, pos3, pos4;
    protected int pPosition;

    PhotonView m_photonView;
    void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer != null)
        {
            pPosition = Random.Range(1, 4);
            switch (pPosition)
            {
                case 1:
                    PhotonNetwork.Instantiate("Player", new Vector3(pos1.position.x,pos1.position.y,pos1.position.z), Quaternion.identity);
                    break;
                case 2:
                    PhotonNetwork.Instantiate("Player", new Vector3(pos2.position.x, pos2.position.y, pos2.position.z), Quaternion.identity);
                    break;
                case 3:
                    PhotonNetwork.Instantiate("Player", new Vector3(pos3.position.x, pos3.position.y, pos3.position.z), Quaternion.identity);
                    break;
                case 4:
                    PhotonNetwork.Instantiate("Player", new Vector3(pos4.position.x, pos4.position.y, pos4.position.z), Quaternion.identity);
                    break;
            }

            if (m_photonView.IsMine)
            {
                PhotonNetwork.Instantiate("LVLManager", new Vector3(0, 0, 0), Quaternion.identity);
            }
            
         
            


        }
    }



}
    