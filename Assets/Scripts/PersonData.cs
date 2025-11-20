[System.Serializable]
public class Person
{
    public string name;
    public int age;
    public string location;
}

[System.Serializable]
public class PersonList
{
    public Person[] people;   // <-- MUST MATCH API KEY
}
