using UnityEngine;

public class SlashHitbox : MonoBehaviour
{
    public float lifetime = 0.3f; // how long the slash exists
    public GameObject[] destroyPrefabs; // assign prefabs it can destroy

    private void Start()
    {
        Destroy(gameObject, lifetime); // destroy itself after lifetime
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object matches any prefab in destroyPrefabs
        foreach (GameObject prefab in destroyPrefabs)
        {
            if (other.gameObject.name.Contains(prefab.name))
            {
                Destroy(other.gameObject);
                break;
            }
        }
    }
}
