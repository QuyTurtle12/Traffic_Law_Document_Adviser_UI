﻿namespace Util.Constants
{
    public static class RoleConstants
    {
        public const int UserValue = 1;
        public const int ExpertValue = 2;
        public const int AdminValue = 3;
        public const int StaffValue = 4;

        public const string User = "User";
        public const string Expert = "Expert";
        public const string Admin = "Admin";
        public const string Staff = "Staff";

        public static string ToRoleName(int v) =>
            v switch
            {
                AdminValue => Admin,
                ExpertValue => Expert,
                StaffValue => Staff,
                _ => User,
            };

        public static int ToRoleValue(string name) =>
            name switch
            {
                Admin => AdminValue,
                Expert => ExpertValue,
                Staff => StaffValue,
                _ => UserValue
            };
    }
}
