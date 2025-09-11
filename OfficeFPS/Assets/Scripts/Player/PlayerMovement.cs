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
    public HealthShieldSystem healthShieldSystem;

    public HudManager hudManager;
    public PlayerSounds playerSounds;

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
    public bool isJumping = false;
    // Input axes
    public float moveX;
    public float moveZ;
    public string lastGroundTag; // stores last touched ground tag

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isDodging = false;
        isJumping = false;
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
        if (isGrounded && !isDodging && !isJumping)
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
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);
        bool grounded = colliders.Length > 0;

        if (!isGrounded && grounded)
        {
            lastGroundTag = colliders[0].gameObject.tag;

            // Debug.Log($"[GroundCheck] Landed on object: {colliders[0].gameObject.name}, tag: {lastGroundTag}");

            mouseMovement.StartBounce();
            isJumping = false;
        }

        isGrounded = grounded;

        Color sphereColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(groundCheck.position, Vector3.down * groundDistance, sphereColor);
    }

    private void Move()
    {
        // Movement relative to player orientation
        Vector3 move = this.transform.right * moveX + this.transform.forward * moveZ;

        // Normalize to avoid diagonal speed boost
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        Vector3 targetVelocity = move * moveSpeed;
        Vector3 velocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        rb.linearVelocity = velocity;
    }


    // private void Jump()
    // {

    //     //This is a shitty fix for jumping while running into a wall
    //     // float jumpForceTarget = 0f;
    //     // Debug.Log($"MoveZ: {moveZ}, WallCheck: {CheckForWall()}, rb. linearVelocity: {rb.linearVelocity}");
    //     // if (CheckForWall() && moveZ > 0 && rb.linearVelocity == Vector3.zero)
    //     // {
    //     //     Debug.Log("Fix Wall jump detected");
    //     //     jumpForceTarget = wallCheckJumpForce;
    //     // }else
    //     // {
    //     //     jumpForceTarget = jumpForce;
    //     // }

    //     Vector3 move = this.transform.right * moveX + this.transform.forward * moveZ;
    //     Vector3 targetVelocity = move * moveSpeedJump;
    //     Vector3 velocity = new Vector3(targetVelocity.x, jumpForce, targetVelocity.z);
    //     rb.linearVelocity = velocity;
    // }

    private void Jump()
    {
        if (!isGrounded) return;
        isJumping = true;

        if (CheckForWall() && moveZ > 0)
        {
            StopCompletely();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jump against wall");
            return;
        }

        Vector3 move = this.transform.right * moveX + this.transform.forward * moveZ;
        Vector3 targetVelocity = move * moveSpeedJump;
        Vector3 velocity = new Vector3(targetVelocity.x, jumpForce, targetVelocity.z);
        rb.linearVelocity = velocity;
    }

    public void StopCompletely()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // also clear input so Move() doesn’t immediately re-apply motion
        moveX = 0f;
        moveZ = 0f;
    }


    private bool CheckForWall()
    {
        float checkDistance = 0.35f;
        Vector3 halfExtents = new Vector3(0.3f, 0.5f, 0.3f); // half of (0.6, 1, 0.6)
        Vector3 origin = transform.position + Vector3.up * halfExtents.y; // center at player's chest
        Vector3 direction = transform.forward;

        if (Physics.BoxCast(origin, halfExtents, direction, out RaycastHit hit, transform.rotation, checkDistance))
        {
            Debug.Log($"[WallCheck] Box hit {hit.collider.name} at distance {hit.distance}");
            return true;
        }

        return false;
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
        healthShieldSystem.isInvincible = true;
        Debug.Log($"Dodge started | isDodging: {isDodging}");

        hudManager.ShowDodgeHud(dodgeDuration);
        hudManager.UseBoost();

        playerSounds.PlayDodgeSound();

        Vector3 moveDir = (this.transform.right * moveX + this.transform.forward * moveZ).normalized;
        if (moveDir.sqrMagnitude < 0.1f)
            moveDir = this.transform.forward;

        moveDir.y = 0.01f; // slight vertical lift if needed

        float elapsed = 0f;
        float baseFov = playerCamera.fieldOfView;
        float targetFov = baseFov + 15f; // expanded FOV

        while (elapsed < dodgeDuration)
        {
            float t = elapsed / dodgeDuration;

            // ---- Movement force ----
            float curveMultiplier = dodgeCurve.Evaluate(t);
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(moveDir * dodgeForce * curveMultiplier, ForceMode.VelocityChange);

            // ---- Camera FOV ----
            // Ease to 65 at midpoint, then back to 60
            float fovCurve = Mathf.Sin(t * Mathf.PI); // goes 0 → 1 → 0 over the dodge
            if(moveZ != 0) // only apply FOV change if moving forward/back
                playerCamera.fieldOfView = Mathf.Lerp(baseFov, targetFov, fovCurve);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Reset states
        playerCamera.fieldOfView = baseFov;
        healthShieldSystem.isInvincible = false;
        isDodging = false;
        hudManager.StartBoostFill(dodgeCooldown);

        yield return new WaitForSeconds(dodgeCooldown);
        dodgeCoroutine = null;
        Debug.Log($"Dodge cooldown ended");
    }

    #endregion
}
