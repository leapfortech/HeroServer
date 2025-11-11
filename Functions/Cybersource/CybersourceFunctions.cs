using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using CyberSource.Api;
using CyberSource.Model;
using CyberSource.Client;
using RestSharp;

namespace HeroServer
{
    public static class CybersourceFunctions
    {
        static Dictionary<string, string> configDictionary;
        static String profileId = null;
        static String x509 = null;
        static String tmxOrgId = null;
        static String tmxServer = null;

        // Params
        public static async void Initialize()
        {
            String merchantID = await new SystemParamDB().GetValue("MerchantId");
            String merchantKeyId = await new SystemParamDB().GetValue("MerchantKeyId");
            String merchantsecretKey = await new SystemParamDB().GetValue("MerchantSecretKey");
            String cybersourceEnv = await new SystemParamDB().GetValue("CybersourceEnv");
            profileId = await new SystemParamDB().GetValue("ProfileId");

            x509 = await new SystemParamDB().GetValue("X509");
            tmxOrgId = await new SystemParamDB().GetValue("TmxOrgId");
            tmxServer = await new SystemParamDB().GetValue("TmxServer");

            configDictionary = new Dictionary<String, String>
            {
                { "merchantID", merchantID },
                { "runEnvironment", cybersourceEnv },
                { "timeout", "100000" },              // RestSharp Default 100s
                { "enableLog", "false" },
                { "logFileMaxSize", "835435" },

                { "authenticationType", "HTTP_SIGNATURE" },
                { "merchantKeyId", merchantKeyId },
                { "merchantsecretKey", merchantsecretKey },

                { "useMetaKey", "false" },
                { "enableClientCert", "false" }
            };
        }

        public static List<String> GetParams()
        {
            List<String> prms =
            [
                configDictionary["merchantID"],
                profileId,
                x509,
                tmxOrgId,
                tmxServer
            ];

            return prms;
        }

        //private static string ByteArrayToString(byte[] ba)
        //{
        //    StringBuilder hex = new StringBuilder(ba.Length * 2);
        //    foreach (byte b in ba)
        //        hex.AppendFormat("{0:x2}", b);
        //    return hex.ToString();
        //}

        public static async Task<String> GetParams(String alice)  //, ILogger logger = null)
        {
            (byte[] bobPublicKey, byte[] commonKey) b;
            do
            {
                b = SecurityFunctions.GetBobKeys(alice);
                //if (b.commonKey.Length == 32)
                //    logger?.LogWarning("CS > OK " + ByteArrayToString(b.commonKey));
                //else
                //    logger?.LogWarning("CS > KO " + ByteArrayToString(b.commonKey) + " (" + b.commonKey.Length + ")");
            }
            while (b.commonKey.Length != 32);

            String prms = $"{configDictionary["merchantID"]}|{profileId}|{x509}|{tmxOrgId}|{tmxServer}";

            (byte[] encPrms, byte[] prmsIV) c;
            try
            {
                c = await SecurityFunctions.Encrypt(b.commonKey, prms);
            }
            catch (Exception ex)
            {
                throw new Exception("C : " + ex.Message);
            }

            byte[] data = SecurityFunctions.ConcatData(b.bobPublicKey, c.encPrms, c.prmsIV);
            String sizes = SecurityFunctions.ConcatSizes(b.bobPublicKey.Length, c.encPrms.Length, c.prmsIV.Length);

            return sizes + Convert.ToBase64String(data);
        }

        // Customer
        public static async Task<TmsV2CustomersResponse> GetCustomer(String csToken)
        {
            return await new CustomerApi(new Configuration(merchConfigDictObj: configDictionary)).GetCustomerAsync(csToken, profileId);
        }

        public static async Task<TmsV2CustomersResponse> RegisterCustomer(String customerId, String customerEmail)
        {
            Tmsv2customersBuyerInformation buyerInformation = new Tmsv2customersBuyerInformation(MerchantCustomerID: customerId, Email: customerEmail);

            PostCustomerRequest request = new PostCustomerRequest(BuyerInformation: buyerInformation);

            return await new CustomerApi(new Configuration(merchConfigDictObj: configDictionary)).PostCustomerAsync(request, profileId);
        }

