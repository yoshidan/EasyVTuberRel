using System.Threading.Tasks;

namespace App.Main.Scripts.Interprocess.Interfaces
{
    public interface IMessageSender
    {
        void SendCommand(Message message);
        
        Task<string> SendQueryAsync(Message message);
    }
}