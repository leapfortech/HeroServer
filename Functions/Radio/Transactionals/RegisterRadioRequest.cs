using System.Collections.Generic;

namespace HeroServer
{
    public class RegisterRadioRequest : RegisterPostRequest
    {
        public Radio Radio { get; set; }
        public List<RadioType> RadioTypes { get; set; }
        public List<RadioLanguage> RadioLanguages { get; set; }

        public RegisterRadioRequest()
        {
        }

        public RegisterRadioRequest(Radio radio,
                                    List<RadioType> radioTypes,
                                    List<RadioLanguage> radioLanguages)
        {
            Radio = radio;
            RadioTypes = radioTypes ?? new List<RadioType>();
            RadioLanguages = radioLanguages ?? new List<RadioLanguage>();
        }
    }
}
