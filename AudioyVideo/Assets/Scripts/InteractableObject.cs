using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public InteractionManager interactionManager;

    public void Interact()
    {
        if (interactionManager != null)
        {
            Debug.Log("Objeto interactuado: " + gameObject.name);
            interactionManager.TriggerEvent(gameObject);
        }
        else
        {
            Debug.LogError("No se ha asignado el InteractionManager en " + gameObject.name);
        }
    }
}
