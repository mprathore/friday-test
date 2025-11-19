using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject continueButton;   // Assign in Inspector

    void Start()
    {
        // Check if save exists
        SaveState save = SaveLoadManager.Instance.LoadSave();

        // If no save or completed game -> hide continue
        if (save == null || save.isCompleted)
        {
            continueButton.SetActive(false);
        }
        else
        {
            continueButton.SetActive(true);
        }
    }

    // NEW GAME
    public void PlayNewGame()
    {
        // delete old save
        PlayerPrefs.DeleteKey("CardMatch_Save_v1");
        PlayerPrefs.Save();

        // load main gameplay scene
        SceneManager.LoadScene("CardGame");
    }

    // RESUME GAME
    public void ResumeGame()
    {
        SceneManager.LoadScene("CardGame");
    }

    // CLEAR SAVE
    public void ClearSave()
    {
        PlayerPrefs.DeleteKey("CardMatch_Save_v1");
        PlayerPrefs.Save();

        // refresh menu UI
        continueButton.SetActive(false);
        Debug.Log("Save cleared!");
    }

    // QUIT (optional)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT GAME (Editor won't close)");
    }
}
