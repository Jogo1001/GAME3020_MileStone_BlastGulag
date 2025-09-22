using UnityEngine;

public class SlashController : MonoBehaviour
{
    [Header("Slash Prefabs")]
    public GameObject slashUpPrefab;
    public GameObject slashDownPrefab;
    public GameObject slashLeftPrefab;
    public GameObject slashRightPrefab;

    [Header("Slash Spawn Points")]
    public Transform slashUpSpawnPoint;
    public Transform slashDownSpawnPoint;
    public Transform slashLeftSpawnPoint;
    public Transform slashRightSpawnPoint;

    [Header("Animator")]
    public Animator playerAnimator;

    public void PerformSlash(Vector2 direction)
    {
        if (direction == Vector2.up && slashUpPrefab != null && slashUpSpawnPoint != null)
        {
            Instantiate(slashUpPrefab, slashUpSpawnPoint.position, Quaternion.identity);
            playerAnimator.SetTrigger("SlashUp");
        }
        else if (direction == Vector2.down && slashDownPrefab != null && slashDownSpawnPoint != null)
        {
            Instantiate(slashDownPrefab, slashDownSpawnPoint.position, Quaternion.identity);
            playerAnimator.SetTrigger("SlashDown");
        }
        else if (direction == Vector2.left && slashLeftPrefab != null && slashLeftSpawnPoint != null)
        {
            Instantiate(slashLeftPrefab, slashLeftSpawnPoint.position, Quaternion.identity);
            playerAnimator.SetTrigger("SlashLeft");
        }
        else if (direction == Vector2.right && slashRightPrefab != null && slashRightSpawnPoint != null)
        {
            Instantiate(slashRightPrefab, slashRightSpawnPoint.position, Quaternion.identity);
            playerAnimator.SetTrigger("SlashRight");
        }
    }
}
