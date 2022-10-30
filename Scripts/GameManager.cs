using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { set; get; }
    private PlayerMotor motor;
    private bool isGameStarted = false;
    public bool IsDead { set; get; }
    
    private float score, coinScore, modifierScore;
    private int lastScore;

    [SerializeField] TextMeshProUGUI scoreText, coinText, modifierText;

    private const int COIN_SCORE_AMOUNT = 5;

    [SerializeField] Animator deathMenuAnim, gameCanvas, menuAnim, diamondAnim;
    [SerializeField] TextMeshProUGUI deadScoreText, deadCoinText, hiscoreText;

    private void Awake()
        {
        instance = this;
        modifierScore = 1;
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        scoreText.text = score.ToString("0");
        modifierText.text = "x" + modifierScore.ToString("0.0");
        coinText.text = coinScore.ToString("0");
        hiscoreText.text = PlayerPrefs.GetInt("Hiscore").ToString();
        }

    // Update is called once per frame
   void Update()
    {
       /* if(InputScript.Instance.Tap && !isGameStarted)
            {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) 
                
            
            } */

        if (isGameStarted && !IsDead)
            {
            
            score += (Time.deltaTime * modifierScore);
           
            if(lastScore != (int)score)
                {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
                }
            
            }
    }

   public void StartGame()
        {
        isGameStarted = true;
        motor.StartRunning();
        FindObjectOfType<GlacierSpawner>().IsScrolling = true;
        FindObjectOfType<CameraMotor>().IsMoving = true;
        gameCanvas.SetTrigger("Show");
        menuAnim.SetTrigger("Hide");
        }
   public void UpdateModifier(float modifierAmount)
        {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
        }

    public void GetCoin()
        {
        coinScore++;
        coinText.text = coinScore.ToString();
        score += COIN_SCORE_AMOUNT;
        scoreText.text = score.ToString("0");
        diamondAnim.SetTrigger("Collect");
        }

    public void OnPlayButton()
        {
        SceneManager.LoadScene("Gameplay");
        }

    public void OnDeath()
        {
        IsDead = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        deadScoreText.text = score.ToString("000");
        deadCoinText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        gameCanvas.SetTrigger("Hide");

        if(score > PlayerPrefs.GetInt("Hiscore"))
            {
            float s = score;
            if (s % 1 == 0)
                s += 1;
            PlayerPrefs.SetInt("Hiscore", (int)s);
            }
        }

    public void OnResetButton()
        {
        PlayerPrefs.SetInt("Hiscore", 0);
        SceneManager.LoadScene("Gameplay");
        }

    public void OnPause()
        {
        Time.timeScale = 0;
        }

   public void OnResume()
        {
        Time.timeScale = 1;
        }
    }
