using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagmentCategories : MonoBehaviour
{
    private class Category
    {
        public string name, color, description;
        public Category() {
        }

        public Category(string name, string color, string description)
        {
            this.name = name;
            this.color = color;
            this.description = description;
        }
    }

    public GameObject AddNewCategoryPanel, HideOtherStaff;
    public GameObject ButtonAdd;
    public GameObject myPrefab;
    public Transform panel;
    public TextMeshProUGUI name, color, description;
    
    private bool HideOrShow = false;
    private int currentCount;
    private float startPositionX = -17, startPositionY = 700, space = 0; 
    
    private DatabaseReference myRef;

    void Start()
    { 
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
        if (currentCount>= 0)
        {
            Category newCategory = new Category( name.text, color.text, description.text);
            string json = JsonUtility.ToJson(newCategory);
            myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Push()
                .SetRawJsonValueAsync(json);
            ShowAddMenu();
            ClearTextBoxes();
        }
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
            ButtonAdd.transform.Rotate(0,0,-45);
        }
        else
        {
            ButtonAdd.transform.Rotate(0,0,45);
        }
    }

    public void ViewCategories()
    {
        GetCount();
        Debug.Log(currentCount);
    }

    private void GetCount()
    {
        DatabaseReference newRef = myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Reference;
        newRef.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("error occured");
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                currentCount = Convert.ToInt32(snapshot.ChildrenCount);
            }
        });
    }


}
