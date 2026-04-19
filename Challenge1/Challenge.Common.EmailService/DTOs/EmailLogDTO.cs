using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace challenge1.Common.EmailService.DTOs
{
    internal class AddEmailLogRequestDTO
    {
        public Guid EmailId { get; set; }
        public string LogType { get; set; } = string.Empty;
        public string LogMessage { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
    }

    internal class AddEmailLogResponseDTO
    {
        public Guid EmailLogId { get; set; }
    }
}
