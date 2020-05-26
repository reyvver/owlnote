using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class CategoryPrefabScript : MonoBehaviour
{
    private MainScreenScript script;
    public new TMP_Text name;
    public string color;
    
    public GameObject butDelete;
    public GameObject pick;

    // Start is called before the first frame update

    private void Start()
    {
        script = GameObject.Find("SceneManager").GetComponent<MainScreenScript>();

        if (name.text == "По умолчанию")
        {
            butDelete.SetActive(false);
        }
    }

    public void OnClickCategory(string operation)
    {
        Color32 col = pick.GetComponent<Image>().color;

        color = ColorUtility.ToHtmlStringRGB(col);
        script.chosenCategory = name.text;
        script.categoryColour = "#" + color;

        if (operation == "delete")
        {
            script.typeDelete = "category";
            script.ShowConfirmDelete();
        }
        
    }
}
