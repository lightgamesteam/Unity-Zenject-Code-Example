using System;
using System.Collections.Generic;
using System.Linq;
using Module.Core.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupProgress : MonoBehaviour
{
    [ShowOnly] public int ID = -1;

    public TextMeshProUGUI PopupLabel;
    public TextMeshProUGUI PopupProgressLabel;

    public Slider PopupSlider;
    public Button PopupLeftButton;
    public Button PopupRightButton;
    
    public PopupProgress Parent;

    public List<PopupProgress> Children = new List<PopupProgress>();

    public Action<int> IsDestroy;

    public PopupProgress Replica(int id)
    {
        PopupProgress progress = Children.SingleOrDefault(p => p.ID == id);
        
        if (progress != null)
        {
            return progress;
        }

        GameObject replica = Instantiate(gameObject, transform.parent);
        
        if (replica.HasComponent(out PopupProgress pp))
        {
            pp.ID = id;
            pp.gameObject.SetActive(true);
            pp.Parent = this;
        
            Children.Add(pp);
            
            return pp;
        }

        return null;
    }
    
    public bool Replica(int id, out PopupProgress popupProgress)
    {
        popupProgress = Replica(id);

        return popupProgress.gameObject.activeSelf;
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    private void OnEnable()
    {
        SetCanvasGroupInteractable(false);
    }

    private void OnDisable()
    {
        SetCanvasGroupInteractable(true);
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
   
    private void OnDestroy()
    {
        SetCanvasGroupInteractable(true);
        Parent.Children.Remove(this);
        IsDestroy?.Invoke(ID);
    }

    private void SetCanvasGroupInteractable(bool isInteractable)
    {
        gameObject.GetAllInScene<CanvasGroup>().ForEach(cg => { cg.interactable = isInteractable; });
    }
}
