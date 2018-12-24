namespace Tenders.Integration.API.Interfaces
{
    public interface IToken
    {
        string Access_token { get; set; }
        string Token_type { get; set; }
        int Expires_in { get; set; }
        string UserName { get; set; }
        string Issued { get; set; }
        string Expires { get; set; }
    }
}
