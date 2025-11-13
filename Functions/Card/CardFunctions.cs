using System;
using System.Collections.Generic;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using CyberSource.Model;

namespace HeroServer
{
    public static class CardFunctions
    {
        static int CardsPerDay = 3;

        public static async void Initialize()
        {
            CardsPerDay = Convert.ToInt32(await new SystemParamDB().GetValue("CardsPerDay"));
        }

        public static async Task<Card> GetById(int cardId)
        {
            return await new CardDB().GetById(Convert.ToInt32(cardId));
        }

        public static async Task<Card> GetByAppUserId(int appUserId, int status = 1)
        {
            return await new CardDB().GetByAppUserId(appUserId, status);
        }

        public static async Task<int> GetIdByAppUserId(int appUserId, int status = 1)
        {
            return await new CardDB().GetIdByAppUserId(appUserId, status);
        }

        public static async Task<Card> Register(CardRegister cardRegister, ILogger logger)
        {
            // How Many ?
            logger?.LogWarning("Register Card Start");

            int cardCount = await new CardDB().GetTodayCount(cardRegister.AppUserId, cardRegister.UtcOffset);
            if (cardCount >= CardsPerDay)
                throw new Exception("You cannot register more than " + CardsPerDay + " cards per day.");

            logger?.LogWarning("Register Card Count");

            // PaymentInstrument Exists ?
            AppUser appUser = await AppUserFunctions.GetById(cardRegister.AppUserId);
            DateTime expirationDate = new DateTime(cardRegister.ExpirationYear, cardRegister.ExpirationMonth, 1).AddMonths(1).AddDays(-1);

            String csToken = null;

            List<Tmsv2customersEmbeddedDefaultPaymentInstrument> paymentInstruments = await CybersourceFunctions.GetPaymentInstrumentsByCustomerAndIdentifier(appUser.CSToken, cardRegister.InstrumentIdentifierId);
            foreach (Tmsv2customersEmbeddedDefaultPaymentInstrument paymentInstrument in paymentInstruments)
            {
                if (Convert.ToInt32(paymentInstrument.Card.ExpirationYear) != expirationDate.Year || Convert.ToInt32(paymentInstrument.Card.ExpirationMonth) != expirationDate.Month)
                    continue;

                logger?.LogWarning("Card Exists !");
                csToken = paymentInstrument.Id;

                Card existingCard = await new CardDB().GetByCSToken(paymentInstrument.Id);
                if (existingCard != null)
                {
                    if (existingCard.Status != 2)
                        throw new Exception("That card has already been registered.");

                    // Deleted card reactivated
                    existingCard.Holder = cardRegister.Holder;
                    existingCard.Status = 1;
                    await new CardDB().SetHolder(existingCard.Id, existingCard.Holder, existingCard.Status);

                    return existingCard;
                }
            }

            logger?.LogWarning("No Existing Card");

            // Card
            String csType = await GenValuesFunctions.GetCodeById("K-CardType", cardRegister.TypeId);

            Tmsv2customersEmbeddedDefaultPaymentInstrumentCard csCard = new Tmsv2customersEmbeddedDefaultPaymentInstrumentCard
            (
                cardRegister.ExpirationMonth.ToString("D02"),
                cardRegister.ExpirationYear.ToString(),
                csType
            );

            // BillTo
            logger?.LogWarning("GetBillTo");
            Tmsv2customersEmbeddedDefaultPaymentInstrumentBillTo csBillTo = await GetBillTo(appUser.Id);

            // Instrument Identifier
            Tmsv2customersEmbeddedDefaultPaymentInstrumentInstrumentIdentifier csInstrumentIdentifier = new Tmsv2customersEmbeddedDefaultPaymentInstrumentInstrumentIdentifier
            (
                cardRegister.InstrumentIdentifierId
            );

            // Payment Instrument
            if (csToken == null)
            {
                try
                {
                    logger?.LogWarning("RegisterPaymentInstrument");
                    csToken = (await CybersourceFunctions.RegisterPaymentInstrument(appUser.CSToken, csCard, csBillTo, csInstrumentIdentifier)).Id;
                }
                catch (Exception ex)
                {
                    throw new Exception("CBSC¶" + ex.Message);
                }
            }

            // CardDB
            Card oldCard = await GetByAppUserId(appUser.Id);

            logger?.LogWarning("Add new Card");
            Card card = new Card(-1, cardRegister.AppUserId, csToken, cardRegister.TypeId, cardRegister.Number, cardRegister.Digits, expirationDate, cardRegister.Holder, 1);
            card.Id = await new CardDB().Add(card);

            if (oldCard != null)
            {
                logger?.LogWarning("Delete Old Card");
                await CybersourceFunctions.DelPaymentInstrument(oldCard.CSToken);
                await new CardDB().SetStatus(oldCard.Id, 0);
            }

            Card deletedCard = await new CardDB().GetByAppUserId(appUser.Id, 2);
            if (deletedCard != null)
            {
                logger?.LogWarning("Delete Deleted Card");
                await CybersourceFunctions.DelPaymentInstrument(deletedCard.CSToken);
                await new CardDB().SetStatus(deletedCard.Id, 0);
            }

            return card;
        }

        public static async Task<Tmsv2customersEmbeddedDefaultPaymentInstrumentBillTo> GetBillTo(int appUserId)
        {
            // JAD : JOIN !!!
            Identity identity = await IdentityFunctions.GetByAppUserId(appUserId, 1);
            Address address = await AddressFunctions.GetByAppUserId(appUserId, 1);
            String eMail = await WebSysUserFunctions.GetEmailByAppUserId(appUserId);

            String country = await GenValuesFunctions.GetStringById("K-Country", address.CountryId, "Code");
            String state = await GenValuesFunctions.GetStringById("K-State",Convert.ToInt32(address.StateId),"Name");
            String city = await GenValuesFunctions.GetStringById("K-City", Convert.ToInt32(address.CityId), "Name");
            

            return new Tmsv2customersEmbeddedDefaultPaymentInstrumentBillTo
            (
                identity.FirstName1,
                identity.LastName1,
                null,
                address.Address1.Length > 60 ? address.Address1[..60] : address.Address1,
                null, //address.Address2 == null ? null : address.Address2.Length > 60 ? address.Address2[..60] : address.Address2,   // JAD : Never tested
                city,
                state,
                address.ZipCode,
                country,
                eMail
            );
        }

        public static async Task SetStatus(int id, int status)
        {
            Card card = await new CardDB().GetById(id);

            if (card.Status == 1 && status == 0)
                status = 2;

            if (!await new CardDB().SetStatus(Convert.ToInt32(id), Convert.ToInt32(status)))
                throw new Exception("Cannot Change Card Status");
        }

        // DELETE
        public static async Task DeleteByAppUserId(int appUserId)
        {
            int id = await GetIdByAppUserId(appUserId);

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new CardDB().DeleteByAppUserId(appUserId);

                scope.Complete();
            }
        }
    }
}