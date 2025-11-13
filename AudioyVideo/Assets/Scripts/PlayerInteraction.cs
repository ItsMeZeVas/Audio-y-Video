using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración de interacción")]
    public Camera playerCamera;
    public float interactDistance = 10000f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;

    private WorldImageDisplay lastImageDisplayed; // <- NUEVO

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            // -------------------------
            // 1. Mostrar imagen del objeto
            // -------------------------
            WorldImageDisplay imageDisplay = hit.collider.GetComponent<WorldImageDisplay>();

            if (imageDisplay != null)
            {
                if (lastImageDisplayed != imageDisplay)
                {
                    if (lastImageDisplayed != null)
                        lastImageDisplayed.HideImage();

                    imageDisplay.ShowImage();
                    lastImageDisplayed = imageDisplay;
                }
            }
            else
            {
                if (lastImageDisplayed != null)
                {
                    lastImageDisplayed.HideImage();
                    lastImageDisplayed = null;
                }
            }

            // -------------------------
            // 2. Interacción con teclas
            // -------------------------
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                foreach (var keyConfig in interactable.interactionKeys)
                {
                    if (Input.GetKeyDown(keyConfig.key))
                    {
                        Debug.Log($"🎯 Interactuando con: {hit.collider.name} → Acción: {keyConfig.actionName}");
                        interactable.Interact(keyConfig.actionName);
                        break;
                    }
                }
            }
        }
        else
        {
            // Si dejo de mirar cualquier objeto con imagen
            if (lastImageDisplayed != null)
            {
                lastImageDisplayed.HideImage();
                lastImageDisplayed = null;
            }
        }
    }
}
