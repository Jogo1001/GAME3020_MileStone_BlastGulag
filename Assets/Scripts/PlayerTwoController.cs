using UnityEngine;

public class PlayerTwoController : MonoBehaviour
{

    public float PlayerMoveSpeed = 5f;
    public bool IsPlayerMoving;

    private Vector2 Movement;
    public Rigidbody2D PlayerRB;
    private Animator GeorgeAnimator;


    public GameObject Bomb_Prefab;
    public Transform BombSpawnPoint;
    public KeyCode bombkey = KeyCode.Return; // Player One

    private void Awake()
    {
        GeorgeAnimator = GetComponent<Animator>();
        PlayerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }




    void Update()
    {

        Movement.x = Input.GetAxisRaw("Horizontal2");
        Movement.y = Input.GetAxisRaw("Vertical2");


        // remove diagonal movement
        if (Movement.y != 0)
        {
            Movement.x = 0;
        }


        // George Walk Animation
        if (Movement != Vector2.zero)
        {

            GeorgeAnimator.SetFloat("MoveX", Movement.x);
            GeorgeAnimator.SetFloat("MoveY", Movement.y);
            IsPlayerMoving = true;

            ResetIdle();
        }
        else
        {
            // George stopped moving
            IsPlayerMoving = false;

            // set idle direction depending on last movement
            if (GeorgeAnimator.GetFloat("MoveX") < 0)
            {
                GeorgeAnimator.SetBool("LeftIdle", true);
               

            }
            else if (GeorgeAnimator.GetFloat("MoveX") > 0)
            {
                GeorgeAnimator.SetBool("RightIdle", true);
            }
            else if (GeorgeAnimator.GetFloat("MoveY") > 0)
            {
                GeorgeAnimator.SetBool("UpIdle", true);
            }
            else if (GeorgeAnimator.GetFloat("MoveY") < 0)
            {
                GeorgeAnimator.SetBool("DownIdle", true);
            }
        }


        if (Input.GetKeyDown(bombkey))
        {
            Instantiate(Bomb_Prefab, BombSpawnPoint.position, Quaternion.identity);
        }

    }
    void FixedUpdate()
    {
        // George Movement
        if (Movement != Vector2.zero)
        {
            PlayerRB.MovePosition(PlayerRB.position + Movement * PlayerMoveSpeed * Time.fixedDeltaTime);

        }

    }
    public void ResetIdle()
    {

        GeorgeAnimator.SetBool("LeftIdle", false);
        GeorgeAnimator.SetBool("RightIdle", false);
        GeorgeAnimator.SetBool("UpIdle", false);
        GeorgeAnimator.SetBool("DownIdle", false);
    }
}
