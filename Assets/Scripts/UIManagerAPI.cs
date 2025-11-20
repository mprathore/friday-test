using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManagerAPI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject listItemPrefab;
    public TMP_Text statusText;

    private void Start()
    {
        StartCoroutine(APIHandler.Instance.GetPersons(OnSuccess, OnError));
        statusText.text = "Loading...";
    }

    void OnSuccess(PersonList list)
    {
        statusText.text = "";

        if (list == null || list.people == null || list.people.Length == 0)
        {
            statusText.text = "No data found!";
            return;
        }

        foreach (Person p in list.people)
        {
            GameObject item = Instantiate(listItemPrefab, contentParent);
            item.GetComponent<ListItemUI>().SetData(p);
        }
    }



    void OnError(string error)
    {
        statusText.text = error;
    }
}
