using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float BombTimeToExplode = 2f;
    public GameObject ExplosionPrefab;

    private Collider2D bombCollider;

    void Start()
    {
        bombCollider = GetComponent<Collider2D>();

        // Disable collider at spawn so player can walk away
        bombCollider.enabled = false;

        // Enable collider after a short delay (so player exits)
        Invoke("EnableCollider", 0.2f);

        Invoke("Explode", BombTimeToExplode);
    }

    void EnableCollider()
    {
        bombCollider.enabled = true;
    }

    void Explode()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
