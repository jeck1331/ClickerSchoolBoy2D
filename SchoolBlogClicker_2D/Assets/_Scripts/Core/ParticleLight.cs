using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleLight : MonoBehaviour
{
    [SerializeField] private Light2D lightPrefab;
    [SerializeField, Range(1, 50)] private int maxLights = 20;
    [SerializeField] private float intensityMultiplier = 1f;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private readonly List<Light2D> lights = new List<Light2D>();

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    private void LateUpdate()
    {
        if (lightPrefab == null) return;

        int count = Mathf.Min(ps.GetParticles(particles), maxLights);

        // Удаляем лишние источники света,
        // если количество частиц уменьшилось.
        while (lights.Count > count)
        {
            int last = lights.Count - 1;

            if (lights[last] != null)
                Destroy(lights[last].gameObject);

            lights.RemoveAt(last);
        }

        // Создаём новые источники для новых частиц.
        while (lights.Count < count)
        {
            Light2D lightInstance = Instantiate(lightPrefab);
            lights.Add(lightInstance);
        }

        var main = ps.main;

        for (int i = 0; i < count; i++)
        {
            var particle = particles[i];
            Light2D light = lights[i];

            Vector3 position = particle.position;

            if (main.simulationSpace == ParticleSystemSimulationSpace.Local)
                position = transform.TransformPoint(position);
            else if (main.simulationSpace == ParticleSystemSimulationSpace.Custom &&
                     main.customSimulationSpace != null)
                position = main.customSimulationSpace.TransformPoint(position);

            light.transform.position = position;

            Color color = particle.GetCurrentColor(ps);
            light.color = color;

            float lifetime = particle.startLifetime > 0f
                ? particle.remainingLifetime / particle.startLifetime
                : 0f;

            light.intensity =
                lightPrefab.intensity * intensityMultiplier *
                color.a * Mathf.Clamp01(lifetime);
        }
    }

    private void OnDestroy()
    {
        foreach (var light in lights)
        {
            if (light != null)
                Destroy(light.gameObject);
        }
    }
}
