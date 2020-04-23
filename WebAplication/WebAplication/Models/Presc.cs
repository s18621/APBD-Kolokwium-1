using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAplication.Models
{
    public class Presc
    {
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public int IdPatient { get; set; }
        public int IdDoctor { get; set; }
    }
}
