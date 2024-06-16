using AssetManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Dtos.ResponseDtos
{
    public class ResponseAssetDto
    {
        public  Guid Id { get; set; }
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Specification { get; set; }
        public DateTime InstalledDate { get; set; }
        public string State { get; set; }
    }
}
