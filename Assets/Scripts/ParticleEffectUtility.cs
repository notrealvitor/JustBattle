using UnityEngine;
using System.Collections;

public static class ParticleEffectUtility
{
    // Static method to play and destroy a particle effect
    public static IEnumerator PlayParticleEffect(string particleName)
    {
        // Load the particle system from the Resources folder by name
        ParticleSystem particleSystem = Resources.Load<ParticleSystem>($"Art/Particles/{particleName}");

        if (particleSystem != null)
        {
            // Define the screen position for the particle effect (x=0, y=-10, z=10)
            Vector3 screenPosition = new Vector3(0, -10, 10);
            Quaternion rotation = Quaternion.Euler(0, 180, 0);

            // Instantiate the particle system at the defined position
            ParticleSystem instantiatedParticle = Object.Instantiate(particleSystem, screenPosition, rotation);

            // Play the particle system
            instantiatedParticle.Play();

            // Wait for the particle system's duration to complete
            yield return new WaitForSeconds(instantiatedParticle.main.duration);

            // Destroy the instantiated particle system after it finishes
            Object.Destroy(instantiatedParticle.gameObject);
        }
        else
        {
            Debug.LogError($"Particle system with name {particleName} not found in Resources/Art/Particles");
        }
    }
}