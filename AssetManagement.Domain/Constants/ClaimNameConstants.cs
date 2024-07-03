using System.Security.Claims;

namespace AssetManagement.Domain.Constants
{
    public static class ClaimNameConstants
    {
        public const string UserId = "id";
        public const string Username = "username";
        public const string TypeId = "typeId";
        public const string Role = ClaimTypes.Role;
        public const string LocationId = "locationId";
        public const string Location = "location";
        public const string IsPasswordChangedFirstTime = "isPasswordChangedFirstTime";
        public const string BlackListTimeStamp = "blTimestamp";
    }
}
