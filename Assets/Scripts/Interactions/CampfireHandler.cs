using UnityEngine;

public class CampfireHandler : BaseInteractableItem
{
    [Header("Campfire Settings")]
    [SerializeField] private GameObject fireParticles;
    [SerializeField] private GameObject smokeParticles;

    private bool isActiveFire;
    
    private void Start()
    {
        base.Start();
        
        // Disable both particles effects at the start
        if (fireParticles != null)
            fireParticles.SetActive(false);

        if (smokeParticles != null)
            smokeParticles.SetActive(false);
    }

    private void Update()
    {
        base.Update();
    }

    public override void ActivateComponent()
    {
        isActiveFire = true;

        // Enable fire and smoke particle effects
        if (fireParticles != null)
            fireParticles.SetActive(true);
        
        if (smokeParticles != null)
            smokeParticles.SetActive(true);

        Debug.Log($"{name} — Feu de camp allumé");
    }

    public override void DeactivateComponent()
    {
        isActiveFire = false;

        // Disable fire and smoke particle effects
        if (fireParticles != null)
            fireParticles.SetActive(false);
        
        if (smokeParticles != null)
            smokeParticles.SetActive(false);


        Debug.Log($"{name} — Feu de camp éteint");
    }
}