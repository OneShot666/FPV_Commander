using UnityEngine;
public class LighterHandler : BaseInteractableItem
{
    public override void ActivateComponent()
    {
        Debug.Log($"{name} — Briquet prêt");
    }

    public override void DeactivateComponent()
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