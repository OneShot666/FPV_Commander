using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BaseInteractableItem : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private float highlightDistance = 3f;
    [SerializeField] private string glowMaterialName = "Glowly";
    
    [Header("Toggle Settings")]
    [SerializeField] private bool isToggleable = true;
    private bool isActive;

    protected MeshRenderer meshRenderer;
    protected Material glowMaterial;
    private Material originalMaterial;
    private Transform player;
    private bool isGlowing;

    protected void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError($"{name} : MeshRenderer manquant !");
            return;
        }

        originalMaterial = meshRenderer.material;

        glowMaterial = Resources.Load<Material>(glowMaterialName);
        if (glowMaterial == null)
        {
            Debug.LogError($"{name} : Impossible de trouver le material '{glowMaterialName}' !");
            return;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj) player = playerObj.transform;
    }

    protected void Update()
    {
        if (!player || glowMaterial == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool shouldGlow = distance <= highlightDistance;
        
        if (shouldGlow != isGlowing)
        {
            SetGlow(shouldGlow);
            isGlowing = shouldGlow;
        }
    }

    protected void SetGlow(bool state)
    {
        if (meshRenderer == null) return;

        if (state)
        {
            meshRenderer.material = glowMaterial;
        }
        else
        {
            meshRenderer.material = originalMaterial;
        }
    }
    
    public void Switch()
    {
        if (isToggleable)
        {
            if (isActive) DeactivateComponent();
            else ActivateComponent();

            isActive = !isActive;
        }
        else
        {
            ActivateComponent();
        }
    }
    
    protected virtual void ActivateComponent()
    {
        Debug.Log($"{name} activé !");
    }

    protected virtual void DeactivateComponent()
    {
        Debug.Log($"{name} désactivé !");
    }
}