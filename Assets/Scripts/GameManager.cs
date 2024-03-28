using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text scoreText;
    public RectTransform deathMenu;
    public GameObject startMenu;
    public GameObject scoreGroup;
    public Text finalScoreText;
    public Text highScoreText;
    public Text finalHighScore;

    public GameObject creditButton;
    public GameObject closeCreditButton;

    public Player thePlayer;

    private GameObject startMenuInstance;

    public bool playGame;
    private bool startKey = false;
    private float deathTime = 0;
    private float replayDelay = 1f;

    public List<Image> lives;
    public Sprite fullHeart;
    public Sprite brokenHeart;

    public bool onCredits;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        instance.playGame = false;
        instance.onCredits = false;

        // Hide the death menu
        CanvasGroup cg = deathMenu.GetComponent<CanvasGroup>();
        cg.alpha = 0;

        closeCreditButton.SetActive(false);

        // Show the proper previous high score
        highScoreText.text = $"highscore: {PlayerPrefs.GetInt("HighScore", 0).ToString()}";

    }

    // Update is called once per frame
    void Update()
    {
        if (!instance.startKey && isStartKey() && onCredits == false)
        {
            instance.startKey = StartGame();
        }
    }

    private bool isStartKey()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) {
            return true;
        }
        return false;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    private void CheckHighScore(int curScore)
    {
        if (curScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", curScore);
        }

    }

    public void GameOver(int finalScore)
    {
        CheckHighScore(finalScore);

        // Reset values
        instance.playGame = false;
        instance.startKey = false;

        // creditButton.SetActive(true);

        deathTime = Time.timeSinceLevelLoad;

        finalScoreText.text = finalScore.ToString();
        finalHighScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();

        // Show the final menu
        CanvasGroup cg = deathMenu.GetComponent<CanvasGroup>();
        cg.alpha = 1;

        // Show the score
        CanvasGroup scoreGrouping = scoreGroup.GetComponent<CanvasGroup>();
        scoreGrouping.alpha = 0;

        // Get rid of the original score text
        scoreText.enabled = false;
        scoreText.text = "0";

    }

    public bool StartGame()
    {
        if (deathTime == 0 || Time.timeSinceLevelLoad - deathTime > replayDelay)
        {
            instance.playGame = true;

            thePlayer.Revive();

            HideAllMenus();

            scoreText.enabled = true;
            scoreText.text = "0";

            // Show the score
            CanvasGroup scoreGrouping = scoreGroup.GetComponent<CanvasGroup>();
            scoreGrouping.alpha = 1;

            // Reset all hearts to full
            foreach (var heart in lives)
            {
                heart.sprite = fullHeart; 
            }

            creditButton.SetActive(false);


            return true;
        }
        else
        {
            return false;
        }
    }

    private void HideAllMenus()
    {
        CanvasGroup start = startMenu.GetComponent<CanvasGroup>();
        start.alpha = 0;

        CanvasGroup end = deathMenu.GetComponent<CanvasGroup>();
        end.alpha = 0;
    }

    public void updateLifeDisplay(int lifeCount)
    {
        if (lifeCount < 0) return;
        int toBeChanged = lives.Count - lifeCount - 1;
        // Change the sprites depending on the number of lives
        for (int i = toBeChanged; i >= 0; i--)
        {
            lives[i].sprite = brokenHeart;
        }

        for (int i = toBeChanged + 1; i < lives.Count; i++)
        {
            lives[i].sprite = fullHeart;
        }

    }

    public void ToggleCredits()
    {
        instance.onCredits = true;
    }

    public void HideCredits()
    {
        instance.onCredits = false;
    }
}
