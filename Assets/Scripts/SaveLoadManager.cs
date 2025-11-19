using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveState
{
    public int boardRows;
    public int boardCols;
    public List<int> cardIDs;
    public List<bool> matchedFlags;
    public int score;
    public int moves;
    public int matches;
    public float timeSeconds;
    public bool isCompleted;

}

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;
    const string KEY = "CardMatch_Save_v1";

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Save(List<int> order, List<bool> matched, int score, int moves, int matches, int rows, int cols, float timeSeconds, bool completed)
    {
        SaveState s = new SaveState();
        s.boardRows = rows;
        s.boardCols = cols;
        s.cardIDs = new List<int>(order);
        s.matchedFlags = new List<bool>(matched);
        s.score = score;
        s.moves = moves;
        s.matches = matches;
        s.timeSeconds = timeSeconds;
        s.isCompleted = completed;

        string json = JsonUtility.ToJson(s);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }


    public SaveState LoadSave()
    {
        if (!PlayerPrefs.HasKey(KEY))
            return null;
        string json = PlayerPrefs.GetString(KEY);
        SaveState s = JsonUtility.FromJson<SaveState>(json);
        return s;
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(KEY);
    }
}
