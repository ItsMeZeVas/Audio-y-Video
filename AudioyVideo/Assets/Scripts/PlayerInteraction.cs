using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración de interacción")]
    public Camera playerCamera;          // Cámara del jugador
    public float interactDistance = 3f;  // Distancia máxima del raycast
    public LayerMask interactableLayer;  // Capa de objetos interactuables
    public KeyCode interactKey = KeyCode.E; // Tecla de interacción genérica

    void Update()
    {

        // Escanea todos los objetos frente al jugador en la capa interactuable
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                // Recorre todas las teclas definidas en el objeto
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

    }
}
