using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagmentCategories : MonoBehaviour
{
    public GameObject AddNewCategoryPanel, HideOtherStaff;
    public GameObject ButtonAdd;
    private bool HideOrShow = false;
    // Start is called before the first frame update
    void Start()
    {
       AddNewCategoryPanel.SetActive(HideOrShow);
       HideOtherStaff.SetActive(HideOrShow);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowAddMenu()
    {
        HideOrShow = !HideOrShow;
        AddNewCategoryPanel.SetActive(HideOrShow);
        HideOtherStaff.SetActive(HideOrShow);
        
        if (HideOrShow)
        {
            ButtonAdd.transform.Rotate(0,0,-45);
        }
        else
        {
            ButtonAdd.transform.Rotate(0,0,45);
        }
    }
}
