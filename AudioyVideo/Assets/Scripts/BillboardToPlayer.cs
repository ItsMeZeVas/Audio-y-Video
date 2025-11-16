using UnityEngine;

public class BillboardToPlayer : MonoBehaviour
{
    public Transform player;
    public string playerTag = "Player";

    void OnEnable()
    {
        // Cuando el objeto se activa, vuelve a buscar al jugador
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }

        if (player == null)
            Debug.LogWarning("BillboardToPlayer: No se encontró jugador con tag " + playerTag);
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Dirección hacia el jugador
        Vector3 dir = player.position - transform.position;

        // Evita errores cuando la distancia es muy pequeña
        if (dir.sqrMagnitude < 0.0001f) return;

        // Rotación hacia el jugador
        Quaternion targetRot = Quaternion.LookRotation(dir);

        // Muchos canvases/planes miran desde atrás → corregimos 180° en Y
        targetRot *= Quaternion.Euler(0f, 180f, 0f);

        transform.rotation = targetRot;
    }
}
