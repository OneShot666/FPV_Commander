using UnityEngine;

public class PillarHandler : BaseInteractableItem
{
    [Header("Pillar Settings")]
    [SerializeField] private Transform pillarBody;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private GameObject healingZone;
    [SerializeField] private Light[] pointLights;
    [SerializeField] private float activeIntensity = 8f;
    [SerializeField] private float inactiveIntensity = 2f;
    [SerializeField] private float lightLerpSpeed = 2f;
    
    private bool isRotating;
    
    void Start()
    {
        base.Start();

        if (healingZone == null)
        {
            Transform zone = transform.Find("HealingZone");
            if (zone != null)
            {
                healingZone = zone.gameObject;
            }

            if (healingZone != null)
            {
                healingZone.SetActive(false);
            }
        }
        
        if (pointLights != null && pointLights.Length > 0)
        {
            foreach (var lt in pointLights)
            {
                if (lt != null) lt.intensity = inactiveIntensity;
            }
        }
    }
    
    void Update()
    {
        base.Update();
        if (isRotating && pillarBody != null)
        {
            pillarBody.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
        if (pointLights != null && pointLights.Length > 0)
        {
            float target = isRotating ? activeIntensity : inactiveIntensity;
            for (int i = 0; i < pointLights.Length; i++)
            {
                Light lt = pointLights[i];
                if (lt == null) continue;
                lt.intensity = Mathf.Lerp(lt.intensity, target, Time.deltaTime * lightLerpSpeed);
            }
        }
    }
    
    protected override void ActivateComponent()
    {
        isRotating = true;
        if (healingZone != null)
            healingZone.SetActive(true);
        if (pointLights != null && pointLights.Length > 0)
        {
            foreach (var lt in pointLights)
            {
                if (lt != null) lt.intensity = activeIntensity;
            }
        }

        Debug.Log($"{name} — Pillier activé");
    }
    
    protected override void DeactivateComponent()
    {
        isRotating = false;
        if (healingZone != null)
            healingZone.SetActive(false);
        if (pointLights != null && pointLights.Length > 0)
        {
            foreach (var lt in pointLights)
            {
                if (lt != null) lt.intensity = inactiveIntensity;
            }
        }

        Debug.Log($"{name} — Pillier désactivé");
    }
}
