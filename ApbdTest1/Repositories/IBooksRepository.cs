using ApbdTest1.Models;

namespace ApbdTest1.Repositories;

public interface IBooksRepository
{
    Task<bool> BookExists(int id);
    Task<BooksDTO> GetBook(int id);
    Task AddNewBookWithEdition(NewBookWithEditionDTO newBookWithEdition);
}