        // Instrument Identifier
        public static async Task<Tmsv2customersEmbeddedDefaultPaymentInstrumentEmbeddedInstrumentIdentifier> RegisterInstrumentIdentifier(String cardNumber)
        {
            Tmsv2customersEmbeddedDefaultPaymentInstrumentEmbeddedInstrumentIdentifierCard card =
                                                new Tmsv2customersEmbeddedDefaultPaymentInstrumentEmbeddedInstrumentIdentifierCard(Number: cardNumber);

            PostInstrumentIdentifierRequest request = new PostInstrumentIdentifierRequest(Card: card);

            return await new InstrumentIdentifierApi(new Configuration(merchConfigDictObj: configDictionary)).PostInstrumentIdentifierAsync(request, profileId);
        }

        // Payment Instrument
        public static async Task<List<Tmsv2customersEmbeddedDefaultPaymentInstrument>> GetPaymentInstrumentsByIdentifier(String instrumentIdentifierId)
        {
            PaymentInstrumentList result = await new InstrumentIdentifierApi(new Configuration(merchConfigDictObj: configDictionary)).GetInstrumentIdentifierPaymentInstrumentsListAsync(instrumentIdentifierId, profileId, null, null);

            if (result.Count == 0)
                return [];  // new List<Tmsv2customersEmbeddedDefaultPaymentInstrument>(0);

            return result.Embedded.PaymentInstruments;
        }

        public static async Task<List<Tmsv2customersEmbeddedDefaultPaymentInstrument>> GetPaymentInstrumentsByCustomer(String customerTokenId)
        {
            PaymentInstrumentList result = await new CustomerPaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).GetCustomerPaymentInstrumentsListAsync(customerTokenId, profileId, null, null);

            if (result.Count == 0)
                return [];  // new List<Tmsv2customersEmbeddedDefaultPaymentInstrument>(0);

            return result.Embedded.PaymentInstruments;
        }

        public static async Task<List<Tmsv2customersEmbeddedDefaultPaymentInstrument>> GetPaymentInstrumentsByCustomerAndIdentifier(String customerTokenId, String instrumentIdentifierId)
        {
            PaymentInstrumentList result = await new CustomerPaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).GetCustomerPaymentInstrumentsListAsync(customerTokenId, profileId, null, null);

            if (result.Count == 0)
                return [];  // new List<Tmsv2customersEmbeddedDefaultPaymentInstrument>(0);

            List<Tmsv2customersEmbeddedDefaultPaymentInstrument> paymentInstruments = [];  // new List<Tmsv2customersEmbeddedDefaultPaymentInstrument>();
            for (int i = 0; i < result.Embedded.PaymentInstruments.Count; i++)
                if (result.Embedded.PaymentInstruments[i].InstrumentIdentifier.Id == instrumentIdentifierId)
                    paymentInstruments.Add(result.Embedded.PaymentInstruments[i]);

