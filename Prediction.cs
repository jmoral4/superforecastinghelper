using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperforcastingHelper
{
    public class Prediction
    {
        public int? Id { get; set; }
        public string Description { get; set; }
        public double Probability { get; set; }
        public string Date { get; set; }
        public string Notes { get; set; }
        public string CreatedAt { get; set; }
        public int? Outcome { get; set; }
    }

}
