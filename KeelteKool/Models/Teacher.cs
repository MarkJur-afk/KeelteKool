using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KeelteKool.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        public string Nimi { get; set; }

        public string Kvalifikatsioon { get; set; }
        public string FotoPath { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}