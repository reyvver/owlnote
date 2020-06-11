using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ViewModel : MonoBehaviour
{
    public static Transform Containter,EventsContainer, NotesContainer, CategoriesContainer, ToDoObjectsContainer, ToDoContainer, AllContainer;

    public static GameObject EventsHeader, NotesHeader, ToDoHeader;

    public static GameObject EventPrefab, NotePrefab, CategoryPrefab, ToDoObjectPrefab, ToDoItemPrefab,  TodoListPrefab, AllContainerPrefab;

    public static GameObject EmptySchedulePanel;
    public static GameObject EmailVerifyPanel;
    public static TextMeshProUGUI emailVerify;
    public static TMP_Dropdown categorySelect, showTypeSelect;
    
    public static string dateSelected;
    public static string selectedCategoryName, selectedCategoryColour, currentKey;

    private static ScrollRect scrollContent;
    
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

    private void OnDestroy()
    {
        EventsValues.Clear();
        CategoriesValues.Clear();
        NotesValues.Clear();
        ToDoValues.Clear();
        toDoObjectsCount = 0;
    }

    private void Start()
    {
        selectedCategoryName = "По умолчанию";
        selectedCategoryColour = "#9792F7";

        events = false;
        notes = false;
        lists = false;

        toDoObjectsCount = 0;

        scrollContent = Containter.GetComponent<ScrollRect>();
        emailVerify = EmailVerifyPanel.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
        
        categorySelect.ClearOptions();
        categorySelect.onValueChanged.AddListener(delegate { SelectOption(); });
        
    }
    

    private static string ReplaceWith(string str)
    {
        string result = "";

        result = str.Replace(str.Contains(@"/") ? "/" : ".", @"\");

        return result;
    }
    private static void CheckEmptyTimetable(string type)
    {
        if(scrollContent!=null)
            scrollContent.enabled = false;

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
        else
        {
            EmptySchedulePanel.SetActive(false);
        }

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

    private void Update()
    {
        if (scrollContent.enabled) return;
        StartCoroutine(WaitD());
    }

    IEnumerator WaitD()
    {
        yield return new WaitForEndOfFrame();
        scrollContent.enabled = true;
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
                Prefabs.CreateEvent(EventPrefab, EventsContainer, currentEvent, true);
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

    public static void ScrollToTheTop()
    {
        scrollContent.ScrollToBottom();
    }
    


    public static void SetCategoriesValues( Dictionary<string, string> newCategoriesValues)
    {
        ClearContent(CategoriesContainer);
        CategoriesValues = newCategoriesValues;
        ShowCategories();
        FillCategorySelect();
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
                Prefabs.CreateNote(NotePrefab, NotesContainer, currentNote, true);
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
        toDoObjectsCount++;
        toDoObjects.Add(toDoObjectsCount.ToString(),value);
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

                Transform listContainer = Prefabs.CreateTodo(TodoListPrefab, ToDoContainer, listName, true);

                var items = currentList.itemsList.OrderBy(o => o.Value);

                foreach (var item in items)
                {
                    Prefabs.CreateTodoItem(ToDoItemPrefab, listContainer, item.Key, Convert.ToBoolean(item.Value),true);
                }
            }
        }
        CheckEmptyTimetable("lists");
    }

    public static void AddTodo(string value)
    {
        DBTodo.AddItemInList(dateSelected,currentKey,value);
    }

    public static void DeleteListItem(string value)
    {
        DBTodo.DeleteListItem(dateSelected,currentKey,value);
    }

    public static void UpdateItemState(string value)
    {
        DBTodo.UpdateItemState(dateSelected,currentKey,value);
    }

    public static void DeleteList()
    {
        DBTodo.DeleteList(dateSelected,currentKey);
    }

    public static void AddList(MTodo newList)
    {
        DBTodo.AddList(dateSelected,newList);
    }

    public static void ShowAllEvents()
    { 
        ClearContent(AllContainer);

        foreach (var Item in EventsValues)
        {
            string date = ReplaceWith(Item.Key);

            Transform currentContent = Prefabs.CreateAllPrefab(AllContainerPrefab, AllContainer, date);

            foreach (MEvent currentEvent in Item.Value)
            {
                Prefabs.CreateEvent(EventPrefab, currentContent, currentEvent, false);
            }
        }
        
    }
    
    public static void ShowAllNotes()
    {
        ClearContent(AllContainer);

        foreach (var Item in NotesValues)
        {
            string date = ReplaceWith(Item.Key);
          
            Transform currentContent = Prefabs.CreateAllPrefab(AllContainerPrefab, AllContainer, date);

            foreach (MNote currentNote in Item.Value)
            {
                Prefabs.CreateNote(NotePrefab,currentContent,currentNote,false);
            }
        }
    }
    
    public static void ShowAllLists()
    {
        ClearContent(AllContainer);

        foreach (var Item in ToDoValues)
        {
            string date = ReplaceWith(Item.Key);
            Transform currentContent = Prefabs.CreateAllPrefab(AllContainerPrefab, AllContainer, date);

            foreach (MTodo currentList in  Item.Value)
            {
                string listName = currentList.nameList;

                Transform listContainer = Prefabs.CreateTodo(TodoListPrefab, currentContent, listName, false);

                var items = currentList.itemsList.OrderBy(o => o.Value);

                foreach (var item in items)
                {
                    Prefabs.CreateTodoItem(ToDoItemPrefab, listContainer, item.Key, Convert.ToBoolean(item.Value),false);
                }
            }

        }
    }

    public static void ClearAllContainer()
    {
        ClearContent(AllContainer);
    }

    public static void FillCategorySelect()
    {
        categorySelect.ClearOptions();
        List<string> values = new List<string>();

        foreach (var category in categoriesValues)
        {
            values.Add(category.Key);
        }
        categorySelect.AddOptions(values);
    }

    private void SelectOption()
    {
        TMP_Text currentOption = categorySelect.captionText;
        string textOption = currentOption.text;
        string newColor = "";

        Debug.Log(textOption);
        
        foreach (var category in categoriesValues)
        {
            if (textOption == category.Key)
            {
                newColor = category.Value;
                break;
            }
        }
        
        if (ColorUtility.TryParseHtmlString(newColor, out var newCol))
        {
           currentOption.color = newCol;
        }
    }

    public static void ShowSelectedEvents()
    {
        ClearContent(AllContainer);

        foreach (var Item in EventsValues)
        {
            string date = ReplaceWith(Item.Key);

            Transform currentContent = Prefabs.CreateAllPrefab(AllContainerPrefab, AllContainer, date);

            foreach (MEvent currentEvent in Item.Value)
            {
                if(currentEvent.CategoryName == categorySelect.captionText.text)
                    Prefabs.CreateEvent(EventPrefab, currentContent, currentEvent, false);
            }

            if (currentContent.childCount == 0)
            {
                currentContent.parent.SetParent(null);
                Destroy(currentContent.parent.gameObject);
            }
        }
    }


    public static void ChangeShowType(string type)
    {
        string resultType = "";
        
        switch (type)
        {
            case "События":
            {
                resultType = "event";
                break;   
            }
            case "Заметки":
            {
                resultType = "note";
                break;
            }
            case "Списки дел":
            {
                resultType = "list";
                break;
            }
        }
        
        DBUser.ChangeShowType(resultType);
    }

    public static void ChangeContainersOrder(string type)
    {
        switch (type)
        {
            case "event":
            {
                EventsContainer.SetAsFirstSibling();
                EventsHeader.transform.SetAsFirstSibling();
                ToDoHeader.transform.SetAsLastSibling();
                ToDoContainer.SetAsLastSibling();
                showTypeSelect.value = 0;
                break;   
            }
            case "note":
            {
                NotesContainer.SetAsFirstSibling();
                NotesHeader.transform.SetAsFirstSibling();
                ToDoHeader.transform.SetAsLastSibling();
                ToDoContainer.SetAsLastSibling();
                showTypeSelect.value = 1;
                break;
            }
            case "list":
            {
                ToDoContainer.SetAsFirstSibling();
                ToDoHeader.transform.SetAsFirstSibling();
                NotesHeader.transform.SetAsLastSibling();
                NotesContainer.SetAsLastSibling();
                showTypeSelect.value = 2;
                break;
            }
        }
    }
}
