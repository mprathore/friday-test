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

    public GameObject previousButton; // <-- ADDED

    private Person[] originalData; // Store API data for search

    // -------- Pagination Variables --------
    private int currentPage = 0;
    private int itemsPerPage = 10;

    // ---------------- Start ----------------
    private void Start()
    {
        StartCoroutine(APIHandler.Instance.GetPersons(OnSuccess, OnError));
        statusText.text = "Loading...";

        previousButton.SetActive(false);  // Hide on first load
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

        // Start at first page
        currentPage = 0;

        // Show first 20
        BuildList(GetPage(originalData, currentPage, itemsPerPage));

        UpdatePaginationButtons();
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

    // ---------------- Pagination: Slice Data ----------------
    Person[] GetPage(Person[] source, int page, int perPage)
    {
        int start = page * perPage;

        if (start >= source.Length)
            return new Person[0]; // No more pages

        int length = Mathf.Min(perPage, source.Length - start);

        Person[] pageData = new Person[length];
        System.Array.Copy(source, start, pageData, 0, length);

        return pageData;
    }

    // ---------------- NEXT Button ----------------
    public void OnNextButtonPressed()
    {
        currentPage++;

        Person[] pageData = GetPage(originalData, currentPage, itemsPerPage);

        if (pageData.Length == 0)
        {
            statusText.text = ("No more pages!");
            currentPage--; // revert
            return;
        }

        BuildList(pageData);
        UpdatePaginationButtons();
    }

    // ---------------- PREVIOUS Button ----------------
    public void OnPreviousButtonPressed()
    {
        if (currentPage == 0)
        {
            return;
        }

        currentPage--;

        BuildList(GetPage(originalData, currentPage, itemsPerPage));
        UpdatePaginationButtons();
    }

    // ---------------- Pagination Button Visibility ----------------
    void UpdatePaginationButtons()
    {
        // Hide previous button on first page
        previousButton.SetActive(currentPage > 0);
    }

    // ---------------- Search Button Logic ----------------
    public void OnSearchButtonPressed()
    {
        string searchText = searchInputField.text.ToLower();
        int option = searchFilterDropdown.value;   // 0 = Name, 1 = Age, 2 = Location

        Person[] filtered = originalData; // fallback

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
                statusText.text = ("Age search requires a number.");
                return;
            }
        }
        else if (option == 2)   // ---- Search by Location ----
        {
            filtered = System.Array.FindAll(originalData, p =>
                p.location.ToLower().Contains(searchText)
            );
        }

        // Reset pagination for filtered data
        currentPage = 0;

        // Show first page of results
        BuildList(GetPage(filtered, currentPage, itemsPerPage));

        UpdatePaginationButtons();
    }
}
