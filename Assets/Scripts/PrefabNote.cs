using UnityEngine;
using TMPro;

public class PrefabNote : MonoBehaviour
{
    public TextMeshProUGUI noteKey, noteValue;

    public void OnDeleteButton()
    {
        ViewModel.currentKey = noteKey.text;
        View.Delete(noteKey, "note");
    }

    public void OnEditButton()
    {
        ViewModel.currentKey = noteKey.text;
        View.UpdateNote(noteValue.text);
    }
}

