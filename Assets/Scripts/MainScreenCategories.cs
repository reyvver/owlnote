using System;
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

    public GameObject AddNewCategoryPanel, ConfirmDeleting;
    public GameObject ButtonAdd;
    public GameObject myPrefab;
    public Transform content;
    public TextMeshProUGUI name, textConfirmDelete;
    public Text colour;
    public string choosenCategory = "";
    public bool _delete;
    
    private bool _dataReceived;

    private List<string[]> valuesCategories = new List<string[]>();
    private DatabaseReference myRef;

    public class categoryClass
    {
        public int count;
        public string colour;

        public categoryClass()
        {
        }

        public categoryClass(int count, string colour)
        {
            this.count = count;
            this.colour = colour;
        }
    }

    void Start()
    {
       // InitializeDatabase();
    // StartCoroutine(viewCategories());
    }

    void Update()
    {
        if (_delete)
        {
            textConfirmDelete.text = choosenCategory;
            ConfirmDeleting.gameObject.SetActive(true);
        }
    }

    public void CloseConfirmPanel()
    {
        _delete = false;
        ConfirmDeleting.gameObject.SetActive(false);
    }

    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");
        myRef = FirebaseDatabase.DefaultInstance.RootReference;
    }


    public void DatabaseOperations(string operation)
    {
        switch (operation)
        {
            case "adding":
            {
                AddNewCategory();
                break;
            }
            case "deleting":
            {
                DeleteCategory();
                break;
            }
        }
    }

    private void AddNewCategory()
    {
        categoryClass newObject = new categoryClass(0, colour.text);
        string json = JsonUtility.ToJson(newObject);
        myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Child(name.text)
            .SetRawJsonValueAsync(json);
        ClearTextBoxes();
        ClearContent();
        AddNewCategoryPanel.SetActive(false);
        StartCoroutine(viewCategories());
    }

    private void ClearTextBoxes()
    {
        name.text = "";
        colour.text = "#000000";
    }

    private void ClearContent()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
            valuesCategories.Clear();
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

    public void generateCategories()
    {
        for (int i = 0; i < valuesCategories.Count; i++)
        {
            GameObject a = Instantiate(myPrefab);
            a.transform.SetParent(content.transform, false);
            generateTextCategories(i);
        }
    }

    private void generateTextCategories(int i)
    {
        Color newCol;

        Transform obj = content.GetChild(content.childCount - 1);
        GameObject nameO = obj.Find("CategoryName").gameObject;
        GameObject countO = obj.Find("CategoryCount").gameObject;
        GameObject colorO = obj.Find("CategoryColor").gameObject;

        nameO.GetComponent<TextMeshProUGUI>().text = valuesCategories[i][0];
        countO.GetComponent<TextMeshProUGUI>().text = valuesCategories[i][1];

        if (ColorUtility.TryParseHtmlString(valuesCategories[i][2], out newCol))
        {
            colorO.GetComponent<Image>().color = newCol;
        }
    }

    private void DeleteCategory()
    {
        myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Child(choosenCategory)
            .RemoveValueAsync();
        ClearContent();
        StartCoroutine(viewCategories());
        _delete = false;
        ConfirmDeleting.gameObject.SetActive(false);
    }

}




