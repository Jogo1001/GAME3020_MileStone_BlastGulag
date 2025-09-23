using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class WinPlayerOne : MonoBehaviour
{


    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerOne"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.isHoldingFlag)
            {


                StartCoroutine(LoadSceneWithDelay("PlayerOneWin", 0.5f));
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
