﻿using AssetManagement.Domain.Common;
using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class ReturnRequest : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid RequestorId { get; set; }
        [Required]
        public Guid ResponderId { get; set; }
        [Required]
        public Guid AssetId { get; set; }
        public User Requestor { get; set; }
        public User Responder { get; set; }
        public Asset Asset { get; set; }
        public TypeRequestState State { get; set; }
        public DateTime ReturnedDate { get; set; }
    }
}
