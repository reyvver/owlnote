using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainScreenCategories : MonoBehaviour
{

    public GameObject AddNewCategoryPanel;
    public GameObject ButtonAdd;
    public GameObject myPrefab;
    public Transform content;
    public TextMeshProUGUI name;
    public TextMeshProUGUI color;

    private bool _visibility;
    private bool _dataReceived;
    private List<string[]> valuesCategories = new List <string[]> ();
    private DatabaseReference myRef;

    public class categoryClass
    {
        public int count;
        public string colour;

        public categoryClass() {
        }

        public categoryClass( int count, string colour) {
            this.count = count;
            this.colour= colour;
        }
    }
    
    void Start() {
      //  AddNewCategoryPanel.SetActive(_visibility);
        initializeDatabase();
        StartCoroutine(viewCategories());
    }

    /*Соединяется с БД*/
    private void initializeDatabase() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");
        myRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /*Операции с данными*/
    public void databaseOperations(string operation) {
        switch (operation) {
            case "adding": {
                addNewCategory();
                break;
            }
            case "deleting": {
                break;
            }
        }
    }

    /*Добавить новую категорию в бд*/
    private void addNewCategory()
    {
        categoryClass newobj = new categoryClass(0, color.text);
        string json = JsonUtility.ToJson(newobj);
        myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Child(name.text)
            .SetRawJsonValueAsync(json);
    }

    private void clearTextBoxes() {
        name.text = "";
        color.text = "красный";
    }

    public void showAddMenu() {
        _visibility = !_visibility;
        AddNewCategoryPanel.SetActive(_visibility);

        if (_visibility) {
            ButtonAdd.transform.Rotate(0, 0, -45);
        }
        else {
            ButtonAdd.transform.Rotate(0, 0, 45);
        }
    }

    IEnumerator viewCategories()
    {
       getData();
       yield return new WaitUntil(() => _dataReceived);
       generateCategories();
    }

    private void getData()
    {
        _dataReceived = false;
        DatabaseReference newRef = myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories")
            .Reference;
   
        newRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("error occured");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log(snapshot.ChildrenCount);
                foreach (DataSnapshot childDataSnapshot in snapshot.Children)
                {
                    string name = childDataSnapshot.Key;
                    string count = childDataSnapshot.Child("count").Value.ToString();
                    string color = childDataSnapshot.Child("colour").Value.ToString();

                    string[] values = {name, count, color};
                    valuesCategories.Add(values);
                }
                _dataReceived = true;
            }
        });
    }

    public void generateCategories() {
        for (int i = 0; i < valuesCategories.Count; i++) {
            GameObject a = Instantiate(myPrefab); 
            a.transform.SetParent(content.transform, false);
            generateTextCategories(i);
        }
    }

    private void generateTextCategories(int i) { 
        Transform obj = content.GetChild(content.childCount - 1);
        GameObject nameO = obj.Find("CategoryName").gameObject;
        GameObject countO = obj.Find("CategoryCount").gameObject;
        GameObject colorO = obj.Find("CategoryColor").gameObject;

        nameO.GetComponent<TextMeshProUGUI>().text = valuesCategories[i][0];
        countO.GetComponent<TextMeshProUGUI>().text = valuesCategories[i][1];
        colorO.GetComponent<Image>().color = Color.cyan;

    }
}




