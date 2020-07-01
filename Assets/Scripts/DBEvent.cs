using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;


public class DBEvent : MonoBehaviour
{
    private Dictionary<string, List<MEvent>> eventsValues = new Dictionary<string, List<MEvent>>();
    private static DatabaseReference _reference;

    public class EventClass
    {
        public string title, endTime, categoryName, categoryColour, description;

        public EventClass(string name, string end_time, string category, string colour, string details)
        {
            title = name;
            endTime = end_time;
            categoryName = category;
            categoryColour = colour;
            description = details;
        }
    }

    private void Start()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://owlnote-dragunovatv.firebaseio.com/");
        _reference = FirebaseDatabase.DefaultInstance.GetReference(
                "/calendar/" + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        _reference.ValueChanged += HandleValueChanged;
        Debug.Log("done events");
    }

    private void OnDestroy()
    {
        try
        {
            _reference.ValueChanged -= HandleValueChanged;
        }
        catch
        {
            Debug.Log("Empty events");
        }
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        eventsValues.Clear();

        DataSnapshot snapshot = args.Snapshot;


        foreach (DataSnapshot childDataSnapshot in snapshot.Children)
        {
            string date = ReplaceWith(childDataSnapshot.Key);
            List<MEvent> dateEvents = new List<MEvent>();

            if (childDataSnapshot.ChildrenCount > 0)
                foreach (DataSnapshot events in childDataSnapshot.Children)
                {
                    MEvent currentEvent = new MEvent()
                    {
                        Title = events.Child("title").Value.ToString(),
                        CategoryName = events.Child("categoryName").Value.ToString(),
                        CategoryColour = events.Child("categoryColour").Value.ToString(),
                        StartTime = events.Key,
                        EndTime = events.Child("endTime").Value.ToString(),
                        Description = events.Child("description").Value.ToString()
                    };
                    if (eventsValues.ContainsKey(date))
                    {
                        eventsValues[date].Add(currentEvent);
                    }
                    else
                    {
                        dateEvents.Add(currentEvent);
                        eventsValues.Add(date, dateEvents);
                    }
                }
        }

        ViewModel.SetEventsValues(eventsValues);
    }

    public static void DBEventDelete(string date, string key)
    {
        string dateEvent = ReplaceWith(date);
        _reference.Child(dateEvent).Child(key).RemoveValueAsync();
    }
    public static void DBEventAdd(string date, Dictionary<string, string> newEvent)
    {
        string dateEvent = ReplaceWith(date);
        string key = newEvent["startTime"];
        newEvent.Remove("startTime");

        string json = JsonUtility.ToJson(ToCurrentClass(newEvent));
        _reference.Child(dateEvent).Child(key).SetRawJsonValueAsync(json);
    }
    public static void DBEventAdd(string date, MEvent newEvent)
    {
        string dateEvent = ReplaceWith(date);
        string key = newEvent.StartTime;

        string json = JsonUtility.ToJson(ToCurrentClass(newEvent));
        _reference.Child(dateEvent).Child(key).SetRawJsonValueAsync(json);
    }
    
    private static EventClass ToCurrentClass(Dictionary<string, string> values)
    {
        string title = values["title"];
        string categoryName = values["categoryName"];
        string categoryColour = values["categoryColour"];
        string endTime = values["endTime"];
        string description = "";
        if (values.ContainsKey("description"))
        {
            description = values["description"];
        }

        EventClass newEvent = new EventClass(title, endTime, categoryName, categoryColour, description);
        return newEvent;
    }
    private static EventClass ToCurrentClass(MEvent values)
    {
        string title = values.Title;
        string categoryName = values.CategoryName;
        string categoryColour = values.CategoryColour;
        string endTime = values.EndTime;
        string description = values.Description;

        EventClass newEvent = new EventClass(title, endTime, categoryName, categoryColour, description);
        return newEvent;
    }


    private static string ReplaceWith(string str)
    {
        string result = "";

        result = str.Replace(str.Contains(@"/") ? "/" : ".", @"\");

        return result;
    }

   
}
