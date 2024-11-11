using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenuController : MonoBehaviour
{
  
    public InputField nicknameInput;
    public GameObject gameSelectorPanel;  
    public GameObject mainMenuButtons;
    public GameObject panelButtons;
 

    void Start()
    {
        gameSelectorPanel.SetActive(false);  

       
    }

    public void OnPlayButtonPressed()
    {
        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            Debug.Log("Introduce un Nickname antes de jugar.");
            return;
        }

        PhotonNetwork.NickName = nicknameInput.text;

        gameSelectorPanel.SetActive(true); 
        panelButtons.SetActive(true);
        mainMenuButtons.SetActive(false);
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }

    public void OnVSButtonPressed()
    {
        if (string.IsNullOrEmpty(nicknameInput.text))
        {
            Debug.Log("Introduce un Nickname antes de jugar.");
            return;
        }



        PhotonNetwork.NickName = nicknameInput.text; 
        PhotonNetwork.JoinOrCreateRoom("SampleScene", new Photon.Realtime.RoomOptions { MaxPlayers = 4 }, null);
   
    }

    public void OnBackButtonPressed()
    {
        gameSelectorPanel.SetActive(false);  
        panelButtons.SetActive(false);
        mainMenuButtons.SetActive(true);
    }

    
}


