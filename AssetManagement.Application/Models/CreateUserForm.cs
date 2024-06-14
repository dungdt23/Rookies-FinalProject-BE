using System.ComponentModel.DataAnnotations;
using AssetManagement.Application.Validation;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Models;

public class CreateUpdateUserForm
{
    [Required]
    [Naming]
    public required string FirstName { get; set; }

    [Required]
    [Naming]
    public required string LastName { get; set; }

    [Required]
    [WorkingAge]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public TypeGender Gender { get; set; } = TypeGender.Female;

    [Required]
    [WorkingDay]
    [LaterThanDateOfBirth(nameof(DateOfBirth))]
    public DateTime JoinedDate { get; set; }

    [Required]
    public Guid TypeId { get; set; }
    
    [Required]

    public Guid LocationId { get; set; }
}
