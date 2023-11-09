using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserContentView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Image _imageView;
    [SerializeField] private TextMeshProUGUI _panelTitleText;
    [SerializeField] private VideoPlayerBase _videoPlayer;

    public Image GetImageView()
    {
        return _imageView;
    }
    
    public GameObject GetImageViewPanel()
    {
        return _imageView.gameObject;
    }
    
    public TextMeshProUGUI GetPanelTitleText()
    {
        return _panelTitleText;
    }
    
    public VideoPlayerBase GetVideoView()
    {
        return _videoPlayer;
    }
    
    public bool GetVideoViewPanel(out GameObject videoPlayerPanel)
    {
        if (_videoPlayer)
        {
            videoPlayerPanel = _videoPlayer.transform.parent.gameObject;
            return true;
        }
        
        videoPlayerPanel = null;
        return false;
    }
    
    public TextMeshProUGUI GetProgressText()
    {
        return _progressText;
    }
}
