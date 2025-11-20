using UnityEngine;
using TMPro;

public class ListItemUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text ageText;
    public TMP_Text locationText;

    public void SetData(Person p)
    {
        nameText.text = p.name;
      
        ageText.text = "Age: " + p.age;
       
        locationText.text = "Location: " + p.location;
           
    }
}
