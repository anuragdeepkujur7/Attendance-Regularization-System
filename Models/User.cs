using System;
using System.Collections.Generic;

namespace Project_6_final.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string Roles { get; set; } = null!;

    public string? Gender { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    public virtual ICollection<Invalidtoken> Invalidtokens { get; set; } = new List<Invalidtoken>();

    public virtual ICollection<Monthlyattendance> Monthlyattendances { get; set; } = new List<Monthlyattendance>();

    public virtual ICollection<Regularizationrequest> RegularizationrequestRequestedByNavigations { get; set; } = new List<Regularizationrequest>();

    public virtual ICollection<Regularizationrequest> RegularizationrequestUsers { get; set; } = new List<Regularizationrequest>();
}
