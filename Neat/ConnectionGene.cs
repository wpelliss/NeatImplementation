using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Neat
{
    internal class ConnectionGene
    {
        public int id { get; set; } // The if of the node taht feeds into the connection
        public int outId { get; set; } // The if of the node that the connection feeds to
        public bool enabled { get; set; }
        public double innov { get; set; }
        public double weight { get; set; } // The weight of the connection
    }
}
