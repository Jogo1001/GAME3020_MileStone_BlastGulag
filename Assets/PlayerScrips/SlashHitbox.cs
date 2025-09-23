using UnityEngine;

public class SlashHitbox : MonoBehaviour
{
    public float lifetime = 0.3f;
    public GameObject[] destroyPrefabs;
  

    private void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BombController bombController = other.GetComponent<BombController>();
        BombControllerPlayerTwo bombtwoController = other.GetComponent<BombControllerPlayerTwo>();

        foreach (GameObject prefab in destroyPrefabs)
        {
            if (other.gameObject.name.Contains(prefab.name))
            {

            
                Destroy(other.gameObject);
                if (bombController != null)
                {
                    bombController.ReturnBomb();
                }
                else if(bombtwoController != null)
                {
                    bombtwoController.ReturnBomb();
                }
                break;

            }
        }
    }
}
