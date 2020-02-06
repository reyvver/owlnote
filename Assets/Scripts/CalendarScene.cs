using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using TMPro;

public class CalendarScene : MonoBehaviour
{
    public TextMeshProUGUI tMonth, tDayOfWeek;
    private List<int> NumberOfDaysInMonths = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        GetDates();
        InitializeDays();
        ViewCalendar();
        Days();
    }

    
    private void GetDates()
    {
        string day_of_week = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
        string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Today.Month);

        tMonth.text = ToTitleCase(month);
        tDayOfWeek.text = ToTitleCase(day_of_week);
    }

    private void Days()
    {
        DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
        GameObject.Find("Day1").GetComponent<TextMeshProUGUI>().text = dtfi.GetShortestDayName(DayOfWeek.Monday);
        GameObject.Find("Day2").GetComponent<TextMeshProUGUI>().text = dtfi.GetShortestDayName(DayOfWeek.Tuesday);
        GameObject.Find("Day3").GetComponent<TextMeshProUGUI>().text = dtfi.GetShortestDayName(DayOfWeek.Wednesday);
        GameObject.Find("Day4").GetComponent<TextMeshProUGUI>().text = dtfi.GetShortestDayName(DayOfWeek.Thursday);
        GameObject.Find("Day5").GetComponent<TextMeshProUGUI>().text = dtfi.GetShortestDayName(DayOfWeek.Friday);
        GameObject.Find("Day6").GetComponent<TextMeshProUGUI>().text = dtfi.GetShortestDayName(DayOfWeek.Saturday);
        GameObject.Find("Day7").GetComponent<TextMeshProUGUI>().text = dtfi.GetShortestDayName(DayOfWeek.Sunday);

    }

    private string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
    private void ViewCalendar()
    {
        DateTime firstday = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        int day = Convert.ToInt32( firstday.DayOfWeek);
        int month = firstday.Month;
        int currentDay = 1;
        int start;
        bool chk = false;
        // Debug.Log( day + "  " + NumberOfDaysInMonths[month - 1]+ "  "+ Difference(day) + "  "+currentDay);
        /*Заполнить текущий месяц*/
        for (int i = 1; i<=6; i++)
        {
            GameObject week = GameObject.Find("Week" + i);

            if (i == 1) start = day; else start = 1;
            
            for (int j = start; j <= 7; j++)
            {
                Transform date = week.transform.Find(j.ToString());
                TextMeshProUGUI numberText = date.Find("Number").GetComponent<TextMeshProUGUI>();
                if (DateTime.Today.Day == currentDay)
                {
                   Image panel = date.Find("Panel").GetComponent<Image>();
                   panel.color = new Color32(174,96,255,255);
                   numberText.color = Color.white;
                }
                numberText.text = currentDay.ToString();

                currentDay++;
                if (currentDay > NumberOfDaysInMonths[month - 1])
                {
                    chk = true;
                    break;
                }
            }
            if (chk)
               break;
        }
    }
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
    private bool WhatTypeOfYear()
    {
        bool chk = false;
        if (DateTime.Today.Year % 4 == 0)
            chk = true;
        return chk;
    }
}
