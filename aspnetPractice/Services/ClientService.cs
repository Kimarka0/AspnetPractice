using aspnetPractice.Data;
using aspnetPractice.Models;

namespace aspnetPractice.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _db;

        public ClientService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Client> RegisterAsync(RegisterModel registerModel)
        {
            Client client = new()
            {
                Name = registerModel.Name,
                Surname = registerModel.Surname,
                Age = registerModel.Age,
                Balance = registerModel.Balance,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),
            };

            await _db.Clients.AddAsync(client);
            await _db.SaveChangesAsync();

            return client;
        }
        public async Task<Client?> UpdateAsync(int id, Client updatedClient)
        {
            Client client = await _db.Clients.FindAsync(id);

            if (client == null) return null;

            client.Name = updatedClient.Name;
            client.Surname = updatedClient.Surname;
            client.Age = updatedClient.Age;
            client.Balance = updatedClient.Balance;

            await _db.SaveChangesAsync();

            return client;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Client client = await _db.Clients.FindAsync(id);

            if (client == null) return false;

            _db.Clients.Remove(client);

            await _db.SaveChangesAsync();

            return true;
        }

    }
}
