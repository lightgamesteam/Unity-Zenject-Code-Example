using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using Zenject;

public class CanvasPanel : MonoBehaviour
{
   [Header("Main View")] 
   [SerializeField] private CanvasGroup canvasPanel;
   [SerializeField] private RectTransform mainPanel;
   [SerializeField] private TextMeshProUGUI panelName;
   
   public Button leftButton;
   public Button rightButton;

   [Header("Content")] 
   public ContentPanel contentPanel;

   [Header("Animation")] 
   public bool playShowAnimationOnEnable = true;
   public float showDuration = 0.3f;
   public float hideDuration = 0.3f;

   [Space]
   public UnityEvent onEnable;
   public UnityEvent onShow;
   
   public UnityEvent onHide;
   public UnityEvent onDisable;

   private void OnEnable()
   {
      onEnable.Invoke();
      if(playShowAnimationOnEnable)
         ShowAnimation();
   }

   private void OnDisable()
   {
      onDisable.Invoke();
   }

   public void CloseCanvasPanel()
   {
      HideAnimation();
   }

   public void SetPanelName(string name)
   {
      panelName?.SetText(name);
   }
   
   public void SetLeftButtonName(string name)
   {
      leftButton.GetComponentInChildren<TextMeshProUGUI>()?.SetText(name);
   }
   
   public void SetRightButtonName(string name)
   {
      rightButton.GetComponentInChildren<TextMeshProUGUI>()?.SetText(name);
   }
   
   public void EnableLeftButton(bool isEnable)
   {
      leftButton.gameObject.SetActive(isEnable);
   }
   
   public void EnableRightButton(bool isEnable)
   {
      rightButton.gameObject.SetActive(isEnable);
   }

   public void ShowAnimation()
   {
      mainPanel.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
      mainPanel.DOScale(1f, showDuration).SetEase(Ease.OutSine);

      StartCoroutine(CanvasPanelAnimation(0, 1, showDuration));
   }
   
   public void HideAnimation()
   {
      mainPanel.transform.localScale = Vector3.one;
      mainPanel.DOScale(0.5f, hideDuration).SetEase(Ease.InSine);

      StartCoroutine(CanvasPanelAnimation(1, 0, hideDuration));
   }
   
   public void DisableCanvasPanel()
   {
      canvasPanel.gameObject?.SetActive(false);
   }
   
   IEnumerator CanvasPanelAnimation(float from, float to, float duration)
   {
      canvasPanel.alpha = from;

      if (from < to)
      {
         while (canvasPanel.alpha < to)
         {
            canvasPanel.alpha -= Time.deltaTime * (from - to) / duration;
            yield return new WaitForEndOfFrame();
         }
         
         onShow.Invoke();
      }
      else
      {
         while (canvasPanel.alpha > to)
         {
            canvasPanel.alpha += Time.deltaTime * (to - from) / duration;
            yield return new WaitForEndOfFrame();
         }

         DisableCanvasPanel();
         onHide.Invoke();
      }
   }
   
   public class Factory : PlaceholderFactory<CanvasPanel>
   {
   }
}
