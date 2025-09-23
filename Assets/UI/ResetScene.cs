using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class ResetScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadSceneWithDelay("GameScene", 4f));
    }

    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        
        yield return new WaitForSecondsRealtime(delay);


        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }
}
