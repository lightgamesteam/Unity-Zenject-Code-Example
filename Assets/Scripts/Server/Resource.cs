using System.Collections.Generic;

namespace TDL.Server
{
    public class LanguageList
    {
        public string Version { get; set; }
        public List<LanguageResource> Languages { get; set; }
        public List<ApplicationResource> ApplicationResources { get; set; }
    }

    public class ApplicationResource
    {
        public string Code { get; set; }
        public List<MessageResource> MessageLocal { get; set; }
    }

    public class LanguageResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Culture { get; set; }
    }

    public class MessageResource
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Culture { get; set; }
    }

    public class ResourceResponse : ResponseBase
    {
        public LanguageList Resources { get; set; }
    }

    public class LinkNameResource
    {
        public string Name { get; set; }
        public string Culture { get; set; }
    }

    public class LinkResource
    {
        public List<LinkNameResource> LinkLocal { get; set; }
        public int SortOrder { get; set; }
        public string Uri { get; set; }
    }

    public class MetaDataResource
    {
        public List<LinkResource> Links { get; set; }
        public List<MetaData> MetaData { get; set; }
    }

    public class MetaData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ResourceRequest
    {
        public string Clientversion { get; set; }
        public string Platform { get; set; }
        public int? AssetId { get; set; }
    }

    public class Resource
    {
        public string Name { get; set; }
        public string Platform { get; set; }
        public int? AssetId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }

    public class AppResourceResponse : ResponseBase
    {
        public List<Resource> Resources { get; set; }
    }

    public class WarningARResponse : ResponseBase
    {
        public string LangCode  { get; set; }
        public string ImageUrl  { get; set; }
    }
}