using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemToMarchingCubes : MonoBehaviour
{
    MarchingCubesGPU_4DNoise volume;
    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;
    
    private bool isInvoked;
    // Start is called before the first frame update
    void Start()
    {
        volume = this.GetComponentInParent<MarchingCubesGPU_4DNoise>();
        Invoke("InitializeParticlesOnStartup", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isInvoked) 
        {
            // MIGHT CAUSE MEM LEAK
            IntializeParticles();
        }
    }  

    void InitializeParticlesOnStartup() 
    {
        InitializeIfNeeded();
        volume.SetControlPoints(BuildParticleList());
        volume.UpdateBuffers();
        volume.InitializeOnStart();
        volume.FinishInitializing();
        isInvoked = true;
    }

    void IntializeParticles() 
    {
        InitializeIfNeeded();
        volume.SetControlPoints(BuildParticleList());
    }

    Vector3[] BuildParticleList() 
    {
        int numParticlesAlive = m_System.GetParticles(m_Particles);
        Vector3[] particleLoc = new Vector3[numParticlesAlive];
        for(int i = 0; i < numParticlesAlive; i++) 
        {
            particleLoc[i] = m_Particles[i].position;
        }
        return particleLoc;
    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
    }

}
