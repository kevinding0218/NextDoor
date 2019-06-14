using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.src.NextDoor.Core.Types
{
    public interface IAuditableEntity
    {
        int CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        int? LastUpdatedBy { get; set; }
        DateTime? LastUpdatedOn { get; set; }
    }
}
