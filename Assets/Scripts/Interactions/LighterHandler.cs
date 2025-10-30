using UnityEngine;
public class LighterHandler : BaseInteractableItem
{
    protected override void ActivateComponent()
    {
        Debug.Log($"{name} — Briquet prêt");
    }

    protected override void DeactivateComponent()
    {
        Debug.Log($"{name} — Briquet rangé");
    }

    public void LightCampfire(CampfireHandler campfire)
    {
        campfire.GetType()
            .GetMethod("ActivateComponent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(campfire, null);
    }
}