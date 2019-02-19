using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Armut.Entity.Tables
{
    public class Skills : BaseEntities
    {
        [StringLength(200)]
        public string Description { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual Users User { get; set; }

        public IQueryable<Users> Users { get; set; }
    }
}
