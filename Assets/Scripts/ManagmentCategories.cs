using System;
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
    private float startPositionX = -334, startPositionY = 225, space = 0; 
    
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
    public void UpdateCategoriesList()
    {
        try
        {
            for (int i = 0; i < currentCount; i++)
            {
                myPrefab.gameObject.transform.Find("CategoryName").GetComponent<TextMeshProUGUI>().text =
                    GetValues(i, "name");
                myPrefab.gameObject.transform.Find("CategoryDetailes").GetComponent<TextMeshProUGUI>().text =
                    GetValues(i, "description");
                myPrefab.gameObject.transform.Find("CategoryColor").GetComponent<Image>().color = Color.blue;
                
                GameObject myobj = Instantiate(myPrefab, new Vector3(startPositionX, startPositionY-space, 0), Quaternion.identity) as GameObject;
                myobj.transform.SetParent(panel.transform,false);
                space += 255;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
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
                    Debug.Log(snapshot.Value);
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
