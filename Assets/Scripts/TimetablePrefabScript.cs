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
    public GameObject AdditionalInfo;

    private bool shown = false;
    void Start()
    {
       script = GameObject.Find("SceneManager").GetComponent<MainScreenScript>();
    }

    public void ShowFullInfo()
    {
        shown = !shown;
        if(shown)
        AdditionalInfo.SetActive(true);
        else  AdditionalInfo.SetActive(false);
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
