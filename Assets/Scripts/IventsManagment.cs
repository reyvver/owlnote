using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using TMPro;
using Object = System.Object;

public class IventsManagment : MonoBehaviour
{
    public TextMeshProUGUI newIventDate, newIventCategory, newIventTitle, newIventStartTime, newIventEndTime, newIventDescription;
    public GameObject CategoryPicker, TimePicker, TimePanel, Success;
    private MainScreenCategories CategoriesScript;
    private MainScreenSceneManager mainScriptSceneManager;
    private ScrollRectScript time;
    private DatabaseReference calendarRef, categoriesRef;
    private string typeTime;
    // Start is called before the first frame update
    
    
    public class IventClass
    {
        public string title, description, end_time;

        public IventClass(string title, string description, string end_time)
        {
            this.title = title;
            this.description = description;
            this.end_time = end_time;
        }
    }
    
    void Start()
    {
        CategoriesScript = GameObject.Find("SceneManager").GetComponent<MainScreenCategories>();
        mainScriptSceneManager = GameObject.Find("SceneManager").GetComponent<MainScreenSceneManager>();
        time = TimePicker.GetComponent<ScrollRectScript>();
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");
        FirebaseUser currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        calendarRef =  FirebaseDatabase.DefaultInstance.GetReference(currentUser.UserId + "/calendar");
        categoriesRef = FirebaseDatabase.DefaultInstance.GetReference(currentUser.UserId + "/categories");
    }
    
    
    public void SelectCategory()
    {
        Debug.Log(CategoriesScript.choosenCategory);
          newIventCategory.text = CategoriesScript.choosenCategory;
          newIventCategory.color = new Color32(146,96,255,255);
          CategoryPicker.SetActive(false);
    }
    public void SelectTime()
    {
        if(typeTime == "start")
        {
            newIventStartTime.text = time.hours + ":" + time.minutes;
        }
        else newIventEndTime.text  = time.hours + ":" + time.minutes;
        
        TimePanel.SetActive(false);
    }
    public void ShowTimePanel(string type)
    {
        TimePanel.gameObject.SetActive(true);
        mainScriptSceneManager.openedPanels.Add(TimePanel);
        typeTime = type;
    }
    public void CloseTimePanel()
    {
        mainScriptSceneManager.openedPanels.RemoveAt(mainScriptSceneManager.openedPanels.Count - 1);
        TimePanel.gameObject.SetActive(false);
    }

    public void AddNewIvent(GameObject obj)
    {
        IventClass newObject = new IventClass(newIventTitle.text,newIventDescription.text,newIventEndTime.text);
        string json = JsonUtility.ToJson(newObject);
        Debug.Log(newIventDate.text+ "  "+newIventStartTime.text );
        calendarRef.Child(newIventDate.text).Child(newIventStartTime.text).SetRawJsonValueAsync(json);
        UpdateCategoryCount();
        Success.transform.SetAsLastSibling();
        mainScriptSceneManager.ShowPanel(Success);
        ClearContent();
    }

    private void UpdateCategoryCount()
    {
        DatabaseReference updateCount = categoriesRef.Child(newIventCategory.text);
        int newCount = Convert.ToInt32(CategoriesScript.categoryCount)+1;
        Dictionary<string, Object> op  =  new Dictionary<string, Object>();
        op["/count/"] = newCount;
        updateCount.UpdateChildrenAsync(op);
    }

    private void ClearContent()
    {
        newIventTitle.text = "";
        newIventDescription.text = "";
        newIventStartTime.text = "Не выбрано";
        newIventEndTime.text = "Не выбрано";
        newIventCategory.text = "Выберите новую категорию";
    }

}
