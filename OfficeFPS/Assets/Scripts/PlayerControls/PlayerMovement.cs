using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public Camera playerCamera;
    private Rigidbody rb;
    public Transform box;
    public MouseMovement mouseMovement;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float moveSpeedJump = 15f;

    [Header("Jump Settings")]
    public float groundDistance = 0.1f;
    public float jumpForce = 2f;

    [Header("Dodge Settings")]
    public float dodgeForce = 5f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 3f;
    [SerializeField] private AnimationCurve dodgeCurve = AnimationCurve.EaseInOut(1, 1, 1, 0.5f); 

    [Header("Movement flags")]
    public bool isGrounded;
    public bool isDodging = false;
    // Input axes
    public float moveX;
    public float moveZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        GroundCheck();
        GetInput();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDodging)
        {
            Dodge();
        }

    }

    void FixedUpdate()
    {
        if (isGrounded && !isDodging)
        {
            Move();
        }
    }

    #region Functionality
    private void GetInput()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
    }

    private void GroundCheck()
    {
        bool grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!isGrounded && grounded)
            mouseMovement.StartBounce();

        isGrounded = grounded;

        // Debug visualization
        Color sphereColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(groundCheck.position, Vector3.down * groundDistance, sphereColor);
    }

    private void Move()
    {
        // Movement relative to player orientation
        Vector3 move = playerCamera.transform.right * moveX + playerCamera.transform.forward * moveZ;

        Vector3 targetVelocity = move * moveSpeed;
        Vector3 velocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        rb.linearVelocity = velocity;
    }


    private void Jump()
    {
        Vector3 move = playerCamera.transform.right * moveX + playerCamera.transform.forward * moveZ;
        Vector3 targetVelocity = move * moveSpeedJump;
        Vector3 velocity = new Vector3(targetVelocity.x, jumpForce, targetVelocity.z);
        rb.linearVelocity = velocity;
    }
    #endregion

    #region Dodge
    private Coroutine dodgeCoroutine;

    private void Dodge()
    {
        Debug.Log($"Attempting dodge | isDodging: {isDodging}");

        if (!isDodging && dodgeCoroutine == null)
            dodgeCoroutine = StartCoroutine(DodgeRoutine());
    }

    private IEnumerator DodgeRoutine()
    {
        isDodging = true;
        Debug.Log($"Dodge started | isDodging: {isDodging}");

        Vector3 moveDir = (playerCamera.transform.right * moveX + playerCamera.transform.forward * moveZ).normalized;
        if (moveDir.sqrMagnitude < 0.1f)
            moveDir = playerCamera.transform.forward;

        moveDir.y = 0.05f; // slight vertical lift if needed

        float elapsed = 0f;

        while (elapsed < dodgeDuration)
        {
            float t = elapsed / dodgeDuration;
            float curveMultiplier = dodgeCurve.Evaluate(t); // value from curve
            rb.linearVelocity = Vector3.zero; // reset momentum each frame for consistency
            rb.AddForce(moveDir * dodgeForce * curveMultiplier, ForceMode.VelocityChange);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDodging = false;

        yield return new WaitForSeconds(dodgeCooldown);
        dodgeCoroutine = null;
        Debug.Log($"Dodge cooldown ended");
    }
    #endregion
}
