using System;
using BestHTTP;
using Newtonsoft.Json;
using TDL.Services;
using UnityEngine;
using Zenject;

namespace Commands
{
    public class CreateDiscoveryDocumentModelCommand : ICommand
    {
        [Inject] 
        private ServerService serverService;
        
        public void Execute()
        {
            serverService.GetDiscoveryDocument();
        }
    }
}