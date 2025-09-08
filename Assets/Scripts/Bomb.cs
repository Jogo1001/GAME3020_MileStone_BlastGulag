using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float BombTimeToExplode = 2f;
    public GameObject ExplosionPrefab;

    void Start()
    {
        Invoke("Explode", BombTimeToExplode);
    }

    void Explode()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject); 
    }
}
