using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Loading : MonoBehaviour
{

    public GameObject loadingObj;
    private static RadialSlider thisSlider;

    private void Awake()
    {
        thisSlider = loadingObj.GetComponent<RadialSlider>();
    }


    public void OnClickDo()
    {
        thisSlider.LerpToTarget = true;
        thisSlider.Value = (float) 0.5;

    }
    
    public static void UpdateLoadingBar(double amount)
    {
        Debug.Log(amount + "d");
        
        if (thisSlider.Value == 1) return;
        thisSlider.LerpToTarget = true;
        thisSlider.Value += (float) amount;
    }
}
