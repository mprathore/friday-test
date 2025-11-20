using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public int cardID;     // assigned by BoardManager
    [HideInInspector] public int slotIndex;  // position index in board
    public Image frontImage;
    public Image backImage;
    public float flipDuration = 0.18f;
    public bool isFlipped
    {
        get; private set;
    }
    public bool isMatched
    {
        get; set;
    }


    CanvasGroup cg;

    void Awake()
    {
        cg = gameObject.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(int id, Sprite sprite, int index, bool startFlipped = false, bool matched = false)
    {
        cardID = id;
        slotIndex = index;
        frontImage.sprite = sprite;
        isMatched = matched;
        if (matched)
        {
            frontImage.gameObject.SetActive(true);
            backImage.gameObject.SetActive(false);
            isFlipped = true;
            Color c = frontImage.color;
            c.a = 0f;              // change alpha
            frontImage.color = c;
            Image img = GetComponent<Image>();
            img.color = c;
        }
        else
        {
            if (startFlipped)
                StartCoroutine(FlipInstantToFront());
            else
                HideInstant();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMatched || isFlipped)
            return;
        GameManager.Instance.CardClicked(this);
    }

    IEnumerator FlipInstantToFront()
    {
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
        isFlipped = true;
        yield break;
    }

    public void HideInstant()
    {
        isFlipped = false;
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
    }

    public void SetMatched()
    {
        isMatched = true;
        // keep front visible
    }

    // Smooth flipping (non-blocking). Returns when finished.
    public IEnumerator FlipToFrontCoroutine()
    {
        // play flip animation: scaleX 1->0 -> swap -> 0->1
        float half = flipDuration * 0.5f;
        yield return ScaleXCoroutine(1f, 0f, half);
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
        yield return ScaleXCoroutine(0f, 1f, half);
        isFlipped = true;
    }

    public IEnumerator FlipToBackCoroutine()
    {
        float half = flipDuration * 0.5f;
        yield return ScaleXCoroutine(1f, 0f, half);
        frontImage.gameObject.SetActive(false);
        backImage.gameObject.SetActive(true);
        yield return ScaleXCoroutine(0f, 1f, half);
        isFlipped = false;
    }

    IEnumerator ScaleXCoroutine(float a, float b, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(a, b, t / duration);
            transform.localScale = new Vector3(v, 1f, 1f);
            yield return null;
        }
        transform.localScale = new Vector3(b, 1f, 1f);
    }
}
