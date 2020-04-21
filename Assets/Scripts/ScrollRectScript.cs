using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.UI.Extensions;
using TMPro;

public class ScrollRectScript : MonoBehaviour
{
    public TMP_Text textResult;
    public GameObject scrollerHour, scrollerMinutes;
    void Update()
    {
        string hours = scrollerHour.GetComponent<UIVerticalScroller>().result;
        string minutes = scrollerMinutes.GetComponent<UIVerticalScroller>().result;
        textResult.text =  hours +  " ч " + minutes+ " мин";
    }
}
