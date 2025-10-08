using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    [Header("Head Bob (movimiento de cámara)")]
    public float bobSpeed = 6f;      // Frecuencia del movimiento
    public float bobAmount = 0.05f;  // Amplitud del movimiento

    private Rigidbody rb;
    private Camera playerCamera;
    private float rotationX = 0f;
    private float defaultCameraY;
    private float bobTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evita volcar el cuerpo
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultCameraY = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        // --- Rotación con el ratón ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void FixedUpdate()
    {
        // --- Movimiento con WASD ---
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = (transform.forward * moveZ + transform.right * moveX).normalized;
        Vector3 targetVelocity = moveDir * moveSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // --- Head Bob ---
        HandleHeadBob(moveDir);
    }

    void HandleHeadBob(Vector3 moveDir)
    {
        if (moveDir.magnitude > 0.1f && rb.velocity.magnitude > 0.5f)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            float newY = defaultCameraY + Mathf.Sin(bobTimer) * bobAmount;
            playerCamera.transform.localPosition = new Vector3(0, newY, 0);
        }
        else
        {
            bobTimer = 0;
            Vector3 currentPos = playerCamera.transform.localPosition;
            playerCamera.transform.localPosition = Vector3.Lerp(currentPos, new Vector3(0, defaultCameraY, 0), Time.deltaTime * 4f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colisión con: " + collision.gameObject.name);
    }
}