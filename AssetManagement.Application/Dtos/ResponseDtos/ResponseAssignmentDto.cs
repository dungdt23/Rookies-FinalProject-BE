using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Dtos.ResponseDtos;

public class ResponseAssignmentDto
{
    public Guid Id { get; set; }
    public string AssetCode { get; set; }
    public string AssetName { get; set;}
    public string Specification {get; set;}
    public string AssignedTo { get; set;}
    public string AssignedBy { get; set;}
    public DateTime AssignedDate { get; set; }
    public string State {get; set;}
    public string? Note { get; set; }

}
