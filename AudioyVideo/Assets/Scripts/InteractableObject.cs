using UnityEngine;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    [System.Serializable]
    public class InteractionKey
    {
        public string actionName;   // Nombre de la acción (ej. "Leer", "Abrir", "EncenderTV")
        public KeyCode key;         // Tecla asociada
    }

    [Header("Configuración de interacción")]
    public InteractionManager interactionManager;
    public List<InteractionKey> interactionKeys = new List<InteractionKey>();

    /// <summary>
    /// Ejecuta la acción asociada al nombre.
    /// </summary>
    public void Interact(string actionName)
    {
        if (interactionManager != null)
        {
            Debug.Log($"🟢 Objeto {gameObject.name} → acción: {actionName}");
            interactionManager.TriggerEvent(gameObject, actionName);
        }
        else
        {
            Debug.LogError($"❌ No se ha asignado el InteractionManager en {gameObject.name}");
        }
    }

    /// <summary>
    /// Comprueba si la tecla está registrada y devuelve el nombre de la acción.
    /// </summary>
    public bool HasKey(KeyCode key, out string actionName)
    {
        foreach (var interaction in interactionKeys)
        {
            if (interaction.key == key)
            {
                actionName = interaction.actionName;
                return true;
            }
        }
        actionName = null;
        return false;
    }
}
