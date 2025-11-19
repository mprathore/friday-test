using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public RectTransform boardParent; // panel with GridLayoutGroup
    public GridLayoutGroup grid;
    public List<Sprite> cardSprites; // unique sprites (each forms one pair)
    public int rows = 3;
    public int cols = 4;
    public float spacing = 8f;
    public bool useSavedStateOnStart = true;

    private List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        ClearBoard();

        // set grid constraint
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = cols;
        grid.spacing = new Vector2(spacing, spacing);

        // autosize cell
        Vector2 cell = CalculateCellSize(rows, cols);
        grid.cellSize = cell;

        // prepare data list (pairs)
        List<int> ids = new List<int>();
        int pairCount = Mathf.Min(cardSprites.Count, (rows * cols) / 2);
        for (int i = 0; i < pairCount; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // If odd cell count, remove last
        while (ids.Count > rows * cols)
            ids.RemoveAt(ids.Count - 1);

        // shuffle
        for (int i = 0; i < ids.Count; i++)
        {
            int r = Random.Range(i, ids.Count);
            int tmp = ids[i];
            ids[i] = ids[r];
            ids[r] = tmp;
        }

        // if saved state exists and requested, load override
        SaveState saved = SaveLoadManager.Instance.LoadSave();
        bool useSave = saved != null && useSavedStateOnStart && saved.boardRows == rows && saved.boardCols == cols && saved.cardIDs != null && saved.cardIDs.Count == ids.Count;

        List<int> finalIDs = new List<int>();
        if (useSave)
        {
            finalIDs = saved.cardIDs; // saved order
        }
        else
            finalIDs = ids;

        // spawn cards
        for (int i = 0; i < finalIDs.Count; i++)
        {
            GameObject go = Instantiate(cardPrefab, boardParent);
            Card c = go.GetComponent<Card>();
            int id = finalIDs[i];
            bool matched = false;
            if (useSave)
                matched = saved.matchedFlags[i];
            Sprite sprite = cardSprites[Mathf.Clamp(id, 0, cardSprites.Count - 1)];
            c.Setup(id, sprite, i, false, matched);
            spawned.Add(go);
        }

        // if loaded, restore score/moves
        if (useSave)
        {
            SaveState s = saved;
            GameManager.Instance.score = s.score;
            GameManager.Instance.matchesFound = s.matches;
            GameManager.Instance.moves = s.moves;
            UIManager.Instance.UpdateScore(s.score);
            UIManager.Instance.UpdateMoves(s.moves);
        }
    }

    Vector2 CalculateCellSize(int r, int c)
    {
        // get available size inside boardParent
        float width = boardParent.rect.width - grid.padding.left - grid.padding.right - (c - 1) * spacing;
        float height = boardParent.rect.height - grid.padding.top - grid.padding.bottom - (r - 1) * spacing;
        float cellW = Mathf.Floor(width / c);
        float cellH = Mathf.Floor(height / r);
        float size = Mathf.Min(cellW, cellH);
        return new Vector2(size, size);
    }

    void ClearBoard()
    {
        foreach (var go in spawned)
            if (go)
                Destroy(go);
        spawned.Clear();
    }

    // called by UI to change layout
    public void SetLayout(int newRows, int newCols)
    {
        rows = newRows;
        cols = newCols;
        GenerateBoard();
    }
}
