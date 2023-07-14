using PSXTransfer.DLL;
using PSXTransfer.WPF.MVVM.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PSXTransfer.WPF.MVVM.Services.Interfaces
{
    public interface IPSXTransferService
    {
        Task AddOrUpdateGame(string filePath);
        Task<Game?> GetGameByTitleID(string titleID);
        Task<IEnumerable<Game>> GetGameList();
        Task GetAllPKGs();

        Task SaveSetting(AppConfig? config);
        Task LoadSetting(AppConfig? config);

        Task Connect(IPAddress ip, int port, UpdataUrlLog urlLog);
    }
}
