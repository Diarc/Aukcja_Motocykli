using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aukcja_Motocykli.Models
{
    public class Make
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

       /* public ICollection<Model> Models { get; set; } */
    }
}
