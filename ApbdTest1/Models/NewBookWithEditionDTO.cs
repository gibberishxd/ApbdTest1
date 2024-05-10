namespace ApbdTest1.Models;

public class NewBookWithEditionDTO
{
    public string Title { get; set; } = string.Empty;
    public int FK_publishing_house { get; set; }
    public string edition_title { get; set; }
    public DateTime release_date { get; set; }

}