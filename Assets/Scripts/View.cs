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
    public GameObject EmptySchedulePanel;
    public GameObject DeleteConfirmPanel;
    public GameObject TimePicker;
    public GameObject SelectTimePanel;
    public GameObject EmailVerifyPanel;
    public GameObject UpdateNotePanel;
    public GameObject TextInputPanel;
    public GameObject categorySelect;
    public GameObject showTypeSelect;
    public GameObject QuickAddMenu;
    public TMP_InputField dateSelected;
    public Transform DatePlatesContainer;
    public Transform PanelSuccessfulOperation;
    public Transform AddEventPanel;
    public Transform HeaderTitle;
    public Text newCategoryColour;
    public  List<GameObject> OpenedPanels;
    
    public static TMP_InputField noteUpdateText;
    public static Transform textInputPanel;

    
    private ScrollRectScript time;
    private string dateCurrent, openedScene;
    private static string typeObject;

    private bool _importantOperation;
    private static Transform deletePanel;
    private TextMeshProUGUI successText;
    

    private GameObject currentTimeSelection;
    private static GameObject noteUpdate;
 

    private List<int> NumberOfDaysInMonths = new List<int>(); // дни в месяце
    private string[] DaysToString = {"ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ", "ВС"};

    private CultureInfo localCultureInfo = new CultureInfo("ru-RU");

    private static Dictionary<string, List<MEvent>> eventsValues = new Dictionary<string, List<MEvent>>();
    private Dictionary<string, string> categoriesValues = new Dictionary<string, string>();
    private RectTransform addNote;
    private RectTransform addEvent;
    private RectTransform addList; 

    [NonSerialized] public TextMeshProUGUI eventStart, eventEnd, errorEventLabel, eventCategory;
    [NonSerialized] public TMP_InputField eventTitle, eventDescription;
    [NonSerialized] public TextMeshProUGUI todayMonth, todayDayOfWeek;
    [NonSerialized] public Button ButtonCurrentDay, ButtonCalendar;
    [NonSerialized] private RectTransform Header, PlusButton, Content;
    [NonSerialized] public TextMeshProUGUI selectedText, EventText;
    [NonSerialized] public GameObject ButtonCancelSort, ButtonSort, CategorySortPanel;

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

        todayMonth = HeaderTitle.GetChild(0).GetComponent<TextMeshProUGUI>();
        todayDayOfWeek = HeaderTitle.GetChild(1).GetComponent<TextMeshProUGUI>();

        todayMonth.text = ToTitleCase(localCultureInfo.DateTimeFormat.GetMonthName(DateTime.Today.Month));
        todayDayOfWeek.text = ToTitleCase(localCultureInfo.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek));

        ButtonCalendar = QuickAddMenu.transform.parent.Find("Footer/MenuFooter/Calendar").GetComponent<Button>();
        ButtonCurrentDay =  QuickAddMenu.transform.parent.Find("Footer/MenuFooter/CurrentDay").GetComponent<Button>();

        noteUpdate = UpdateNotePanel;
        noteUpdateText = UpdateNotePanel.transform.Find("Panel/NoteText").GetComponent<TMP_InputField>();

        addNote = QuickAddMenu.transform.Find("AddNote").GetComponent<RectTransform>();
        addEvent = QuickAddMenu.transform.Find("AddEvent").GetComponent<RectTransform>();
        addList = QuickAddMenu.transform.Find("AddToDo").GetComponent<RectTransform>();
        
        Header = QuickAddMenu.transform.parent.Find("Header").GetComponent<RectTransform>();
        PlusButton = QuickAddMenu.transform.parent.Find("PlusButton").GetComponent<RectTransform>();
        Content = EmptySchedulePanel.transform.parent.GetComponent<RectTransform>();

        selectedText = PanelSuccessfulOperation.parent.Find("AllInfoPanel/PanelHeader/Events/Text").GetComponent<TextMeshProUGUI>();
        EventText = selectedText;

        ButtonSort = PanelSuccessfulOperation.parent.Find("AllInfoPanel/ButtonSort").gameObject;
        ButtonCancelSort = PanelSuccessfulOperation.parent.Find("AllInfoPanel/ButtonCancel").gameObject;
        CategorySortPanel = PanelSuccessfulOperation.parent.Find("AllInfoPanel/SortByCategory").gameObject;
        
        PlusButton.gameObject.SetActive(false);
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
        if (OpenedPanels.Count > 0)
        {
            //int lastPanelIndex = OpenedPanels.Count - 1;

            if (QuickAddMenu)
            {
                CloseQuickAddMenu();
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
        ButtonCurrentDay.enabled = false;
        ButtonCalendar.enabled = false;
        
        Header.DOAnchorPos(new Vector2(0f,-203), 0.5f);
        Content.DOAnchorPos(new Vector2(0f,-714f), 1f);
        StartCoroutine( WaitDo(obj, true,0f));
        PlusButton.gameObject.SetActive(false);
        openedScene = "current day";
    }
    public void OpenCalendarPage(GameObject obj)
    {
        ButtonCurrentDay.enabled = false;
        ButtonCalendar.enabled = false;
        PlusButton.gameObject.SetActive(true);
        Header.DOAnchorPos(new Vector2(0f,226f), 0.5f);
        Content.DOAnchorPos(new Vector2(0f,1200f), 0.7f);
        StartCoroutine( WaitDo(obj, false, 0.45f));
        openedScene = "calendar";
    }
    public void OpenFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator WaitDo(GameObject obj, bool show, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(show);
        yield return new WaitForSeconds(1- duration);
        ButtonCurrentDay.enabled = true;
        ButtonCalendar.enabled = true;
    }

    private IEnumerator ButtonDo(float duration)
    {
        yield return new WaitForSeconds(duration);
        ButtonCurrentDay.enabled = true;
        ButtonCalendar.enabled = true;
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
                values.Add("description", eventDescription.text);
            
            ViewModel.AddNewEvent(values);
            ShowSuccessOperation("Добавлен пункт расписания");
            ClearEventInfo();
        }
        else
            errorEventLabel.text = "Ошибка: не все обязательные поля заполнены.";
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
        DeleteConfirmPanel.SetActive(false);
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

        if (value != "")
        {
            ViewModel.AddTodoObject(value);
            input.text = "";
        }
    }

    public void OnButtonAdmitTextInput(TMP_InputField value)
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

    private void ShowSuccessOperation(string message)
    {
        CloseAll();
        successText.text = message;
        OpenPanel(PanelSuccessfulOperation.gameObject);
    }

    public void AddNewList(Transform ObjectsSection)
    {
        string newListName = ObjectsSection.parent.Find("TitleSection/TitleInput").GetComponent<TMP_InputField>().text;

        Transform items = ObjectsSection.Find("Scroll view/Viewport/ContentTodoObjects");
        if (newListName != "" && items.childCount > 0)
        {
            Dictionary<string, bool> newListItems = new Dictionary<string, bool>();

            foreach (Transform item in items)
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
        }
    }

    public void ShowSettings(GameObject SettingsPanel)
    {
        RectTransform settingPosition = SettingsPanel.GetComponent<RectTransform>();
        settingPosition.DOAnchorPos(Vector2.zero, 0.4f);
    }
    
    public void ShowAllInfo(GameObject SettingsPanel)
    {
        ViewModel.ShowAllEvents();
        ShowSettings(SettingsPanel);
    }
    public void CloseAllInfo(GameObject SettingsPanel)
    {
        ViewModel.ClearAllContainer();
        SetSelectedText();
        CloseSettings(SettingsPanel);
    }

    public void CloseSettings(GameObject SettingsPanel)
    {
        RectTransform settingPosition = SettingsPanel.GetComponent<RectTransform>();
        settingPosition.DOAnchorPos(new Vector2(1080,0), 0.4f);
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

   
}