using System;
using System.Collections.Generic;
using System.Text;

namespace Armut.Entity.Tables
{
    public class BaseEntities
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long UpdatedUserId { get; set; }
        public DateTime DeletedDate { get; set; }
        public long DeletedUserId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
