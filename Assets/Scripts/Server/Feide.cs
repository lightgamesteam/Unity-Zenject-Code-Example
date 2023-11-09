using System;
using System.Collections.Generic;

namespace TDL.Server
{
    public class UserInfo
    {
        public string eduPersonPrincipalName { get; set; }
        public string[] eduPersonEntitlement { get; set; }
        public string norEduPersonNIN { get; set; }
        public string[] eduPersonAffiliation { get; set; }
        public string eduPersonPrimaryAffiliation { get; set; }
        public string displayName { get; set; }
        public Group[] UserGroup { get; set; }
    }

    public class Group
    {
        public string[] orgType { get; set; }
        public string norEduOrgNIN { get; set; }
        public string displayName { get; set; }
        public bool _public { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public Membership membership { get; set; }
        public string eduOrgLegalName { get; set; }
        public string mail { get; set; }
        public string parent { get; set; }
    }

    public class Membership
    {
        public string basic { get; set; }
        public string displayName { get; set; }
        public string[] affiliation { get; set; }
        public string primaryAffiliation { get; set; }
        public bool primarySchool { get; set; }
    }

    public class FeideUserResponse
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }        
        public List<UserGrade> Grades { get; set; }
        public int SubscriptionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SubscriptionName { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }       
    }

    public class UserGrade
    {
        public int GradeId { get; set; }
        public string GradeName { get; set; }
    }
}