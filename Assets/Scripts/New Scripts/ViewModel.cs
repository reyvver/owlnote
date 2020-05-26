using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ViewModel : MonoBehaviour
{
    public static Transform EventsContainer, NotesContainer, CategoriesContainer;

    public static GameObject EventsHeader, NotesHeader;

    public static GameObject EventPrefab, NotePrefab, CategoryPrefab;

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





    private void Start()
    {
        selectedCategoryName = "По умолчанию";
        selectedCategoryColour = "#9792F7";

        emailVerify = EmailVerifyPanel.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
    }


    private static string ReplaceWith(string str)
    {
        string result = "";

        result = str.Replace(str.Contains(@"/") ? "/" : ".", @"\");

        return result;
    }
    private static void CheckEmptyTimetable()
    {
        if (!CheckCurrentSection(EventsContainer, EventsHeader) && !CheckCurrentSection(NotesContainer, NotesHeader))
        {
            EmptySchedulePanel.SetActive(true);
        }
        else EmptySchedulePanel.SetActive(false);

        Canvas.ForceUpdateCanvases();
    }
    private static bool CheckCurrentSection(Transform container, GameObject header)
    {
        if (container.childCount == 0)
        {
            container.gameObject.SetActive(false);
            header.SetActive(false);
            return false;
        }
        else
        {
            container.gameObject.SetActive(true);
            header.SetActive(true);
            return true;
        }
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

        CheckEmptyTimetable();

    }
    public static void AddNewEvent(Dictionary<string, string> newEvent)
    {
        newEvent["categoryColour"] = selectedCategoryColour;
        newEvent["categoryName"] = selectedCategoryName;
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
    public static void DeleteCategory(string key)
    {
        DBCategory.DBCategoryDelete(key);
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

  

}
