using ApbdTest1.Models;
using ApbdTest1.Repositories;

namespace ApbdTest1.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;

    public BooksController(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }

    [HttpGet("api/books/{id}/editions")]
    public async Task<IActionResult> GetBookEditions(int id)
    {
        if (!await _booksRepository.BookExists(id))
            return NotFound($"Book with given ID - {id} does not exist");

        var book = await _booksRepository.GetBook(id);

        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> AddBookWithEdition(NewBookWithEditionDTO newBookWithEdition)
    {
        await _booksRepository.AddNewBookWithEdition(newBookWithEdition);

        return Created(Request.Path.Value ?? "api/books", newBookWithEdition);
    }
    
}