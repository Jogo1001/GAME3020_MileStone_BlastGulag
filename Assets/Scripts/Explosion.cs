using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float duration = 0.5f;
    public float explosionRange = 2f;
    public LayerMask destructibleMask;

    void Start()
    {
        Destroy(gameObject, duration);

      
        ExplodeInDirection(Vector2.up);
        ExplodeInDirection(Vector2.down);
        ExplodeInDirection(Vector2.left);
        ExplodeInDirection(Vector2.right);
    }

    void ExplodeInDirection(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, explosionRange, destructibleMask);

        if (hit.collider != null)
        {
           
            Destroy(hit.collider.gameObject);
        }
    }
}
