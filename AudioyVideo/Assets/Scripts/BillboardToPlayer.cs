using UnityEngine;

public class BillboardToPlayer : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player == null) return;

        // Mira al jugador
        transform.LookAt(player.position);

        // Corrige que muchos quads miran hacia atrás
        transform.Rotate(0, 180f, 0);
    }
}
