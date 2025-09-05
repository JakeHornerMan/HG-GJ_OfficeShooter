using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public Camera camera;
    private Rigidbody rb;
    public Transform box;
    public MouseMovement mouseMovement;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float groundDistance = 0.1f;
    public float jumpForce = 2f;

    [Header("Movement variables")]
    public bool isGrounded;
    // Input axes
    public float moveX;
    public float moveZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GroundCheck();
        GetInput();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        Move();
        Visuals();
    }

    #region Functionality
    private void GetInput()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
    }

    private void GroundCheck()
    {
        Ray ray = new Ray(groundCheck.position, Vector3.down);
        bool grounded = Physics.Raycast(ray, groundDistance, groundMask);
        
        if (!isGrounded && grounded)
            mouseMovement.StartBounce();

        isGrounded = grounded;

        // Debug visualization
        Color rayColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(groundCheck.position, Vector3.down * groundDistance, rayColor);

        Debug.Log("Is Grounded: " + isGrounded);
    }

    private void Move()
    {
        // Movement relative to player orientation
        Vector3 move = camera.transform.right * moveX + camera.transform.forward * moveZ;

        Vector3 targetVelocity = move * moveSpeed;
        Vector3 velocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        rb.linearVelocity = velocity;
    }


    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }
    #endregion

    #region Visuals
    private void Visuals()
    {
        
    }

    #endregion
}
