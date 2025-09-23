using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinPlayerTwo : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTwo"))
        {
            PlayerTwoController player = other.GetComponent<PlayerTwoController>();
            if (player != null && player.isHoldingFlag)
            {


                StartCoroutine(LoadSceneWithDelay("PlayerTwoWin", 0.5f));
            }
            else
            {

                return;
            }
        }
    }
    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
