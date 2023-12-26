using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserOperationClaim
    {
        [Key]
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public int OperationClaimId { get; set; }
    }
}
