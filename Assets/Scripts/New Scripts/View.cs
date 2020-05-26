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

    [Header("Headers of containers")] 
    public GameObject EventsHeader;
    public GameObject NotesHeader;

    [Header("Prefabs")]
    public GameObject EventPrefab;
    public GameObject NotePrefab;
    public GameObject CategoryPrefab;

    [Header("UI elements")] 
    public GameObject EmptySchedulePanel;
    public GameObject DeleteConfirmPanel;
    public GameObject TimePicker;
    public GameObject SelectTimePanel;
    public GameObject EmailVerifyPanel;
    
    public TMP_InputField dateSelected;
    public Transform DatePlatesContainer;
    public Transform PanelSuccessfulOperation;
    public Transform AddEventPanel;

    public Text newCategoryColour;

    private ScrollRectScript time;
    private string dateCurrent;
    private static string typeDelete;
    private static string currentKey;
    private bool _importantOperation;
    private static GameObject deletePanel;
    private TextMeshProUGUI successText;
    private GameObject currentTimeSelection;

    private List<int> NumberOfDaysInMonths = new List<int>(); // дни в месяце
    private string[] DaysToString = new[] {"ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ", "ВС"};
    public  List<GameObject> OpenedPanels;


    private static Dictionary<string, List<MEvent>> eventsValues = new Dictionary<string, List<MEvent>>();
    private Dictionary<string, string> categoriesValues = new Dictionary<string, string>();




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
        
        Debug.Log("done with viewmodel");
    }

    private void Start()
    {
        dateCurrent = DateTime.Today.ToString("dd/MM/yyyy", new CultureInfo("ru-RU"));
        dateSelected.text = dateCurrent;
        deletePanel = DeleteConfirmPanel;
        time = TimePicker.GetComponent<ScrollRectScript>();

        successText = PanelSuccessfulOperation.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        
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
                // if (_importantOperation) LogOutScript.LogOutUser();
                //   else
                // {
                CloseAll();
                // }
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
        string dateToString = newDate.ToString("dd/MM/yyyy", new CultureInfo("ru-RU"));
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

    private void SetDefaultCategory()
    {
        AddEventPanel.GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "По умолчанию";
        ViewModel.selectedCategoryName = "По умолчанию";
        ViewModel.selectedCategoryColour = "#9792F7";
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

    public static void Delete(TextMeshProUGUI key, string type, string other)
    {
        typeDelete = type;
        
        deletePanel.gameObject.SetActive(true);
        deletePanel.transform.SetAsLastSibling();
        
            UpdateDeleteMessage(key.text);
            currentKey = key.text;
    }
    
    private static void UpdateDeleteMessage(string title)
    {
        TextMeshProUGUI message = deletePanel.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        
        switch (typeDelete)
        {
            case "event":
            {
                message.text = "пункт расписания " + "'"+ title + "'";
                break;
            }
            case "category":
            {
                message.text = "категорию " + "'" + title + "'";
                break;
            }
        }
    }

    public void DeleteInfo()
    {
        switch (typeDelete)
        {
            case "event":
            {
                ViewModel.DeleteEvent();
                break;
            }
            case "note":
            {
                break;
            }
            case "category":
            {
                ViewModel.DeleteCategory(currentKey);
                break;
            }
        }
        DeleteConfirmPanel.gameObject.SetActive(false);
        DeleteConfirmPanel.transform.SetAsFirstSibling();
    }


    public void AddNewEvent()
    {
        TMP_InputField title = AddEventPanel.GetChild(1).GetChild(1).GetComponent<TMP_InputField>();
        TextMeshProUGUI startTime = AddEventPanel.GetChild(3).GetChild(2).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI endTime = AddEventPanel.GetChild(3).GetChild(3).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        TMP_InputField description = AddEventPanel.GetChild(4).GetChild(2).GetChild(1).GetComponent<TMP_InputField>();
        
        TextMeshProUGUI errorLabel = AddEventPanel.GetChild(5).GetComponent<TextMeshProUGUI>();
        
        if (title.text != "" && startTime.text != "Не выбрано" & endTime.text != "Не выбрано")
        {
            
           Dictionary<string, string> values = new Dictionary<string, string>
           {
               {"title", title.text}, {"startTime", startTime.text}, {"endTime", endTime.text}
           };
           
           if (description.text!="")
            values.Add("description", description.text);
           
           ViewModel.AddNewEvent(values);
           CloseAll();
           successText.text = "Добавлен пункт расписания";
           OpenPanel(PanelSuccessfulOperation.gameObject);
           SetDefaultCategory();
        }
        else
            errorLabel.text = "Ошибка: не все обязательные поля заполнены.";
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

}