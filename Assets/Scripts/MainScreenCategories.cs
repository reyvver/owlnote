using System;
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
    public GameObject myPrefab;
    public Transform content;
    public TextMeshProUGUI nameCategory;
    public Text colour;

    private bool _dataReceived;

    private List<string[]> valuesCategories = new List<string[]>();
    private DatabaseReference categoriesRef;
    private FirebaseUser currentUser;

    public class categoryClass
    {
        public int count;
        public string colour;

        public categoryClass(int count, string colour)
        {
            this.count = count;
            this.colour = colour;
        }
    }

    void Start()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");

        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        
        categoriesRef = FirebaseDatabase.DefaultInstance.GetReference("/categories/" + currentUser.UserId);

        categoriesRef.ValueChanged += HandleValueChanged;
    }

    private void OnDestroy()
    {
        categoriesRef.ValueChanged -= HandleValueChanged;
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
           // Debug.LogError(args.DatabaseError.Message);
            return;
        }
        //Debug.Log("handle value changed : category");
        ClearContent();

        DataSnapshot snapshot = args.Snapshot;

        if (snapshot.ChildrenCount == 0)
            AddDefaultCategory();
        else
        {
            foreach (DataSnapshot childDataSnapshot in snapshot.Children)
            {
                string name = childDataSnapshot.Key;
                string color = (string) childDataSnapshot.GetValue(true);

                string[] values = {name, color};

                valuesCategories.Add(values);
            }

            GenerateCategories();
        }

    }
    
      
    private void AddDefaultCategory()
    {
        categoriesRef.Child("По умолчанию").SetValueAsync("#9792F7");
    }


    public void AddNewCategory()
   {
       categoriesRef.Child(nameCategory.text).SetValueAsync(colour.text);
       ClearTextBoxes();
       AddNewCategoryPanel.SetActive(false);
  }
   
    private void GenerateCategories()
    {
        for (int i = 0; i < valuesCategories.Count; i++)
        {
          /*  GameObject a = Instantiate(myPrefab);
            a.transform.SetParent(content.transform, false);
            generateTextCategories(i);*/
          // GameObject nameObj = currentObject.Find("CategoryName").gameObject;
          // GameObject colorObj = currentObject.Find("CategoryColor").gameObject;

          GameObject a = Instantiate(myPrefab);
          Transform currentObject = a.transform;
          
          Transform colorObj = currentObject.GetChild(0);
          Transform nameObj = currentObject.GetChild(1);
          
          nameObj.GetComponent<TextMeshProUGUI>().text = valuesCategories[i][0];
          if (ColorUtility.TryParseHtmlString(valuesCategories[i][1], out var newCol))
          {
              colorObj.GetComponent<Image>().color = newCol;
          }
          currentObject.SetParent(content.transform, false);
          
        }
    }

    private void generateTextCategories(int i)
    {
        Color newCol;

        Transform obj = content.GetChild(content.childCount - 1);
        GameObject nameO = obj.Find("CategoryName").gameObject;
        GameObject colorO = obj.Find("CategoryColor").gameObject;

        nameO.GetComponent<TextMeshProUGUI>().text = valuesCategories[i][0];

        if (ColorUtility.TryParseHtmlString(valuesCategories[i][1], out newCol))
        {
            colorO.GetComponent<Image>().color = newCol;
        }
    }

    private void ClearTextBoxes()
    {
        nameCategory.text = "";
        colour.text = "#000000";
    }

    private void ClearContent()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        valuesCategories.Clear();
    }

}




