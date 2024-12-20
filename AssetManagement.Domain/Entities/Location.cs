﻿using AssetManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class Location : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(200)]
        public required string LocationName { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Asset> Assets { get; set; }
        public ICollection<ReturnRequest> ReturnRequests { get; set; }

    }
}
