using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FollowParticle : MonoBehaviour
{

    public GameObject follower;

    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;
    
    private void LateUpdate()
    {
        if (follower == null)
            return;

        InitializeIfNeeded();

        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = m_System.GetParticles(m_Particles);

        // Change only the particles that are alive
        if (numParticlesAlive != 0)
        {
            if (!follower.activeSelf)
                follower.SetActive(true);

            follower.transform.localPosition = m_Particles[0].position;
        }
            
            
        else
            follower.SetActive(false);
    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }
}