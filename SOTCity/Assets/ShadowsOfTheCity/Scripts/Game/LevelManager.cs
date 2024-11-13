using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager instance;

    PhotonView m_photonView;
    LevelManagerState m_currentState;

    // Contadores
    private int totalPlayers = 0;
    private int eliminatedPlayers = 0;

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
    }

    void Start()
    {
        m_photonView = GetComponent<PhotonView>();

        setLevelManagerSate(LevelManagerState.Waiting);
    }

    void setNewRoleEvent()
    {
        byte m_ID = 1; // Código del Evento (1...199)
        object content = "Asignación de nuevo rol...";
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(m_ID, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public LevelManagerState CurrentState { get { return m_currentState; } }
    public LevelManagerState getLevelManagerSate()
    {
        return m_currentState;
    }

    public void setLevelManagerSate(LevelManagerState p_newState)
    {
        if (p_newState == m_currentState)
        {
            return;
        }
        m_currentState = p_newState;

        switch (m_currentState)
        {
            case LevelManagerState.None:
                break;
            case LevelManagerState.Waiting:
                break;
            case LevelManagerState.Playing:
                playing();
                break;
        }
    }

    void playing()
    {
        assignRole();
        setNewRoleEvent();
    }

    void assignRole()
    {
        Player[] m_playersArray = PhotonNetwork.PlayerList;
        List<GameplayRole> m_gameplayRolesToBeAssign = new List<GameplayRole> { GameplayRole.Innocent, GameplayRole.Innocent, GameplayRole.Traitor, GameplayRole.Traitor };
        m_gameplayRolesToBeAssign = m_gameplayRolesToBeAssign.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < m_playersArray.Length; i++)
        {
            Hashtable m_playerProperties = new Hashtable();
            var role = m_gameplayRolesToBeAssign[Random.Range(0, m_gameplayRolesToBeAssign.Count)].ToString();
            m_playerProperties["Role"] = role;
            m_playersArray[i].SetCustomProperties(m_playerProperties);
            m_gameplayRolesToBeAssign.Remove(m_gameplayRolesToBeAssign[Random.Range(0, m_gameplayRolesToBeAssign.Count - 1)]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        totalPlayers++;
        if (totalPlayers >= 4)
        {
            StartCoroutine(timerToStart());
        }
    }

    IEnumerator timerToStart()
    {
        yield return new WaitForSeconds(3);
        setLevelManagerSate(LevelManagerState.Playing);
    }

    public void OnPlayerEliminated()
    {
        eliminatedPlayers++;
        CheckForEndGame();
    }

    void CheckForEndGame()
    {
        int innocentCount = PhotonNetwork.PlayerList.Count(p => (string)p.CustomProperties["Role"] == "Innocent");
        int traitorCount = PhotonNetwork.PlayerList.Count(p => (string)p.CustomProperties["Role"] == "Traitor");

        if (eliminatedPlayers >= totalPlayers - innocentCount && innocentCount > 0)
        {
            EndGame("Los Inocentes han ganado");
        }
        else if (eliminatedPlayers >= totalPlayers - traitorCount && traitorCount > 0)
        {
            EndGame("Los Traidores han ganado");
        }
    }

    void EndGame(string result)
    {
        m_photonView.RPC("AnnounceEndGame", RpcTarget.All, result);
    }

    [PunRPC]
    void AnnounceEndGame(string result)
    {
        // Mostrar el canvas con el resultado
        //CanvasManager.Instance.ShowEndGame(result);
    }
}

public enum LevelManagerState
{
    None,
    Waiting,
    Playing
}

public enum GameplayRole
{
    Innocent,
    Traitor
}
