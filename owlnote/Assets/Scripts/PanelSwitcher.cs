using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Button       nextButton, previousButton;

    public uint currentPanelIndex = 0;

    private void OnEnable()
    {
        nextButton.onClick.AddListener(Next);
        previousButton.onClick.AddListener(Previous);
    }
    
    private void Previous()
    {
        if (currentPanelIndex > 0)
            SwitchTo(currentPanelIndex - 1);
    }
    
    private void Next()
    {
        if (currentPanelIndex < panels.Length - 1)
            SwitchTo(currentPanelIndex + 1);
    }


    private void SwitchTo(uint panelIndex)
    {
        if (panelIndex >= panels.Length)
        {
            Debug.LogError("Panel index is out of range");
            return;
        }

        panels[currentPanelIndex].SetActive(false);
        currentPanelIndex = panelIndex;
        panels[currentPanelIndex].SetActive(true);
    }
}