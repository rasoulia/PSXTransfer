using PSXTransfer.DLL;
using PSXTransfer.WPF.MVVM.Commands;
using PSXTransfer.WPF.MVVM.Models;
using PSXTransfer.WPF.MVVM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace PSXTransfer.WPF.MVVM.ViewModels
{
    public class PSXTransferViewModel : ViewModelBase
    {
        private readonly IPSXTransferService _psx;
        private void AddUrl(UrlInfo ui)
        {
            try
            {
                ui.PsnUrl = ui.PsnUrl?.Split('?')[0];
                if (PSXTools.RegexUrl(ui.PsnUrl))
                {
                    Uri? uri = new(ui.PsnUrl!);
                    string fileName = Path.GetFileName(uri.LocalPath);
                    string titleID = Path.GetFileName(uri.LocalPath).Split('-')[1].Replace("_00", "");
                    Game? game = _psx.GetGameByTitleID(titleID).Result;

                    LogViewModel log = new()
                    {
                        FilePath = File.Exists(Path.Combine(game?.LocalPath ?? "", fileName)) ? Path.Combine(game?.LocalPath ?? "", fileName) : "File Not Found",
                        Link = ui.PsnUrl,
                        TitleID = titleID,
                        Title = Directory.GetParent(Path.Combine(game?.LocalPath ?? "", fileName))?.Name
                    };

                    AppConfig.Instance().LocalFileDirectory = game?.LocalPath ?? "";

                    if (!_logList.Any(lg => lg.Link == log.Link))
                    {
                        _logList.Insert(0, log);
                        OnPropertyChanged(nameof(LogList));
                    }
                }
            }
            catch
            {

            }
            }
        }
        public PSXTransferViewModel(IPSXTransferService psx)
        {
            _psx = psx;
            IPAddress localIP = IPAddress.Parse("192.168.137.1");
            if (AddressList.Any(ip => ip.Equals(localIP)))
            {
                Address = localIP;
            }
            else
            {
                Address = AddressList[0];
            }

            Port = 8080;
            IsConnected = true;
        }

        public List<IPAddress> AddressList => PSXTools.GetHostIp();
        public Dictionary<int, string> BufferList => Enumerable.Range(2, 15).ToDictionary(s => (int)Math.Pow(2, s), s => (int)Math.Pow(2, s) % 1024 == 0 ? $"{(int)Math.Pow(2, s) / 1024} MB" : $"{(int)Math.Pow(2, s)} KB");
        private readonly List<LogViewModel> _logList = new();
        public ObservableCollection<LogViewModel> LogList => new(_logList);

        private IPAddress? _address;
        public IPAddress? Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        private int _port;
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        public string? Rule
        {
            get => AppConfig.Instance().Rule;
            set
            {
                AppConfig.Instance().Rule = value;
                OnPropertyChanged();
            }
        }
        public int BufferSize
        {
            get => AppConfig.Instance().BufferSize;
            set
            {
                AppConfig.Instance().BufferSize = value;
                OnPropertyChanged();
            }
        }

        public bool IsAutoFind
        {
            get => AppConfig.Instance().IsAutoFindFile;
            set
            {
                AppConfig.Instance().IsAutoFindFile = value;
                OnPropertyChanged();
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get
            {
                _psx.Connect(Address!, Port, AddUrl);
                return _isConnected;
            }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _updateDatabase;
        public ICommand UpdateDatabase
        {
            get
            {
                _updateDatabase ??= new RelayCommand(UpdateDatabaseCommand);
                return _updateDatabase;
            }
        }

        private async void UpdateDatabaseCommand(object? obj)
        {
            await _psx.GetAllPKGs();
            MessageBox.Show("Database Updated");
        }

        private ICommand? _clearLog;
        public ICommand? ClearLog
        {
            get
            {
                _clearLog ??= new RelayCommand(ClearLogCommand);
                return _clearLog;
            }
        }

        private void ClearLogCommand(object? obj)
        {
            _logList.Clear();
            OnPropertyChanged(nameof(LogList));
        }
    }
}