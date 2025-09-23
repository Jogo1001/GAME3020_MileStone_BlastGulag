using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;   // singleton

    [Header("Kill UI")]
    public Text killTextPlayer1;
    public Text killTextPlayer2;

    [Header("Timer UI")]
    public Text timerText;          
    public float matchDuration = 60; 

    private float remainingTime;

    private int killsPlayer1;
    private int killsPlayer2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        killsPlayer1 = 0;
        killsPlayer2 = 0;

        // set remaining time to match duration
        remainingTime = matchDuration;
    }

    private void Start()
    {
        UpdateKillText();
        UpdateTimerText();
    }

    private void Update()
    {
        // countdown timer
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0f) remainingTime = 0f;
            UpdateTimerText();

            if (remainingTime == 0f)
            {
                EndMatch();
            }
        }
    }

    public void AddKill(int killerPlayerIndex)
    {
        if (killerPlayerIndex == 1)
        {
            killsPlayer1++;
        }
        else if (killerPlayerIndex == 2)
        {
            killsPlayer2++;
        }

        UpdateKillText();
    }

    public void ResetKills()
    {
        killsPlayer1 = 0;
        killsPlayer2 = 0;
        UpdateKillText();
    }

    private void UpdateKillText()
    {
        if (killTextPlayer1 != null)
            killTextPlayer1.text = $" Score : {killsPlayer1}";
        if (killTextPlayer2 != null) 
            killTextPlayer2.text = $" Score : {killsPlayer2}";
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void EndMatch()
    {
  
        if (killsPlayer1 > killsPlayer2)
        {
          
            StartCoroutine(LoadSceneWithDelay("PlayerOneWin", 0.5f));
        }
        else if (killsPlayer2 > killsPlayer1)
        {
          
            StartCoroutine(LoadSceneWithDelay("PlayerTwoWin", 0.5f));
        }
        else
        {
         
            StartCoroutine(LoadSceneWithDelay("DrawScene", 0.5f));
        }

     

 
   
        Time.timeScale = 0f;
    }

    public int GetKills(int playerIndex)
    {
        return playerIndex == 1 ? killsPlayer1 : killsPlayer2;
    }

    public void RestartMatch()
    {
        Time.timeScale = 1f;
        killsPlayer1 = 0;
        killsPlayer2 = 0;
        remainingTime = matchDuration;
        UpdateKillText();
        UpdateTimerText();
    }
    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(sceneName);
    }
}
