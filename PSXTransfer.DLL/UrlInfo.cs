﻿namespace PSXTransfer.DLL
{
    public class UrlInfo
    {
        public UrlInfo() { }

        public UrlInfo(string psnurl, string replacepath)
        {
            PsnUrl = psnurl;
            ReplacePath = replacepath;
        }

        public string? PsnUrl { get; set; }

        public string? ReplacePath { get; set; }

        public string? Host { get; set; }
    }
}
