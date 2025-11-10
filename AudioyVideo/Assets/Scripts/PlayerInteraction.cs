using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;          // cámara del jugador
    public float interactDistance = 3f;  // distancia máxima del raycast
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactableLayer;  // capa opcional para filtrar objetos interactuables

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
            {
                // ¿Tiene el objeto un InteractableObject?
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    Debug.Log($"Interactuando con: {hit.collider.name}");
                    interactable.Interact();
                }
            }
        }
    }
}
