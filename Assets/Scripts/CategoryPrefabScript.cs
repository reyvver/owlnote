using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditorInternal;
using UnityEngine.SceneManagement;


public class CategoryPrefabScript : MonoBehaviour
{
    private MainScreenCategories script;
    public TMP_Text name;
    // Start is called before the first frame update

    private void Start()
    {
        script = GameObject.Find("SceneManager").GetComponent<MainScreenCategories>();
    }

    public void OnClickCategory(string operation)
    {
        script.choosenCategory = name.text;

        if (operation == "delete")
            script._delete = true;
    }
}
