﻿using AssetManagement.Application.Validation;
using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AssetManagement.Application.Dtos.RequestDtos
{
    public class RequestUserCreateDto
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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TypeGender Gender { get; set; } = TypeGender.Female;

        [Required]
        [WorkingDay]
        [LaterThanDateOfBirth(nameof(DateOfBirth))]
        public DateTime JoinedDate { get; set; }

        [Required]
        public string Type { get; set; }

        [JsonIgnore]

        public Guid LocationId { get; set; }
    }
}
