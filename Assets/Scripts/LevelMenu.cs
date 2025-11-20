using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public GameObject levelPanel;

    public void OpenLevels()
    {
        levelPanel.SetActive(true);
    }

    public void CloseLevels()
    {
        levelPanel.SetActive(false);
    }

    public void EasyLevel()
    {
        PlayerPrefs.DeleteKey("CardMatch_Save_v1");
        PlayerPrefs.Save();
        GlobalLevelSettings.rows = 2;
        GlobalLevelSettings.cols = 4;
        SceneManager.LoadScene("CardGame");
    }

    public void MediumLevel()
    {
        PlayerPrefs.DeleteKey("CardMatch_Save_v1");
        PlayerPrefs.Save();
        GlobalLevelSettings.rows = 3;
        GlobalLevelSettings.cols = 4;
        SceneManager.LoadScene("CardGame");
    }

    public void HardLevel()
    {
        PlayerPrefs.DeleteKey("CardMatch_Save_v1");
        PlayerPrefs.Save();
        GlobalLevelSettings.rows = 4;
        GlobalLevelSettings.cols = 4;
        SceneManager.LoadScene("CardGame");
    }
}
