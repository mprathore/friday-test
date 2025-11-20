using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class APIHandler : MonoBehaviour
{
    public static APIHandler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void BacktoMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    public IEnumerator GetPersons(Action<PersonList> onSuccess, Action<string> onError)
    {
        string url = "https://mocki.io/v1/681008e3-453b-4d04-aada-ded85075fd2c";

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke("API Error: " + req.error);
                yield break;
            }

            string rawJson = req.downloadHandler.text;
           // Debug.Log("Raw JSON: " + rawJson);

            PersonList list = JsonUtility.FromJson<PersonList>(rawJson);
            onSuccess?.Invoke(list);
        }
    }

}
