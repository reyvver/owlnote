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
   }

   public void OnDeleteButton()
   { 
       ViewModel.currentKey = listName.text;
       ViewModel.DeleteListItem(itemName.text);
   }

   public void OnChangeState()
   {
       if (checkmark == true)
        ViewModel.UpdateItemState(itemName.text);
   }
}
