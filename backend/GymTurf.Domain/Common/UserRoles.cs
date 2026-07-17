using System.Collections.Generic;

namespace GymTurf.Domain.Common;

public static class UserRoles
{
    public const string SuperAdmin = "Super Admin";
    public const string SystemOwner = "System Owner";
    public const string BranchManager = "Branch Manager";
    public const string Receptionist = "Receptionist";
    public const string Member = "Member";
    public const string Guest = "Guest";

    public static readonly IReadOnlyList<string> AllRoles = new List<string>
    {
        SuperAdmin,
        SystemOwner,
        BranchManager,
        Receptionist,
        Member,
        Guest
    };
}
