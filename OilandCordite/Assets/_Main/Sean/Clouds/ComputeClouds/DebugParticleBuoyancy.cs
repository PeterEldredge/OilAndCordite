using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugParticleBuoyancy : MonoBehaviour
{
    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;
    Vector3[] m_InitialParticleLocations;
    public float DistanceThreshold;
    public float ParticleRecoverySpeed;

    private void Start() 
    {
        Invoke("BuildInitialParticleLocation", 1.0f);
    }

    private void LateUpdate()
    {

        InitializeIfNeeded();

        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = m_System.GetParticles(m_Particles);

        if(m_InitialParticleLocations == null || m_InitialParticleLocations.Length < 1) 
        {
            return;
        }

        // Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++)
        {
            //m_Particles[i].velocity += Vector3.up * ParticleRecoverySpeed;
            Vector3 particlePostion = m_Particles[i].position;
            Vector3 initialParticlePosition = m_InitialParticleLocations[i];
            if(Vector3.Distance(particlePostion, initialParticlePosition) > DistanceThreshold) 
            {
                m_Particles[i].position = Vector3.MoveTowards(particlePostion, initialParticlePosition, ParticleRecoverySpeed);
            }
        }

        // Apply the particle changes to the Particle System
        m_System.SetParticles(m_Particles, numParticlesAlive);
    }

    void BuildInitialParticleLocation() 
    {
        int numParticlesAlive = m_System.GetParticles(m_Particles);
        m_InitialParticleLocations = new Vector3[m_Particles.Length];
        for(int i  = 0; i < numParticlesAlive; i++) 
        {
            m_InitialParticleLocations[i] = m_Particles[i].position;
        }
    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }
}