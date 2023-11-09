using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Vimeo.Player;
using UnityEngine.Video;

[RequireComponent(typeof(Slider))]
public class SeekSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public VimeoPlayer vimeoPlayer;
    public VideoPlayer videoPlayer;

    private bool _dragging = false;
    private bool _isPlaying = false;
    private Slider _mySlider;

    private void Awake()
    {
        _mySlider = GetComponent<Slider>();
    }

    public void OnPointerDown(PointerEventData e)
    {
        _dragging = true;
        _isPlaying = videoPlayer.isPlaying;

        if (vimeoPlayer.enabled)
        {
            vimeoPlayer.Pause();
        }
        else
        {
            videoPlayer.Pause();
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (vimeoPlayer.enabled)
        {
            vimeoPlayer.Seek(_mySlider.normalizedValue);
            if (_isPlaying)
            {
                vimeoPlayer.Play();
            }
        }
        else
        {
            videoPlayer.frame = (long)_mySlider.value;
            if (_isPlaying)
            {
                videoPlayer.Play();
            }
        }

        _dragging = false;
    }

    void Update()
    {
        if (vimeoPlayer != null && vimeoPlayer.enabled) 
        {
            if (!_dragging && vimeoPlayer.GetProgress() < 1)
            {
                //_mySlider.normalizedValue = vimeoPlayer.GetProgress();
                _mySlider.normalizedValue = (float)videoPlayer.frame / videoPlayer.frameCount;
            }
        }
        else
        {
            if (!_dragging)
            {
               _mySlider.value = videoPlayer.frame;
            }
        }
    }
}
