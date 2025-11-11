using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class WebSysUser
    {
        public int Id { get; set; }
        public String AuthUserId { get; set; }
        public String Email { get; set; }
        public String Roles { get; set; }
        public int PhoneCountryId { get; set; }
        public String Phone { get; set; }
        public String Pin { get; set; }
        public int PinFails { get; set; } = 0;
        public DateTime? PinDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int WebSysUserStatusId { get; set; }

        public String[] GetRolesAsArray() => Roles.Split('|');
        public List<String> GetRolesAsList() => [.. Roles.Split('|')];
        public bool HasRole(String role) => Roles.Contains(role);

        public WebSysUser()
        {

        }

        public WebSysUser(int id, String authUserId, String email, String roles, int phoneCountryId, String phone,
                          String pin, int pinFails, DateTime? pinDateTime,
                          DateTime createDateTime, DateTime updateDateTime, int webSysUserStatusId)
        {
            Id = id;
            AuthUserId = authUserId;
            Email = email;
            Roles = roles;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Pin = pin;
            PinFails = pinFails;
            PinDateTime = pinDateTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            WebSysUserStatusId = webSysUserStatusId;
        }

        public WebSysUser(int id, String authUserId, String email, String roles, int phoneCountryId, String phone, int webSysUserStatusId)
        {
            Id = id;
            AuthUserId = authUserId;
            Email = email;
            Roles = roles;
            PhoneCountryId = phoneCountryId;
            Phone = phone;
            Pin = null;
            PinFails = 0;
            PinDateTime = null;
            WebSysUserStatusId = webSysUserStatusId;
        }
    }
}
