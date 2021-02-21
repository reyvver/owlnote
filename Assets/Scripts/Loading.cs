using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public GameObject loadingObj;
    
    private void Start()
    {
        StartCoroutine(CloseLoadingMenu());
    }

    private IEnumerator CloseLoadingMenu()
    {
        yield return new WaitForSeconds(2.5f);        
        loadingObj.GetComponent<Image>().CrossFadeAlpha(0,1.5f,false);
        yield return new WaitForSeconds(1.5f);
        loadingObj.SetActive(false);
    }
    
}
