using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrefabCalendarDay : MonoBehaviour
{
    public Transform currentObject;

    private TextMeshProUGUI currentNumber;
    private Image currentPanel;
    
    private void Start()
    {
       currentNumber = currentObject.Find("Number").GetComponent<TextMeshProUGUI>();
       currentPanel = currentObject.Find("Panel").GetComponent<Image>();
    }

    public void OnClick()
    {
        if (CalendarScene.selectedObj)
        {
            Deselect(CalendarScene.selectedObj.transform);
        }

        CalendarScene.selectedObj = currentObject;
        Select();

        if (currentNumber.text != "")
        {
            CalendarScene.currentDayNumber = Convert.ToInt32(currentNumber.text);
            CalendarScene.buttonShowSelected.SetActive(true);
        }
        else   CalendarScene.buttonShowSelected.SetActive(false);
    }

    private void Select()
    {
        if (currentNumber.text!="")
            currentPanel.color = new Color32(255, 96, 253, 255);
    }

    private void Deselect(Transform obj)
    {
        TextMeshProUGUI numberText = obj.Find("Number").GetComponent<TextMeshProUGUI>();
        
        if (CalendarScene.currentMonth == DateTime.Today.Month && CalendarScene.currentYear == DateTime.Today.Year && numberText.text == DateTime.Today.Day.ToString())
        {
            obj.transform.Find("Panel").GetComponent<Image>().color = new Color32(146, 96, 255, 255);
        }
        else obj.transform.Find("Panel").GetComponent<Image>().color = Color.white;
    }


}
