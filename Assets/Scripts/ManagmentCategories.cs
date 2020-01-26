using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ManagmentCategories : MonoBehaviour
{
    private class Category
    {
        public string color, description;

        public Category()
        {
        }

        public Category(string color, string description)
        {
            this.color = color;
            this.description = description;
        }
    }

    public GameObject AddNewCategoryPanel, HideOtherStaff;

    public GameObject ButtonAdd;

    public GameObject myPrefab;
    public Transform panel;
    private float startPositionX = -17, startPositionY = 700, space = 0;
    public TextMeshProUGUI name, color, description;

    private bool HideOrShow = false;
    private List<string[]> valuesCategories = new List<string[]>();
    private int currentCount = -1;


    private DatabaseReference myRef;

    void Start()
    {
        ViewCategories();
        AddNewCategoryPanel.SetActive(HideOrShow);
        HideOtherStaff.SetActive(HideOrShow);
        InitializeDatabase();
    }

    /*Соединяется с БД*/
    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");
        myRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /*Операции с данными*/
    public void DatabaseOperations(string operation)
    {
        switch (operation)
        {
            case "adding":
            {
                AddNewCategory();
                break;
            }
            case "editing":
            {
                ShowAddMenu();
                break;
            }
            case "deleting":
            {
                break;
            }
        }

    }

    /*Добавить новую категорию в бд*/
    private void AddNewCategory()
    {
        Category newCategory = new Category(color.text, description.text);
        string json = JsonUtility.ToJson(newCategory);
        myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Child(name.text)
            .SetRawJsonValueAsync(json);
        ShowAddMenu();
        ClearTextBoxes();
    }

    private void ClearTextBoxes()
    {
        name.text = "";
        description.text = "";
        color.text = "красный";
    }

    public void ShowAddMenu()
    {
        HideOrShow = !HideOrShow;
        AddNewCategoryPanel.SetActive(HideOrShow);
        HideOtherStaff.SetActive(HideOrShow);

        if (HideOrShow)
        {
            ButtonAdd.transform.Rotate(0, 0, -45);
        }
        else
        {
            ButtonAdd.transform.Rotate(0, 0, 45);
        }
    }

    public async void ViewCategories()
    {
        //await Task.Run(() => GetCount()); 
        //Debug.Log(currentCount + " s");
        await Task.Run(() => GetData());
        Debug.Log(valuesCategories.Count);
        GenerateCategories();
    }

    public void GetCount()
    {
        DatabaseReference newRef = myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories")
            .Reference;
        newRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                currentCount = Convert.ToInt32(snapshot.ChildrenCount);
            }
        });
        Thread.Sleep(200);
    }

    private void GetData()
    {
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
                foreach (DataSnapshot childDataSnapshot in snapshot.Children)
                {
                    string name = childDataSnapshot.Child("name").Value.ToString();
                    string description = childDataSnapshot.Child("description").Value.ToString();
                    string color = childDataSnapshot.Child("color").Value.ToString();

                    string[] values = {name, description, color};
                    valuesCategories.Add(values);
                }
            }
        });
        Thread.Sleep(300);
    }

    public void GenerateCategories()
    {
        for (int i = 0; i < valuesCategories.Count; i++)
        {
            GameObject myobj = Instantiate(myPrefab, new Vector3(startPositionX, (startPositionY - space), 0),
                Quaternion.identity) as GameObject;
            myobj.transform.SetParent(panel.transform, false);
            space += 260;
            GenerateTextCategories(i);
        }
    }

    private void GenerateTextCategories(int i)
    {
        Transform obj = panel.GetChild(panel.childCount - 1);
        GameObject nameO = obj.Find("CategoryName").gameObject;
        GameObject descriptionO = obj.Find("CategoryDescription").gameObject;
        GameObject colorO = obj.Find("CategoryColor").gameObject;

        nameO.GetComponent<TextMeshProUGUI>().text = valuesCategories[i][0];
        descriptionO.GetComponent<TextMeshProUGUI>().text = valuesCategories[i][1];
        colorO.GetComponent<Image>().color = Color.cyan;

    }
}




