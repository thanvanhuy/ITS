namespace VVA.ITS.WebApp.Interfaces
{
    public interface IPdfService
    {
        byte[] GeneratePdf(string htmlContent);
        byte[] GeneratePdfWeigt(string htmlContent);
    }
}
