﻿using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class License : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateOnly ExpirationDate { get; set; }
    public ICollection<UserLicense> UserLicenses { get; set; } = [];
}