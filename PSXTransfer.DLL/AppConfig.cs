namespace PSXTransfer.DLL
{
    public class AppConfig
    {
        private static AppConfig? _instance;
        private static readonly object Lock = new();
        public static AppConfig Instance()
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    _instance = new AppConfig();
                }
            }

            return _instance;
        }

        public string? Rule { get; set; } = "*.pkg|*.pup";

        public string? Host { get; set; } = "";

        public bool IsAutoFindFile { get; set; } = true;

        public string? LocalFileDirectory { get; set; } = "D:\\PSXTransfer";

        public int BufferSize { get; set; } = 512;
    }
}
