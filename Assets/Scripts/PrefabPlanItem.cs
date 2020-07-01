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
        checkmark.onValueChanged.AddListener(delegate { OnChangeState(checkmark);});
    }

    public void OnDeleteButton()
    {
        CalendarScene.DeleteItemInPlan(itemName.text);
    }

    private void OnChangeState(Toggle change)
    {
        CalendarScene.UpdateItemState(itemName.text,change.isOn);
    }
}