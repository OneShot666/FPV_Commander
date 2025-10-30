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
    private Material[] originalMaterials;
    private Material glowMaterial;
    private Transform player;
    private bool isGlowing;

    protected void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (meshRenderers.Length == 0)
        {
            Debug.LogError($"{name} : Aucun MeshRenderer trouvé !");
            return;
        }
        
        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }
        
        glowMaterial = Resources.Load<Material>(glowMaterialName);
        if (glowMaterial == null)
        {
            Debug.LogError($"{name} : Impossible de trouver le material '{glowMaterialName}' !");
            return;
        }
        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj) player = playerObj.transform;
        
        if (inactiveText != null) inactiveText.gameObject.SetActive(false);
        if (activeText != null) activeText.gameObject.SetActive(false);
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
        
        HandleTextDisplay(distance <= highlightDistance);
    }
    
    private void HandleTextDisplay(bool isClose)
    {
        if (!isClose)
        {
            if (inactiveText != null) inactiveText.gameObject.SetActive(false);
            if (activeText != null) activeText.gameObject.SetActive(false);
            return;
        }
        
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