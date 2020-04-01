using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class AuthorizationScene : MonoBehaviour
{
    public GameObject PanelEmail, HideOtherStaff;
    private bool _visibility;
    
    public void OnClickEmail()
    {
        _visibility = !_visibility;
        HideorShow(_visibility);
    }

    void Update()
    {
            if (Input.GetKeyDown(KeyCode.Escape) && _visibility)
            {
                _visibility = !_visibility;
                HideorShow(_visibility);
            }
    }

    private void HideorShow(bool var)
    {
        PanelEmail.SetActive(var);
        HideOtherStaff.SetActive(var);
    }
}
