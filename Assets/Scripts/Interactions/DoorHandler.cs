using UnityEngine;

public class DoorHandler : BaseInteractableItem
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorBody;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float rotationSpeed = 3f;

    private Vector3 closedEuler;
    private Vector3 targetEuler;
    private bool isAnimating;

    private void Awake()
    {
        if (doorBody == null && transform.childCount > 0)
            doorBody = transform.GetChild(0);

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
        if (isAnimating && doorBody != null)
        {
            doorBody.localEulerAngles = Vector3.Lerp(
                doorBody.localEulerAngles,
                targetEuler,
                Time.deltaTime * rotationSpeed
            );

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