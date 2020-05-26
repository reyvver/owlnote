using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Prefabs : MonoBehaviour
{
    public static void CreateEvent(GameObject prefab, Transform container, MEvent currentEvent)
    {
        GameObject newObj = Instantiate(prefab, container, false);
        Transform currentObject = newObj.transform;
        
        Transform mainContent = currentObject.GetChild(0);
        Transform additionalContent = currentObject.GetChild(1);
        
        Transform colorObj = mainContent.GetChild(0);
        Transform titleObj = mainContent.GetChild(1);
        Transform startTime =  mainContent.GetChild(2).GetChild(0);
        Transform endTime =  mainContent.GetChild(2).GetChild(2);
        
        Transform category =  additionalContent.GetChild(0).GetChild(1);
        Transform description = additionalContent.GetChild(1).GetChild(1).GetChild(0);


        titleObj.GetComponent<TextMeshProUGUI>().text = currentEvent.Title;
        startTime.GetComponent<TextMeshProUGUI>().text = currentEvent.StartTime;
        endTime.GetComponent<TextMeshProUGUI>().text = currentEvent.EndTime;
        category.GetComponent<TextMeshProUGUI>().text = currentEvent.CategoryName;
        description.GetComponent<TextMeshProUGUI>().text = currentEvent.Description;
        
                
        if (ColorUtility.TryParseHtmlString(currentEvent.CategoryColour, out var newCol))
        {
            colorObj.GetComponent<Image>().color = newCol;
        }
    }
    
    public static void CreateCategory(GameObject prefab, Transform container, string name, string colour)
    {
        GameObject newObj = Instantiate(prefab, container, false);
        Transform currentObject = newObj.transform;

        Transform categoryColour = currentObject.GetChild(0);
        Transform categoryName = currentObject.GetChild(1);


        categoryName.GetComponent<TextMeshProUGUI>().text = name;

        if (ColorUtility.TryParseHtmlString(colour, out var newCol))
        {
            categoryColour.GetComponent<Image>().color = newCol;
        }
    }
}
