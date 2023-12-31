﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vimeo.Player;

namespace Vimeo.Controls.Player
{
    public class PlayButton : MonoBehaviour
    {
        public VimeoPlayer vimeoPlayer;
        public GameObject playButtonText;

        void Start()
        {
            vimeoPlayer.OnPlay += VideoPlay;
            vimeoPlayer.OnPause += VideoPause;
        }

        private void VideoPlay()
        {
            if (playButtonText != null)
            {
                Text txt = playButtonText.GetComponent<Text>();
                txt.text = "Pause";
            }
        }

        private void VideoPause()
        {
            if (playButtonText != null)
            {
                Text txt = playButtonText.GetComponent<Text>();
                txt.text = "Play";
            }
        }
    }
}