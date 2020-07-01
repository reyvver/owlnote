using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrefabToDo : MonoBehaviour
{
   public TextMeshProUGUI itemName;
   public GameObject thisItem;
   private TextMeshProUGUI listName;
   private Toggle checkmark;

   private void Start()
   {
       listName = thisItem.transform.parent.parent.Find("PanelHeaderTodo/Title").GetComponent<TextMeshProUGUI>();
       checkmark = thisItem.GetComponent<Toggle>();
       checkmark.onValueChanged.AddListener(delegate { OnChangeState(checkmark);});
   }

   public void OnDeleteButton()
   { 
       ViewModel.currentKey = listName.text;
       ViewModel.DeleteListItem(itemName.text);
   }

   private void OnChangeState(Toggle change)
   {
       ViewModel.currentKey = listName.text;
      ViewModel.UpdateItemState(itemName.text, checkmark.isOn); 
   }
}
