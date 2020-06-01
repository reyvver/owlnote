using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrefabToDoList : MonoBehaviour
{
    public TextMeshProUGUI listName;

    public void OnAddButton()
    {
        
    }

    public void OnDeleteButton()
    {
        ViewModel.currentKey = listName.text;
        View.Delete(listName,"list");
    }
}
