using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.Types.Domain
{
    public interface IAuditableEntity
    {
        int? CreatedBy { get; set; }
        DateTime? CreatedOn { get; set; }
        int? LastUpdatedBy { get; set; }
        DateTime? LastUpdatedOn { get; set; }
    }
}
