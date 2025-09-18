using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float playerMoveSpeed = 5f;
    public Rigidbody2D playerRB;
    public bool isPlayerMoving;

    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down; // default facing down
    private Animator georgeAnimator;

    [Header("Slash")]
    public SlashController slashController; // reference to the slash controller script

    private void Awake()
    {
        georgeAnimator = GetComponent<Animator>();
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // --- Movement Input ---
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.y != 0) movement.x = 0;

        if (movement != Vector2.zero)
        {
            lastDirection = movement.normalized;
        }

        // --- Animation ---
        if (movement != Vector2.zero)
        {
            georgeAnimator.SetFloat("MoveX", movement.x);
            georgeAnimator.SetFloat("MoveY", movement.y);
            isPlayerMoving = true;
            ResetIdle();
        }
        else
        {
            isPlayerMoving = false;

            if (georgeAnimator.GetFloat("MoveX") < 0)
                georgeAnimator.SetBool("LeftIdle", true);
            else if (georgeAnimator.GetFloat("MoveX") > 0)
                georgeAnimator.SetBool("RightIdle", true);
            else if (georgeAnimator.GetFloat("MoveY") > 0)
                georgeAnimator.SetBool("UpIdle", true);
            else if (georgeAnimator.GetFloat("MoveY") < 0)
                georgeAnimator.SetBool("DownIdle", true);
        }

        // --- Attack Input ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (slashController != null)
                slashController.PerformSlash(lastDirection);
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            playerRB.MovePosition(playerRB.position + movement * playerMoveSpeed * Time.fixedDeltaTime);
        }
    }

    public void ResetIdle()
    {
        georgeAnimator.SetBool("LeftIdle", false);
        georgeAnimator.SetBool("RightIdle", false);
        georgeAnimator.SetBool("UpIdle", false);
        georgeAnimator.SetBool("DownIdle", false);
    }
}