            return paymentInstruments;
        }

        public static async Task<Tmsv2customersEmbeddedDefaultPaymentInstrument> RegisterPaymentInstrument(String customerTokenId,
                                                                                                           Tmsv2customersEmbeddedDefaultPaymentInstrumentCard card,
                                                                                                           Tmsv2customersEmbeddedDefaultPaymentInstrumentBillTo billTo,
                                                                                                           Tmsv2customersEmbeddedDefaultPaymentInstrumentInstrumentIdentifier instrumentIdentifier)
        {
            PostCustomerPaymentInstrumentRequest request = new PostCustomerPaymentInstrumentRequest(_Default: true, Card: card, BillTo: billTo, InstrumentIdentifier: instrumentIdentifier);

            return await new CustomerPaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).PostCustomerPaymentInstrumentAsync(customerTokenId, request, profileId);
        }

        public static async Task<Tmsv2customersEmbeddedDefaultPaymentInstrument> SetDefaultPaymentInstrument(String customerTokenId, String paymentInstrumentId)
        {
            PatchCustomerPaymentInstrumentRequest request = new PatchCustomerPaymentInstrumentRequest(Id: paymentInstrumentId, _Default: true);

            return await new CustomerPaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).PatchCustomersPaymentInstrumentAsync(customerTokenId, paymentInstrumentId, request, profileId);
        }

        public static async Task<Tmsv2customersEmbeddedDefaultPaymentInstrument> SetDefaultPaymentInstrument(String paymentInstrumentId)
        {
            PatchPaymentInstrumentRequest request = new PatchPaymentInstrumentRequest(_Default: true);

            return await new PaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).PatchPaymentInstrumentAsync(paymentInstrumentId, request, profileId);
        }

        // Transactions
        public static async Task<TransactionResponse> GetTransaction(String transactionId)
        {
            TransactionResponse transactionResponse = new TransactionResponse();
            RestResponse restResponse = null;
            Configuration configuration = new Configuration(merchConfigDictObj: configDictionary);
            try
            {
                TransactionDetailsApi transactionDetailsApi = new TransactionDetailsApi(configuration)
                {
                    ExceptionFactory = (name, response) => { restResponse = (RestResponse)response; return Configuration.DefaultExceptionFactory(name, response); }
                };
                ApiResponse<TssV2TransactionsGet200Response> apiResponse = await transactionDetailsApi.GetTransactionAsyncWithHttpInfo(transactionId);
                transactionResponse.StatusCode = apiResponse.StatusCode;
                transactionResponse.Data200 = apiResponse.Data;
            }
            catch
            {
                transactionResponse.StatusCode = (int)restResponse.StatusCode;
                transactionResponse.Data0 = restResponse.ErrorMessage;
            }

            return transactionResponse;
        }

        public static async Task<TssV2TransactionsGet200Response> GetTransactionCore(String transactionId)
        {
            return await new TransactionDetailsApi(new Configuration(merchConfigDictObj: configDictionary)).GetTransactionAsync(transactionId);
        }

        public static async Task<VoidResponse> VoidTransaction(String transactionId, String reference)
        {
            VoidResponse voidResponse = new VoidResponse();
            RestResponse restResponse = null;
            Configuration configuration = new Configuration(merchConfigDictObj: configDictionary);
            try
            {
                VoidApi voidApi = new VoidApi(configuration)
                {
                    ExceptionFactory = (name, response) => { restResponse = (RestResponse)response; return Configuration.DefaultExceptionFactory(name, response); }
                };
                ApiResponse<PtsV2PaymentsVoidsPost201Response> apiResponse = await voidApi.VoidPaymentAsyncWithHttpInfo(new VoidPaymentRequest(new Ptsv2paymentsidreversalsClientReferenceInformation(reference)), transactionId);
                voidResponse.StatusCode = apiResponse.StatusCode;
                voidResponse.Data201 = apiResponse.Data;
            }
            catch
            {
                voidResponse.StatusCode = (int)restResponse.StatusCode;
                if (voidResponse.StatusCode >= 400)
                    voidResponse.Data400 = (PtsV2PaymentsVoidsPost400Response)configuration.ApiClient.Deserialize(restResponse, typeof(PtsV2PaymentsVoidsPost400Response));
                else
                    voidResponse.Data0 = restResponse.ErrorMessage;
            }

            return voidResponse;
        }

        public static async Task<RefundResponse> RefundTransaction(String transactionId, String reference, String amount, String currency)
        {
            RefundResponse refundResponse = new RefundResponse();
            RestResponse restResponse = null;
            Configuration configuration = new Configuration(merchConfigDictObj: configDictionary);
            try
            {
                RefundApi voidApi = new RefundApi(configuration)
                {
                    ExceptionFactory = (name, response) => { restResponse = (RestResponse)response; return Configuration.DefaultExceptionFactory(name, response); }
                };
                ApiResponse<PtsV2PaymentsRefundPost201Response> apiResponse = await voidApi.RefundPaymentAsyncWithHttpInfo(new RefundPaymentRequest(new Ptsv2paymentsClientReferenceInformation(reference),
                                                                                                                                                    OrderInformation: new Ptsv2paymentsidrefundsOrderInformation
                                                                                                                                                    (new Ptsv2paymentsidcapturesOrderInformationAmountDetails(amount, currency))
                                                                                                                                                   ), transactionId);
                refundResponse.StatusCode = apiResponse.StatusCode;
                refundResponse.Data201 = apiResponse.Data;
            }
            catch
            {
                refundResponse.StatusCode = (int)restResponse.StatusCode;
                if (refundResponse.StatusCode >= 400)
                    refundResponse.Data400 = (PtsV2PaymentsRefundPost400Response)configuration.ApiClient.Deserialize(restResponse, typeof(PtsV2PaymentsRefundPost400Response));
                else
                    refundResponse.Data0 = restResponse.ErrorMessage;
            }

            return refundResponse;
        }

        // Delete
        public static async Task DelPaymentInstrument(String paymentInstrumentTokenId)
        {
            await new PaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).DeletePaymentInstrumentAsync(paymentInstrumentTokenId, profileId);
        }

        public static async Task<List<Tmsv2customersEmbeddedDefaultPaymentInstrument>> DelPaymentInstrumentsByIdentifier(String instrumentIdentifierTokenId)
        {
            List<Tmsv2customersEmbeddedDefaultPaymentInstrument> paymentInstruments = await GetPaymentInstrumentsByIdentifier(instrumentIdentifierTokenId);
            for (int i = 0; i < paymentInstruments.Count; i++)
                await new PaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).DeletePaymentInstrumentAsync(paymentInstruments[i].Id, profileId);
            return paymentInstruments;
        }

        public static async Task<List<Tmsv2customersEmbeddedDefaultPaymentInstrument>> DelPaymentInstrumentsByCustomerAndIdentifier(String customerTokenId, String instrumentIdentifierTokenId)
        {
            List<Tmsv2customersEmbeddedDefaultPaymentInstrument> paymentInstruments = await GetPaymentInstrumentsByCustomerAndIdentifier(customerTokenId, instrumentIdentifierTokenId);
            for (int i = 0; i < paymentInstruments.Count; i++)
                await new PaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).DeletePaymentInstrumentAsync(paymentInstruments[i].Id, profileId);
            return paymentInstruments;
        }

        public static async Task<List<Tmsv2customersEmbeddedDefaultPaymentInstrument>> DelPaymentInstrumentsByCustomer(String customerTokenId)
        {
            List<Tmsv2customersEmbeddedDefaultPaymentInstrument> paymentInstruments = await GetPaymentInstrumentsByCustomer(customerTokenId);
            for (int i = 0; i < paymentInstruments.Count; i++)
                await new PaymentInstrumentApi(new Configuration(merchConfigDictObj: configDictionary)).DeletePaymentInstrumentAsync(paymentInstruments[i].Id, profileId);
            return paymentInstruments;
        }
    }

    public class TransactionResponse
    {
        public int StatusCode { get; set; } = -1;
        public TssV2TransactionsGet200Response Data200 { get; set; } = null;
        public String Data0 { get; set; } = null;

        public TransactionResponse()
        {
        }
    }

    public class VoidResponse
    {
        public int StatusCode { get; set; } = -1;
        public PtsV2PaymentsVoidsPost201Response Data201 { get; set; } = null;
        public PtsV2PaymentsVoidsPost400Response Data400 { get; set; } = null;
        public String Data0 = null;

        public VoidResponse()
        {
        }
    }

    public class RefundResponse
    {
        public int StatusCode { get; set; } = -1;
        public PtsV2PaymentsRefundPost201Response Data201 { get; set; } = null;
        public PtsV2PaymentsRefundPost400Response Data400 { get; set; } = null;
        public String Data0 = null;

        public RefundResponse()
        {
        }
    }
}