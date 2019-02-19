using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Armut.Entity.Tables
{
    public class Users : BaseEntities
    {
        #region Kişisel Bilgiler
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(30)]
        public string MiddleName { get; set; }

        [StringLength(30)]
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }

        [StringLength(300)]
        public string ImagePath { get; set; }

        [StringLength(30)]
        public string PhoneNumber { get; set; }

        [StringLength(30)]
        public string ScoialMediaUrl { get; set; }
        #endregion

        #region Adres Bilgileri
        [ForeignKey("CountryId")]
        public int CountryId { get; set; }
        public virtual Counties Counties { get; set; }


        [StringLength(300)]
        public string Address { get; set; }

        [ForeignKey("CityId")]
        public int CityId { get; set; }
        public virtual Cities Cities { get; set; }


        [ForeignKey("DistirctId")]
        public int DistirctId { get; set; }
        public virtual District District { get; set; }
        #endregion

        [StringLength(30)]
        public string Job { get; set; }

        public IQueryable<Skills> Skills { get; set; }

    }
}
