using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using FirebaseAdmin.Auth;

namespace HeroServer
{
    public static class VisionFunctions
    {
        // FACES
        public static async Task<LeapResponse> DetectFacesCount(HttpContext httpContext, VisionRequest request)
        {
            LeapResponse response = await LeapServerHelper.Post<VisionRequest, LeapResponse>("vision/DetectFacesCount", request);
            //await SaveUsage(httpContext, 1, response);
            return response;
        }

        public static async Task<LeapResponse> DetectFaces(HttpContext httpContext, VisionRequest request)
        {
            LeapResponse response = await LeapServerHelper.Post<VisionRequest, LeapResponse>("vision/DetectFaces", request);
            //await SaveUsage(httpContext, 2, response);
            return response;
        }

        public static async Task<LeapResponse> CompareFaces(HttpContext httpContext, VisionRequest request)
        {
            LeapResponse response = await LeapServerHelper.Post<VisionRequest, LeapResponse>("vision/CompareFaces", request);
            //await SaveUsage(httpContext, 3, response);
            return response;
        }

        // DPI
        public static async Task<LeapResponse> ExtractDpiFront(HttpContext httpContext, VisionRequest request)
        {
            LeapResponse response = await LeapServerHelper.Post<VisionRequest, LeapResponse>("vision/ExtractDpiFront", request);
            await SaveUsage(httpContext, 4, request.Image, response);
            return response;
        }

        public static async Task<LeapResponse> ExtractDpiBack(HttpContext httpContext, VisionRequest request)
        {
            LeapResponse response = await LeapServerHelper.Post<VisionRequest, LeapResponse>("vision/ExtractDpiBack", request);
            await SaveUsage(httpContext, 5, request.Image, response);
            return response;
        }

        // LIVENESS 3D

        public static async Task<LeapResponse> Liveness3dKeys()
        {
            return await LeapServerHelper.Get<LeapResponse>("vision/Liveness3dKeys");
        }

        public static async Task<LeapResponse> Liveness3dToken()
        {
            return await LeapServerHelper.Get<LeapResponse>("vision/Liveness3dToken");
        }

        public static async Task<LeapResponse> Liveness3d(HttpContext httpContext, VisionLiveness3dRequest liveness3dRequest)
        {
            LeapResponse response = await LeapServerHelper.Post<VisionLiveness3dRequest, LeapResponse>("vision/Liveness3d", liveness3dRequest);
            await SaveUsage(httpContext, 6, liveness3dRequest.AuditTrailImage, response);
            return response;
        }

        // LEAP USAGE

        private static async Task SaveUsage(HttpContext httpContext, int productId, String image, LeapResponse response)
        {
            FirebaseToken token = await FirebaseFunctions.GetAuthToken(httpContext);
            int appUserId = await AppUserFunctions.GetIdByAuthUserId((String)token.Claims["user_id"]);

            if (image != null && image.Length > 0)
            {
                int count = await new LeapUsageDB().GetCountByProductId(appUserId, productId, 1);
                String containerName = $"user{appUserId:D08}";
                await StorageFunctions.CreateContainer(containerName);
                if (productId == 4)
                    await StorageFunctions.UpdateFile(containerName, $"idfr{appUserId:D08}|{count:D02}", "jpg", Convert.FromBase64String(image));
                if (productId == 5)
                    await StorageFunctions.UpdateFile(containerName, $"idbk{appUserId:D08}|{count:D02}", "jpg", Convert.FromBase64String(image));
            }

            LeapUsage leapUsage = new LeapUsage(appUserId, productId, response.Code, response.Message);
            await new LeapUsageDB().Add(leapUsage);
        }

        private static async Task SaveUsage(HttpContext httpContext, int productId, LeapResponse response)
        {
            await SaveUsage(httpContext, productId, null, response);
        }
    }
}