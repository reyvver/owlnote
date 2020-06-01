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
        
        Transform colorObj = mainContent.Find("EventColour");
        Transform titleObj = mainContent.Find("EventTitle");
        Transform startTime = mainContent.Find("TimePanel/TimeStart");
        Transform endTime =  mainContent.Find("TimePanel/TimeEnd");

        Transform category = additionalContent.Find("Category/CategoryText");
        Transform description = additionalContent.Find("Description/TextScroll/DescriptionText");


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

    public static void CreateNote(GameObject prefab, Transform container, MNote currentNote)
    {
        GameObject newObj = Instantiate(prefab, container, false);
        Transform currentObject = newObj.transform;

        Transform key = currentObject.GetChild(0);
        Transform value = currentObject.Find("TextContainer/Text");

        key.GetComponent<TextMeshProUGUI>().text = currentNote.key;
        value.GetComponent<TextMeshProUGUI>().text = currentNote.value;
    }

    public static void CreateTodoObject(GameObject prefab, Transform container, string value, string index)
    {
        GameObject newObj = Instantiate(prefab, container, false);
        Transform currentObject = newObj.transform;
        
        Transform label = currentObject.GetChild(1);
        Transform key = currentObject.GetChild(3);

        label.GetComponent<TextMeshProUGUI>().text = value;
        key.GetComponent<TextMeshProUGUI>().text = index;
    }
    
    public static Transform CreateTodo(GameObject prefab, Transform container, string name)
    {
        GameObject newObj = Instantiate(prefab, container, false);
        Transform currentObject = newObj.transform;

        Transform label = currentObject.Find("PanelHeaderTodo/Title");
        label.GetComponent<TextMeshProUGUI>().text = name;

        return currentObject.GetChild(1);
    }
    
    public static void CreateTodoItem(GameObject prefab, Transform container, string value, bool check)
    {
        GameObject newObj = Instantiate(prefab, container, false);
        Transform currentObject = newObj.transform;
        Transform label = currentObject.GetChild(1);
        Toggle checkmark = currentObject.GetComponent<Toggle>();
        checkmark.isOn = check;
        label.GetComponent<TextMeshProUGUI>().text = value;
    }
    
    /*
    public static void CreateTodo(GameObject prefab, Transform container, string value, string index)
    {
        GameObject newObj = Instantiate(prefab, container, false);
        Transform currentObject = newObj.transform;
        
        Transform label = currentObject.GetChild(1);
        Transform key = currentObject.GetChild(2);

        label.GetComponent<TextMeshProUGUI>().text = value;
        key.GetComponent<TextMeshProUGUI>().text = index;
    }*/
}
