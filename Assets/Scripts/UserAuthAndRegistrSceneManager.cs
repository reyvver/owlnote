﻿using System;
using UnityEngine;
using System.Collections.Generic;

public class UserAuthAndRegistrSceneManager : MonoBehaviour
{
    public GameObject  PanelConfrim;
    private GameObject currentGameObject;
    public List<GameObject> openedPanels;
    private bool _visibility, _support;

    private void Start()
    {
        GameObject.Find("WelcomeScene").transform.SetAsLastSibling();
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
                    openedPanels.Clear();
                }
                else
                {
                    openedPanels[openedPanels.Count - 1].SetActive(false);
                    openedPanels.RemoveAt(openedPanels.Count - 1);
                }
            }
            else
            {
                try
                {
                    GameObject.Find("RegistrationScene").transform.SetAsFirstSibling();
                }
                catch (Exception e)
                {
                }
  
            }
        }
  
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
