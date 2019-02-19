using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Armut.Entity.Tables
{
    public class Cities
    {
        public int Id { get; set; }

        [StringLength(30)]
        public string Description { get; set; }

        public IQueryable<Users> Users { get; set; }
    }
}
