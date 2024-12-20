﻿using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.RequestDtos
{
    public class RequestGetAllReturnRequestDto
    {
        public int Page { get; set; } = 1;
        public int PerPage { get; set; } = 15;
        public ReturnRequestSortField SortField { get; set; } = ReturnRequestSortField.RequestedDate;
        public TypeOrder SortOrder { get; set; } = TypeOrder.Ascending;
        public TypeRequestState? RequestState { get; set; }
        public DateOnly? ReturnedDate { get; set; }
        [MaxLength(200)]
        public string? Search { get; set; }
    }

    public enum ReturnRequestSortField
    {
        RequestedDate = 1,
        AssetCode,
        AssetName,
        RequestBy,
        AssetAssignedDate,
        RespondBy,
        ReturnedDate,
        State
    }
}
