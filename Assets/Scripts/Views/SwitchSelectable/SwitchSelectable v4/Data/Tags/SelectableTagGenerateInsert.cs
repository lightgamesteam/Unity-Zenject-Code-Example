using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class SelectableTagGenerateInsert :  SelectableTag
{
    public Selectable mySelectable;
    public bool isInsertMySelectable;

    public bool isInvokeOnOutLayer = false;
    
    public List<Selectable> layer = new List<Selectable>();
    
    [Space(20)]
    public OutLayerEvent onOutLayerEvent = new OutLayerEvent();

    private void OnEnable()
    {
        mySelectable = gameObject.GetComponent<Selectable>();
    }
}

[Serializable]
public class OutLayerEvent : UnityEvent
{
            
}