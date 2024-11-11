using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public enum PlayerStates
{
    NONE,
    isIdle,
    isMoving,
    isJumping

}
public class Players : MonoBehaviourPunCallbacks, IOnEventCallback
{
    
    //References
    public PlayerStates pStates;
    public Rigidbody rb;
    [SerializeField]
    GameObject playerArrow;
    [SerializeField]
    GameObject playerCam;
    

    //Movement
    float dirX, dirY;
    [SerializeField]
    float speed;
    
    //Jump
    public float jumpForce = 5f;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius;

    //Attack
    [SerializeField]
    BoxCollider m_attackNPC_boxCollider;
    [SerializeField]
    BoxCollider m_attack_boxCollider;

    //Stats
    int m_Life;

    //Photon
    PhotonView m_PV;


    //Particles
    ParticleSystem m_particleSystem;
    [SerializeField]
    GameObject m_particleSystemPrefab;

    //TextMesh

    // Start is called before the first frame update
    #region UnityMethods
    void Start()
    {
        m_PV = GetComponent<PhotonView>();
        m_particleSystem = GetComponent<ParticleSystem>();
        playerCam = GetComponent<GameObject>();
        playerCam.SetActive(m_PV.IsMine);
        pStates = PlayerStates.isMoving;
        if (m_PV.IsMine)
        {
            playerArrow.SetActive(true);
        }
        else
        {
            playerArrow.SetActive(false);
        }
        m_Life = 1;
        m_attackNPC_boxCollider.enabled = false;
        m_particleSystem.Stop();
     
    }

    // Update is called once per frame
    void Update()
    { 
        if (m_PV.IsMine)
        {
            Movement();
            Jumping();
        }
        
        #region PlayerFSM

        switch (pStates)
        {
            case PlayerStates.isIdle:

                if (rb.velocity.magnitude >= 0.5f)
                {
                    pStates = PlayerStates.isMoving;
                }
                else
                {
                    pStates = PlayerStates.isIdle;
                }
                break;
            case PlayerStates.isMoving:
                if (rb.velocity.y >= 1.0f)
                {
                    pStates = PlayerStates.isJumping;
                }
             
                break;
            case PlayerStates.isJumping:
                if (isGrounded)
                {
                    pStates = PlayerStates.isIdle;
                }
                break;
        
        }

        #endregion

        if(Input.GetKeyDown(KeyCode.M))
        {
            m_attackNPC_boxCollider.enabled = true;   
        }
        else
        {
            m_attackNPC_boxCollider.enabled = false;
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            m_attackNPC_boxCollider.enabled = true;
        }
        else
        {
            m_attackNPC_boxCollider.enabled = false;
        }
       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            other.GetComponent<NPCMovement>().DestroyNPC();
        }
        else if (other.CompareTag("Damage"))
        {
            m_PV.RPC("takingDamage", RpcTarget.All, 1);
        }
    }
    #endregion

    #region PlayerMethods
    protected void Movement()
    {
        //Move
        
        dirX = Input.GetAxis("Horizontal");
        dirY = Input.GetAxis("Vertical");
        rb.velocity = new Vector3(dirX * speed, rb.velocity.y, dirY * speed);
        
       
       
    }
    protected void Jumping()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);


        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

    }


    protected void damageOtherPlayer (Player p_otherPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Hashtable playerStats = new Hashtable();
            playerStats["damage"] = 1;
            p_otherPlayer.SetCustomProperties(playerStats);
        }

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("damage"))
        {
            m_Life -= (int)changedProps["damage"];
            //Modificar la vida del usuario
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 1)
        {
            string data = (string)photonEvent.CustomData;
            GetNewGameplayRole();
        }
    }

    public void GetNewGameplayRole()
    {

    }

    [PunRPC]
    void takingDamage(int damage)
    {
        print("Se murio");
        m_Life -= damage;
        if(m_Life <= 0)
        {
            
            StartCoroutine(waitForParticleSystem());
        }
    }

    IEnumerator waitForParticleSystem()
    {
        PhotonView.Destroy(gameObject);

        GameObject particles = Instantiate(m_particleSystemPrefab, transform.position, Quaternion.identity);


        ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        

        yield return new WaitForSeconds(particleSystem.main.duration);
    }
    #endregion



}
