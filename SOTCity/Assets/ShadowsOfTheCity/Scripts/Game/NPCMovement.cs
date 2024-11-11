using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class NPCMovement : MonoBehaviour
{
    PhotonView m_PV;
    NavMeshAgent m_agent;
    [SerializeField]
    float m_moveRadius;

    [SerializeField]
    GameObject particleSystemPrefab; 

    // Start is called before the first frame update
    void Start()
    {
        m_PV = GetComponent<PhotonView>();
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_agent.pathPending && m_agent.remainingDistance < 0.5f)
        {
            moveRandomPosition();
        }
    }

    void moveRandomPosition()
    {
        Vector3 m_randomDirection = Random.insideUnitSphere * m_moveRadius;
        m_randomDirection += transform.position;

        NavMeshHit m_hit;
        if (NavMesh.SamplePosition(m_randomDirection, out m_hit, m_moveRadius, NavMesh.AllAreas))
        {
            m_agent.SetDestination(m_hit.position);
        }
    }

    public void DestroyNPC()
    {
        m_agent.speed = 0;
        m_PV.RPC("destroyCurrentNPC", RpcTarget.All);
    }

    [PunRPC]
    void destroyCurrentNPC()
    {
        StartCoroutine(waitForParticleSystem());
    }

    IEnumerator waitForParticleSystem()
    {
        PhotonView.Destroy(gameObject);

        GameObject particles = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);

      
        ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        Destroy(particles);

        yield return new WaitForSeconds(particleSystem.main.duration);

        

        
       
    }
}
