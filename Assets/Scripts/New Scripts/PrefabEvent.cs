using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
       Animator animator = AdditionalInfoPanel.GetComponent<Animator>();
       bool isShown = animator.GetBool("shown");
       animator.SetBool("shown", !isShown);
   }

   private void OnDeleteButton()
   {
       View.Delete(Title,"event");
       ViewModel.currentKey = StartDay.text;
   }
   
}
