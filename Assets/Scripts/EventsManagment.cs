using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventsManagment : MonoBehaviour
{
    public TextMeshProUGUI newIventDate, newIventCategory, newIventTitle, newIventStartTime, newIventEndTime;
    public GameObject CategoryPicker, TimePicker, TimePanel, Success, EmptySchedule;
    private MainScreenSceneManager mainScriptSceneManager;
    private MainScreenScript mainScript;
    
    private FirebaseUser currentUser;
    
    public GameObject myPrefab;
    public Transform content;
    private List<string[]> valuesEvents = new List<string[]>();
    
    //
    private GameObject objSceneManager;
    
    private ScrollRectScript time;
    private DatabaseReference calendarRef, calendarDayRef;
    private string typeTime;

    public DateTime dateToday;

    private bool _dataReceived;

    public class IventClass
    {
        public string title, end_time, category, colour;

        public IventClass(string title, string end_time, string category, string colour)
        {
            this.title = title;
            this.end_time = end_time;
            this.category = category;
            this.colour = colour;
        }
    }

    private void Awake()
    {
        objSceneManager = GameObject.Find("SceneManager");
        mainScript = objSceneManager.GetComponent<MainScreenScript>();
        mainScriptSceneManager = objSceneManager.GetComponent<MainScreenSceneManager>();
    }

    void Start()
    {
        dateToday = DateTime.Today;
        //  mainScript = GameObject.Find("SceneManager").GetComponent<MainScreenScript>();
      //  mainScriptSceneManager = GameObject.Find("SceneManager").GetComponent<MainScreenSceneManager>();
        time = TimePicker.GetComponent<ScrollRectScript>();
        InitializeDatabase();
    }

    private void Update()
    {
        newIventDate.text = dateToday.ToString("dd/MM/yyyy", new CultureInfo("ru-RU"));
    }

    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");
        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        calendarRef =  FirebaseDatabase.DefaultInstance.GetReference( "/calendar/"+ currentUser.UserId);
        calendarRef.ValueChanged += HandleValueChanged;
    }
    private void OnDestroy()
    {
        try
        {
            calendarRef.ValueChanged -= HandleValueChanged;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            //Debug.LogError(args.DatabaseError.Message);
            return;
        }

        //Debug.Log("handle value changed :event");

        ClearContent();

        DataSnapshot snapshot = args.Snapshot;

        string date = ReplaceWith(dateToday.ToString("dd/MM/yyyy", new CultureInfo("ru-RU")));  
        snapshot = snapshot.Child(date);

        if (CheckEmptySchedule(snapshot.ChildrenCount))
        {
            foreach (DataSnapshot childDataSnapshot in snapshot.Children)
            {
                string start_time = childDataSnapshot.Key;
                string title = childDataSnapshot.Child("title").Value.ToString();
                string colour = childDataSnapshot.Child("colour").Value.ToString();
                string end_time = childDataSnapshot.Child("end_time").Value.ToString();

                string[] values = {title, start_time, end_time, colour};
                valuesEvents.Add(values);
            }

            generateEvents();
    }

    }

    private bool CheckEmptySchedule(long count)
    {
        if (count ==0)
        {
            EmptySchedule.SetActive(true);
            return true;
        }
        else
        {
            EmptySchedule.SetActive(false);
            return true;
        }
    }

    private void generateEvents()
    {
        if (CheckEmptySchedule(valuesEvents.Count))
        {
            for (int i = 0; i < valuesEvents.Count; i++)
            {
                /*GameObject a = Instantiate(myPrefab);
                 a.transform.SetParent(content.transform, false);
                 generateTextEvents(i);*/
                
                GameObject a = Instantiate(myPrefab);
                Transform currentObject = a.transform;
                
                Transform colorObj = currentObject.GetChild(0);
                Transform titleObj = currentObject.GetChild(1);
                Transform startTime =  currentObject.GetChild(4).GetChild(0);
                Transform endTime =  currentObject.GetChild(4).GetChild(2);
                
                titleObj.GetComponent<TextMeshProUGUI>().text = valuesEvents[i][0];
                startTime.GetComponent<TextMeshProUGUI>().text = valuesEvents[i][1];
                endTime.GetComponent<TextMeshProUGUI>().text = valuesEvents[i][2];
                
                if (ColorUtility.TryParseHtmlString(valuesEvents[i][3], out var newCol))
                {
                    colorObj.GetComponent<Image>().color = newCol;
                }
          
                currentObject.SetParent(content.transform, false);

            }
        }
    }

    private void generateTextEvents(int i)
    {
        Color newCol;

        Transform obj = content.GetChild(content.childCount - 1);
        
        GameObject title = obj.Find("IventTitle").gameObject;
        GameObject timeStart = obj.Find("TimePanel").Find("TimeStart").gameObject;
        GameObject timeEnd = obj.Find("TimePanel").Find("TimeEnd").gameObject;
        GameObject colour = obj.Find("IventColour").gameObject;

        title.GetComponent<TextMeshProUGUI>().text = valuesEvents[i][0];
        timeStart.GetComponent<TextMeshProUGUI>().text = valuesEvents[i][1];
        timeEnd.GetComponent<TextMeshProUGUI>().text = valuesEvents[i][2];
        
        if (ColorUtility.TryParseHtmlString(valuesEvents[i][3], out newCol))
        {
            colour.GetComponent<Image>().color = newCol;
        }
    }
    

    public void SelectCategory()
    {
        newIventCategory.text = mainScript.chosenCategory;
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

    public void AddNewEvent()
    {
        string selectedColour = mainScript.categoryColour;
        IventClass newObject = new IventClass(newIventTitle.text,newIventEndTime.text, newIventCategory.text, selectedColour);
        
        string json = JsonUtility.ToJson(newObject);
        string date = ReplaceWith(newIventDate.text);
        
        calendarRef.Child(date).Child(newIventStartTime.text).SetRawJsonValueAsync(json);
        Success.transform.SetAsLastSibling();
        mainScriptSceneManager.ShowPanel(Success);
        ClearInputs();
    }
    
    private void ClearInputs()
    {
        newIventTitle.text = "";
        newIventStartTime.text = "Не выбрано";
        newIventEndTime.text = "Не выбрано";
        newIventCategory.text = "Выберите категорию...";
    }

    private void ClearContent()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
 
        }
        valuesEvents.Clear();
    }

    private string ReplaceWith(string str)
    {
        string result = "";

             if (str.Contains(@"/"))
             {
                 result = str.Replace("/", @"\");
             }
             else
             {
                 result = str.Replace(".", @"\");
             }
             return result;
    
    }

    public void UpdateContent()
    {
        ClearContent();
   
        string date = ReplaceWith(dateToday.ToString("dd/MM/yyyy", new CultureInfo("ru-RU")));  
        calendarDayRef =  FirebaseDatabase.DefaultInstance.GetReference( "/calendar/"+ currentUser.UserId + "/"+ date).Reference;
        StartCoroutine(viewIvents());
    }
    
    
    IEnumerator viewIvents()
    {
       // Debug.Log(dateToday);
        getData();
        yield return new WaitUntil(() => _dataReceived);
        generateEvents();
    }
    
    private void getData()
    {
        _dataReceived = false;

        calendarDayRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
               // Debug.Log("error occured");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                
                foreach (DataSnapshot childDataSnapshot in snapshot.Children)
                {
                    string start_time = childDataSnapshot.Key;
                    string title = childDataSnapshot.Child("title").Value.ToString();
                    string colour = childDataSnapshot.Child("colour").Value.ToString();
                    string end_time = childDataSnapshot.Child("end_time").Value.ToString();

                    string[] values = {title, start_time , end_time, colour};
                    valuesEvents.Add(values);
                }
                _dataReceived = true;
            }
        });
    }

}
