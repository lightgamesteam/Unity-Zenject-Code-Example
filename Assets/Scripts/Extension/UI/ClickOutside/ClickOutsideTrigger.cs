using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOutsideTrigger : MonoBehaviour, IPointerDownHandler
{
    private readonly Dictionary<ClickOutside, Action> _action = new Dictionary<ClickOutside, Action>();
 
    public void AddTrigger (ClickOutside go, Action action)
    {
        gameObject.SetActive(true);
        
        if(!_action.ContainsKey(go))
            _action.Add(go, action);
    }
    
    public void RemoveTrigger (ClickOutside go)
    {
        if(_action.ContainsKey(go))
            _action.Remove(go);
        
        if (_action.Count < 1)
            gameObject.SetActive(false);
    }
   
    public void OnPointerDown(PointerEventData eventData)
    {
        InvokeTrigger();
    }

    public void InvokeTrigger()
    {
        if (_action.Count > 0)
        {
            List<ClickOutside> co = new List<ClickOutside>();
            
            _action?.Keys.ToList().ForEach(a =>
            {
                //if (!a.IsPined())
                //{
                    co.Add(a);
                    _action[a]?.Invoke();
                //}
            });
        
            co.ForEach(c => { _action.Remove(c); });
            co.Clear();
        }
    }
}