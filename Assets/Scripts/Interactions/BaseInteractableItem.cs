using UnityEngine;
using TMPro;

[RequireComponent(typeof(Transform))]
public class BaseInteractableItem : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private float highlightDistance = 3f;
    [SerializeField] private string glowMaterialName = "Glowly";
    [SerializeField] private TMP_Text inactiveText;
    [SerializeField] private TMP_Text activeText;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 2f, 0);

    [Header("Toggle Settings")]
    [SerializeField] private bool isToggleable = true;
    private bool isActive;

    private MeshRenderer[] meshRenderers;
    private Material[] originalMaterials; // Stores the original materials
    private Material glowMaterial;
    private Transform player;
    private bool isGlowing;

    protected void Start()
    {
        // Get all MeshRenderers from an object and its children
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (meshRenderers.Length == 0)
        {
            Debug.LogError($"{name} : Aucun MeshRenderer trouvé !");
            return;
        }
        
        // Save the original materials to restore them later
        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }
        
        // Load the glow material
        glowMaterial = Resources.Load<Material>(glowMaterialName);
        if (glowMaterial == null)
        {
            Debug.LogError($"{name} : Impossible de trouver le material '{glowMaterialName}' !");
            return;
        }
        
        // Find the player in the scene with the tag "Player"
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj) player = playerObj.transform;
        
        // Hide UI texts at the start
        if (inactiveText != null) inactiveText.gameObject.SetActive(false);
        if (activeText != null) activeText.gameObject.SetActive(false);
    }

    protected void Update()
    {
        if (!player || glowMaterial == null) return;
        
        // Calculate the distance between player and an object
        float distance = Vector3.Distance(player.position, transform.position);
        bool shouldGlow = distance <= highlightDistance;
        
        // Set the material glow based on the distance
        if (shouldGlow != isGlowing)
        {
            SetGlow(shouldGlow);
            isGlowing = shouldGlow;
        }
        
        // Manage the UI text based on the distance
        HandleTextDisplay(distance <= highlightDistance);
    }
    
    private void HandleTextDisplay(bool isClose)
    {
        // Hide all texts if the player is too far
        if (!isClose)
        {
            if (inactiveText != null) inactiveText.gameObject.SetActive(false);
            if (activeText != null) activeText.gameObject.SetActive(false);
            return;
        }
        
        // Show the correct text based on the state
        if (isActive)
        {
            if (inactiveText != null) inactiveText.gameObject.SetActive(false);
            if (activeText != null)
            {
                activeText.gameObject.SetActive(true);
                activeText.transform.position = Camera.main.WorldToScreenPoint(transform.position + textOffset);
            }
        }
        else
        {
            if (activeText != null) activeText.gameObject.SetActive(false);
            if (inactiveText != null)
            {
                inactiveText.gameObject.SetActive(true);
                inactiveText.transform.position = Camera.main.WorldToScreenPoint(transform.position + textOffset);
            }
        }
    }
    
    // Set the material glow on all renderers
    protected void SetGlow(bool state)
    {
        if (meshRenderers == null) return;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = state ? glowMaterial : originalMaterials[i];
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