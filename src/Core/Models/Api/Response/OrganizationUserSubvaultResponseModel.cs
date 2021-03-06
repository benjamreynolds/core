﻿using System;
using Bit.Core.Models.Data;

namespace Bit.Core.Models.Api
{
    public class OrganizationUserSubvaultResponseModel : ResponseModel
    {
        public OrganizationUserSubvaultResponseModel(SubvaultUserDetails details,
            string obj = "organizationUserSubvault")
            : base(obj)
        {
            if(details == null)
            {
                throw new ArgumentNullException(nameof(details));
            }

            Id = details.Id.ToString();
            Name = details.Name;
            SubvaultId = details.SubvaultId.ToString();
            ReadOnly = details.ReadOnly;
            Admin = details.Admin;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string SubvaultId { get; set; }
        public bool ReadOnly { get; set; }
        public bool Admin { get; set; }
    }
}
