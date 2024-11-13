using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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
    // Referencias
    public PlayerStates pStates;
    public Rigidbody rb;
    [SerializeField]
    GameObject playerArrow;
    [SerializeField]
    GameObject playerCam;

    // Movimiento
    float dirX, dirY;
    [SerializeField]
    float speed;

    // Salto
    public float jumpForce = 5f;
    bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius;

    // Ataque
    [SerializeField]
    BoxCollider m_attackNPC_boxCollider;
    [SerializeField]
    BoxCollider m_attack_boxCollider;

    // Stats
    int m_Life;

    // Photon
    PhotonView m_PV;

    // Partículas
    ParticleSystem m_particleSystem;
    [SerializeField]
    GameObject m_particleSystemPrefab;

    void Start()
    {
        m_PV = GetComponent<PhotonView>();
        m_particleSystem = GetComponent<ParticleSystem>();
        playerCam.SetActive(m_PV.IsMine);
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

    void Update()
    {
        if (m_PV.IsMine)
        {
            Movement();
            Jumping();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            m_attackNPC_boxCollider.enabled = true;
        }
        else
        {
            m_attackNPC_boxCollider.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.K))
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

    protected void Movement()
    {
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

    [PunRPC]
    void takingDamage(int damage)
    {
        m_Life -= damage;
        if (m_Life <= 0)
        {
            StartCoroutine(waitForParticleSystem());
            LevelManager.instance.OnPlayerEliminated(); // Notificar al LevelManager que el jugador fue eliminado
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

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 1)
        {
            string data = (string)photonEvent.CustomData;
            // Obtener el nuevo rol
        }
    }
}
