using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PrefabEvent : MonoBehaviour
{
   private bool isShown = false;
   public GameObject deleteButton;
   public TextMeshProUGUI StartDay, Title;

   private void Start()
   {
       deleteButton.GetComponent<Button>().onClick.AddListener(OnDeleteButton);
   }

   public void ShowAdditionalInfo(GameObject AdditionalInfoPanel)
   {
       isShown = !isShown;
       RectTransform settingPosition = AdditionalInfoPanel.GetComponent<RectTransform>();
       
       if (isShown)
       {
           settingPosition.DOSizeDelta(new Vector2(785, 570), 0.25f);
       }
       else
       {
           settingPosition.DOSizeDelta(new Vector2(785, 0), 0.25f);
       }
   }

   private void OnDeleteButton()
   {
       View.Delete(Title,"event");
       ViewModel.currentKey = StartDay.text;
   }
   
}
