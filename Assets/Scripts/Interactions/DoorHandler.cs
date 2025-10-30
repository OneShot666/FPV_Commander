using UnityEngine;

public class DoorHandler : BaseInteractableItem
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorBody;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float rotationSpeed = 3f;

    private Vector3 closedEuler; // Original rotation of the door
    private Vector3 targetEuler; // The rotation the door will have
    private bool isAnimating;

    private void Awake()
    {
        if (doorBody == null && transform.childCount > 0)
            doorBody = transform.GetChild(0);
        
        // Store the original rotation of the door 
        if (doorBody != null)
            closedEuler = doorBody.localEulerAngles;
    }

    private void Start()
    {
        base.Start();
    }

    private void Update()
    {
        base.Update();
        
        // Make the door rotate until its reach the wanted rotation
        if (isAnimating && doorBody != null)
        {
            doorBody.localEulerAngles = Vector3.Lerp(
                doorBody.localEulerAngles,
                targetEuler,
                Time.deltaTime * rotationSpeed
            );
            
            // Stop the animation when the door as the wanted rotation
            if (Vector3.Distance(doorBody.localEulerAngles, targetEuler) < 0.1f)
                isAnimating = false;
        }
    }

    protected override void ActivateComponent()
    {
        targetEuler = closedEuler + new Vector3(0f, openAngle, 0f);
        isAnimating = true;

        Debug.Log($"{name} — Porte ouverte");
    }

    protected override void DeactivateComponent()
    {
        targetEuler = closedEuler;
        isAnimating = true;

        Debug.Log($"{name} — Porte fermée");
    }
}