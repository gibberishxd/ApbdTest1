using System.Runtime.InteropServices.JavaScript;

namespace ApbdTest1.Models;

public class BooksDTO
{
    public int PK { get; set; }
    public string Title { get; set; }
    public string editionTitle { get; set; }
    public string publishingHouseName { get; set; }
    public DateTime releaseDate { get; set; }
}

public class PublishingHousesDTO
{
    public int PK { get; set; }
    public string Name { get; set; }
    public string OwnerFirstNa { get; set; } = null!;
    public string OwnerLastNa { get; set; }
}

public class BooksEditionsDTO
{
    public int PK { get; set; }
    public PublishingHousesDTO PublishingHouse { get; set; }
    public BooksDTO Book { get; set; }
    public string EditionTitle { get; set; }
    public DateTime ReleaseDate { get; set; }
}

public class GenresDTO
{
    public int PK { get; set; }
    public string Name { get; set; }
}

public class AuthorsDTO
{
    public int PK { get; set; }
    public string FirstNa { get; set; }
    public string LastNa { get; set; }
}