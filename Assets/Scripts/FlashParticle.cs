using System.Collections;

using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FlashParticle : MonoBehaviour
{
    public float duration = .5f;
    public float intensity = 1;
    public Color lightColor = Color.white;

    public Light lightSource;

    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;
    private int lastCount = 0;

    private bool activated = false;

    private void LateUpdate()
    {
        if (lightSource == null)
            return;

        InitializeIfNeeded();

        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = m_System.GetParticles(m_Particles);

        if (numParticlesAlive != 0 && activated == false && numParticlesAlive > lastCount)
        {
            StartCoroutine(StartLight());
        }
        lastCount = numParticlesAlive;
        if (activated)
            lightSource.transform.position = m_Particles[0].position;
    }

    private IEnumerator StartLight()
    {
        Debug.Log("Start Light");
        if (lightSource == null)
            yield return null;

        activated = true;

        lightSource.intensity = intensity;
        lightSource.color = lightColor;
        yield return new WaitForSeconds(duration);
        StartCoroutine(FadeLight());
    }

    private IEnumerator FadeLight()
    {
        if (lightSource == null)
            yield return null;

        while (lightSource.intensity > 0)
        {
            yield return new WaitForSeconds(.01f);
            lightSource.intensity -= .1f;
        }
        activated = false;
    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];

    }
}