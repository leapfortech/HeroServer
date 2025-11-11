using System;
using System.IO;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class HtmlHelper
    {
        static String ButtonMailHtml;
        static String SingleInputHtml;
        static String ConfirmResultHtml;

        public static async void Initialize(String wwwRootPath)
        {
            ButtonMailHtml = await File.ReadAllTextAsync(wwwRootPath + "\\resources\\ButtonMail.html");
            SingleInputHtml = await File.ReadAllTextAsync(wwwRootPath + "\\resources\\SingleInput.html");
            ConfirmResultHtml = await File.ReadAllTextAsync(wwwRootPath + "\\resources\\ConfirmResult.html");
        }

        public static String GetButtonMailHtml(String title, String text, String button, String link)
        {
            return ButtonMailHtml.Replace("BUTTON_MAIL_TITLE", title).Replace("BUTTON_MAIL_TEXT", text).Replace("BUTTON_MAIL_BUTTON", button).Replace("BUTTON_MAIL_LINK", link);
        }

        public static String GetSingleInputHtml(String title, String text, String type, String pattern, String placeholder, String button, String link)
        {
            return SingleInputHtml.Replace("SINGLE_INPUT_TITLE", title).Replace("SINGLE_INPUT_TEXT", text).Replace("SINGLE_INPUT_TYPE", type.ToLower()).Replace("SINGLE_INPUT_PATTERN", pattern).
                                   Replace("SINGLE_INPUT_HOLDER", placeholder).Replace("SINGLE_INPUT_BUTTON", button).Replace("SINGLE_INPUT_LINK", link);
        }

        public static String GetConfirmResultHtml(String title, String result, String color)
        {
            return ConfirmResultHtml.Replace("CONFIRM_RESULT_TITLE", title).Replace("CONFIRM_RESULT_TEXT", result).Replace("CONFIRM_RESULT_COLOR", color);
        }
    }
}
