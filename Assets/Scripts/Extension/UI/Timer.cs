using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Timer : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private DateTime StartDateTime = DateTime.Now;

    private void Awake()
    {
        if (_text == null)
            _text = GetComponent<TextMeshProUGUI>();
    }

    public void OnEnable()
    {
        StartDateTime = DateTime.Now;
    }

    void Update()
    {
        var currentTime = DateTime.Now - StartDateTime;
        _text.text = $"{currentTime.Minutes:00}:{currentTime.Seconds:00}";
    }
}
