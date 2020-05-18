using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using TMPro;

public class CalendarScene : MonoBehaviour
{

    public TextMeshProUGUI monthText, nextText, previousText, currentYearText;
    private int[] daysPerMonth = new[] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
    public int currentYear, currentMonth;
    
    private CultureInfo localCultureInfo = new CultureInfo("ru-RU");

    void Start()
    {
        currentYear = DateTime.Today.Year;
        currentMonth = DateTime.Today.Month;
        CheckTypeOfYear();
        FillCalendar();
        UpdateMonthTimeline();

    }

    private void FillCalendar()
    {
        //Определяется день недели первого дня в месяце
        int firstDayOfWeek = (int) new DateTime(currentYear, currentMonth, 1).DayOfWeek;
        int currentDay = 1; //Номер дня (Будет менятся от 1 до 30 или 31)
        bool chk = false; //Переменная для проверки полной заполненности месяца
        if (firstDayOfWeek == 0) firstDayOfWeek = 7;
        if (firstDayOfWeek == 6) firstDayOfWeek = 1;
        
        //Для первой недели:
        GameObject firstWeek = GameObject.Find("Week1");
        for (int i = firstDayOfWeek; i <= 7; i++)
        {
            Transform date = firstWeek.transform.Find(Convert.ToString(i)); //день в неделе
            TextMeshProUGUI numberText = date.Find("Number").GetComponent<TextMeshProUGUI>(); //находится его текстовое поле
            numberText.text = currentDay.ToString(); //присваевается номер
            ColourCurrentDay(date, currentDay);
            currentDay++;
        }

        //Для последующих недель
        for (int i = 2; i <= 6; i++)
        {
            GameObject week = GameObject.Find("Week" + i); //берется следующая неделя
            for (int j = 1; j <= 7; j++)
            {
                Transform date = week.transform.Find(Convert.ToString(j)); //день в неделе
                TextMeshProUGUI
                    numberText = date.Find("Number").GetComponent<TextMeshProUGUI>(); //находится его текстовое поле
                numberText.text = currentDay.ToString(); //присваевается номер
                ColourCurrentDay(date, currentDay);

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
    }

    private void ColourCurrentDay(Transform date, int currentDay)
    {
        if (DateTime.Today.Day == currentDay && currentMonth == DateTime.Today.Month && currentYear == DateTime.Today.Year) //закрашиваем сегодняшний день
        {
            TextMeshProUGUI
                numberText = date.Find("Number").GetComponent<TextMeshProUGUI>(); //находится его текстовое поле
            Image panel = date.Find("Panel").GetComponent<Image>();
            panel.color = new Color32(146, 96, 255, 255);
            numberText.color = Color.white;
        }
    }

    private void CheckTypeOfYear() //Если год високосный, то в феврале 29 дней
    {
        if (DateTime.Today.Year % 4 == 0)
            daysPerMonth[1] = 29;
    }

    public void NextMonth()
    {
        ClearAll();
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
        ClearAll();
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
        int previous = currentMonth-1;
        int next = currentMonth+1;
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

    private void ClearAll()
    {
        for (int i = 1; i <= 6; i++)
        {
            GameObject week = GameObject.Find("Week" + i); 
            
            for (int j = 1; j <= 7; j++)
            {
                Transform date = week.transform.Find(Convert.ToString(j));
                TextMeshProUGUI numberText = date.Find("Number").GetComponent<TextMeshProUGUI>(); 
                numberText.text = "";
                Image panel = date.Find("Panel").GetComponent<Image>();
                panel.color = new Color32(255,255,255,255);
                numberText.color = Color.black;
            }
        }
    }
    public string ToTitleCase(string str)
    {
        return localCultureInfo.TextInfo.ToTitleCase(str.ToLower());
    }
}