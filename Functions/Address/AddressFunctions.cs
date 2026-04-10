using System;
using System.Collections.Generic;
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

        public static async Task<long> UpdateCity(AddressCity addressCity)
        {
            long id = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                Address address = await new AddressDB().GetById(addressCity.AddressId);

                address.CountryId = addressCity.CountryId;
                address.StateId = addressCity.StateId;
                address.CityId = addressCity.CityId;

                id = await RegisterByAppUser(addressCity.AppUserId, address);

                scope.Complete();
            }

            return id;
        }

        public static async Task<long> UpdateByAppUserId(long appUserId, Address address)
        {
            long id = -1;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                (long addressAppUserId, long addressId) = await new AddressAppUserDB().GetIdsByAppUserId(appUserId, 1);

                if ((addressAppUserId == -1) || (addressId == -1))
                    throw new Exception("AppUser not Found");

                await new AddressDB().UpdateStatus(addressId, 0);
                await new AddressAppUserDB().UpdateStatus(addressAppUserId, 0);

                address.Status = 1;
                id = await new AddressDB().Add(address);
                await new AddressAppUserDB().Add(new AddressAppUser(-1, appUserId, id, DateTime.Now, DateTime.Now, 1));

                scope.Complete();
            }

            return id;
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new AddressDB().UpdateStatus(id, status);
        }

        public static async Task DeleteByAppUserId(long appUserId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                List<long> addressIds = await new AddressAppUserDB().GetAddressIdsByAppUserId(appUserId);

                for (int i = 0; i < addressIds.Count; i++)
                    await new AddressDB().DeleteById(addressIds[i]);

                await new AddressAppUserDB().DeleteByAppUserId(appUserId);

                scope.Complete();
            }
        }
    }
}