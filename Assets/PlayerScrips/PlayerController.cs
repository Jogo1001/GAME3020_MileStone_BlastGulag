using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float PlayerMoveSpeed = 5f;
    public Rigidbody2D playerRB;
    public bool isPlayerMoving;

    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down;
    private Animator PlayerOneAnimator;

    [Header("Slash")]
    public SlashController slashController;

    [Header("Stun Settings")]
    public float stunDuration = 3f;
    public bool isStunned = false;

    [Header("Visual Feedback")]
    public SpriteRenderer spriteRenderer; 
    public Color flashColor = Color.red;  
    public float flashInterval = 0.2f;


    //new
    [Header("Flag Holding")]
    public bool isHoldingFlag = false;
    public Transform flagHoldPoint; 
    private Flag heldFlag;
    private float originalMoveSpeed;

    public Transform ResetFlagPosition;

    public int playerIndex = 1;

    private void Awake()
    {
        PlayerOneAnimator = GetComponent<Animator>();
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;

      
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originalMoveSpeed = PlayerMoveSpeed; //new
    }

    void Update()
    {
      
        if (isStunned)
        {
          
            movement = Vector2.zero;
        }
        else
        {
      
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            if (movement.y != 0) movement.x = 0;

            if (movement != Vector2.zero)
            {
                lastDirection = movement.normalized;
            }
        }

     
        if (movement != Vector2.zero)
        {
            PlayerOneAnimator.SetFloat("MoveX", movement.x);
            PlayerOneAnimator.SetFloat("MoveY", movement.y);
            isPlayerMoving = true;
            ResetIdle();
        }
        else
        {
            isPlayerMoving = false;

            if (PlayerOneAnimator.GetFloat("MoveX") < 0)
                PlayerOneAnimator.SetBool("LeftIdle", true);
            else if (PlayerOneAnimator.GetFloat("MoveX") > 0)
                PlayerOneAnimator.SetBool("RightIdle", true);
            else if (PlayerOneAnimator.GetFloat("MoveY") > 0)
                PlayerOneAnimator.SetBool("UpIdle", true);
            else if (PlayerOneAnimator.GetFloat("MoveY") < 0)
                PlayerOneAnimator.SetBool("DownIdle", true);
        }

        //new
        if (isHoldingFlag && Input.GetKeyDown(KeyCode.F))
        {
            DropFlag();
        }

        if (!isStunned && !isHoldingFlag && Input.GetKeyDown(KeyCode.E))
        {
            if (slashController != null)
                slashController.PerformSlash(lastDirection);
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            playerRB.MovePosition(playerRB.position + movement * PlayerMoveSpeed * Time.fixedDeltaTime);
        }
    }

    public void ResetIdle()
    {
        PlayerOneAnimator.SetBool("LeftIdle", false);
        PlayerOneAnimator.SetBool("RightIdle", false);
        PlayerOneAnimator.SetBool("UpIdle", false);
        PlayerOneAnimator.SetBool("DownIdle", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
          
            // Find who spawned this explosion
            Explosion explosion = other.GetComponent<Explosion>();
            if (explosion != null && explosion.owner != null)
            {
                // Pass the owner GameObject to DeathSequence
                DeathSequence(explosion.owner.gameObject);
            }
            else
            {
                // no owner found (e.g. environment explosion)
                DeathSequence(null);
            }
        }
    }

    public void DeathSequence(GameObject killerObject)
    {
        Debug.Log("Player " + playerIndex + " died");

        if (killerObject != null)
        {
            // check if killer is PlayerController
            PlayerController bombOwner1 = killerObject.GetComponentInParent<PlayerController>();
            if (bombOwner1 != null)
            {
                if (bombOwner1.playerIndex != playerIndex)
                {
                    GameManager.Instance.AddKill(bombOwner1.playerIndex);
                }
            }

            // check if killer is PlayerTwoController (friendly fire / self kill)
            PlayerTwoController bombOwner2 = killerObject.GetComponentInParent<PlayerTwoController>();
            if (bombOwner2 != null)
            {
                if (bombOwner2.playerIndex != playerIndex)
                {
                    GameManager.Instance.AddKill(bombOwner2.playerIndex);
                }
            }
        }

        // apply stun after death
        StartCoroutine(StunPlayer());
    }

    private IEnumerator StunPlayer()
    {
        if (isStunned) yield break;
        isStunned = true;
        ResetFlag();


        float timer = 0f;
        Color originalColor = spriteRenderer.color;
        bool toggle = false;

        while (timer < stunDuration)
        {
            spriteRenderer.color = toggle ? flashColor : originalColor;
            toggle = !toggle;
            timer += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.color = originalColor; 
        isStunned = false;
    }

    //new
    public void PickUpFlag(Flag flag)
    {
        heldFlag = flag;
        isHoldingFlag = true;

        // attach flag
        flag.AttachToPlayer(flagHoldPoint != null ? flagHoldPoint : transform);

        // reduce speed
        originalMoveSpeed = PlayerMoveSpeed;
        PlayerMoveSpeed *= 0.5f; // half speed
    }

    public void DropFlag()
    {
        if (heldFlag != null)
        {
            heldFlag.Drop(transform.position);
            heldFlag = null;
        }

        isHoldingFlag = false;
        PlayerMoveSpeed = originalMoveSpeed;
    }

    public void ResetFlag()
    {
        if (!isStunned) return;

        if (isHoldingFlag && heldFlag != null)
        {

            Vector3 resetPos = ResetFlagPosition != null ? ResetFlagPosition.position : transform.position;
            heldFlag.Drop(resetPos);


            heldFlag = null;
            isHoldingFlag = false;


            PlayerMoveSpeed = originalMoveSpeed;


        }

    }
}
