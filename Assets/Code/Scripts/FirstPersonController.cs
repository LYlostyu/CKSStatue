using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public CharacterController controller;

    [Header("移動設定")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("跳躍/重力")]
    public float gravity = -9.81f;          // 建議先回到 -9.81 再調
    public float jumpHeight = 2f;
    public float fallMultiplier = 2.0f;     // 下墜加速倍率（1.5~2.5 常見）
    public float groundStickVelocity = -2f; // 貼地用的小下壓

    [Header("視角設定")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("狀態偵測（可選）")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    float verticalVelocity;
    bool isGrounded;
    Camera cam;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }

    void Update()
    {
        // 1) Grounded：你可以二選一
        // A: 用 CharacterController 自帶的（通常更省事）
        isGrounded = controller.isGrounded;

        // B: 你要堅持用 CheckSphere，也OK，但半徑建議小一點
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = groundStickVelocity;

        // 2) 視角
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (cam != null) cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 3) 水平移動
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 horizontal = (transform.right * x + transform.forward * z) * currentSpeed;

        // 4) 跳躍
        if (Input.GetButtonDown("Jump") && isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // 5) 重力（下墜更快）
        float g = gravity;
        if (verticalVelocity < 0f) g *= fallMultiplier;
        verticalVelocity += g * Time.deltaTime;

        // 6) 合併 Move 一次（更穩）
        Vector3 velocity = horizontal + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
