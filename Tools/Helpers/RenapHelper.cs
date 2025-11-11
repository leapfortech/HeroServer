using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class RenapHelper
    {
        public static bool CompareName(String name1, String name2)
        {
            if (String.IsNullOrEmpty(name1) || String.IsNullOrEmpty(name2))
                return true;

            return String.Compare(name1, name2, CultureInfo.InvariantCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;
        }

        public static bool CompareDate(DateTime? date1, DateTime date2)
        {
            return !date1.HasValue || date1.Value.Date == date2.Date;
        }

        public static bool CompareGender(int gender1, int gender2)
        {
            return gender1 < 0 || gender1 == gender2;
        }

        public static bool CompareMaritalStatus(int maritalStatus1, int maritalStatus2)
        {
            return maritalStatus1 < 0 || maritalStatus1 == maritalStatus2;
        }

        public static bool CompareNationality(int nat, String natIds)
        {
            if (nat < 0)
                return true;

            String[] nats = natIds.Split('|');
            for (int i = 0; i < nats.Length; i++)
                if (Convert.ToInt32(nats[i]) == nat)
                    return true;
            return false;
        }

        // MRZ

        private static readonly int[] Weights = [ 7, 3, 1 ];

        public static SortedDictionary<char, int> GetMappedDictionnary()
        {
            string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            SortedDictionary<char, int> mappedValues = [];
            char[] charArr = charset.ToCharArray();
            for (int i = 0; i < charArr.Length; ++i)
                mappedValues[charArr[i]] = i;
            mappedValues['<'] = 0;
            return mappedValues;
        }
        
        public static int ComputeMRZWeight(String line)
        {
            int sum = 0;
            SortedDictionary<char, int> mappedValues = GetMappedDictionnary();
            for (int i = 0, j = 0; i < line.Length; ++i, ++j)
                sum += mappedValues[line[i]] * Weights[j % 3];
            return sum;
        }
        public static bool CheckMRZValidator(String line, char validator)
        {
            return (ComputeMRZWeight(line) % 10) == (validator - '0');
        }

        // RENAP
        public static DateTime ConvertToDate(String date)
        {
            return new DateTime(Convert.ToInt32(date[6..10]), Convert.ToInt32(date[3..5]), Convert.ToInt32(date[..2]));
        }

        public static async Task<int> ConvertGenderToId(String gender)
        {
            return await GenValuesFunctions.GetIdByField("K-Gender", "Name", BeautifyName(gender));
        }

        public static async Task<int> ConvertMaritalStatusToId(String maritalStatus)
        {
            return await GenValuesFunctions.GetIdByField("K-MaritalStatus", "Name", BeautifyName(maritalStatus));
        }

        public static async Task<int> ConvertCountryToId(String country)
        {
            return await GenValuesFunctions.GetIdByField("K-Country", "Name", BeautifyName(country));
        }

        public static async Task<int> ConvertStateToId(String state)
        {
            return await GenValuesFunctions.GetIdByField("K-State", "Name", BeautifyName(state));
        }

        public static async Task<int> ConvertCityToId(String city)
        {
            return await GenValuesFunctions.GetIdByField("K-City", "Name", BeautifyName(city));
        }

        public static bool CompareCountry(int country1, int country2)
        {
            return country1 < 0 || country1 == country2;
        }

        public static bool CompareState(int state1, int state2)
        {
            return state1 < 0 || state1 == state2;
        }

        public static bool CompareCity(int city1, int city2)
        {
            return city1 < 0 || city1 == city2;
        }

        private static String BeautifyName(String name)
        {
            if (name == null)
                return null;

            StringBuilder result = new StringBuilder();
            bool upper = true;
            for (int i = 0; i < name.Length; i++)
            {
                if (!char.IsLetter(name[i]))
                {
                    result.Append(name[i]);
                    upper = true;
                    continue;
                }

                result.Append(upper ? char.ToUpper(name[i]) : char.ToLower(name[i]));
                upper = false;
            }

            return result.ToString();
        }
    }
}
