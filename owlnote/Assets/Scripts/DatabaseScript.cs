using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using TMPro;

public class DatabaseScript : MonoBehaviour
{
    public TextMeshProUGUI tMonth, tDayOfWeek;
    
    // Start is called before the first frame update
    void Start()
    {
        GetDates();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDates()
    {
        string day_of_week = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
        string month  = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Today.Month);
        
        tMonth.text = ToTitleCase(month);
        tDayOfWeek.text = ToTitleCase(day_of_week);
    }

    /*Процедура, возвращающая слово с заглавной буквы*/
    public string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
}
