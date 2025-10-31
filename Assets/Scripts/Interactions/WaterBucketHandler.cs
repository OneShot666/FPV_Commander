using UnityEngine;

public class WaterBucketHandler : BaseInteractableItem
{
    [Header("Water Settings")]
    //[SerializeField] private int waterAmount = 1;

    private bool isCarried;

    public override void ActivateComponent()
    {
        isCarried = true;
        //Debug.Log($"{name} — Sceau ramassé");
    }

    public override void DeactivateComponent()
    {
        isCarried = false;
        Debug.Log($"{name} — Sceau utilisé ou reposé");
    }
    
    public void PourWaterOnCampfire(CampfireHandler campfire)
    {
        if (!isCarried) return;
        
        campfire.GetType()
            .GetMethod("DeactivateComponent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(campfire, null);
        
        DeactivateComponent();
    }
}