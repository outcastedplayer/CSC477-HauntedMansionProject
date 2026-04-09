using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 3f;
    public LayerMask interactableLayer = ~0;
    public KeyCode interactKey = KeyCode.E;

    [Header("Debug")]
    public bool showDebug = true;

    public InteractionState CurrentState { get; private set; } = InteractionState.Ready;

    IInteractable currentTarget;
    Camera playerCamera;

    void Awake()
    {
        playerCamera = GetComponent<Camera>();
        if (!playerCamera) playerCamera = GetComponentInChildren<Camera>();
        if (!playerCamera) playerCamera = GetComponentInParent<Camera>();
        if (!playerCamera) playerCamera = Camera.main;
    }

    void Start()
    {
        if (showDebug)
        {
            if (playerCamera)
                Debug.Log("[InteractionSystem] Ready on camera: " + playerCamera.name);
            else
                Debug.LogError("[InteractionSystem] NO CAMERA FOUND — raycast won't work!");

            Debug.Log("[InteractionSystem] Interact key: " + interactKey);
            Debug.Log("[InteractionSystem] Range: " + interactRange);
            Debug.Log("[InteractionSystem] State: " + CurrentState);
        }
    }

    void Update()
    {
        if (CurrentState == InteractionState.Disabled) return;
        if (!playerCamera) return;

        // Check input FIRST, before raycast clears the target
        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            if (showDebug) Debug.Log("[InteractionSystem] E pressed — target: " + currentTarget.GetPromptText());
            CurrentState = InteractionState.Interacting;
            currentTarget.Interact();
            CurrentState = InteractionState.Hovering;
            return;
        }

        if (Input.GetKeyDown(interactKey) && showDebug)
            Debug.Log("[InteractionSystem] E pressed — target: NONE");

        PerformRaycast();
    }

    void PerformRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        // Draw the ray in Scene view (green = hitting something, red = hitting nothing)
        if (showDebug)
            Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            if (showDebug)
                Debug.DrawLine(ray.origin, hit.point, Color.green);

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null)
                interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null && interactable.CanInteract())
            {
                if (currentTarget == null && showDebug)
                    Debug.Log("[InteractionSystem] Found: " + interactable.GetPromptText() + " on " + hit.collider.gameObject.name);

                currentTarget = interactable;
                CurrentState = InteractionState.Hovering;
                return;
            }
            else if (showDebug && interactable != null)
            {
                // Has IInteractable but CanInteract returned false
                Debug.DrawLine(ray.origin, hit.point, Color.blue);
            }
        }

        if (currentTarget != null)
        {
            if (showDebug) Debug.Log("[InteractionSystem] Lost target");
            currentTarget = null;
            CurrentState = InteractionState.Ready;
        }
    }

    public void SetEnabled(bool enabled)
    {
        if (enabled)
        {
            CurrentState = InteractionState.Ready;
        }
        else
        {
            CurrentState = InteractionState.Disabled;
            currentTarget = null;
        }

        if (showDebug) Debug.Log("[InteractionSystem] " + (enabled ? "ENABLED" : "DISABLED"));
    }

    public IInteractable GetCurrentTarget() => currentTarget;
}

