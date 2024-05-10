using ApbdTest1.Models;
using Microsoft.Data.SqlClient;

namespace ApbdTest1.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;

    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> BookExists(int id)
    {
        var query = "SELECT 1 FROM BOOKS WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task AddNewBookWithEdition(NewBookWithEditionDTO newBookWithEdition)
    {
        var insert = @"INSERT INTO BOOKS VALUES (@Title);
                        SELECT @@IDENTITY AS PK;";
                        
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = insert;
        command.Parameters.AddWithValue("@Title", newBookWithEdition.Title);
        
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            var id = await command.ExecuteScalarAsync();

            command.Parameters.Clear();
            command.CommandText =
                "INSERT INTO BOOKS_EDITIONS VALUES (@FK_publishing_house, @FK_book, @edition_title, @release_date)";
            command.Parameters.AddWithValue("@FK_publishing_house", newBookWithEdition.FK_publishing_house);
            command.Parameters.AddWithValue("@FK_book", id);
            command.Parameters.AddWithValue("@edition_title", newBookWithEdition.edition_title);
            command.Parameters.AddWithValue("@release_date", newBookWithEdition.release_date);

            await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
    

    public async Task<BooksDTO> GetBook(int id)
    {
        var query =  @"SELECT 
							BOOKS.PK AS id,
							BOOKS.Title AS bookTitle,
							BOOKS_EDITIONS.edition_title AS editionTitle,
                            PUBLISHING_HOUSES.name AS publishingHouseName,
                            BOOKS_EDITIONS.release_date AS releaseDate
                            FROM BOOKS
                            JOIN BOOKS_EDITIONS ON BOOKS_EDITIONS.FK_BOOK = BOOKS.PK
                            JOIN PUBLISHING_HOUSES ON BOOKS_EDITIONS.FK_publishing_house = PUBLISHING_HOUSES.PK
                            WHERE BOOKS.PK = @PK";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("PK", id);
	    
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var bookId = reader.GetOrdinal("id");
        var bookTitle = reader.GetOrdinal("bookTitle");
        var editionTitle = reader.GetOrdinal("editionTitle");
        var publishingHouseName = reader.GetOrdinal("publishingHouseName");
        var release_date = reader.GetOrdinal("releaseDate");

        BooksDTO bookDTO = null;

        while (await reader.ReadAsync())
        {
            if (bookDTO is null)
            {
                bookDTO = new BooksDTO()
                {
                    PK = reader.GetInt32(bookId),
                    Title = reader.GetString(bookTitle),
                    editionTitle = reader.GetString(editionTitle),
                    publishingHouseName = reader.GetString(publishingHouseName),
                    releaseDate = reader.GetDateTime(release_date)
                };
            }
        }

        if (bookDTO is null) throw new Exception();

        return bookDTO;
    }
}