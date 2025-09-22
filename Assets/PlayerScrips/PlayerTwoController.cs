using UnityEngine;
using System.Collections;

public class PlayerTwoController : MonoBehaviour
{
    [Header("Movement")]
    public float PlayerMoveSpeed = 5f;
    public bool IsPlayerMoving;
    public Rigidbody2D PlayerRB;

    private Vector2 Movement;
    private Vector2 lastDirection = Vector2.down;
    private Animator PlayerTwoAnimation;

    [Header("Slash")]
    public SlashController slashController;

    [Header("Stun Settings")]
    public float stunDuration = 3f;        
    public Color flashColor = Color.red;   
    public float flashInterval = 0.2f;   

    public bool isStunned = false;
    public SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Flag Holding")]
    public bool isHoldingFlag = false;
    public Transform flagHoldPoint;
    private Flag heldFlag;
    private float originalMoveSpeed;

    [Header("Reset Flag Position")]
    public Transform ResetFlagPosition;

    [Header("Reset Player Position")]
    public Transform ResetPlayerPosition;

    [Header("Slash Layer")]
    [SerializeField] public LayerMask slashLayer;

    public int playerIndex = 2; // kill tracking

    private void Awake()
    {
        PlayerTwoAnimation = GetComponent<Animator>();
        PlayerRB.constraints = RigidbodyConstraints2D.FreezeRotation;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
      
        if (isStunned)
        {
            Movement = Vector2.zero;
        }
        else
        {
            Movement.x = Input.GetAxisRaw("Horizontal2");
            Movement.y = Input.GetAxisRaw("Vertical2");

          
            if (Movement.y != 0) Movement.x = 0;

            if (Movement != Vector2.zero)
                lastDirection = Movement.normalized;

           
   
        }

        // Animations
        if (Movement != Vector2.zero)
        {
            PlayerTwoAnimation.SetFloat("MoveX", Movement.x);
            PlayerTwoAnimation.SetFloat("MoveY", Movement.y);
            IsPlayerMoving = true;
            ResetIdle();
        }
        else
        {
            IsPlayerMoving = false;

            // idle direction depending on last movement
            if (PlayerTwoAnimation.GetFloat("MoveX") < 0)
                PlayerTwoAnimation.SetBool("LeftIdle", true);
            else if (PlayerTwoAnimation.GetFloat("MoveX") > 0)
                PlayerTwoAnimation.SetBool("RightIdle", true);
            else if (PlayerTwoAnimation.GetFloat("MoveY") > 0)
                PlayerTwoAnimation.SetBool("UpIdle", true);
            else if (PlayerTwoAnimation.GetFloat("MoveY") < 0)
                PlayerTwoAnimation.SetBool("DownIdle", true);
        }
        if (isHoldingFlag && Input.GetKeyDown(KeyCode.Keypad0))
        {
            DropFlag();
        }
        if (!isStunned && !isHoldingFlag && Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (slashController != null)
                slashController.PerformSlash(lastDirection);
        }
       
    }

    void FixedUpdate()
    {
        if (Movement != Vector2.zero)
        {
            PlayerRB.MovePosition(PlayerRB.position + Movement * PlayerMoveSpeed * Time.fixedDeltaTime);
        }
    }

    public void ResetIdle()
    {
        PlayerTwoAnimation.SetBool("LeftIdle", false);
        PlayerTwoAnimation.SetBool("RightIdle", false);
        PlayerTwoAnimation.SetBool("UpIdle", false);
        PlayerTwoAnimation.SetBool("DownIdle", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if ((slashLayer.value & (1 << other.gameObject.layer)) > 0)
        {
        

            // Stun player for 1 second
            StartCoroutine(StunPlayer(0.1f));
            ResetFlag();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            Explosion explosion = other.GetComponent<Explosion>();
            if (explosion != null && explosion.owner != null)
            {
                DeathSequence(explosion.owner.gameObject);
                PlayerRB.position = ResetPlayerPosition.position; 
                transform.position = ResetPlayerPosition.position;
            }
            else
            {
                DeathSequence(null);
            }

        }
 
    }


    public void DeathSequence(GameObject killerObject)
    {


        // Try PlayerController first
        PlayerController bombOwner = killerObject.GetComponentInParent<PlayerController>();
        if (bombOwner != null)
        {
            if (bombOwner.playerIndex != playerIndex)
            {
                GameManager.Instance.AddKill(bombOwner.playerIndex);
            }
            // self-kill optional: skip adding kills
        }
        else
        {
            // Try PlayerTwoController as owner too
            PlayerTwoController bombOwner2 = killerObject.GetComponentInParent<PlayerTwoController>();
            if (bombOwner2 != null && bombOwner2.playerIndex != playerIndex)
            {
                GameManager.Instance.AddKill(bombOwner2.playerIndex);
            }
        }

        StartCoroutine(StunPlayer(3f));
    }
    private IEnumerator StunPlayer(float duration)
    {
        if (isStunned) yield break;
        isStunned = true;
        ResetFlag();

        // Flash sprite
        float elapsed = 0f;
        while (elapsed < stunDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = flashColor;
                yield return new WaitForSeconds(flashInterval);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashInterval);
            }
            elapsed += flashInterval * 2f;
        }

        // Restore color and unstun
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        isStunned = false;
    }
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
