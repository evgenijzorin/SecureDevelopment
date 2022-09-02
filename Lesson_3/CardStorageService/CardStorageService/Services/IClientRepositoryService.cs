using CardStorageService.Data;

namespace CardStorageService.Services
{
    public interface IClientRepositoryService : IRepository<Client, int>
    {
        Client GetByCardId(string cardId);  
    }
}
