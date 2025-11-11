using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class RenapFunctions
    {
        public static async Task<RenapIdentity> GetIdentityByCui(String cui)
        {
            Dictionary<String, String> query = new Dictionary<String, String> { { "cui", cui } };
            return await LeapServerHelper.Get<RenapIdentity>("renap/Identity", query);
        }

        public static async Task<RenapIdentityInfo> GetIdentityInfoByCui(String cui)
        {
            Dictionary<String, String> query = new Dictionary<String, String> { { "cui", cui } };
            return await LeapServerHelper.Get<RenapIdentityInfo>("renap/IdentityInfo", query);
        }

        public static async Task<RenapIdentityInfo> GetIdentityInfo(int appUserId)
        {
            RenapIdentity renapIdentity = await RenapIdentityFunctions.GetByAppUserId(appUserId);

            String containerName;
            if (renapIdentity != null)
            {
                containerName = "user" + appUserId.ToString("D08");
                byte[] byFace = await StorageFunctions.ReadFile(containerName, "rpfc" + appUserId.ToString("D08"), "jpg");
                if (byFace != null)
                    return new RenapIdentityInfo(renapIdentity, byFace == null ? null : Convert.ToBase64String(byFace));
            }


            String cui = await new IdentityDB().GetCuiByAppUserId(appUserId);

            RenapIdentityInfo renapIdentityInfo;

            try
            {
                renapIdentityInfo = await GetIdentityInfoByCui(cui);
            }
            catch (Exception ex)
            {
                throw new Exception("ERR : " + ex.Message);
            }
            if (renapIdentityInfo == null)
                return null;

            if (renapIdentity != null)
                await RenapIdentityFunctions.UpdateStatus(renapIdentity.Id, 2);

            // Names
            renapIdentityInfo.RenapIdentity.FirstName1 = BeautifyName(renapIdentityInfo.RenapIdentity.FirstName1);
            renapIdentityInfo.RenapIdentity.FirstName2 = BeautifyName(renapIdentityInfo.RenapIdentity.FirstName2);
            renapIdentityInfo.RenapIdentity.FirstName3 = BeautifyName(renapIdentityInfo.RenapIdentity.FirstName3);
            renapIdentityInfo.RenapIdentity.LastName1 = BeautifyName(renapIdentityInfo.RenapIdentity.LastName1);
            renapIdentityInfo.RenapIdentity.LastName2 = BeautifyName(renapIdentityInfo.RenapIdentity.LastName2);
            renapIdentityInfo.RenapIdentity.LastNameMarried = BeautifyName(renapIdentityInfo.RenapIdentity.LastNameMarried);

            // Birth
            renapIdentityInfo.RenapIdentity.Nationality = BeautifyName(renapIdentityInfo.RenapIdentity.Nationality);
            renapIdentityInfo.RenapIdentity.BirthCountry = BeautifyName(renapIdentityInfo.RenapIdentity.BirthCountry);
            renapIdentityInfo.RenapIdentity.BirthState = BeautifyName(renapIdentityInfo.RenapIdentity.BirthState);
            renapIdentityInfo.RenapIdentity.BirthCity = BeautifyName(renapIdentityInfo.RenapIdentity.BirthCity);

            // Others
            renapIdentityInfo.RenapIdentity.CedulaResidence = BeautifyName(renapIdentityInfo.RenapIdentity.CedulaResidence);
            renapIdentityInfo.RenapIdentity.Occupation = BeautifyName(renapIdentityInfo.RenapIdentity.Occupation);

            renapIdentityInfo.RenapIdentity.AppUserId = appUserId;
            renapIdentityInfo.RenapIdentity.Id = await RenapIdentityFunctions.Add(renapIdentityInfo.RenapIdentity);

            containerName = "user" + appUserId.ToString("D08");
            await StorageFunctions.CreateFile(containerName, "rpfc" + appUserId.ToString("D08"), "jpg", Convert.FromBase64String(renapIdentityInfo.Face));

            return renapIdentityInfo;
        }

        private static String BeautifyName(String name)
        {
            if (String.IsNullOrEmpty(name))
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

        public static async Task<LeapResponse> CompareNames(RenapRequest request)
        {
            return await LeapServerHelper.Post<RenapRequest, LeapResponse>("renap/CompareNames", request);
        }

        public static async Task<LeapResponse> CompareIdentity(RenapRequest request)
        {
            return await LeapServerHelper.Post<RenapRequest, LeapResponse>("renap/CompareIdentity", request);
        }

        public static async Task<LeapResponse> CompareFace(RenapRequest request)
        {
            return await LeapServerHelper.Post<RenapRequest, LeapResponse>("renap/CompareFace", request);
        }
    }
}