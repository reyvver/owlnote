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

    public GameObject AddNewCategoryPanel, ConfirmDeleting;
    public GameObject myPrefab;
    public Transform content;
    public TextMeshProUGUI name, textConfirmDelete;
    public Text colour;
    public string choosenCategory = "", categoryCount;
    public bool _delete;
    
    private bool _dataReceived;

    private List<string[]> valuesCategories = new List<string[]>();
    private DatabaseReference  categoriesRef;
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

    private void Update()
    {
        if(_delete)
        {
            textConfirmDelete.text = choosenCategory;
            ConfirmDeleting.gameObject.SetActive(true);
        }
    }

    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");

        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

        categoriesRef = FirebaseDatabase.DefaultInstance.GetReference(currentUser.UserId + "/categories");
      
        categoriesRef.ValueChanged += HandleValueChanged;
    }
    
    private void OnDestroy()
    {
        categoriesRef.ValueChanged -= HandleValueChanged;
    }
    
   void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        
        Debug.Log("handle value changed");

        ClearContent();
        
        DataSnapshot snapshot = args.Snapshot;

        foreach (DataSnapshot childDataSnapshot in snapshot.Children)
        {
            string name = childDataSnapshot.Key;
            if (name != "calendar")
            {
                string count = childDataSnapshot.Child("count").Value.ToString();
                string color = childDataSnapshot.Child("colour").Value.ToString();

                string[] values = {name, count, color};
                valuesCategories.Add(values);
            }
        }

        generateCategories();

        
    }
   
   public void AddNewCategory()
   {
       categoryClass newObject = new categoryClass(0, colour.text);
       string json = JsonUtility.ToJson(newObject);
       categoriesRef.Child(name.text).SetRawJsonValueAsync(json);
       ClearTextBoxes();
       AddNewCategoryPanel.SetActive(false);
  }

   public void DeleteCategory()
    {
        categoriesRef.Child(choosenCategory).RemoveValueAsync();
        _delete = false;
        ConfirmDeleting.gameObject.SetActive(false);
    }
   
    private void generateCategories()
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
    
    
    public void CloseConfirmPanel()
    {
        _delete = false;
        ConfirmDeleting.gameObject.SetActive(false);
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

}




