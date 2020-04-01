using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using Firebase.Auth;
using TMPro;

public class MainScreenScene : MonoBehaviour
{
    private List<int> NumberOfDaysInMonths = new List<int>();
    private List<GameObject> NumberPlates = new List<GameObject>();
    private List<GameObject> NumberDays = new List<GameObject>();
    public TextMeshProUGUI tCurrentDay, tMonth, tDayOfWeek;
    public TextMeshProUGUI verifyEmail;
    public GameObject panelVerify, blurBackground, barrier;

    private bool _chk_email;

    // Start is called before the first frame update
    void Start()
    {
        InitializeDays();
        GetDates();

        StartCoroutine(CheckUserEmail());
    }

    IEnumerator CheckUserEmail()
    {
        ReloadUser();
        yield return new WaitUntil(() => _chk_email);
        if (FirebaseAuth.DefaultInstance.CurrentUser.UserId != "AVATC0nCWxd1l3saRQhbdoTFjVI3")
          {
              FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
              if (FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified == false)
               {
                   panelVerify.SetActive(true);
                   blurBackground.SetActive(true);
                   barrier.SetActive(true);
                   verifyEmail.text = FirebaseAuth.DefaultInstance.CurrentUser.Email;
               }
          }
    }

    private void ReloadUser()
    {
        FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync().ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("Отменено");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("Произошла ошибка:  " + task.Exception);
                return;
            }

            _chk_email = true;
        });
    }



    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
        
        GameObject.Find("NumberPlate0").transform.Find("BackPanel").GetComponent<Image>().color = new Color32(144, 96, 255, 255);
        GameObject.Find("NumberPlate0").transform.Find("Number").GetComponent<TextMeshProUGUI>().color = new Color32(255,255,255,255);
        GameObject.Find("NumberPlate0").transform.Find("Text").GetComponent<TextMeshProUGUI>().color = new Color32(255,255,255,255);
        
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
        GameObject panel = GameObject.Find("NumberPanel");
        foreach (Transform obj in  panel.transform)
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
    
    
}
