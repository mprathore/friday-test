using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SoundManager soundManager;
    public UIManager uiManager;

    public int comboCount = 0;
    public int comboBonus = 2; 

   
    private List<Card> flippedBuffer = new List<Card>();

    // stats
    public int score = 0;
    public int moves = 0;
    public int matchesFound = 0;

    public float revealDelay = 0.7f; 

    public GameObject combo;
   


    void Awake()
    {
        Instance = this;
    }

 
    public void CardClicked(Card c)
    {
        
        StartCoroutine(HandleCardFlip(c));
    }

    IEnumerator HandleCardFlip(Card c)
    {
       
        soundManager.PlayFlip();

       
        yield return StartCoroutine(c.FlipToFrontCoroutine());

    
        flippedBuffer.Add(c);

      
        TryQueuePairs();
    }

    void TryQueuePairs()
    {
      
        List<Card> available = flippedBuffer.FindAll(x => x != null && x.isFlipped && !x.isMatched);
        while (available.Count >= 2)
        {
            Card a = available[0];
            Card b = available[1];

           
            flippedBuffer.Remove(a);
            flippedBuffer.Remove(b);
           
            available.RemoveAt(0);
            available.RemoveAt(0);

        
            StartCoroutine(CheckPairCoroutine(a, b));
        }
    }

    IEnumerator CheckPairCoroutine(Card a, Card b)
    {
      
        yield return new WaitForSeconds(revealDelay);

       
        if (a.isMatched || b.isMatched)
            yield break;

        moves++;
        uiManager.UpdateMoves(moves);

        if (a.cardID == b.cardID)
        {
            // MATCH
            a.SetMatched();   
            b.SetMatched();
           
            matchesFound++;
            score += 1;



            uiManager.UpdateScore(score);
            soundManager.PlayMatch();
            Color c = a.frontImage.color;
            c.a = 0f;              // change alpha
            a.frontImage.color = c;
            b.frontImage.color = c;
            a.GetComponent<Image>().color = c;
            b.GetComponent<Image>().color = c;

            comboCount += 1;

           
            if (comboCount >= 2)   
            {
                score += comboBonus;  // extra reward
                uiManager.UpdateScore(score);
               // Debug.Log("COMBO achieved! +" + comboBonus);
                StartCoroutine(EnableForHalfSecond(combo));

            }
            // check win
            if (uiManager != null && uiManager.CheckWinCondition())
            {
                yield return new WaitForSeconds(0.3f);
                soundManager.PlayGameOver();
                uiManager.ShowWin();
            }
        }
        else
        {
            comboCount = 0;
            // MISMATCH: 
            soundManager.PlayMismatch();
           
            StartCoroutine(a.FlipToBackCoroutine());
            StartCoroutine(b.FlipToBackCoroutine());
        }
    }

    // Save helper (might be called by UI)
    public int GetScore() => score;
    public int GetMoves() => moves;
    public int GetMatches() => matchesFound;

    public IEnumerator EnableForHalfSecond(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(false);
    }

}
