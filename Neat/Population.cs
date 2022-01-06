using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Neat
{
    internal class Population
    {
        public int inputs { get; set; }
        public int outputs { get; set; }
        public int popSize { get; set; }
        public double excessCoeff { get; set; }
        public double weightDiffCoeff { get; set; }
        public double diffThresh { get; set; }
        public NeuralNetwork[] population { get; set; }
        public NeuralNetwork[][] species { get; set; }

        public Population(int _inputs, int _outputs, int _popsize)
        {
            inputs = _inputs;
            outputs = _outputs;
            popSize = _popsize;
            excessCoeff = 1;
            weightDiffCoeff = 2;
            diffThresh = 1.5;
            species = new NeuralNetwork[][] { };
            population = new NeuralNetwork[] { };
            NodeGene[] nodes = new NodeGene[] { };
            for (int i = 0; i < inputs; i++)
            {
                NodeGene nodeGene = new NodeGene(i, Helpers.NeatHelper.GeneType.Input);
                var lstNodes = nodes.ToList();
                lstNodes.Add(nodeGene);
                nodes = lstNodes.ToArray();
            }
            for (int i = 0; i < outputs; i++)
            {
                NodeGene nodeGene = new NodeGene(i + inputs, Helpers.NeatHelper.GeneType.Output);
                var lstNodes = nodes.ToList();
                lstNodes.Add(nodeGene);
                nodes = lstNodes.ToArray();
            }
            for (int i = 0; i < popSize; i++)
            {
                ConnectionGene[] connections = new ConnectionGene[0];
                NeuralNetwork nn = new NeuralNetwork(nodes, connections);
                nn.mutate();
                population.Append(nn);
            }
        }

        public void speciatePopulation()
        {
            Random mathRandom = new Random();
            foreach (NeuralNetwork nn in population)
            {
                bool speciesFound = false;
                foreach (NeuralNetwork[] s in species)
                {
                    if (s.Length != 0)
                    {
                        if (!speciesFound)
                        {
                            NeuralNetwork rep = s[(int)Math.Floor((double)mathRandom.Next() * s.Length)];
                            double diff = ((excessCoeff * nn.disjointAndExcess(rep)) / (Math.Max(rep.connectionGenes.Length + nn.connectionGenes.Length, 1))) + weightDiffCoeff * nn.weightDiff(rep);
                            if (diff < diffThresh)
                            {
                                s.Append(nn);
                                speciesFound = true;
                            }
                        }
                    }
                }
                if (!speciesFound)
                {
                    List<NeuralNetwork> newSpecies = new List<NeuralNetwork>();
                    newSpecies.Add(nn);
                    species.Append(newSpecies.ToArray());
                }
            }
        }

        public double avgFitness()
        {
            return population.Length != 0 ? population.Average(p => p.fitness) : 0;
        }

        public NeuralNetwork chooseParent(NeuralNetwork[] s)
        {
            Random mathRandom = new Random();
            double threshold = mathRandom.Next() * s.Sum(t => t.fitness);
            double sum = 0;
            foreach (NeuralNetwork n in s)
            {
                sum += n.fitness;
                if (sum > threshold)
                {
                    return n;
                }
            }
            return s.FirstOrDefault();
        }

        public void doGeneration()
        {
            var popFitness = this.avgFitness();
            Array.Clear(population, 0, population.Length);
            int amtLeft = popSize;
            foreach (NeuralNetwork[] s in species)
            {
                int newIndividualsCount = (int)Math.Ceiling((double)s.Sum(nn => nn.fitness / popFitness ) * s.Length);
                amtLeft -= newIndividualsCount;
                if (amtLeft < 0)
                {
                    newIndividualsCount += amtLeft;
                    amtLeft = 0;
                }

                NeuralNetwork[] newPeeps = new NeuralNetwork[] { };

                for (int i = 0; i < newIndividualsCount; i++)
                {
                    NeuralNetwork parent1 = this.chooseParent(s);
                    NeuralNetwork parent2 = this.chooseParent(s);
                    NeuralNetwork baby;
                    if (parent1.fitness > parent2.fitness)
                    {
                        baby = parent1.crossover(parent2);
                    }
                    else
                    {
                        baby = parent2.crossover(parent1);
                    }
                    baby.mutate();
                    newPeeps.Append(baby);
                    population.Append(baby);
                }
            }
            foreach (NeuralNetwork[] s in species)
            {
                foreach (NeuralNetwork nn in s)
                {
                    nn.vestigial = true;
                }
            }
            species = species.Where(s => s.Length != 0).ToArray();
            speciatePopulation();
            for (int i = 0; i < species.Length; i++)
            {
                species[i] = species[i].Where(x => !x.vestigial).ToArray();
            }
            species = species.Where(s => s.Length != 0).ToArray();
        }
    }
}
