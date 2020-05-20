using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberPlatePrefabScript : MonoBehaviour
{
    private EventsManagment script;
    private MainScreenNotes notes;
    public TextMeshProUGUI number;
    private Transform content;

    private string text;
    // Start is called before the first frame update
    void Start()
    {
        script = GameObject.Find("SceneManager").GetComponent<EventsManagment>();
        notes = GameObject.Find("SceneManager").GetComponent<MainScreenNotes>();
        content = GameObject.Find("NumberPanel").transform;
    }

    public void OnClickPlate()
    {
        DateTime today = DateTime.Today;
        
        int day = Convert.ToInt32(number.text);
        int month = today.Month;
        int year = today.Year;
        
        if (today > new DateTime(year, month, day))
        {
           // Debug.Log("следующий месяц");
            month++;
            if (month == 13)
            {
                month = 1;
                year++;
            }
        }
        
        DateTime newDate = new DateTime(year,month,day);

        script.dateToday = newDate;
        text = newDate.Day.ToString();

        script.UpdateContent();
        notes.UpdateContent();
        CleanAll();
    }
    
    private void CleanAll()
    {
        for (int i = 0; i <= 9; i++)
        {
            Transform plate = content.Find("NumberPlate" + i);
            
            TextMeshProUGUI numberText = plate.Find("Number").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI textText = plate.Find("Text").GetComponent<TextMeshProUGUI>();
            Image panel = plate.Find("BackPanel").GetComponent<Image>();
            
            if (numberText.text == text)
                {
                    panel.color = new Color32(146,96,255,255);
                    numberText.color = Color.white;
                    textText.color = Color.white;
                }
                else
                {
                    panel.color = new Color32(245,245,245,255);
                    numberText.color =  new Color32(108,108,108,255); 
                    textText.color = new Color32(108,108,108,255); 
                }
            
        }
    }
}
