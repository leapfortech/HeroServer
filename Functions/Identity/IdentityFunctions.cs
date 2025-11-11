using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class IdentityFunctions
    {
        // GET
        public static async Task<List<Identity>> GetAll(int status)
        {
            return await new IdentityDB().GetAll(status);
        }

        public static async Task<List<IdentityFull>> GetFullAll(int status)
        {
            return await new IdentityDB().GetFullAll(status);
        }

        public static async Task<Identity> GetById(int id)
        {
            return await new IdentityDB().GetById(id);
        }

        public static async Task<Identity> GetByAppUserId(int appUserId, int status)
        {
            return await new IdentityDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<IdentityFull> GetFullByAppUserId(int appUserId, int status)
        {
            return await new IdentityDB().GetFullByAppUserId(appUserId, status);
        }

        public static async Task<int> GetIdByAppUserId(int appUserId, int status = 1)
        {
            return await new IdentityDB().GetIdByAppUserId(appUserId, status);
        }

        public static async Task<IdentityInfo> GetInfoByAppUserId(int appUserId, int status)
        {
            Identity identity = await new IdentityDB().GetByAppUserId(appUserId, status);
            DpiPhoto dpiPhoto = await GetDpiPhoto(appUserId, true);

            return new IdentityInfo(identity, dpiPhoto);
        }

        public static async Task<DpiPhoto> GetDpiPhoto(int appUserId, bool loadPortrait = false)
        {
            String dpiFront = null, dpiBack = null, dpiPortrait = null;
            String containerName = $"user{appUserId:D08}";

            byte[] dpiImage = await StorageFunctions.ReadFile(containerName, $"idfr{appUserId:D08}", "jpg");
            if (dpiImage != null)
                dpiFront = Convert.ToBase64String(dpiImage);

            dpiImage = await StorageFunctions.ReadFile(containerName, $"idbk{appUserId:D08}", "jpg");
            if (dpiImage != null)
                dpiBack = Convert.ToBase64String(dpiImage);

            if (loadPortrait)
            {
                dpiImage = await StorageFunctions.ReadFile(containerName, $"idprt{appUserId:D08}", "jpg");
                if (dpiImage != null)
                    dpiPortrait = Convert.ToBase64String(dpiImage);
            }

            return new DpiPhoto(dpiFront, dpiBack, dpiPortrait);
        }

        public static async Task<IdentityBoardInfo> GetBoardInfoByAppUserId(int appUserId, int status)
        {
            Identity identity = await new IdentityDB().GetByAppUserId(appUserId, status);
            DpiBoardPhoto dpiBoardPhoto = await GetDpiBoardPhoto(appUserId, true);

            return new IdentityBoardInfo(identity, dpiBoardPhoto);
        }

        public static async Task<DpiBoardPhoto> GetDpiBoardPhoto(int appUserId, bool loadPortrait = false)
        {
            List<String> dpiFronts = [];
            List<String> dpiBacks = [];
            String dpiPortrait = null;
            byte[] dpiImage = null;
            String containerName = $"user{appUserId:D08}";

            int count = await new LeapUsageDB().GetCountByProductId(appUserId, 4, 1);
            if (count == 0)
            {
                dpiImage = await StorageFunctions.ReadFile(containerName, $"idfr{appUserId:D08}", "jpg");
                if (dpiImage != null)
                    dpiFronts.Add(Convert.ToBase64String(dpiImage));
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    dpiImage = await StorageFunctions.ReadFile(containerName, $"idfr{appUserId:D08}|{i:D02}", "jpg");
                    if (dpiImage != null)
                        dpiFronts.Add(Convert.ToBase64String(dpiImage));
                }
            }

            count = await new LeapUsageDB().GetCountByProductId(appUserId, 5, 1);
            if (count == 0)
            {
                dpiImage = await StorageFunctions.ReadFile(containerName, $"idbk{appUserId:D08}", "jpg");
                if (dpiImage != null)
                    dpiBacks.Add(Convert.ToBase64String(dpiImage));
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    dpiImage = await StorageFunctions.ReadFile(containerName, $"idbk{appUserId:D08}|{i:D02}", "jpg");
                    if (dpiImage != null)
                        dpiBacks.Add(Convert.ToBase64String(dpiImage));
                }
            }

            if (loadPortrait)
            {
                dpiImage = await StorageFunctions.ReadFile(containerName, $"idprt{appUserId:D08}", "jpg");
                if (dpiImage != null)
                    dpiPortrait = Convert.ToBase64String(dpiImage);
            }

            return new DpiBoardPhoto(dpiFronts, dpiBacks, dpiPortrait);
        }

        public static async Task<String> GetPortraitByAppUserId(int appUserId)
        {
            String portrait = null;
            
            byte[] portraitImg = await StorageFunctions.ReadFile($"user{appUserId:D08}", $"prt{appUserId:D08}", "jpg");

            if (portraitImg != null)
                portrait = Convert.ToBase64String(portraitImg);

            return portrait;
        }

        public static async Task<List<Identity>> GetAllByAppUserId(int appUserId, int status)
        {
            return await new IdentityDB().GetAllByAppUserId(appUserId, status);
        }

        public static async Task<String> GetSignature(int signatureId)
        {
            return (await new SignatureDB().GetById(signatureId)).Strokes;
        }

        // REGISTER
        public static async Task<int> Register(IdentityRegister identityRegister)
        {
            if (await new IdentityDB().GetByCui(identityRegister.IdentityInfo.Identity.DpiCui) != null)
                throw new Exception("El número de documento ya fue registrado. Revisa e intenta de nuevo.");

            int identityId;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new IdentityDB().UpdateStatusByAppUserId(identityRegister.IdentityInfo.Identity.AppUserId, 1, 0);

                identityRegister.IdentityInfo.Identity.Status = 1;
                identityId = await new IdentityDB().Add(identityRegister.IdentityInfo.Identity);
                identityRegister.IdentityInfo.Identity.Id = identityId;

                if (identityRegister.IdentityInfo.Identity.IsPep == 1)
                    await PepFunctions.RegisterByAppUser(identityRegister.IdentityInfo.Identity.AppUserId, identityRegister.Pep);

                if (identityRegister.IdentityInfo.Identity.HasPepIdentity == 1)
                    await PepIdentityFunctions.RegisterByAppUser(identityRegister.IdentityInfo.Identity.AppUserId, identityRegister.PepIdentityRequests);

                if (identityRegister.IdentityInfo.Identity.IsCpe == 1)
                    await CpeFunctions.RegisterByAppUser(identityRegister.IdentityInfo.Identity.AppUserId, identityRegister.Cpe);

                // DpiPhoto
                String appUserId = identityRegister.IdentityInfo.Identity.AppUserId.ToString("D08");
                String containerName = "user" + appUserId;
                await StorageFunctions.CreateContainer(containerName);

                if (identityRegister.IdentityInfo.DpiPhoto.DpiFront.Length > 0)
                {
                    await StorageFunctions.UpdateFile(containerName, "idfr" + appUserId, "jpg", Convert.FromBase64String(identityRegister.IdentityInfo.DpiPhoto.DpiFront));
                    await new LeapUsageDB().UpdateStatusByProductId(identityRegister.IdentityInfo.Identity.AppUserId, 4, 1, 0);
                }

                if (identityRegister.IdentityInfo.DpiPhoto.DpiBack.Length > 0)
                {
                    await StorageFunctions.UpdateFile(containerName, "idbk" + appUserId, "jpg", Convert.FromBase64String(identityRegister.IdentityInfo.DpiPhoto.DpiBack));
                    await new LeapUsageDB().UpdateStatusByProductId(identityRegister.IdentityInfo.Identity.AppUserId, 5, 1, 0);
                }

                if (identityRegister.IdentityInfo.DpiPhoto.DpiPortrait.Length > 0)
                    await StorageFunctions.UpdateFile(containerName, "idprt" + appUserId, "jpg", Convert.FromBase64String(identityRegister.IdentityInfo.DpiPhoto.DpiPortrait));

                if (identityRegister.Portrait.Length > 0)
                    await StorageFunctions.UpdateFile(containerName, "prt" + appUserId, "jpg", Convert.FromBase64String(identityRegister.Portrait));

                // AppUser Status
                await AppUserFunctions.UpdateStatus(identityRegister.IdentityInfo.Identity.AppUserId, 2);

                scope.Complete();
            }

            return identityRegister.IdentityInfo.Identity.Id;
        }

        public static async Task<int> RegisterInvestment(Identity identity)
        {
            int identityId;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                identity.Status = 1;
                identityId = await new IdentityDB().Add(identity);
                identity.Id = identityId;

                scope.Complete();
            }

            return identity.Id;
        }

        // ADD
        public static async Task<int> Add(Identity identity)
        {
            return await new IdentityDB().Add(identity);
        }

        public static async Task<int> Copy(int id, int status = -1)
        {
            Identity identity = await new IdentityDB().GetById(id);
            if (status != -1)
                identity.Status = status;
            return await new IdentityDB().Add(identity);
        }

        public static async Task<int> CopyByAppUserId(int appUserId, int status = -1)
        {
            Identity identity = await new IdentityDB().GetByAppUserId(appUserId);
            if (status != -1)
                identity.Status = status;
            return await new IdentityDB().Add(identity);
        }

        // UPDATE
        public static async Task<int> Update(Identity identity)
        {
            int identityId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (identity.Status == 1)
                {
                    await new IdentityDB().UpdateStatusByAppUserId(identity.AppUserId, 1, 0);

                    identityId = await new IdentityDB().Add(identity);
                }
                else if (identity.Status == 2)
                {
                    identityId = identity.Id;

                    await new IdentityDB().Update(identity);
                }

                scope.Complete();
            }

            return identityId;
        }

        public static async Task UpdateDpiFront(int appUserId, String dpiPhotos)
        {
            String[] photos = dpiPhotos.Split('|');
            await UpdateDpiFront(appUserId, photos[0], photos[1]);
        }

        public static async Task UpdateDpiFront(int appUserId, String dpiFront, String dpiPortrait)
        {
            if (String.IsNullOrEmpty(dpiFront) && String.IsNullOrEmpty(dpiPortrait))
                throw new ArgumentException("No Data to Update.");

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!String.IsNullOrEmpty(dpiFront))
                {
                    String id = appUserId.ToString("D08");
                    String containerName = "user" + id;

                    await StorageFunctions.UpdateCFile(containerName, "idfr" + id, "jpg", Convert.FromBase64String(dpiFront));
                }

                if (!String.IsNullOrEmpty(dpiFront))
                {
                    String id = appUserId.ToString("D08");
                    String containerName = "user" + id;

                    await StorageFunctions.UpdateCFile(containerName, "idprt" + id, "jpg", Convert.FromBase64String(dpiPortrait));
                }

                scope.Complete();
            }
        }

        public static async Task UpdateDpiBack(int appUserId, String dpiBack)
        {
            if (String.IsNullOrEmpty(dpiBack))
                throw new ArgumentException("No Data to Update.");

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                String id = appUserId.ToString("D08");
                String containerName = "user" + id;

                await StorageFunctions.UpdateCFile(containerName, "idbk" + id, "jpg", Convert.FromBase64String(dpiBack));

                scope.Complete();
            }
        }

        public static async Task UpdatePortrait(int appUserId, String portrait)
        {
            if (String.IsNullOrEmpty(portrait))
                throw new ArgumentException("No Data to Update.");

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                String id = appUserId.ToString("D08");
                String containerName = "user" + id;

                await StorageFunctions.UpdateCFile(containerName, "prt" + id, "jpg", Convert.FromBase64String(portrait));

                scope.Complete();
            }
        }

        public static async Task<int> UpdateInfo(IdentityInfo identityInfo, bool verifyDpiCui = true)
        {
            Identity identity = await new IdentityDB().GetByAppUserId(identityInfo.Identity.AppUserId);

            if (verifyDpiCui && identity.DpiCui != identityInfo.Identity.DpiCui)
                throw new Exception("El número de documento es diferente al del registro. Revisa e intenta de nuevo.");

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new IdentityDB().UpdateStatusByAppUserId(identityInfo.Identity.AppUserId, 1, 0);

                identityInfo.Identity.Status = 1;
                identityInfo.Identity.Id = await new IdentityDB().Add(identityInfo.Identity);

                if (identityInfo.DpiPhoto.DpiFront.Length > 0 || identityInfo.DpiPhoto.DpiBack.Length > 0 || identityInfo.DpiPhoto.DpiPortrait.Length > 0)
                {
                    String appUserId = identityInfo.Identity.AppUserId.ToString("D08");
                    String containerName = "user" + appUserId;
                    await StorageFunctions.CreateContainer(containerName);

                    if (identityInfo.DpiPhoto.DpiFront.Length > 0)
                        await StorageFunctions.UpdateFile(containerName, "idfr" + appUserId, "jpg", Convert.FromBase64String(identityInfo.DpiPhoto.DpiFront));

                    if (identityInfo.DpiPhoto.DpiBack.Length > 0)
                        await StorageFunctions.UpdateFile(containerName, "idbk" + appUserId, "jpg", Convert.FromBase64String(identityInfo.DpiPhoto.DpiBack));

                    if (identityInfo.DpiPhoto.DpiPortrait.Length > 0)
                        await StorageFunctions.UpdateFile(containerName, "idprt" + appUserId, "jpg", Convert.FromBase64String(identityInfo.DpiPhoto.DpiPortrait));
                }

                scope.Complete();
            }

            return identityInfo.Identity.Id;
        }

        public static async Task<bool> UpdateVersion(int appUserId, String version, int status = 1)
        {
            return await new IdentityDB().UpdateVersion(appUserId, version, status);
        }

        public static async Task<bool> UpdateSerie(int appUserId, String serie, int status = 1)
        {
            return await new IdentityDB().UpdateSerie(appUserId, serie, status);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new IdentityDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByAppUserId(int appUserId, int curStatus, int newStatus)
        {
            return await new IdentityDB().UpdateStatusByAppUserId(appUserId, curStatus, newStatus);
        }

        // DELETE

        public static async Task Delete(int id)
        {
            await new IdentityDB().DeleteById(id);
        }

        public static async Task DeleteByAppUserId(int appUserId)
        {
            await new IdentityDB().DeleteByAppUserId(appUserId);
        }
    }
}