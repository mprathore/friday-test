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
    public int comboBonus = 2; // bonus score for combo

    // click buffer: we accept any number of flips; every time two distinct flipped & unmatched cards are available we spawn a pair-check coroutine
    private List<Card> flippedBuffer = new List<Card>();

    // stats
    public int score = 0;
    public int moves = 0;
    public int matchesFound = 0;

    public float revealDelay = 0.7f; // how long to show pair before hiding / marking

   


    void Awake()
    {
        Instance = this;
    }

    // Called by Card when clicked
    public void CardClicked(Card c)
    {
        // Immediately start flipping animation (non-blocking)
        StartCoroutine(HandleCardFlip(c));
    }

    IEnumerator HandleCardFlip(Card c)
    {
        // play flip sound
        soundManager.PlayFlip();

        // flip visually (don't block other clicks)
        yield return StartCoroutine(c.FlipToFrontCoroutine());

        // add to buffer
        flippedBuffer.Add(c);

        // if there are 2 or more non-matched unique cards ready, take the oldest two that are not matched
        TryQueuePairs();
    }

    void TryQueuePairs()
    {
        // find first two cards that are flipped and not matched and not currently under checking
        List<Card> available = flippedBuffer.FindAll(x => x != null && x.isFlipped && !x.isMatched);
        while (available.Count >= 2)
        {
            Card a = available[0];
            Card b = available[1];

            // remove them from buffer (so further clicks don't re-use same)
            flippedBuffer.Remove(a);
            flippedBuffer.Remove(b);
            // remove from available list for next loop
            available.RemoveAt(0);
            available.RemoveAt(0);

            // start comparing pair in its own coroutine
            StartCoroutine(CheckPairCoroutine(a, b));
        }
    }

    IEnumerator CheckPairCoroutine(Card a, Card b)
    {
        // add a small reveal delay so player sees both
        yield return new WaitForSeconds(revealDelay);

        // if either became matched by other logic meanwhile, skip
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

            // CHECK FOR COMBO BONUS
            if (comboCount == 2)   // 2 matches = 4 cards = COMBO
            {
                score += comboBonus;  // give extra reward
                uiManager.UpdateScore(score);
                Debug.Log("COMBO achieved! +" + comboBonus);
                comboCount = 0; // reset after bonus
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
            // MISMATCH: play sound then flip back
            soundManager.PlayMismatch();
            // allow a short time then flip them back (non-blocking)
            StartCoroutine(a.FlipToBackCoroutine());
            StartCoroutine(b.FlipToBackCoroutine());
        }
    }

    // Save helper (might be called by UI)
    public int GetScore() => score;
    public int GetMoves() => moves;
    public int GetMatches() => matchesFound;
}
