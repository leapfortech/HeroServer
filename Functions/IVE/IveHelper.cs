using System;
using System.Threading.Tasks;

namespace HeroServer
{
    public class IveHelper
    {
        public static DateTime ConvertStringToDateTime(String fecha)
        {
            try
            {
                return new DateTime(int.Parse(fecha.Substring(0, 4)), int.Parse(fecha.Substring(5, 2)), int.Parse(fecha.Substring(8, 2)));
            }
            catch
            {
                return new DateTime(1, 1, 1);
            }
        }

        public static string ConvertDateTimeToString(DateTime fecha)
        {
            string anio = "0000";
            string mes = "00";
            string dia = "00";
            try
            {
                anio = String.Format("{0:0000}", fecha.Year);
                mes = String.Format("{0:00}", fecha.Month);
                dia = String.Format("{0:00}", fecha.Day);
                return anio + mes + dia;
            }
            catch
            {
                return "00000000";
            }
        }

        public static String ConvertDepartmentCode(int departamentoCode)
        {
            return String.Format("{0:00}", departamentoCode);
        }

        public static String ConvertCityCode(int cityCode)
        {
            return String.Format("{0:0000}", cityCode);
        }

        public static async Task<String[]> ConvertStringArray(String data)
        {
            String[] input = data.Split("|");
            String[] result = new String[input.Length];

            for (int i = 0; i < input.Length; i++)
                result[i] = await GenValuesFunctions.GetStringById("K-Country", Convert.ToInt32(input[i]), "Alpha2");
            
            return result;
        }

        public static int ConvertBoolean(Boolean trigger)
        {
            return trigger ? 1 : 0;
        }

        public static string ConvertSN(int value)
        {
            String res;
            if (value == 1)
                res = "S";
            else
                res = "N";
            return res;
        }

        public static string ConvertIA(int value)
        {
            String res;
            if (value == 0)
                res = "I";
            else
                res = "A";
            return res;
        }

        public static string ConvertCR(int value)
        {
            String res;
            if (value == 1)
                res = "C";
            else
                res = "R";
            return res;
        }
    }
}
