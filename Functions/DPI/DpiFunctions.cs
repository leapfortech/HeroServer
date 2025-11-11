using System;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class DpiFunctions
    {
        public static async Task<short> CheckMRZ(Identity identity, String MRZ)
        {
            if (MRZ.Length != 90)                                                                                     return -1;

            short res = 0;

            if (MRZ[..5] != "IDGTM")                                                                                  res += 1 << 14;
            //if (!DocumentHelper.CheckMRZValidator(MRZ.Substring(5, 32) + MRZ.Substring(38, 6), MRZ[59]))              res += 1 << 15;

            int idx = MRZ.IndexOf("<<", 60);
            if (!RenapHelper.CompareName(identity.FirstName1, MRZ[(idx + 2)..].Replace('<', ' ').Trim()))          res += 1 << 0;
            if (!RenapHelper.CompareName(identity.LastName1, MRZ[60..idx].Replace('<', ' ')))                      res += 1 << 3;

            if (identity.DpiCui != String.Concat(MRZ.AsSpan(5, 9), MRZ.AsSpan(15, 4)))                                res += 1 << 6;

            if (identity.DpiDueDate.Date != new DateTime(Convert.ToInt32(MRZ.Substring(38, 2)) + 2000,
                                                         Convert.ToInt32(MRZ.Substring(40, 2)),
                                                         Convert.ToInt32(MRZ.Substring(42, 2))).Date)                 res += 1 << 8;

            int year = Convert.ToInt32(MRZ.Substring(30, 2)) + 2000;
            if (year > DateTime.Today.Year - 18) year -= 100;
            if (identity.BirthDate.Date != new DateTime(year, Convert.ToInt32(MRZ.Substring(32, 2)),
                                                             Convert.ToInt32(MRZ.Substring(34, 2))).Date)             res += 1 << 10;

            if ((identity.GenderId == 1 && MRZ[37] != 'F') || (identity.GenderId == 2 && MRZ[37] != 'M'))             res += 1 << 11;
            if ((identity.MaritalStatusId == 1 && MRZ[19] != 'C') ||
                (identity.MaritalStatusId == 2 && MRZ[19] != 'S' && MRZ[19] != 'U'))                                  res += 1 << 12;

            int nationalityId = await GenValuesFunctions.GetIdByCode("K-Country", MRZ.Substring(45, 3));
            if (!RenapHelper.CompareNationality(nationalityId, identity.NationalityIds))                           res += 1 << 13;

            return res;
        }

        public static short CheckMRZ(Dpi dpi)
        {
            if (dpi.MRZ.Length != 90)                                                                                 return -1;

            short res = 0;

            if (dpi.MRZ[..5] != "IDGTM")                                                                              res += 1 << 14;
            //if (!DocumentHelper.CheckMRZValidator(MRZ.Substring(5, 32) + MRZ.Substring(38, 6), MRZ[59]))              res += 1 << 15;

            int idx = dpi.MRZ.IndexOf("<<", 60);
            if (!RenapHelper.CompareName(dpi.FirstName1, dpi.MRZ[(idx + 2)..].Replace('<', ' ').Trim()))           res += 1 << 0;
            if (!RenapHelper.CompareName(dpi.LastName1, dpi.MRZ[60..idx].Replace('<', ' ')))                       res += 1 << 3;

            if (dpi.CUI != String.Concat(dpi.MRZ.AsSpan(5, 9), dpi.MRZ.AsSpan(15, 4)))                                res += 1 << 6;

            if (dpi.DueDate.Date != new DateTime(Convert.ToInt32(dpi.MRZ.Substring(38, 2)) + 2000,
                                                 Convert.ToInt32(dpi.MRZ.Substring(40, 2)),
                                                 Convert.ToInt32(dpi.MRZ.Substring(42, 2))).Date)                     res += 1 << 8;

            int year = Convert.ToInt32(dpi.MRZ.Substring(30, 2)) + 2000;
            if (year > DateTime.Today.Year - 18) year -= 100;
            if (dpi.BirthDate.Date != new DateTime(year, Convert.ToInt32(dpi.MRZ.Substring(32, 2)),
                                                         Convert.ToInt32(dpi.MRZ.Substring(34, 2))).Date)             res += 1 << 10;

            if ((dpi.GenderId == 1 && dpi.MRZ[37] != 'F') || (dpi.GenderId == 2 && dpi.MRZ[37] != 'M'))               res += 1 << 11;
            if ((dpi.MaritalStatusId == 1 && dpi.MRZ[19] != 'C') ||
                (dpi.MaritalStatusId == 2 && dpi.MRZ[19] != 'S' && dpi.MRZ[19] != 'U'))                               res += 1 << 12;

            if (dpi.MRZ.Substring(45, 3) != dpi.Nationality)                                                          res += 1 << 13;

            return res;
        }
    }
}