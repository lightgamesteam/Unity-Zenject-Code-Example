using System;
using System.Collections.Generic;
using TDL.Server;

namespace TDL.Models
{
    public class MetaDataModel
    {
        public Action OnTermLinksUpdate;
        
        public LinkResource Link;
        public Dictionary<string, string> LinkLocal { get; set; }
        public string LinkTerm  { get; set; }
        public int MaxFavorites;
        public int MaxRecent;
    }
}