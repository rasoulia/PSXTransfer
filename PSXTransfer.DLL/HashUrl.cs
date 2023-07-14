﻿namespace PSXTransfer.DLL
{
    public class HashUrl
    {
        private static HashUrl? _instance;
        private static readonly object Lock = new();

        public static HashUrl Instance()
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    _instance = new HashUrl();
                }
            }

            return _instance;
        }

        public string PsnLocalPath(string psnurl)
        {
            try
            {
                if (!AppConfig.Instance().IsAutoFindFile)
                {
                    return string.Empty;
                }

                //If you turn on automatic matching of local files, then automatically find
                string filename = GetUrlFileName(psnurl);
                if (!string.IsNullOrEmpty(filename))
                {
                    return AppConfig.Instance().LocalFileDirectory + "\\" + filename;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetUrlFileName(string psnurl)
        {
            List<string>? rules = AppConfig.Instance().Rule!.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (rules.Count <= 0)
            {
                return string.Empty;
            }

            rules = rules.Select(r => r.Replace("*", "")).ToList();
            string filename = string.Empty;
            rules.ForEach(r =>
            {
                if (psnurl.IndexOf(r) > 0)
                {
                    filename = psnurl[..(psnurl.IndexOf(r) + r.Length)];
                    filename = filename[(filename.LastIndexOf("/") + 1)..];
                }
            });

            return filename;
        }
    }
}
