using Microsoft.EntityFrameworkCore;
using PSXTransfer.DLL;
using PSXTransfer.WPF.MVVM.DataContext;
using PSXTransfer.WPF.MVVM.Models;
using PSXTransfer.WPF.MVVM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PSXTransfer.WPF.MVVM.Services
{
    public class PSXTransferService : IPSXTransferService
    {
        private HttpListenerHelp? _listener;

        public async Task AddOrUpdateGame(string file)
        {
            using PSXTransferDataContext db = new();

            string? titleID = Path.GetFileName(file).Split('-')[1].Replace("_00", "");
            string? filePath = Path.GetDirectoryName(file)!;
            string? title = Directory.GetParent(file)?.Name;
            string console = titleID.Contains("CUSA") ? "PS4" : "PS5";

            Game? gameInfo = await GetGameByTitleID(titleID);

            if (gameInfo is null)
            {
                gameInfo = new()
                {
                    TitleID = titleID,
                    LocalPath = filePath,
                    Title = title,
                    Console = console,
                };

                await db.AddAsync(gameInfo);
            }

            else if (gameInfo.LocalPath == filePath)
            {
                return;
            }

            else
            {
                gameInfo.LocalPath = filePath;

                db.Update(gameInfo);
            }

            await db.SaveChangesAsync();
        }

        public async Task<Game?> GetGameByTitleID(string titleID)
        {
            using PSXTransferDataContext db = new();

            Game? gamePath = await db.Set<Game>().FirstOrDefaultAsync(g => g.TitleID == titleID);

            return await Task.FromResult(gamePath);
        }

        public async Task<IEnumerable<Game>> GetGameList()
        {
            using PSXTransferDataContext db = new();

            DbSet<Game> allGame = db.Set<Game>();

            return await Task.FromResult(allGame);
        }

        public async Task GetAllPKGs()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives.ToList())
            {
                if (drive.IsReady)
                {
                    string[] excludes = new[] { $"{drive.Name}System Volume Information", $"{drive.Name}$RECYCLE.BIN", $"{drive.Name}Recovery" };
                    IEnumerable<string> folders = Directory.EnumerateDirectories(drive.Name, "*.*", SearchOption.TopDirectoryOnly).Except(excludes);

                    foreach (string folder in folders)
                    {
                        try
                        {
                            ILookup<string, string> files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith(".pkg", StringComparison.OrdinalIgnoreCase)).ToLookup(f => Path.GetFileName(f).Split('-')[1].Replace("_00", ""), f => f);

                            foreach (IGrouping<string, string> file in files)
                            {
                                await AddOrUpdateGame(file.First());
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
        }

        public async Task SaveSetting(AppConfig? config)
        {
            if (!Directory.Exists("Settings"))
            {
                Directory.CreateDirectory("Settings");
            }

            string? fileName = "Settings\\Settings.json";
            JsonSerializerOptions? options = new() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(config, options);
            File.WriteAllText(fileName, jsonString);

            await Task.CompletedTask;
        }

        public async Task LoadSetting(AppConfig? config)
        {
            string? fileName = "Settings\\Settings.json";

            if (!File.Exists(fileName))
            {
                await SaveSetting(config);
            }

            string? json = File.ReadAllText(fileName);
            AppConfig? settings = JsonSerializer.Deserialize<AppConfig>(json);

            if (settings is null)
            {
                return;
            }

            AppConfig.Instance().Rule = settings.Rule;
            AppConfig.Instance().Host = settings.Host;
            AppConfig.Instance().IsAutoFindFile = settings.IsAutoFindFile;
            AppConfig.Instance().LocalFileDirectory = settings.LocalFileDirectory;
            AppConfig.Instance().BufferSize = settings.BufferSize;
        }

        public async Task Connect(IPAddress ip, int port, UpdataUrlLog urlLog)
        {
            if (_listener != null)
            {
                _listener.Dispose();
                _listener = null;
            }
            else
            {
                _listener = new(ip, port, urlLog);
                _listener.Start();
            }

            await Task.CompletedTask;
        }
    }
}
