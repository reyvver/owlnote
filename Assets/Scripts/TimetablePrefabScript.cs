using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class TimetablePrefabScript : MonoBehaviour
{
    private MainScreenScript script;
    private string eventTime, eventTitle;

    public TextMeshProUGUI title, startTime;

    void Start()
    {
        script = GameObject.Find("SceneManager").GetComponent<MainScreenScript>();
    }

    public void OnClickEvent(string operation)
    {
        script.chosenTime = startTime.text;
        script.chosenTitle = title.text;
        
        if (operation == "delete")
            {
            script.typeDelete = "event";
            script.ShowConfirmDelete();
        }
    }
}
