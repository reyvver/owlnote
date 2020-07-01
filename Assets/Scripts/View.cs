using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class View : MonoBehaviour
{
    [Header("Containers, where items will be shown")]
    public Transform Container;
    public Transform EventsContainer;
    public Transform NotesContainer;
    public Transform CategoriesContainer;
    public Transform ToDoObjectsContainer;
    public Transform ToDoContainer;
    public Transform AllContainer;
    public Transform PlansContainer;


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
    public GameObject AllPrefab;
    public GameObject PlanItemPrefab;

    [Header("UI elements")] 
    public GameObject Schedule;
    public GameObject EmptySchedulePanel;
    public GameObject DeleteConfirmPanel;
    public GameObject EventConfirmPanel;
    public GameObject TimePicker;
    public GameObject SelectTimePanel;
    public GameObject EmailVerifyPanel;
    public GameObject UpdateNotePanel;
    public GameObject TextInputPanel;
    public GameObject categorySelect;
    public GameObject showTypeSelect;
    public GameObject QuickAddMenu;
    public TMP_InputField dateSelected;
    public TMP_InputField dateSelectedCalendar;
    public Transform DatePlatesContainer;
    public Transform PanelSuccessfulOperation;
    public Transform AddEventPanel;
    public Transform HeaderTitle;
    public GameObject SelectedCalendarDay;
    public GameObject AddNewListPanel;
    public Text newCategoryColour;
    public  List<GameObject> OpenedPanels;
    
    public static TMP_InputField noteUpdateText;
    public static TextMeshProUGUI ConfirmMessage, ConfirmText;
    public static Transform textInputPanel;

    private ScrollRectScript time;
    private string dateCurrent, openedScene;
    private static string typeObject;
    private string previousDate;

    private bool _importantOperation;
    private static Transform deletePanel;
    private TextMeshProUGUI successText;
    

    private GameObject currentTimeSelection;
    private static GameObject noteUpdate, eventUpdate;
 

    private List<int> NumberOfDaysInMonths = new List<int>(); // дни в месяце
    private string[] DaysToString = {"ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ", "ВС"};

    private CultureInfo localCultureInfo = new CultureInfo("ru-RU");

    private static Dictionary<string, List<MEvent>> eventsValues = new Dictionary<string, List<MEvent>>();
    private Dictionary<string, string> categoriesValues = new Dictionary<string, string>();
    private RectTransform addNote;
    private RectTransform addEvent;
    private RectTransform addList;
    private Dictionary<string, string> valuesNewEvent;
    private static string eventOperation, lastTime;
    
    public static TextMeshProUGUI eventStart, eventEnd, errorEventLabel, eventCategory;
    public static TMP_InputField eventTitle, eventDescription;
    [NonSerialized] public TextMeshProUGUI todayMonth, todayDayOfWeek;
    [NonSerialized] public Button ButtonCurrentDay, ButtonCalendar;
    [NonSerialized] private RectTransform Header, PlusButton, Content;
    [NonSerialized] public TextMeshProUGUI selectedText, EventText, newEventMessage, newEventText;
    [NonSerialized] public GameObject ButtonCancelSort, ButtonSort, CategorySortPanel, MainContent, Blocker;
    [NonSerialized] public Toggle separateEvent;
    [NonSerialized] public TMP_InputField listName;
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
        ViewModel.Containter = Container;
        ViewModel.AllContainerPrefab = AllPrefab;
        ViewModel.AllContainer = AllContainer;
        ViewModel.categorySelect = categorySelect.GetComponent<TMP_Dropdown>();
        ViewModel.showTypeSelect = showTypeSelect.GetComponent<TMP_Dropdown>();

        ViewModel.PlanItemPrefab = PlanItemPrefab;
        ViewModel.PlansContainer = PlansContainer;

        Debug.Log("done with viewmodel");
    }
    private void Start()
    {
        openedScene = "current day";
        eventOperation = "add";
        dateCurrent = DateTime.Today.ToString("dd/MM/yyyy", localCultureInfo);
        dateSelected.text = dateCurrent;
        deletePanel = DeleteConfirmPanel.transform;
        textInputPanel = TextInputPanel.transform;
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
        separateEvent = AddEventPanel.Find("TimePickSection/PanelSeparate/Check").GetComponent<Toggle>();

        todayMonth = HeaderTitle.GetChild(0).GetComponent<TextMeshProUGUI>();
        todayDayOfWeek = HeaderTitle.GetChild(1).GetComponent<TextMeshProUGUI>();

        todayMonth.text = ToTitleCase(localCultureInfo.DateTimeFormat.GetMonthName(DateTime.Today.Month));
        todayDayOfWeek.text = ToTitleCase(localCultureInfo.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek));

        ButtonCalendar = QuickAddMenu.transform.parent.Find("Footer/MenuFooter/Calendar").GetComponent<Button>();
        ButtonCurrentDay =  QuickAddMenu.transform.parent.Find("Footer/MenuFooter/CurrentDay").GetComponent<Button>();

        noteUpdate = UpdateNotePanel;
        noteUpdateText = UpdateNotePanel.transform.Find("Panel/NoteText").GetComponent<TMP_InputField>();
        eventUpdate = AddEventPanel.parent.gameObject;
        
        addNote = QuickAddMenu.transform.Find("AddNote").GetComponent<RectTransform>();
        addEvent = QuickAddMenu.transform.Find("AddEvent").GetComponent<RectTransform>();
        addList = QuickAddMenu.transform.Find("AddToDo").GetComponent<RectTransform>();
        
        Header = QuickAddMenu.transform.parent.Find("Header").GetComponent<RectTransform>();
        PlusButton = QuickAddMenu.transform.parent.Find("PlusButton").GetComponent<RectTransform>();
        Content = Schedule.GetComponent<RectTransform>();
        MainContent = Schedule.transform.parent.gameObject;
        Blocker = MainContent.transform.Find("Blocker").gameObject;

        selectedText = PanelSuccessfulOperation.parent.Find("AllInfoPanel/PanelHeader/Events/Text").GetComponent<TextMeshProUGUI>();
        EventText = selectedText;

        ButtonSort = PanelSuccessfulOperation.parent.Find("AllInfoPanel/ButtonSort").gameObject;
        ButtonCancelSort = PanelSuccessfulOperation.parent.Find("AllInfoPanel/ButtonCancel").gameObject;
        CategorySortPanel = PanelSuccessfulOperation.parent.Find("AllInfoPanel/SortByCategory").gameObject;

        listName = AddNewListPanel.transform.Find("TitleSection/TitleInput").GetComponent<TMP_InputField>();
        
        PlusButton.gameObject.SetActive(false);
        
        ConfirmMessage =  deletePanel.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        ConfirmText = deletePanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        
        newEventMessage =   EventConfirmPanel.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        newEventText =  EventConfirmPanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        
        
        InitialiseDatesPlates(); //инициализируем панель для быстрого выбора даты
        ShowSelectedDatePanel(DatePlatesContainer.GetChild(0)); // сегодняшняя дата
    }
    private void Update()
    {
       
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
        if (DeleteConfirmPanel)
        {
            DeleteConfirmPanel.SetActive(false);
        }
        if (TextInputPanel)
        {
            TextInputPanel.SetActive(false);
        }
        
        if(eventUpdate)
            eventUpdate.SetActive(false);
        
        if(noteUpdate)
            noteUpdate.SetActive(false);
        
        ClearEventInfo();
        ClearListInfo();
        
        if (OpenedPanels.Count > 0)
        {
            
            if (QuickAddMenu)
            {
                CloseQuickAddMenu();
            }

            if (SelectedCalendarDay)
            {
                openedScene = "calendar";
            }

            if (PanelSuccessfulOperation)
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
        dateSelected.text = ChangeDate(currentDay);
        // OnClickChangeDateSelected(currentDay); // Изменяем текущую дату и относительно нее показываем расписание
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
                backPanel.color = InterfaceTheme.GreyLightCustom;
                numberText.color = InterfaceTheme.GreyDarkCustom;
                textText.color = InterfaceTheme.GreyDarkCustom;
            }
            else
            {
                backPanel.color = InterfaceTheme.PurpleCustom;
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
        ClearEventInfo();
        ClearListInfo();
        
        if(eventUpdate)
            eventUpdate.SetActive(false);
        
        if(noteUpdate)
            noteUpdate.SetActive(false);
        
        StartCoroutine(AnimationForQuickAddMenu(0, 0,0.05f));
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
        dateSelected.text = previousDate;
        ButtonCurrentDay.enabled = false;
        ButtonCalendar.enabled = false;
        Schedule.SetActive(true);
        Header.DOAnchorPos(new Vector2(0f,-203), 0.5f);
        Content.DOAnchorPos(new Vector2(0f,-184f), 1f);
        StartCoroutine( WaitDo(obj, true,0f,false));
        PlusButton.gameObject.SetActive(false);
        openedScene = "current day";
        Debug.Log(previousDate);
    }
    public void OpenCalendarPage(GameObject obj)
    {
        previousDate = dateSelected.text;    
        ButtonCurrentDay.enabled = false;
        ButtonCalendar.enabled = false;
        PlusButton.gameObject.SetActive(true);
        Header.DOAnchorPos(new Vector2(0f,226f), 0.5f);
        Content.DOAnchorPos(new Vector2(0f,1690f), 0.7f);
        StartCoroutine( WaitDo(obj, false, 0.45f,true));
        openedScene = "calendar";
        Debug.Log(previousDate);
    }
    public void OpenFirstScene()
    {
        SceneManager.LoadScene(0);
    }
    public void ShowSettings(GameObject SettingsPanel)
    {
        RectTransform settingPosition = SettingsPanel.GetComponent<RectTransform>();
        settingPosition.DOAnchorPos(Vector2.zero, 0.4f);
    }
    public void ShowAllInfo(GameObject SettingsPanel)
    {
        MainContent.SetActive(false);
        ViewModel.ShowAllEvents();
        ShowSettings(SettingsPanel);
    }
    public void CloseAllInfo(GameObject SettingsPanel)
    {
        MainContent.SetActive(true);
        ViewModel.ClearAllContainer();
        SetSelectedText();
        CloseSettings(SettingsPanel);
    }
    public void CloseSettings(GameObject SettingsPanel)
    {
        RectTransform settingPosition = SettingsPanel.GetComponent<RectTransform>();
        settingPosition.DOAnchorPos(new Vector2(1080,0), 0.4f);
    }
    public void ShowSelectedDayPanel(GameObject Panel)
    {
        ButtonCurrentDay.enabled = false;
        ButtonCalendar.enabled = false;
        
        Panel.SetActive(true);
        //previousDate = dateCurrent;
        int dayNumber = CalendarScene.currentDayNumber;
        string day = "";

        int monthNumber = CalendarScene.currentMonth;
        string month = "";

        int year = CalendarScene.currentYear;
        if (dayNumber < 10)
            day = "0" + dayNumber;
        else day = dayNumber.ToString();
        
        if (monthNumber < 10)
            month = "0" + monthNumber;
        else month = monthNumber.ToString();
        
        if(dayNumber<DateTime.Today.Day && monthNumber <= DateTime.Today.Month || year<DateTime.Today.Year)
            Blocker.SetActive(true);
        else Blocker.SetActive(false);
        
        dateSelected.text = day + "." + month+ "." + year;
        dateSelectedCalendar.text = dateSelected.text;
        Content.DOAnchorPos(new Vector2(0f,55f), 0f);
        PlusButton.gameObject.SetActive(false);
        Schedule.SetActive(true);
        
        
    }
    public void CloseSelectedDayPanel(GameObject Panel)
    {
        ButtonCurrentDay.enabled = true;
        ButtonCalendar.enabled = true;
        
        Panel.SetActive(false);
        PlusButton.gameObject.SetActive(true);
        Schedule.SetActive(false);
        Content.DOAnchorPos(new Vector2(0f,1690f), 0f);
        dateSelectedCalendar.text = "";
    }
    public void ShowQuickAddMenu()
    {
        OpenPanel(QuickAddMenu);
        StartCoroutine(AnimationForQuickAddMenu(1, 1, 0.25f));
    }
    public void CloseQuickAddMenu()
    {
        CloseLastPanel();
        StartCoroutine(AnimationForQuickAddMenu(0, 0,0.05f));
    }
    
    
    
    private IEnumerator WaitDo(GameObject obj, bool show, float duration, bool close)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(show);
        yield return new WaitForSeconds(1- duration);
        ButtonCurrentDay.enabled = true;
        ButtonCalendar.enabled = true;
        if(close)
            Schedule.SetActive(false);
        else   Schedule.SetActive(true);

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
            ShowSuccessOperation("Добавлена новая заметка");
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
            {
                values.Add("description", eventDescription.text);
            }    
            
            dateCurrent = dateSelected.text;
            valuesNewEvent = values;
            
            if (CompareTime(eventStart.text, eventEnd.text))
            {
                CheckExistence(dateCurrent,values["startTime"],"eventNew");
            }
            else
            {
                if (separateEvent.isOn)
                {
                    DateTime currentDay = Convert.ToDateTime(dateCurrent);
                    currentDay = currentDay.AddDays(1f);
                    CheckExistence(ChangeDate(currentDay.Day.ToString()), "00:00","eventSeparated");
                }
                else
                {
                    errorEventLabel.text = "Ошибка: неправильно выбрано время";
                }
            }
        }
        else
            errorEventLabel.text = "Ошибка: не все обязательные поля заполнены.";
    }
    public static void UpdateEvent(string currentValue)
    {
        typeObject = "eventNew";
        eventOperation = "update";
        eventUpdate.transform.Find("PanelAddNewIvent/TimePickSection/PanelSeparate").gameObject.SetActive(false);
        bool exists = ViewModel.CheckIfExist(ViewModel.dateSelected, currentValue);
        
        if (exists)
        {
            eventUpdate.SetActive(true);
            MEvent currentEvent = ViewModel.existEvent;

            eventCategory.text = currentEvent.CategoryName;
            eventDescription.text = currentEvent.Description;
            eventEnd.text = currentEvent.EndTime;
            eventStart.text = currentEvent.StartTime;
            eventTitle.text = currentEvent.Title;

            ViewModel.selectedCategoryColour = currentEvent.CategoryColour;
            ViewModel.selectedCategoryName = currentEvent.CategoryName;
        }
    }
    
    private void CheckExistence(string date, string time,string type)
    {
        typeObject = type;
        bool exist = ViewModel.CheckIfExist(date, time);
      
        if (exist) //если событие с таким временем уже есть
        {
            if(eventOperation == "add")
                CheckSeparatedEvents(time);
            else
            {
                AddOneEvent();
            }
            //проверить, хочет ли перезаписать
        }
        else // если нет, то просто создаем
        {
            if (eventOperation == "add")
            {
                switch (typeObject)
                {
                    case "eventNew":
                    {
                        AddOneEvent();
                        break;
                    }
                    case "eventSeparated":
                    {
                        AddSeparatedEvent(); 
                        break;
                    }
                }
            }
            else
            {
                ViewModel.UpdateItem(valuesNewEvent);
                ShowSuccessOperation("Обновлено событие");
                ClearEventInfo();
            }
        }
    }
    
    public void ShowSelectTimePanel(GameObject panel)
    {
        currentTimeSelection = panel;
        
        OpenPanel(SelectTimePanel);
        
    }
    private void CheckSeparatedEvents(string time)
    {
        EventConfirmPanel.SetActive(true);
        EventConfirmPanel.transform.SetAsLastSibling();
        newEventText.text = "Событие с таким временем ("+time+") уже существует";
        newEventMessage.text = "Перезаписать событие? Предыдущее удалиться";
        
    }

    public void AdditionEvent()
    {
        switch (typeObject)
        {
            case "eventNew":
            {
                AddOneEvent();
                break;
            }
            case "eventSeparated":
            {
                AddSeparatedEvent();
                break;
            }
        }
        EventConfirmPanel.SetActive(false);
        EventConfirmPanel.transform.SetAsLastSibling();
        ClearEventInfo();
    }

    private void AddOneEvent()
    {
        ViewModel.AddNewEvent(valuesNewEvent);
        if(eventOperation =="add")
            ShowSuccessOperation("Добавлен пункт расписания");
        else ShowSuccessOperation("Обновлено успешно");
        ClearEventInfo();
    }

    private void AddSeparatedEvent()
    {
        string endTime = valuesNewEvent["endTime"];
        valuesNewEvent["endTime"] = "23:59";
        ViewModel.AddNewEvent(valuesNewEvent);

        DateTime currentDay = Convert.ToDateTime(dateCurrent);
        currentDay = currentDay.AddDays(1f);

        ViewModel.dateSelected = ChangeDate(currentDay.Day.ToString());

        valuesNewEvent["startTime"] = "00:00";
        valuesNewEvent["endTime"] = endTime;
        ViewModel.AddNewEvent(valuesNewEvent);
        ShowSuccessOperation("Добавлены пункты расписания");
        ClearEventInfo();
        ViewModel.dateSelected = dateSelected.text;
    }

    public static void Delete(TextMeshProUGUI key, string type)
    {
        typeObject = type;
        
        deletePanel.gameObject.SetActive(true);
        deletePanel.SetAsLastSibling();
        
        UpdateDeleteMessage(key.text);
    }
    private static void UpdateDeleteMessage(string title)
    {
        ConfirmText.text = "Вы уверены, что хотите удалить";
        
        switch (typeObject)
        {
            case "event":
            {
                ConfirmMessage.text = "пункт расписания " + "'"+ title + "' ?";
                break;
            }
            case "category":
            {
                ConfirmMessage.text = "категорию " + "'" + title + "' ?";
                break;
            }
            case "note":
            {
                ConfirmMessage.text = "выбранную заметку?";
                break;
            }
            case "list":
            {
                ConfirmMessage.text = "список " + "'" + title + "' ?";
                break;
            }
            case "user":
            {
                ConfirmText.text = "Вы уверены, что хотите выйти?";
                ConfirmMessage.text = "";
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
            case "user":
            {
                DBUser.LogOutUser();
                break;
            }
        }
        DeleteConfirmPanel.SetActive(false);
        DeleteConfirmPanel.transform.SetAsFirstSibling();
    }

    public void LogOut()
    {
        typeObject = "user";
        deletePanel.gameObject.SetActive(true);
        deletePanel.SetAsLastSibling();
        
        UpdateDeleteMessage("");
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
        eventOperation = "add";
        ViewModel.selectedCategoryName = "По умолчанию";
        ViewModel.selectedCategoryColour = "#9792F7";
        time.SetDefaultTime();
        separateEvent.isOn = false;
        eventUpdate.transform.Find("PanelAddNewIvent/TimePickSection/PanelSeparate").gameObject.SetActive(true);
    }
    private void ClearListInfo()
    {
        listName.text = "";
        for (var i = ToDoObjectsContainer.childCount - 1; i >= 0; i--)
        {
            var objectA = ToDoObjectsContainer.GetChild(i);
            objectA.SetParent(null);
            Destroy(objectA.gameObject);
        }
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

        if (value != "")
        {
            ViewModel.AddTodoObject(value);
            input.text = "";
        }
    }
    public void OnButtonAdmitTextInput(TMP_InputField value)
    {
        if (value.text != "")
        {
            if (openedScene == "current day")
            {
                ViewModel.AddTodo(value.text);
                value.text = "";
                TextInputPanel.SetActive(false);
            }
            else
            {

                CalendarScene.AddNewItemInPlan(value.text);
                value.text = "";
                TextInputPanel.SetActive(false);
            }
        }
    }
    private void ShowSuccessOperation(string message)
    {
        CloseAll();
        successText.text = message;
        OpenPanel(PanelSuccessfulOperation.gameObject);
    }

    public void AddNewList(Transform ObjectsSection)
    {
        string newListName = listName.text;

        if (newListName != "" && ToDoObjectsContainer.childCount > 0)
        {
            Dictionary<string, bool> newListItems = new Dictionary<string, bool>();

            foreach (Transform item in ToDoObjectsContainer)
            {
                string title = item.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                if (!newListItems.ContainsKey(title))
                    newListItems.Add(title, false);
            }

            MTodo newList = new MTodo
            {
                nameList = newListName,
                itemsList = newListItems
            };

            ViewModel.AddList(newList);
            ShowSuccessOperation("Добавлен новый список дел");
            ClearListInfo();
        }
    }

    IEnumerator AnimationForQuickAddMenu(float x, float y, float duration)
    {
        addNote.DOScale(new Vector3(x,y), duration);
        yield return new WaitForSeconds(0.08f);
        addEvent.DOScale(new Vector3(x,y), duration);
        yield return new WaitForSeconds(0.08f);
        addList.DOScale(new Vector3(x,y), duration);
    }
 
    
    
    
    public void ShowAllEvents(TextMeshProUGUI textLabel)
    {
        SelectTab(textLabel);
        ButtonSort.SetActive(true);
        ViewModel.ShowAllEvents();
        StartCoroutine(WaitForSelectTab(textLabel));
    }
    public void ShowAllNotes(TextMeshProUGUI textLabel)
    {
        SelectTab(textLabel);
        ButtonSort.SetActive(false);
        ViewModel.ShowAllNotes();
        StartCoroutine(WaitForSelectTab(textLabel));
    }
    public void ShowAllLists(TextMeshProUGUI textLabel)
    {
        SelectTab(textLabel);
        ButtonSort.SetActive(false);
        ViewModel.ShowAllLists();
        StartCoroutine(WaitForSelectTab(textLabel));
    }

    
    
    private void SelectTab(TextMeshProUGUI textLabel)
    {
        if (selectedText)
        {
            selectedText.color = InterfaceTheme.GreyDarkCustom;
        }

        selectedText = textLabel;
        selectedText.color = InterfaceTheme.PurpleCustom;
        
        if(ButtonCancelSort)
            ButtonCancelSort.SetActive(false);
    }
    private void SetSelectedText()
    {
        SelectTab(EventText);
    }
    public void SortEvents()
    {
        ButtonCancelSort.SetActive(true);
        CategorySortPanel.SetActive(false);
        ViewModel.ShowSelectedEvents();
    }
    public void CancelSort()
    {
        ButtonCancelSort.SetActive(false);
        ViewModel.ShowAllEvents();
    }
    IEnumerator WaitForSelectTab(TextMeshProUGUI textLabel)
    {
        GameObject tab = textLabel.transform.parent.gameObject;
        tab.GetComponent<Button>().enabled = false;
        yield return new WaitForSeconds(1f);
        tab.GetComponent<Button>().enabled = true;
    }
    public void ChangeShowType(GameObject dropdown)
    {
        string value = dropdown.GetComponent<TMP_Dropdown>().captionText.text;
        ViewModel.ChangeShowType(value);
    }

    private bool CompareTime(string startTime, string endTime)
    {
        bool result = false;
        
        int startHour = Convert.ToInt32(startTime[0] +""+ startTime[1]);
        int startMinutes = Convert.ToInt32(startTime[3] + ""+startTime[4]);

        int endHour = Convert.ToInt32(endTime[0] + ""+endTime[1]);
        int endMinutes = Convert.ToInt32(endTime[3] +""+ endTime[4]);

        if (startHour < endHour)
            result = true;
        else if (startHour == endHour && startMinutes < endMinutes)
            result = true;
        
        return result;
    }

    private string ChangeDate(string date)
    {
        string result = "";
        
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
        result = newDate.ToString("dd/MM/yyyy", localCultureInfo);
        return result;
    }
}