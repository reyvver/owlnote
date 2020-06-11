using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Collections.Generic;
using TMPro;

public class CalendarScene : MonoBehaviour
{
    [Header("UI elements")] public Transform TimelinePanel;
    public Transform CalendarPanel;
    private List<Transform> weeks = new List<Transform>();
    private TextMeshProUGUI monthText, nextText, previousText, currentYearText;

    private int[] daysPerMonth = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
    public static int currentYear, currentMonth;
    public static Transform selectedObj { get; set; }
    public static Transform currentDay { get; set; }

    private CultureInfo localCultureInfo = new CultureInfo("ru-RU");

    void Start()
    {
        monthText = TimelinePanel.Find("CurrentMonth/Text").GetComponent<TextMeshProUGUI>();
        currentYearText = TimelinePanel.Find("CurrentMonth/CurrentYear").GetComponent<TextMeshProUGUI>();


        nextText = TimelinePanel.Find("NextMonth").GetComponent<TextMeshProUGUI>();
        previousText = TimelinePanel.Find("PreviousMonth").GetComponent<TextMeshProUGUI>();


        foreach (Transform currentWeek in CalendarPanel)
        {
            weeks.Add(currentWeek);
        }

        currentYear = DateTime.Today.Year;
        currentMonth = DateTime.Today.Month;
        CheckTypeOfYear();
        FillCalendar();
        UpdateMonthTimeline();

    }

    private void FillCalendar()
    {
        ClearCalendar();
        //Определяется день недели первого дня в месяце
        int firstDayOfWeek = (int) new DateTime(currentYear, currentMonth, 1).DayOfWeek;
        int currentDay = 1; //Номер дня (Будет менятся от 1 до 30 или 31)
        bool chk = false; //Переменная для проверки полной заполненности месяца
        if (firstDayOfWeek == 0) firstDayOfWeek = 7;
        if (firstDayOfWeek == 6) firstDayOfWeek = 1;


        int weekNumber = 0;
        Transform currentWeek = weeks[weekNumber];

        for (int i = firstDayOfWeek; i <= 7; i++)
        {
            Transform date = currentWeek.Find(Convert.ToString(i));
            TextMeshProUGUI
                numberText = date.Find("Number").GetComponent<TextMeshProUGUI>(); //находится его текстовое поле
            numberText.text = currentDay.ToString(); //присваевается номер
            currentDay++;
        }

        for (int i = 2; i <= 6; i++)
        {
            weekNumber++;
            currentWeek = weeks[weekNumber]; //берется следующая неделя
            for (int j = 1; j <= 7; j++)
            {
                Transform date = currentWeek.Find(Convert.ToString(j)); //день в неделе
                TextMeshProUGUI
                    numberText = date.Find("Number").GetComponent<TextMeshProUGUI>(); //находится его текстовое поле
                numberText.text = currentDay.ToString(); //присваевается номер

                currentDay++;
                if (currentDay > daysPerMonth[currentMonth - 1])
                {
                    chk = true;
                    break; //выход из внутреннего цикла
                }
            }

            if (chk)
                break; //выход из внешнего цикла
        }

        ShowTodayDate();
    }

    private void CheckTypeOfYear() //Если год високосный, то в феврале 29 дней
    {
        if (DateTime.Today.Year % 4 == 0)
            daysPerMonth[1] = 29;
    }

    public void NextMonth()
    {
        currentMonth++;
        if (currentMonth == 13)
        {
            currentYear++;
            currentMonth = 1;
        }

        UpdateMonthTimeline();
        FillCalendar();
    }

    public void PreviousMonth()
    {
        currentMonth--;
        if (currentMonth == 0)
        {
            currentYear--;
            currentMonth = 12; 
        }

        UpdateMonthTimeline();
        FillCalendar();
    }

    private void UpdateMonthTimeline()
    {
        int previous = currentMonth - 1;
        int next = currentMonth + 1;
        if (previous == 0) previous = 12;
        if (next == 13) next = 1;
        currentYearText.text = currentYear.ToString();
        monthText.text = MonthName(currentMonth);
        nextText.text = MonthName(next);
        previousText.text = MonthName(previous);
    }

    private string MonthName(int index)
    {
        string str = localCultureInfo.DateTimeFormat.GetMonthName(index);
        return ToTitleCase(str);
    }

    private void ShowTodayDate()
    {
        if (selectedObj)
        {
            ClearDayPlate(selectedObj);
        }

        if (currentMonth == DateTime.Today.Month && currentYear == DateTime.Today.Year)
        {
            foreach (Transform week in weeks)
            {
                foreach (Transform day in week)
                {
                    TextMeshProUGUI numberText = day.Find("Number").GetComponent<TextMeshProUGUI>();
                    Image panel = day.Find("Panel").GetComponent<Image>();

                    if (numberText.text == DateTime.Today.Day.ToString())
                    {
                        panel.color = InterfaceTheme.PurpleCustom;
                        numberText.color = Color.white;
                        currentDay = day;
                        break;
                    }
                }
            }
        }
        else
        {
            if (currentDay)
            {
                ClearDayPlate(currentDay);
            }
        }
    }

    private void ClearCalendar()
    {
        foreach (Transform week in weeks)
        {
            foreach (Transform day in week)
            {
                TextMeshProUGUI numberText = day.Find("Number").GetComponent<TextMeshProUGUI>();
                numberText.text = "";
            }
        }
    }

    private void ClearDayPlate(Transform day)
    {
        TextMeshProUGUI numberText = day.Find("Number").GetComponent<TextMeshProUGUI>();
        Image panel = day.Find("Panel").GetComponent<Image>();

        panel.color = Color.white;
        numberText.color = Color.black;
    }

    public string ToTitleCase(string str)
    {
        return localCultureInfo.TextInfo.ToTitleCase(str.ToLower());
    }

    public void ToToday()
    {
        currentMonth = DateTime.Today.Month;
        currentYear = DateTime.Today.Year;
        UpdateMonthTimeline();
        FillCalendar();
    }


}