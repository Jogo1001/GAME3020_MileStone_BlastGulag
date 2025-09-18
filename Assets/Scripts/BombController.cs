using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.UIElements;

public class BombController : MonoBehaviour
{
    public GameObject bombprefab;
    public Transform bombspawnpoint;
 
    public KeyCode inputkey = KeyCode.Space;
    public float bombfusetime = 3f;
    public int bombamount = 1;
    private int bombsremaining;



    public Explosion explosionPrefab;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;
    public LayerMask explosionLayerMask;



    public Tilemap destructibleTiles;
    public Destructible destructiblePrefab;
    private void OnEnable()
    {
        bombsremaining = bombamount;

    }

    private void Update()
    {
        if (bombsremaining > 0 && Input.GetKeyDown(inputkey))
        {
            StartCoroutine(PlaceBomb());
        }
    }

    private IEnumerator PlaceBomb()
    {
        Vector2 ExplosionSpawnPosition = transform.position;

        GameObject bomb = Instantiate(bombprefab, bombspawnpoint.position, Quaternion.identity);
        bombsremaining--;

        yield return new WaitForSeconds(bombfusetime);

        if (bomb != null)
        {
            ExplosionSpawnPosition = bomb.transform.position;


            Explosion explosion = Instantiate(explosionPrefab, ExplosionSpawnPosition, Quaternion.identity);
            explosion.SetActiveRenderer(explosion.start);
            explosion.DestroyAfter(explosionDuration);

            Explode(ExplosionSpawnPosition, Vector2.up, explosionRadius);
            Explode(ExplosionSpawnPosition, Vector2.down, explosionRadius);
            Explode(ExplosionSpawnPosition, Vector2.left, explosionRadius);
            Explode(ExplosionSpawnPosition, Vector2.right, explosionRadius);


            Destroy(bomb);
        }

        bombsremaining++;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            other.isTrigger = false;
        }
    }
    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0)
        {
            return; 
        }



        position += direction;

        if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask))
        {

            ClearDestructible(position);
            return;
        }

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, direction, length - 1);
    }
    private void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if (tile != null)
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleTiles.SetTile(cell, null);
        }
    }
}
