using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewModel : MonoBehaviour
{
    public static Transform EventsContainer, NotesContainer, CategoriesContainer, ToDoObjectsContainer, ToDoContainer;

    public static GameObject EventsHeader, NotesHeader, ToDoHeader;

    public static GameObject EventPrefab, NotePrefab, CategoryPrefab, ToDoObjectPrefab, ToDoItemPrefab,  TodoListPrefab;

    public static GameObject EmptySchedulePanel;
    public static GameObject EmailVerifyPanel;
    public static TextMeshProUGUI emailVerify;

    public static string dateSelected;
    public static string selectedCategoryName, selectedCategoryColour, currentKey;
    
    
    private static Dictionary<string, List<MEvent>> eventsValues = new Dictionary<string, List<MEvent>>();
    private static Dictionary<string, List<MEvent>> EventsValues
    {
        get { return eventsValues; }
        set
        {
            eventsValues = value;
        }
    }
    
    
    
    private static Dictionary<string, string> categoriesValues = new Dictionary<string,  string>();
    private static Dictionary<string, string> CategoriesValues
    {
        get { return categoriesValues; }
        set
        {
            categoriesValues = value;
        }
    }


    
    private static Dictionary<string, List<MNote>> notesValues = new Dictionary<string, List<MNote>>();
    private static Dictionary<string, List<MNote>> NotesValues
    {
        get { return notesValues; }
        set
        {
            notesValues = value;
        }
    }

    private static int toDoObjectsCount ;
    private static Dictionary<string, string> toDoObjects = new Dictionary<string, string>();

    
    private static Dictionary<string, List<MTodo>> todoValues = new Dictionary<string, List<MTodo>>();
    private static Dictionary<string, List<MTodo>> ToDoValues
    {
        get { return todoValues; }
        set
        {
            todoValues = value;
        }
    }


    private static bool events, notes, lists;
    

    private void Start()
    {
        selectedCategoryName = "По умолчанию";
        selectedCategoryColour = "#9792F7";

        events = false;
        notes = false;
        lists = false;

        toDoObjectsCount = 0;

        emailVerify = EmailVerifyPanel.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
    }


    private static string ReplaceWith(string str)
    {
        string result = "";

        result = str.Replace(str.Contains(@"/") ? "/" : ".", @"\");

        return result;
    }
    private static void CheckEmptyTimetable(string type)
    {
        switch (type)
        {
            case "events":
            {
                if (!CheckCurrentSection(EventsContainer, EventsHeader))
                    events = false;
                else events = true;
                break;
            }
            
            case "notes":
            {
                if (!CheckCurrentSection(NotesContainer, NotesHeader))
                    notes = false;
                else notes = true;
                break;
            }
            
            case "lists":
            {
                if (!CheckCurrentSection(ToDoContainer, ToDoHeader))
                    lists = false;
                else lists = true;
                break;
            }
        }
        
        if (!events && !notes && !lists)
        {
            EmptySchedulePanel.SetActive(true);
        }
        else EmptySchedulePanel.SetActive(false);
        
        Canvas.ForceUpdateCanvases();
    }
    private static bool CheckCurrentSection(Transform container, GameObject header)
    {
        bool result = false;
        if (container.childCount == 0)
        {
            container.gameObject.SetActive(false);
            header.SetActive(false);
        }
        else
        {
            container.gameObject.SetActive(true);
            header.SetActive(true);
            result = true;
        }

        return result;
    }
    
    
    
    
    public static void SetEventsValues(Dictionary<string, List<MEvent>> newEventsValues)
    {
        EventsValues = newEventsValues;
        ShowEvents();
    }
    public static void ShowEvents()
    {
        ClearContent(EventsContainer);
        
        string currentDate = ReplaceWith(dateSelected);

        if (EventsValues.ContainsKey(currentDate))
        {
            List<MEvent> values = EventsValues[currentDate]; //берем выбранный день
            foreach (MEvent currentEvent in values) //для всех событий в этом дне
            {
                Prefabs.CreateEvent(EventPrefab, EventsContainer, currentEvent);
            }

        }

        CheckEmptyTimetable("events");
    }
    
    public static void AddNewEvent(Dictionary<string, string> newEvent)
    {
        newEvent.Add("categoryColour",selectedCategoryColour);
        newEvent.Add("categoryName",selectedCategoryName);
        DBEvent.DBEventAdd(dateSelected, newEvent);
    }
    public static void DeleteEvent()
    {
        DBEvent.DBEventDelete(dateSelected, currentKey);
    }
    
    

    public static void SetCategoriesValues( Dictionary<string, string> newCategoriesValues)
    {
        ClearContent(CategoriesContainer);
        CategoriesValues = newCategoriesValues;
        ShowCategories();
    }
    private static void ShowCategories()
    {
        foreach (var category in categoriesValues)
        {
            Prefabs.CreateCategory(CategoryPrefab, CategoriesContainer, category.Key, category.Value);
        }
    }
    public static void AddNewCategory(string [] values)
    {
        DBCategory.DBEventAdd(values[0],values[1]);
    }
    public static void DeleteCategory()
    {
        DBCategory.DBCategoryDelete(currentKey);
    }
    
    private static void ClearContent(Transform content)
    {
        if(content.childCount > 0)
            for (var i = content.childCount - 1; i >= 0; i--)
            {
                var objectA = content.GetChild(i);
                objectA.SetParent(null);
                Destroy(objectA.gameObject);
            } 
    }

 
 
    public static void SetNotesValues(Dictionary<string, List<MNote>> newNotesValues)
    {
        NotesValues = newNotesValues;
        ShowNotes();
    }
    public static void ShowNotes()
    {
        ClearContent(NotesContainer);

        string currentDate = ReplaceWith(dateSelected);
        
        if (NotesValues.ContainsKey(currentDate))
        {
            List<MNote> values = NotesValues[currentDate]; //берем выбранный день
            foreach (MNote currentNote in values) //для всех событий в этом дне
            {
                Prefabs.CreateNote(NotePrefab, NotesContainer, currentNote);
            }
        }
 
        CheckEmptyTimetable("notes");

    }
    public static void DeleteNote()
    {
       DBNote.DBNoteDelete(dateSelected, currentKey);
    }
    public static void AddNewNote(string value)
    {
       DBNote.DBNoteAdd(dateSelected,value);
    }
    public static void UpdateNote(string value)
    {
        DBNote.DBNoteUpdate(dateSelected,currentKey,value);
    }

    
    
    public static void DeleteToDoObject(string key)
    {
        toDoObjectsCount--;
        toDoObjects.Remove(key);
        ShowTodoObjects();
    }
    public static void AddTodoObject(string value)
    {
        Debug.Log(value);
        toDoObjects.Add(toDoObjectsCount.ToString(),value);
        toDoObjectsCount++;
        Prefabs.CreateTodoObject(ToDoObjectPrefab,ToDoObjectsContainer,value,toDoObjectsCount.ToString());
    }
    private static void ShowTodoObjects()
    {
        ClearContent(ToDoObjectsContainer);
        
        foreach (var obj in toDoObjects)
        {
            Prefabs.CreateTodoObject(ToDoObjectPrefab,ToDoObjectsContainer,obj.Value,obj.Key);
        }
    }
    
    
    public static void SetTodoValues(Dictionary<string, List<MTodo>> newTodoValues)
    {
        ToDoValues = newTodoValues;
        ShowTodo();
    }

    public static void ShowTodo()
    {
        ClearContent(ToDoContainer);
                
        string currentDate = ReplaceWith(dateSelected);

        if (ToDoValues.ContainsKey(currentDate))
        {
            List<MTodo> values = ToDoValues[currentDate]; 
            foreach (MTodo currentList in values)
            {
                string listName = currentList.nameList;

                Transform listContainer = Prefabs.CreateTodo(TodoListPrefab, ToDoContainer, listName);

                foreach (var item in currentList.itemsList)
                {
                    Prefabs.CreateTodoItem(ToDoItemPrefab, listContainer, item.Key, Convert.ToBoolean(item.Value));
                }
            }
        }

        CheckEmptyTimetable("lists");
        
    }

    public static void AddTodo(string listName)
    {
        
    }

    public static void DeleteList()
    {
        DBTodo.DeleteList(dateSelected,currentKey);
    }


}
