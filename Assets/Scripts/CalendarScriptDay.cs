using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CalendarScriptDay : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnClick(GameObject obj)
    {
        CleanAll();
        TextMeshProUGUI txt = obj.gameObject.transform.Find("Number").GetComponent<TextMeshProUGUI>();
        if (txt.text != "")
            obj.gameObject.transform.Find("Panel").GetComponent<Image>().color = new Color32(255, 96, 253, 255);
    }

    private void CleanAll()
    {
        for (int i = 1; i <= 6; i++)
        {
            GameObject week = GameObject.Find("Week" + i);

            for (int j = 1 ; j <= 7; j++)
            {
                Transform date = week.transform.Find(j.ToString());
                TextMeshProUGUI numberText = date.Find("Number").GetComponent<TextMeshProUGUI>();
                Image panel = date.Find("Panel").GetComponent<Image>();
                if (DateTime.Today.Day.ToString() == numberText.text)
                {
                    panel.color = new Color32(146,96,255,255);
                    numberText.color = Color.white;
                }
                else
                {
                    panel.color = new Color32(255,255,255,255);
                    numberText.color = Color.black;
                }
            }
        }
}
}
