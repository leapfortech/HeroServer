using System;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class AddressFunctions
    {
        // GET
        public static async Task<Address> GetById(int id)
        {
            return await new AddressDB().GetById(id);
        }

        public static async Task<Address> GetByAppUserId(int appUserId, int status)
        {
            int addressId = await new AddressAppUserDB().GetAddressIdByAppUserId(appUserId, status);

            return await new AddressDB().GetById(addressId);
        }

        public static async Task<AddressInfo> GetInfoByAppUserId(int appUserId, int status)
        {
            AddressAppUser addressAppUser = await new AddressAppUserDB().GetByAppUserId(appUserId, status);
            Address address = await new AddressDB().GetById(addressAppUser.AddressId);

            String containerName = $"user{appUserId:D08}";

            String[] householdBills = new String[addressAppUser.HouseholdBillCount];
            for (int i = 0; i < addressAppUser.HouseholdBillCount; i++)
            {
                byte[] byhouseholdBill = await StorageFunctions.ReadFile(containerName, $"hsbl{appUserId:D08}|{i:D02}", "jpg");
                householdBills[i] = byhouseholdBill == null ? null : Convert.ToBase64String(byhouseholdBill);
            }

            return new AddressInfo(address, householdBills);
        }

        public static async Task<Address> GetByProjectId(int projectId, int status = 1)
        {
            return await new AddressProjectDB().GetAddressByProjectId(projectId, status);
        }

        public static async Task<Address> GetByInvestmentIdentityId(int investmentIdentityId, int status)
        {
            int addressId = await new AddressInvestmentIdentityDB().GetIdByInvestmentIdentityId(investmentIdentityId, status);

            return await new AddressDB().GetById(addressId);
        }

        // ADD
        public static async Task<int> Add(Address address)
        {
            address.Status = 1;
            return await new AddressDB().Add(address);
        }

        public static async Task<int> Copy(int id, int status = -1)
        {
            Address address = await new AddressDB().GetById(id);
            if (status != -1)
                address.Status = status;
            return await new AddressDB().Add(address);
        }

        public static async Task<(int, int)> CopyByAppUserId(int appUserId, int status = -1)
        {
            int addressAppUserId = -1;
            int addressId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AddressAppUser addressAppUser = await new AddressAppUserDB().GetByAppUserId(appUserId);
                if (status != -1)
                    addressAppUser.Status = status;
                addressAppUserId = await new AddressAppUserDB().Add(addressAppUser);

                Address address = await new AddressDB().GetById(addressAppUser.AddressId);
                if (status != -1)
                    address.Status = status;
                addressId = await new AddressDB().Add(address);

                scope.Complete();
            }

            return (addressAppUserId, addressId);
        }

        public static async Task<int> RegisterByAppUser(int appUserId, AddressInfo addressInfo)
        {
            int addressId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AddressAppUser addressAppUser = await new AddressAppUserDB().GetByAppUserId(appUserId);
                if (addressAppUser != null)
                {
                    await new AddressAppUserDB().UpdateStatus(addressAppUser.Id, 1, 0);
                    await new AddressDB().UpdateStatus(addressAppUser.AddressId, 1, 0);
                }

                addressInfo.Address.Status = 1;
                addressId = await new AddressDB().Add(addressInfo.Address);

                addressAppUser = new AddressAppUser(-1, appUserId, addressId, 0, DateTime.Now, DateTime.Now, 1);
                addressAppUser.Id = await new AddressAppUserDB().Add(addressAppUser);

                // AppUser Status
                await AppUserFunctions.UpdateStatus(appUserId, 3);

                scope.Complete();
            }

            // Board & HouseholdBills

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int identityId = await IdentityFunctions.CopyByAppUserId(appUserId, 2);
                (int addressAppUserId, int boardAddressId) = await CopyByAppUserId(appUserId, 2);
                await OnboardingFunctions.Add(new Onboarding(appUserId, identityId, boardAddressId));

                if (addressInfo.HouseholdBills != null && addressInfo.HouseholdBills.Length > 0)
                {
                    int householdBillCount = 0;

                    String containerName = $"user{appUserId:D08}";
                    await StorageFunctions.CreateContainer(containerName);

                    for (int i = 0; i < addressInfo.HouseholdBills.Length; i++)
                    {
                        if (String.IsNullOrEmpty(addressInfo.HouseholdBills[i]))
                            continue;

                        await StorageFunctions.UpdateFile(containerName, $"hsbl{appUserId:D08}|{householdBillCount:D02}", "jpg", Convert.FromBase64String(addressInfo.HouseholdBills[i]));
                        householdBillCount++;
                    }

                    await new AddressAppUserDB().UpdateHouseholdCount(addressAppUserId, householdBillCount);
                }

                scope.Complete();
            }

            return addressId;
        }

        public static async Task<int> RegisterByProject(int projectId, Address address)
        {
            int addressId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                address.Status = 1;
                addressId = await new AddressDB().Add(address);

                AddressProject addressProject = await new AddressProjectDB().GetByProjectId(projectId);
                if (addressProject == null)
                {
                    addressProject = new AddressProject(-1, projectId, addressId, DateTime.Now, DateTime.Now, 1);
                    await new AddressProjectDB().Add(addressProject);
                }
                else
                {
                    await new AddressDB().UpdateStatus(addressProject.AddressId, 1, 0);
                    addressProject.AddressId = addressId;
                    await new AddressProjectDB().Update(addressProject);
                }

                scope.Complete();
            }

            return addressId;
        }

        public static async Task<int> RegisterByInvestmentIdentity(int investmentIdentityId, Address address)
        {
            int addressId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                address.Status = 1;
                addressId = await new AddressDB().Add(address);

                AddressInvestmentIdentity addressInvestmentIdentity = await new AddressInvestmentIdentityDB().GetByInvestmentIdentityId(investmentIdentityId);
                if (addressInvestmentIdentity == null)
                {
                    addressInvestmentIdentity = new AddressInvestmentIdentity(-1, investmentIdentityId, addressId, DateTime.Now, DateTime.Now, 1);
                    await new AddressInvestmentIdentityDB().Add(addressInvestmentIdentity);
                }
                else
                {
                    await new AddressDB().UpdateStatus(addressInvestmentIdentity.AddressId, 1, 0);
                    addressInvestmentIdentity.AddressId = addressId;
                    await new AddressInvestmentIdentityDB().Update(addressInvestmentIdentity);
                }

                scope.Complete();
            }

            return addressId;
        }

        // UPDATE
        public static async Task<int> Update(Address address)
        {
            int addressId = -1;

            if (await new AddressDB().Update(address))
                addressId = address.Id;

            return addressId;
        }

        public static async Task<int> UpdateByAppUser(int appUserId, Address address)
        {
            int addressId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AddressAppUser addressAppUser = await new AddressAppUserDB().GetByAppUserId(appUserId, address.Status);
                if (addressAppUser == null)
                    throw new Exception("AppUser not Found");

                if (address.Status == 1)
                {
                    await UpdateStatus(address.Id, 0);
                    addressId = await new AddressDB().Add(address);

                    addressAppUser.AddressId = addressId;
                    await new AddressAppUserDB().Update(addressAppUser);
                }
                else if (address.Status == 2)
                {
                    addressId = address.Id;

                    await new AddressDB().Update(address);
                }

                scope.Complete();
            }

            return addressId;
        }

        public static async Task<int> UpdateInfoByAppUser(int appUserId, AddressInfo addressInfo)
        {
            int addressId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AddressAppUser addressAppUser = await new AddressAppUserDB().GetByAppUserId(appUserId, addressInfo.Address.Status);
                if (addressAppUser == null)
                    throw new Exception("AppUser not Found");

                if (addressInfo.Address.Status == 1)
                {
                    await UpdateStatus(addressInfo.Address.Id, 0);
                    addressId = await Add(addressInfo.Address);

                    addressAppUser.AddressId = addressId;
                    await new AddressAppUserDB().Update(addressAppUser);
                }
                else if (addressInfo.Address.Status == 2)
                {
                    addressId = addressInfo.Address.Id;

                    await new AddressDB().Update(addressInfo.Address);
                }

                await UpdateHouseholdBills(appUserId, addressAppUser.Id, addressInfo.HouseholdBills);

                scope.Complete();
            }

            return addressId;
        }

        public static async Task UpdateHouseholdBills(int appUserId, String[] householdBills, int status)
        {
            int addressAppUserId = await new AddressAppUserDB().GetIdByAppUserId(appUserId, status);
            if (addressAppUserId == -1)
                throw new Exception("Address AppUser not Found");

            await UpdateHouseholdBills(appUserId, addressAppUserId, householdBills);
        }

        public static async Task UpdateHouseholdBills(int appUserId, int addressAppUserId, String[] householdBills)
        {
            int householdBillCount = 0;

            if (householdBills != null && householdBills.Length > 0)
            {
                String containerName = $"user{appUserId:D08}";
                String filename = $"hsbl{appUserId:D08}";

                await DeleteSoftHouseholdBills(containerName, filename);

                await StorageFunctions.CreateContainer(containerName);
                for (int i = 0; i < householdBills.Length; i++)
                {
                    if (String.IsNullOrEmpty(householdBills[i]))
                        continue;

                    await StorageFunctions.UpdateFile(containerName, $"{filename}|{householdBillCount:D02}", "jpg", Convert.FromBase64String(householdBills[i]));
                    householdBillCount++;
                }
            }

            await new AddressAppUserDB().UpdateHouseholdCount(addressAppUserId, householdBillCount);
        }

        private static async Task DeleteSoftHouseholdBills(String containerName, String filename)
        {
            for (int idx = 0; ; idx++)
                if (!await StorageFunctions.DeleteSoftFile(containerName, $"{filename}|{idx:D02}", "jpg"))
                    break;
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new AddressDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByAppUserId(int appUserId, int curStatus, int newStatus)
        {
            int addressId = await new AddressAppUserDB().GetAddressIdByAppUserId(appUserId, curStatus);

            await new AddressAppUserDB().UpdateStatusByAppUserId(appUserId, curStatus, newStatus);
            return await new AddressDB().UpdateStatus(addressId, curStatus, newStatus);
        }
    }
}