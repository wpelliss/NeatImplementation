using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp2.Helpers.NeatHelper;

namespace ConsoleApp2.Neat
{
    internal class NodeGene
    {
        public int id { get; set; }
        public GeneType geneType { get; set; }
        public double value { get; set; }

        public NodeGene()
        {

        }

        public NodeGene(int _id, GeneType _geneType)
        {
            id = _id;
            geneType = _geneType;
        }
    }
}
