using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrefabCategory : MonoBehaviour
{
    public GameObject deleteButton;
    public GameObject selectCategory;
    public GameObject pick;
    public TextMeshProUGUI categoryName;

    private void Start()
    {
        deleteButton.GetComponent<Button>().onClick.AddListener(OnDeleteButton);
        selectCategory.GetComponent<Button>().onClick.AddListener(OnSelectCategory);
        
        if (categoryName.text == "По умолчанию")
            deleteButton.SetActive(false);
    }
    
    private void OnDeleteButton()
    { 
        View.Delete(categoryName,"category","");
    }

    private void OnSelectCategory()
    {
        Color32 col = pick.GetComponent<Image>().color;
        string color = ColorUtility.ToHtmlStringRGB(col);
        
        ViewModel.selectedCategoryColour = "#" + color;
        ViewModel.selectedCategoryName = categoryName.text;
    }

}