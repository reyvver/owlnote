
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CategoryPrefabScript : MonoBehaviour
{
    private MainScreenCategories script;
    public TMP_Text name, count;

    // Start is called before the first frame update

    private void Start()
    {
        script = GameObject.Find("SceneManager").GetComponent<MainScreenCategories>();
    }

    public void OnClickCategory(string operation)
    {
        script.choosenCategory = name.text;
        script.categoryCount = count.text;

        if (operation == "delete")
            script._delete = true;
    }
}
