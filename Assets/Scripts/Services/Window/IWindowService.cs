using System;
using TDL.Constants;
using UnityEngine;

namespace TDL.Services
{
    public interface IWindowService
    {
        void AddWindow(Enum key, GameObject window);
        void ShowWindow(Enum key);
        bool IsCurrentWindow(WindowConstants windowConstants);
    }   
}