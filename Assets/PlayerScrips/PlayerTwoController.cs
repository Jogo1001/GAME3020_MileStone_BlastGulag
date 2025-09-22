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
    public float flashInterval = 0.1f;   

    public bool isStunned = false;
    public SpriteRenderer spriteRenderer;
    private Color originalColor;

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

           
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                if (slashController != null)
                    slashController.PerformSlash(lastDirection);
            }
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            Explosion explosion = other.GetComponent<Explosion>();
            if (explosion != null && explosion.owner != null)
            {
                DeathSequence(explosion.owner.gameObject);
            }
            else
            {
                DeathSequence(null);
            }
        }
    }


    public void DeathSequence(GameObject killerObject)
    {
        if (killerObject == null)
        {
            StartCoroutine(StunPlayer());
            return;
        }

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

        StartCoroutine(StunPlayer());
    }
    private IEnumerator StunPlayer()
    {
        if (isStunned) yield break;
        isStunned = true;

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
}
