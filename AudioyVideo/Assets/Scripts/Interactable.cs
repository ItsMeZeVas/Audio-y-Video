using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectName = "Objeto";
    public bool isPlayerNearby = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log($"Estás cerca del {objectName}. Presiona E para interactuar.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public virtual void Interact()
    {
        Debug.Log($"Has interactuado con {objectName}");
        // Aquí más adelante dispararemos el evento o acción
    }
}
