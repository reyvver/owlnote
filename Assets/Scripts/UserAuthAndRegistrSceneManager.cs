using System;
using UnityEngine;
using System.Collections.Generic;

public class UserAuthAndRegistrSceneManager : MonoBehaviour
{
    public GameObject objSceneManager, WelcomeScene, RegistrationScene,  PanelConfrim;
    private GameObject currentGameObject;
    private UserAuthAndRegistryScript scriptManagment;
    public List<GameObject> openedPanels;
    private bool _visibility, _support;

    private void Start()
    {
        WelcomeScene.transform.SetAsLastSibling();
        scriptManagment = objSceneManager.GetComponent<UserAuthAndRegistryScript>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            
            if (openedPanels.Count > 0)
            {
                if (openedPanels[openedPanels.Count - 1] == PanelConfrim)
                {
                    for (int i = 0; i < openedPanels.Count; i++)
                    {
                        openedPanels[i].SetActive(false);
                    }
          
                }
                else
                {
                    openedPanels[openedPanels.Count - 1].SetActive(false);
                    openedPanels.RemoveAt(openedPanels.Count - 1);
                }
            }
            else {

                try
                {
                    RegistrationScene.transform.SetAsFirstSibling();
                }
                catch (Exception e)
                {
                }
  
            }
            
            if (openedPanels.Count == 0)
            {
                scriptManagment.ClearInputs();
            }
        }

    
    }
    
    public void CloseAll()
    {
        for (int i = 0; i < openedPanels.Count; i++)
        {
            openedPanels[i].SetActive(false);
        }
        openedPanels.Clear();
        scriptManagment.ClearInputs();
    }
    public void ClosePanel(GameObject obj)
    {
        openedPanels.RemoveAt(openedPanels.Count-1);
        obj.SetActive(false);
    }
    public void ShowPanel(GameObject obj)
    {
        openedPanels.Add(obj);
        obj.SetActive(true);
    }
    public void ShowScene(GameObject obj)
    {
        obj.transform.SetAsLastSibling();
    }
}
