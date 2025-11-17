using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    [Header("Head Bob (movimiento de cámara)")]
    public float bobSpeed = 6f;
    public float bobAmount = 0.05f;

    [Header("Footstep Controller")]
    public FootstepController footstepController;

    private Rigidbody rb;
    private Camera playerCamera;
    private float rotationX = 0f;
    private float defaultCameraY;
    private float bobTimer = 0f;

    private float previousSin = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultCameraY = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        // Rotación del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDir = (transform.forward * moveZ + transform.right * moveX).normalized;
        Vector3 targetVelocity = moveDir * moveSpeed;

        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        HandleHeadBob(moveDir);
    }

    void HandleHeadBob(Vector3 moveDir)
    {
        float speed = rb.velocity.magnitude;
        bool isMoving = speed > 1f && moveDir.magnitude > 0.1f;

        // ---- HEAD BOB ----
        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobSpeed;

            float sinValue = Mathf.Sin(bobTimer);
            float newY = defaultCameraY + sinValue * bobAmount;

            // Cruce por cero hacia abajo → paso
            if (previousSin > 0f && sinValue <= 0f)
            {
                if (footstepController != null)
                    footstepController.PlayFootstep();
            }

            previousSin = sinValue;

            playerCamera.transform.localPosition =
                new Vector3(playerCamera.transform.localPosition.x, newY, playerCamera.transform.localPosition.z);
        }
        else
        {
            // ---- RESPIRACIÓN ----
            float breathingSpeed = 1f;
            float breathingAmount = 0.02f;

            bobTimer += Time.deltaTime * breathingSpeed;

            float sinValue = Mathf.Sin(bobTimer);
            float newY = defaultCameraY + sinValue * breathingAmount;

            previousSin = 0; // Se reinicia para no disparar pasos al retomar movimiento

            playerCamera.transform.localPosition =
                new Vector3(playerCamera.transform.localPosition.x, newY, playerCamera.transform.localPosition.z);
        }
    }
}
