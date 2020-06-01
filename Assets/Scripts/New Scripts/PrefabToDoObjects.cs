using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrefabToDoObjects : MonoBehaviour
{
    public TextMeshProUGUI key;
    
    public void OnDeleteButton()
    {
        Debug.Log("нажали");
        ViewModel.DeleteToDoObject(key.text);
    }

}
