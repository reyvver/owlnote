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

    private List<int> categoriesId = new List<int>();
    private bool HideOrShow = false;
    private int currentCount;
    private float startPositionX = -17, startPositionY = 700, space = 0; 
    
    private DatabaseReference myRef;

    void Start()
    { 
       AddNewCategoryPanel.SetActive(HideOrShow);
       HideOtherStaff.SetActive(HideOrShow);
       InitializeDatabase();
       GetCurrentCount();
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
            myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Child(currentCount.ToString()).SetRawJsonValueAsync(json);
            UpdateCurrentCount(currentCount++);
            ShowAddMenu();
            ClearTextBoxes();
        }
    }
    /*Узнать количество категорий*/
    private void GetCurrentCount()
    {
        myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories")
            .Child("countCategories")
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    currentCount = -1;
                }
                else if (task.IsCompleted) {
                    DataSnapshot snapshot = task.Result;
                    currentCount = Convert.ToInt32(snapshot.Value);
                }
            });
    }
    /*Возвращает все id*/
    public void GetCategoriesId()
    {
        DatabaseReference rf = myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories");
        rf.GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    currentCount = -1;
                }
                else if (task.IsCompleted) {
                    DataSnapshot snapshot = task.Result;
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        var id = childSnapshot.Key;
                        Regex rg = new Regex(@"\D");
                        if (!rg.IsMatch(id))
                        {
                            categoriesId.Add(Convert.ToInt32(id));
                        }
                    }
                    Debug.Log(categoriesId.Count);
                }
            });
        Debug.Log(categoriesId.Count);
    }
    /*Обновить после выполнения операции добавления или удаления*/
    private void UpdateCurrentCount(int newNumber)
    {
        myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Child("countCategories")
            .SetValueAsync(currentCount).ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log("me");
                }
                else if (task.IsCompleted) {
                   Debug.Log("nice");
                }
            });
    }
    /*Очистить поля после выполнения операции*/
    private void ClearTextBoxes()
    {
        name.text = "";
        description.text = "";
        color.text = "красный";
    }
    /*Визуализация*/
    public void ViewCategories()
    {
       // categoriesId.Clear();
        GetCategoriesId();
        GenerateCategories();
    }

    private void GenerateCategories()
    {

        try
        {
            for (int i = 0; i < currentCount; i++)
            {
                GameObject myobj = Instantiate(myPrefab, new Vector3(startPositionX, (startPositionY-space), 0), Quaternion.identity) as GameObject;
                myobj.transform.SetParent(panel.transform,false);
                space += 260;
            }

          //  GenerateTextCategories();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    private void GenerateTextCategories()
    {  
        GameObject panel = GameObject.Find("ListOfCategories");
        int i = 0;
        foreach (Transform obj in  panel.transform)
        {
            GameObject name = obj.Find("CategoryName").gameObject;
            GameObject description = obj.Find("CategoryDescription").gameObject;
            GameObject color = obj.Find("CategoryColor").gameObject;

            name.GetComponent<TextMeshProUGUI>().text = GetValues(categoriesId[i], "name");
            description.GetComponent<TextMeshProUGUI>().text = GetValues(categoriesId[i], "description");
            color.GetComponent<Image>().color = Color.cyan;
            i++;
        }
    }
    /*Получить значение свойства текущей категории*/
    private string GetValues(int id,string propety)
    {
        string result = "empty";
        myRef.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("categories").Child(id.ToString()).Child(propety)
            .GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log("error occured");
                }
                else if (task.IsCompleted) {
                    DataSnapshot snapshot = task.Result;
                    result = snapshot.Value.ToString();
                }
            });
        return result;
    }
    /*Показать/скрыть меню добавления*/
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
}
