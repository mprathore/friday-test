using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text scoreText;
    public Text movesText;
    public Text timerText;
    public GameObject winPanel;
    public BoardManager boardManager;

    float startTime;
    bool running = true;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        startTime = Time.time;
        UpdateScore(GameManager.Instance.GetScore());
        UpdateMoves(GameManager.Instance.GetMoves());
    }

    void Update()
    {
        if (!running)
            return;
        float t = Time.time - startTime;
        timerText.text = $"Time: {FormatTime(t)}";
    }

    string FormatTime(float s)
    {
        int mins = (int)(s / 60);
        int secs = (int)(s % 60);
        return $"{mins:00}:{secs:00}";
    }

    public void UpdateScore(int v)
    {
        scoreText.text = "Score: " + v;
    }

    public void UpdateMoves(int v)
    {
        movesText.text = "Moves: " + v;
    }

    public bool CheckWinCondition()
    {
        // If all pairs matched (simple: count matches equals pairs)
        // BoardManager knows rows*cols/2
        int totalPairs = (boardManager.rows * boardManager.cols) / 2;
        return GameManager.Instance.GetMatches() >= totalPairs;
    }

    public void ShowWin()
    {
        winPanel.SetActive(true);
        running = false;
        SaveLoadManager.Instance.ClearSave();   // no resume if game already won

    }
    public void HideWin()
    {
        winPanel.SetActive(false);
        running = true;
    }


    // Save current game
    public void SaveGame()
    {
        List<int> order = new List<int>();
        List<bool> matched = new List<bool>();

        for (int i = 0; i < boardManager.boardParent.childCount; i++)
        {
            var card = boardManager.boardParent.GetChild(i).GetComponent<Card>();
            order.Add(card.cardID);
            matched.Add(card.isMatched);
        }

        float timeElapsed = Time.time - startTime;

        SaveLoadManager.Instance.Save(order, matched,
        GameManager.Instance.score,
        GameManager.Instance.moves,
        GameManager.Instance.matchesFound,
        boardManager.rows,
        boardManager.cols,
        timeElapsed,
        false // game is NOT finished yet
        );


        Debug.Log("Game Saved Successfully!");
    }


    public void LoadGame()
    {
        boardManager.useSavedStateOnStart = true;
        boardManager.GenerateBoard();

        // Reset timer
        startTime = Time.time;
        running = true;

        winPanel.SetActive(false);
    }


    public void ClearSave()
    {
        SaveLoadManager.Instance.ClearSave();
    }

    public void RestartGame()
    {
        // Clear gameplay stats
        GameManager.Instance.score = 0;
        GameManager.Instance.moves = 0;
        GameManager.Instance.matchesFound = 0;

        UpdateScore(0);
        UpdateMoves(0);

        // Reset timer
        startTime = Time.time;
        running = true;

        // Clear any matched cards and reload board
        boardManager.GenerateBoard();

        // Hide win panel
        winPanel.SetActive(false);
    }

    public void SaveAndQuit()
    {
        SaveGame();   // save progress

        // Go back to main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

}
