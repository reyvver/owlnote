using System;
using System.Globalization;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class View : MonoBehaviour
{
    [Header("Containers, where items will be shown")]
    public Transform EventsContainer;
    public Transform NotesContainer;
    public Transform CategoriesContainer;
    public Transform ToDoObjectsContainer;
    public Transform ToDoContainer;

    [Header("Headers of containers")] 
    public GameObject EventsHeader;
    public GameObject NotesHeader;
    public GameObject ToDoHeader;

    [Header("Prefabs")]
    public GameObject EventPrefab;
    public GameObject NotePrefab;
    public GameObject CategoryPrefab;
    public GameObject ToDoObjectPrefab;
    public GameObject ToDoListPrefab;
    public GameObject ToDoItemPrefab;

    [Header("UI elements")] 
    public GameObject EmptySchedulePanel;
    public GameObject DeleteConfirmPanel;
    public GameObject TimePicker;
    public GameObject SelectTimePanel;
    public GameObject EmailVerifyPanel;
    public GameObject UpdateNotePanel;
    
    public TMP_InputField dateSelected;
    public Transform DatePlatesContainer;
    public Transform PanelSuccessfulOperation;
    public Transform AddEventPanel;
    public Transform HeaderTitle;
    public Text newCategoryColour;

    private ScrollRectScript time;
    private string dateCurrent;
    private static string typeObject;
    private static string currentKey;
    private bool _importantOperation;
    private static GameObject deletePanel;
    private TextMeshProUGUI successText;

    private GameObject currentTimeSelection;
    private static GameObject noteUpdate;
 
    public static TMP_InputField noteUpdateText;
    
    private List<int> NumberOfDaysInMonths = new List<int>(); // дни в месяце
    private string[] DaysToString = new[] {"ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ", "ВС"};
    public  List<GameObject> OpenedPanels;
    private CultureInfo localCultureInfo = new CultureInfo("ru-RU");

    private static Dictionary<string, List<MEvent>> eventsValues = new Dictionary<string, List<MEvent>>();
    private Dictionary<string, string> categoriesValues = new Dictionary<string, string>();


    [NonSerialized] public TextMeshProUGUI eventStart, eventEnd, errorEventLabel, eventCategory;
    [NonSerialized] public TMP_InputField eventTitle, eventDescription;
    [NonSerialized] public TextMeshProUGUI todayMonth, todayDayOfWeek;

    private Color32 _greyLightCustom = new Color32(245, 245, 245, 255);
    private Color32 _greyDarkCustom = new Color32(108, 108, 108, 255);
    private Color32 _purpleCustom = new Color32(146, 96, 255, 255);


    private void Awake()
    {
        ViewModel.EventsContainer = EventsContainer;
        ViewModel.EventPrefab = EventPrefab;
        ViewModel.EventsHeader = EventsHeader;
        
        ViewModel.NotesContainer = NotesContainer;
        ViewModel.NotePrefab = NotePrefab;
        ViewModel.NotesHeader = NotesHeader;

        ViewModel.CategoryPrefab = CategoryPrefab;
        ViewModel.CategoriesContainer = CategoriesContainer;
        
        
        ViewModel.EmptySchedulePanel = EmptySchedulePanel;
        ViewModel.EmailVerifyPanel = EmailVerifyPanel;

        ViewModel.ToDoObjectPrefab = ToDoObjectPrefab;
        ViewModel.ToDoObjectsContainer = ToDoObjectsContainer;

        ViewModel.ToDoContainer = ToDoContainer;
        ViewModel.ToDoHeader = ToDoHeader;
        ViewModel.ToDoItemPrefab = ToDoItemPrefab;
        ViewModel.TodoListPrefab = ToDoListPrefab;
        
        Debug.Log("done with viewmodel");
    }

    private void Start()
    {
        dateCurrent = DateTime.Today.ToString("dd/MM/yyyy", localCultureInfo);
        dateSelected.text = dateCurrent;
        deletePanel = DeleteConfirmPanel;
        time = TimePicker.GetComponent<ScrollRectScript>();

        successText = PanelSuccessfulOperation.Find("PanelSuccess/TextMessage").GetComponent<TextMeshProUGUI>();

        eventStart = AddEventPanel.Find("TimePickSection/PanelSetStartTime/PanelSelectButton/TimeStart")
            .GetComponent<TextMeshProUGUI>();
        eventEnd = AddEventPanel.Find("TimePickSection/PanelSetEndTime/PanelSelectButton/TimeEnd")
            .GetComponent<TextMeshProUGUI>();
        eventCategory = AddEventPanel.Find("CategorySection/SelectCategoryButton/CategoryPickLabel")
            .GetComponent<TextMeshProUGUI>();
        errorEventLabel = AddEventPanel.GetChild(5).GetComponent<TextMeshProUGUI>();

        eventTitle = AddEventPanel.Find("TitleSection/TitleInput").GetComponent<TMP_InputField>();
        eventDescription = AddEventPanel.Find("AdditionalInfoSection/DescriptionPanel/Description")
            .GetComponent<TMP_InputField>();

        todayMonth = HeaderTitle.GetChild(0).GetComponent<TextMeshProUGUI>();
        todayDayOfWeek = HeaderTitle.GetChild(1).GetComponent<TextMeshProUGUI>();

        todayMonth.text = ToTitleCase(localCultureInfo.DateTimeFormat.GetMonthName(DateTime.Today.Month));
        todayDayOfWeek.text = ToTitleCase(localCultureInfo.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek));


        noteUpdate = UpdateNotePanel;
        noteUpdateText = UpdateNotePanel.transform.Find("Panel/NoteText").GetComponent<TMP_InputField>();

        InitialiseDatesPlates(); //инициализируем панель для быстрого выбора даты
        ShowSelectedDatePanel(DatePlatesContainer.GetChild(0)); // сегодняшняя дата
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (OpenedPanels.Count > 0)
        {
            int lastPanelIndex = OpenedPanels.Count - 1;

            if (DeleteConfirmPanel.active)
            {
                DeleteConfirmPanel.SetActive(false);
            }
            else

            if (OpenedPanels[lastPanelIndex] == PanelSuccessfulOperation)
            {
                CloseAll();
            }
            else
            {
                CloseLastPanel();
            }
        }
    }

    
    /*При выборе новой даты*/
    public void OnClickDatePanel(Transform DatePanel)
    {
        string currentDay = DatePanel.GetChild(1).GetComponent<TextMeshProUGUI>().text; //выбранный день
        ShowSelectedDatePanel(DatePanel); // Цветом выделяем выбранный день 
        OnClickChangeDateSelected(currentDay); // Изменяем текущую дату и относительно нее показываем расписание
    }
    /*Получаем дату и обновляем DateSelected*/
    private void OnClickChangeDateSelected(string date)
    {
        DateTime today = DateTime.Today;

        int day = Convert.ToInt32(date);
        int month = today.Month;
        int year = today.Year;

        if (today > new DateTime(year, month, day))
        {
            month++;

            if (month == 13) //если выбранный месяц декабрь, то он сменится на январь и год увеличится на 1 
            {
                month = 1;
                year++;
            }
        }

        DateTime newDate = new DateTime(year, month, day);
        string dateToString = newDate.ToString("dd/MM/yyyy", localCultureInfo);
        dateSelected.text = dateToString;
    }
    /*Цветом показываем какой день выбран*/
    private void ShowSelectedDatePanel(Transform selectedPanel)
    {
        foreach (Transform panel in DatePlatesContainer)
        {
            Image backPanel = panel.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI numberText = panel.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI textText = panel.GetChild(2).GetComponent<TextMeshProUGUI>();

            if (panel != selectedPanel)
            {
                backPanel.color = _greyLightCustom;
                numberText.color = _greyDarkCustom;
                textText.color = _greyDarkCustom;
            }
            else
            {
                backPanel.color = _purpleCustom;
                numberText.color = Color.white;
                textText.color = Color.white;
            }
        }
    }
    /*Если изменилась дата, относительно которой надо показывать расписание*/
    public void OnDateSelectedChanged()
    {
        ViewModel.dateSelected = dateSelected.text;
        ViewModel.ShowEvents();
        ViewModel.ShowNotes();
        ViewModel.ShowTodo();
    }
    
    
    

    /*Определяет количество дней в каждом месяце*/
    private void InitializeDays()
    {
        NumberOfDaysInMonths.Add(31); //January
        if (WhatTypeOfYear())
            NumberOfDaysInMonths.Add(29); //February
        else NumberOfDaysInMonths.Add(28); //February
        NumberOfDaysInMonths.Add(31); //March
        NumberOfDaysInMonths.Add(30); //April
        NumberOfDaysInMonths.Add(31); //May
        NumberOfDaysInMonths.Add(30); //June
        NumberOfDaysInMonths.Add(31); //July
        NumberOfDaysInMonths.Add(31); //August
        NumberOfDaysInMonths.Add(30); //September
        NumberOfDaysInMonths.Add(31); //October
        NumberOfDaysInMonths.Add(30); //November
        NumberOfDaysInMonths.Add(31); //December
    }
    /*Високосный год или нет*/
    private bool WhatTypeOfYear()
    {
        bool chk = DateTime.Today.Year % 4 == 0;
        return chk;
    }
    private void InitialiseDatesPlates()
    {
        InitializeDays();

        DateTime currentDay = DateTime.Today;

        foreach (Transform panel in DatePlatesContainer)
        {
            TextMeshProUGUI numberText = panel.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI textText = panel.GetChild(2).GetComponent<TextMeshProUGUI>();

            numberText.text = currentDay.Day.ToString();
            textText.text = DaysToString[(int) currentDay.DayOfWeek];

            currentDay += TimeSpan.FromDays(1);

        }
    }




    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        OpenedPanels.Add(panel);
    }
    public void CloseLastPanel()
    {
        int index = OpenedPanels.Count - 1;
        OpenedPanels[index].SetActive(false);
        OpenedPanels.RemoveAt(index);
    }
    public void CloseAll()
    {
        foreach (GameObject obj in OpenedPanels)
        {
            obj.SetActive(false);
        }

        OpenedPanels.Clear();
    }
    public void OpenScene(GameObject scene)
    {
        scene.SetActive(true);
        scene.transform.SetAsLastSibling();
    }
    public void CloseScene(GameObject scene)
    {
        scene.SetActive(false);
        scene.transform.SetAsFirstSibling();
    }
    public void OpenCurrentDay(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void OpenCalendarPage(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void OpenFirstScene()
    {
        SceneManager.LoadScene(0);
    }
    

    public void SelectCategory()
    {
        AddEventPanel.GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            ViewModel.selectedCategoryName;
        CloseLastPanel();
    }
    public void AddCategory(TMP_InputField keyCategory)
    {
        string categoryName = keyCategory.text;
        
        if (categoryName != "")
        {
            string[] values = {categoryName, newCategoryColour.text};
            ViewModel.AddNewCategory(values);
            CloseLastPanel();
        }

    }
    public void AddNote(TMP_InputField valueNote)
    {
        if (valueNote.text != "")
        {
            ViewModel.AddNewNote(valueNote.text);
            CloseAll();
            successText.text = "Добавлена новая заметка";
            OpenPanel(PanelSuccessfulOperation.gameObject);
            valueNote.text = "";
        }
    }
    public void AddNewEvent()
    {
        if (eventTitle.text != "" && eventStart.text != "Не выбрано" && eventEnd.text != "Не выбрано")
        {

            Dictionary<string, string> values = new Dictionary<string, string>
            {
                {"title", eventTitle.text}, {"startTime", eventStart.text}, {"endTime", eventEnd.text}
            };

            if (eventDescription.text != "")
                values.Add("description", eventDescription.text);
            
            ViewModel.AddNewEvent(values);
            CloseAll();
            successText.text = "Добавлен пункт расписания";
            OpenPanel(PanelSuccessfulOperation.gameObject);
            ClearEventInfo();
        }
        else
            errorEventLabel.text = "Ошибка: не все обязательные поля заполнены.";
    }
    
    
    public static void Delete(TextMeshProUGUI key, string type)
    {
        typeObject = type;
        
        deletePanel.gameObject.SetActive(true);
        deletePanel.transform.SetAsLastSibling();
        
        UpdateDeleteMessage(key.text);
        currentKey = key.text;
    }
    private static void UpdateDeleteMessage(string title)
    {
        TextMeshProUGUI message = deletePanel.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        
        switch (typeObject)
        {
            case "event":
            {
                message.text = "пункт расписания " + "'"+ title + "' ?";
                break;
            }
            case "category":
            {
                message.text = "категорию " + "'" + title + "' ?";
                break;
            }
            case "note":
            {
                message.text = "выбранную заметку?";
                break;
            }
            case "list":
            {
                message.text = "список " + "'" + title + "' ?";
                break;
            }
        }
    }
    public void DeleteInfo()
    {
        switch (typeObject)
        {
            case "event":
            {
                ViewModel.DeleteEvent();
                break;
            }
            case "note":
            {
                ViewModel.DeleteNote();
                break;
            }
            case "category":
            {
                ViewModel.DeleteCategory();
                break;
            }
            case "list":
            {
                ViewModel.DeleteList();
                break;
            }
        }
        DeleteConfirmPanel.gameObject.SetActive(false);
        DeleteConfirmPanel.transform.SetAsFirstSibling();
    }



  

    public void ShowSelectTimePanel(GameObject panel)
    {
        currentTimeSelection = panel;
        
        OpenPanel(SelectTimePanel);
        
    }
    public void SetTime()
    {
        TextMeshProUGUI timeText = currentTimeSelection.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        string parent = currentTimeSelection.transform.parent.name;
        
        if (parent == "PanelSetStartTime")
        {
            timeText.text = time.hours + ":" + time.minutes;
        }
        else timeText.text = time.hours + ":" + time.minutes;

        CloseLastPanel();
    }

    
    private void ClearEventInfo()
    {
        eventDescription.text = "";
        eventEnd.text = "Не выбрано";
        eventStart.text = "Не выбрано";
        eventTitle.text = "";
        eventCategory.text = "По умолчанию";
        errorEventLabel.text = "";
        
        ViewModel.selectedCategoryName = "По умолчанию";
        ViewModel.selectedCategoryColour = "#9792F7";
        time.SetDefaultTime();
    }
    private string ToTitleCase(string str)
    {
        return localCultureInfo.TextInfo.ToTitleCase(str.ToLower());
    }


    public static void UpdateNote(string currentValue)
    {
        typeObject = "note";
        noteUpdate.SetActive(true);
        noteUpdate.transform.SetAsLastSibling();
        noteUpdateText.text = currentValue;
    }


    public void UpdateProperties()
    {
        switch (typeObject)
        {
            case "note":
            {
                if(noteUpdateText.text!="")
                ViewModel.UpdateNote(noteUpdateText.text);
                UpdateNotePanel.SetActive(false);
                noteUpdateText.text = "";
                break;
            }
        }
    }
    public void AddNewTodoItem(TMP_InputField input)
    {
        string value = input.text;

        if (value == "") value = "По умолчанию";
        ViewModel.AddTodoObject(value);
        input.text = "";
    }
}