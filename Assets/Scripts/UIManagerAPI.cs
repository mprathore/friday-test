using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManagerAPI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject listItemPrefab;
    public TMP_Text statusText;

    public TMP_Dropdown searchFilterDropdown;
    public TMP_InputField searchInputField;

    private Person[] originalData; // Store API data for search

    // ---------------- Start ----------------
    private void Start()
    {
        StartCoroutine(APIHandler.Instance.GetPersons(OnSuccess, OnError));
        statusText.text = "Loading...";
    }

    // ---------------- API Success ----------------
    void OnSuccess(PersonList list)
    {
        statusText.text = "";

        if (list == null || list.people == null || list.people.Length == 0)
        {
            statusText.text = "No data found!";
            return;
        }

        // Save original API data
        originalData = list.people;

        // Show full list initially
        BuildList(originalData);
    }

    // ---------------- API Error ----------------
    void OnError(string error)
    {
        statusText.text = error;
    }

    // ---------------- Build List UI ----------------
    void BuildList(Person[] people)
    {
        // Remove old items
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Add new items
        foreach (Person p in people)
        {
            GameObject item = Instantiate(listItemPrefab, contentParent);
            item.GetComponent<ListItemUI>().SetData(p);
        }
    }

    // ---------------- Search Button Logic ----------------
    public void OnSearchButtonPressed()
    {
        string searchText = searchInputField.text.ToLower();
        int option = searchFilterDropdown.value;   // 0 = Name, 1 = Age, 2 = Location

        Person[] filtered = originalData; // fallback to original list

        if (option == 0)   // ---- Search by Name ----
        {
            filtered = System.Array.FindAll(originalData, p =>
                p.name.ToLower().Contains(searchText)
            );
        }
        else if (option == 1)   // ---- Search by Age ----
        {
            if (int.TryParse(searchText, out int ageValue))
            {
                filtered = System.Array.FindAll(originalData, p =>
                    p.age == ageValue
                );
            }
            else
            {
                Debug.Log("Age search requires a number.");
                return;
            }
        }
        else if (option == 2)   // ---- Search by Location ----
        {
            filtered = System.Array.FindAll(originalData, p =>
                p.location.ToLower().Contains(searchText)
            );
        }

        // Show filtered results
        BuildList(filtered);
    }
}
