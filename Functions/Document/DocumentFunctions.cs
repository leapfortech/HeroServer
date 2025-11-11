using System;
using System.IO;
using System.Threading.Tasks;


namespace HeroServer
{
    public static class DocumentFunctions
    {
        public static async Task<String> CreateAgreement(String path, String eMail)
        {
            String agreementHtml = await File.ReadAllTextAsync(path + "\\resources\\CustomerAgreement.html");

            byte[] agreementPdf = BuildPdf(agreementHtml);

            //DEV
            //await MailHelper.SendMail(eMail, "Test", "Test PDF", customerAgreement, true);

            //PROD
            await MailHelper.SendMailAttach(eMail, "Test","Test PDF", agreementPdf, agreementHtml);

            return Convert.ToBase64String(agreementPdf);
        }

        
        private static byte[] BuildPdf(string html)
        {
            //SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            //SelectPdf.PdfDocument agreement = converter.ConvertHtmlString(html);
            //byte[] fileDocument = agreement.Save();
            //agreement.Close();
            //return fileDocument;
            return null;
        }
    }
}