using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupBubble : MonoBehaviour
{
    [SerializeField] private string key;
    public float timeout = 10f;
    public GameObject panel;
    public TextMeshProUGUI textMesh;
    public bool showOnce = true;
    private bool _isShowed = false;
    private bool _canShow = false;
    
    private void OnEnable()
    {
        StartShow();
    }

    private void StartShow()
    {
        if (!_canShow)
        {
            AsyncProcessorService.Instance.Wait(0.5f, StartShow);
            return;
        }

        if (showOnce)
        {
            if (!_isShowed)
            {
                _isShowed = true;
                Show();
            }
        }
        else
        {
            Show();
        }
    }

    public string GetKey()
    {
        return key;
    }

    public void SetText(string text)
    {
        _canShow = true;
        textMesh.text = text;
    }

    public void Show()
    {
        panel.SetActive(true);
        AsyncProcessorService.Instance.Wait(timeout, () => panel.SetActive(false));
    }
    
    
}
