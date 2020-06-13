using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrefabPlanItem : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public GameObject thisItem;
    private TextMeshProUGUI listName;
    private Toggle checkmark;

    private void Start()
    {
        checkmark = thisItem.GetComponent<Toggle>();
    }

    public void OnDeleteButton()
    {
        CalendarScene.DeleteItemInPlan(itemName.text);
    }

    public void OnChangeState()
    {
        if(checkmark)
            CalendarScene.UpdateItemState(itemName.text,checkmark);
    }
}