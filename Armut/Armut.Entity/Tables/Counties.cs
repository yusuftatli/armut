using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Armut.Entity.Tables
{
    [Table("Counties")]
    public class Counties
    {
        public int Id { get; set; }

        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(30)]
        public string Description { get; set; }

        public IQueryable<Users> Users { get; set; }
    }
}
