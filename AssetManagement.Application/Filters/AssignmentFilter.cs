

using AssetManagement.Domain.Constants;

namespace AssetManagement.Application.Filters;

public class AssignmentFilter
{
    public Guid LocationId { get; set; }
    public Guid userId { get; set; }
    public UserType? UserType { get; set; }
    public string? SearchString { get; set; }
    public bool IsAscending { get; set; } = true;
    public FieldAssignmentType FieldFilter { get; set; } = FieldAssignmentType.AssetCode;
}
public enum FieldAssignmentType
{
    AssetCode,
    AssetName,
    AssignedTo,
    AssignedBy,
    AssignedDate,
    State
}
