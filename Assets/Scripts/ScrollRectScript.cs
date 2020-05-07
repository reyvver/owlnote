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
    void Update()
    { 
        hours = scrollerHour.GetComponent<UIVerticalScroller>().result;
        minutes = scrollerMinutes.GetComponent<UIVerticalScroller>().result;
        textResult.text =  hours +  " ч " + minutes+ " мин";
    }

}
