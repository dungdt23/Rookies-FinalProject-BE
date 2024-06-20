

using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Filters;

public class AssignmentFilter
{
    public Guid LocationId { get; set; }
    public Guid? UserId { get; set; }
    public UserType UserType { get; set; }
    public string? SearchString { get; set; }
    public bool IsAscending { get; set; } = true;
    public TypeAssignmentState? StateFilter { get; set; }
    public FieldAssignmentType FieldFilter { get; set; } = FieldAssignmentType.AssetCode;
    public DateTime? AssignedDateFilter {get; set; }
}
public enum FieldAssignmentType
{
    AssetCode = 1,
    AssetName,
    AssignedTo,
    AssignedBy,
    AssignedDate,
    State
}
