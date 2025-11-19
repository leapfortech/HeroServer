using System;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class AddressFunctions
    {
        // GET
        public static async Task<Address> GetById(long id)
        {
            return await new AddressDB().GetById(id);
        }

        public static async Task<Address> GetByAppUserId(long appUserId, int status)
        {
            long addressId = await new AddressAppUserDB().GetAddressIdByAppUserId(appUserId, status);

            return await new AddressDB().GetById(addressId);
        }

        // ADD
        public static async Task<long> Add(Address address)
        {
            address.Status = 1;
            return await new AddressDB().Add(address);
        }

        public static async Task<long> Copy(long id, int status = -1)
        {
            Address address = await new AddressDB().GetById(id);
            if (status != -1)
                address.Status = status;
            return await new AddressDB().Add(address);
        }

        public static async Task<(long, long)> CopyByAppUserId(long appUserId, int status = -1)
        {
            long addressAppUserId = -1;
            long addressId = -1;

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

        public static async Task<long> RegisterByAppUser(long appUserId, Address address)
        {
            long addressId = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AddressAppUser addressAppUser = await new AddressAppUserDB().GetByAppUserId(appUserId);
                if (addressAppUser != null)
                {
                    await new AddressAppUserDB().UpdateStatus(addressAppUser.Id, 1, 0);
                    await new AddressDB().UpdateStatus(addressAppUser.AddressId, 1, 0);
                }

                address.Status = 1;
                addressId = await new AddressDB().Add(address);

                addressAppUser = new AddressAppUser(-1, appUserId, addressId, DateTime.Now, DateTime.Now, 1);
                addressAppUser.Id = await new AddressAppUserDB().Add(addressAppUser);

                // AppUser Status
                await AppUserFunctions.UpdateStatus(appUserId, 3);

                scope.Complete();
            }

            return addressId;
        }

        // UPDATE
        public static async Task<long> Update(Address address)
        {
            long addressId = -1;

            if (await new AddressDB().Update(address))
                addressId = address.Id;

            return addressId;
        }

        public static async Task<long> UpdateByAppUser(long appUserId, Address address)
        {
            long addressId = -1;

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

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new AddressDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByAppUserId(long appUserId, int curStatus, int newStatus)
        {
            long addressId = await new AddressAppUserDB().GetAddressIdByAppUserId(appUserId, curStatus);

            await new AddressAppUserDB().UpdateStatusByAppUserId(appUserId, curStatus, newStatus);
            return await new AddressDB().UpdateStatus(addressId, curStatus, newStatus);
        }
    }
}