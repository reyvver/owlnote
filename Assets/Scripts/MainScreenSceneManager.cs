using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using TMPro;

public class MainScreenSceneManager : MonoBehaviour
{
    public GameObject FirstScene, objSceneManager;
    private MainScreenScript MainScreenScript;
    private LogOut LogOutScript;
    public GameObject TodayPlate, NumberPanel;


    private List<int> NumberOfDaysInMonths = new List<int>();
    private List<GameObject> NumberPlates = new List<GameObject>();
    private List<GameObject> NumberDays = new List<GameObject>();
    public GameObject AddPanel, ButtonAdd;
    public TextMeshProUGUI tCurrentDay, tMonth, tDayOfWeek;
    public GameObject panelSuccess;

    public List<GameObject> openedPanels;
    private bool _visibility;
    public bool  _importantOperation;

    // Start is called before the first frame update
    void Start()
    {
        MainScreenScript = objSceneManager.GetComponent<MainScreenScript>();
        LogOutScript = objSceneManager.GetComponent<LogOut>();
        FirstScene.transform.SetAsLastSibling();
        InitializeDays();
        GetDates();
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (openedPanels.Count > 0)
        {
            if (openedPanels[openedPanels.Count - 1] == panelSuccess)
            {
                if (_importantOperation) LogOutScript.LogOutUser();
                else
                {
                    _visibility = true;
                    CloseAll();
                }
            }
            else
            {
                openedPanels[openedPanels.Count - 1].SetActive(false);
                openedPanels.RemoveAt(openedPanels.Count - 1);
            }
        }

        if (openedPanels.Count == 0 && _visibility)
        {
            _visibility = !_visibility;
            ButtonAdd.transform.Rotate(0, 0, 45);
        }
        
    }
    
    public void CloseAll()
    {
        for (int i = 0; i < openedPanels.Count; i++)
        {
            openedPanels[i].SetActive(false);
        }
        openedPanels.Clear();

        if (_visibility)
        {
            ButtonAdd.transform.Rotate(0, 0, 45);
            _visibility = !_visibility;
        }
    }
    
    /*Выводит всю информацию о месяце, датах и тд в текстовые инпуты, которые есть на сцене*/
    public void GetDates()
    {
        string day = DateTime.Today.Day.ToString();
        string day_of_week = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
        string month  = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Today.Month);
            
        tMonth.text = ToTitleCase(month);
        tDayOfWeek.text = ToTitleCase(day_of_week);
        tCurrentDay.text = day;

        WriteDays();
    }
    /*Относительно текущей даты определяет следующие*/
    private void WriteDays()
    {
        InitializeDatesPanels();
        
        int currentDay = Convert.ToInt32(tCurrentDay.text);
        int currentMonth = DateTime.Today.Month;
        
        TodayPlate.transform.Find("BackPanel").GetComponent<Image>().color = new Color32(144, 96, 255, 255);
        TodayPlate.transform.Find("Number").GetComponent<TextMeshProUGUI>().color = Color.white;
        TodayPlate.transform.Find("Text").GetComponent<TextMeshProUGUI>().color =  Color.white;
        
        for (int i = 0; i < NumberPlates.Count; i++)
        {
            NumberPlates[i].GetComponent<TextMeshProUGUI>().text = Convert.ToString(currentDay);
            currentDay++;
            if (currentDay > NumberOfDaysInMonths[currentMonth-1])
                currentDay = 1;
        }

        DateTime currentDayName =  DateTime.Today;
        DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
        for (int i = 0; i < NumberDays.Count; i++)
        {
            NumberDays[i].GetComponent<TextMeshProUGUI>().text = Convert.ToString(dtfi.GetShortestDayName(currentDayName.DayOfWeek));
            currentDayName += TimeSpan.FromDays(1);
        }
 }
    /*Делает ссылку на объекты, содержащиеся в NumberPanel*/
    private void InitializeDatesPanels()
    {
        int i = 1;
        foreach (Transform obj in  NumberPanel.transform)
        {
            GameObject plate = obj.Find("Number").gameObject;
            NumberPlates.Add(plate.gameObject);
            GameObject text = obj.Find("Text").gameObject;
            NumberDays.Add(text.gameObject);
            i++;
        }
    }
    /*Определяет количество дней в каждом месяце*/
    private void InitializeDays()
    {
        NumberOfDaysInMonths.Add(31); //January
        if (WhatTypeOfYear())
        NumberOfDaysInMonths.Add(29);//February
        else NumberOfDaysInMonths.Add(28);//February
        NumberOfDaysInMonths.Add(31);//March
        NumberOfDaysInMonths.Add(30);//April
        NumberOfDaysInMonths.Add(31);//May
        NumberOfDaysInMonths.Add(30);//June
        NumberOfDaysInMonths.Add(31);//July
        NumberOfDaysInMonths.Add(31);//August
        NumberOfDaysInMonths.Add(30);//September
        NumberOfDaysInMonths.Add(31);//October
        NumberOfDaysInMonths.Add(30);//November
        NumberOfDaysInMonths.Add(31);//December
    }
    /*Високосный или нет год*/
    private bool WhatTypeOfYear()
    {
        bool chk = false;
        if (DateTime.Today.Year % 4 == 0)
            chk = true;
        return chk;
    }
    /*Процедура, возвращающая слово с заглавной буквы*/
    public string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }

    public void ShowPanel(GameObject obj)
    {
        MainScreenScript.ClearText();
        openedPanels.Add(obj);
        obj.SetActive(true);
    }

    public void showAddMenu() {
        _visibility = !_visibility;
        AddPanel.SetActive(_visibility);
        openedPanels.Add(AddPanel);

        if (_visibility) {
            ButtonAdd.transform.Rotate(0, 0, -45);
        }
        else {
            ButtonAdd.transform.Rotate(0, 0, 45);
        }
    }
    

    public void ShowScene(GameObject obj)
    {
        obj.transform.SetAsLastSibling();
    }

    public void ClosePanel(GameObject obj)
    {
        obj.SetActive(false);
    }
}
