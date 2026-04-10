using aspnetPractice.Models;

namespace aspnetPractice.Services
{
    public interface IClientService
    {
        Task<Client> RegisterAsync(RegisterModel registerModel);

        Task<Client?> UpdateAsync(int id, Client updatedClient);

        Task<bool> DeleteAsync(int id);
    }
}
