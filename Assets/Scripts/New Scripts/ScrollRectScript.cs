using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.UI.Extensions;
using TMPro;

public class ScrollRectScript : MonoBehaviour
{
    public TMP_Text textResult;
    public string hours, minutes;
    public GameObject scrollerHour, scrollerMinutes;

    private UIVerticalScroller hoursScroll, minutesScroll;

    private void Start()
    {
        hoursScroll = scrollerHour.GetComponent<UIVerticalScroller>();
        minutesScroll = scrollerMinutes.GetComponent<UIVerticalScroller>();
    }

    private void Update()
    { 
        hours =hoursScroll.result;
        minutes = minutesScroll.result;
        textResult.text =  hours +  " ч " + minutes+ " мин";
    }

    public void SetDefaultTime()
    {
        hoursScroll.SnapToElement(0);
        minutesScroll.SnapToElement(4);
    }
    
}
