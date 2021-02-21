using UnityEngine;
using TMPro;

public class PrefabToDoObjects : MonoBehaviour
{
    public TextMeshProUGUI key;
    
    public void OnDeleteButton()
    {
        ViewModel.DeleteToDoObject(key.text);
    }

}
