using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainScreenNotes : MonoBehaviour
{

    public GameObject AddNewNotePanel, Success;
    public GameObject myPrefab;
    public Transform content;
    public TextMeshProUGUI noteText;
    
    private List<string[]> valuesNotes = new List<string[]>();
    private DatabaseReference notesRef, noteDayRef;
    private FirebaseUser currentUser;

    private MainScreenSceneManager mainScriptSceneManager;
    private EventsManagment script;
    private MainScreenScript mainScript;

    private bool _dataReceived;
    
    private void Awake()
    {
        mainScript = GameObject.Find("SceneManager").GetComponent<MainScreenScript>();
        mainScriptSceneManager = GameObject.Find("SceneManager").GetComponent<MainScreenSceneManager>();
        script = GameObject.Find("SceneManager").GetComponent<EventsManagment>();
    }

    void Start()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");

        currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        
        notesRef = FirebaseDatabase.DefaultInstance.GetReference("/notes/" + currentUser.UserId);
        notesRef.ValueChanged += HandleValueChanged;
    }

    private void OnDestroy()
    {
        notesRef.ValueChanged -= HandleValueChanged;
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
           // Debug.LogError(args.DatabaseError.Message);
            return;
        }
        ClearContent();
        DataSnapshot snapshot = args.Snapshot;
        
        string date = ReplaceWith(mainScript.newIventDate.text);
        snapshot = snapshot.Child(date);

        if(snapshot.ChildrenCount > 0) foreach (DataSnapshot childDataSnapshot in snapshot.Children)
        {
            string name = childDataSnapshot.Key;

            string text = (string) childDataSnapshot.GetValue(true);
            string[] values = {name, text};

            valuesNotes.Add(values);
        }
        GenerateNotes();
        mainScriptSceneManager.CheckEmptyTimetable();
    }


    public void AddNewNote()
    {
        string date = ReplaceWith(mainScript.newIventDate.text);
        string key = notesRef.Child(date).Push().Key;
        notesRef.Child(date).Child(key).SetValueAsync(noteText.text);
        Success.transform.SetAsLastSibling();
        mainScriptSceneManager.ShowPanel(Success);
        ClearTextBoxes();
        AddNewNotePanel.SetActive(false);
    }

    private void GenerateNotes()
    {
        for (int i = 0; i < valuesNotes.Count; i++)
        { 
            Debug.Log(valuesNotes[i][0]);
            GameObject a = Instantiate(myPrefab);
          Transform currentObject = a.transform;

          Transform key = currentObject.GetChild(0);
          Transform text = currentObject.GetChild(1).GetChild(0);
          
          key.GetComponent<TMP_Text>().text = valuesNotes[i][0];
          text.GetComponent<TMP_Text>().text = valuesNotes[i][1];

          currentObject.SetParent(content.transform, false);
        }
    }
    
    private void ClearTextBoxes()
    {
        noteText.text = "";
    }

    private void ClearContent()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        valuesNotes.Clear();
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

        DateTime dateToday = script.dateToday;
        string date = ReplaceWith(dateToday.ToString("dd/MM/yyyy", new CultureInfo("ru-RU")));
        Debug.Log(date);
        noteDayRef =  FirebaseDatabase.DefaultInstance.GetReference( "/notes/"+ currentUser.UserId + "/"+ date).Reference;
        StartCoroutine(viewNotes());
    }
    
    
    IEnumerator viewNotes()
    {
        getData();
        yield return new WaitUntil(() => _dataReceived);
        GenerateNotes();
        mainScriptSceneManager.CheckEmptyTimetable();
    }
    
    private void getData()
    {
        _dataReceived = false;

        noteDayRef.GetValueAsync().ContinueWith(task =>
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
                    string name = childDataSnapshot.Key;

                    string text = (string) childDataSnapshot.GetValue(true);
                    string[] values = {name, text};

                    valuesNotes.Add(values);
                }
                _dataReceived = true;
            }
        });
    }
    

}




