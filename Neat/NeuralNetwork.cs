using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp2.Helpers;

namespace ConsoleApp2.Neat
{
    internal class NeuralNetwork
    {
        public static double innovationNumber;
        public NodeGene[] storage { get; set; }
        public NodeGene[] nodeGenes { get; set; }
        public ConnectionGene[] connectionGenes { get; set; }
        public double fitness { get; set; }
        public bool vestigial { get; set; }

        public NeuralNetwork(NodeGene[] _nodeGenes, ConnectionGene[] _connectionGenes)
        {
            storage = _nodeGenes.OrderBy(n => n.id).ThenBy(n => n.geneType).ToArray();
            connectionGenes = _connectionGenes;
            innovationNumber = 0;
            fitness = 0;
            vestigial = false;
        }

        public double[] feedForward(double[] input)
        {
            var nodes = storage.Where(s => s.geneType == NeatHelper.GeneType.Input).ToArray();
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].value = input[i];
            }
            storage = nodes.ToArray();

            foreach (NodeGene node in storage.Where(s => s.geneType != NeatHelper.GeneType.Input))
            {
                foreach(ConnectionGene connectionGene in connectionGenes.Where(connection => connection.enabled && connection.outId == node.id))
                {
                    node.value += storage.Where(s => s.id == connectionGene.id).FirstOrDefault().value * connectionGene.weight;
                }
                node.value = Helper.sigmoid(node.value);
            }

            double[] outputs = storage.Where(s => s.geneType == NeatHelper.GeneType.Output).OrderBy(o => o.id).Select(p => p.value).ToArray();

            foreach (NodeGene node in storage)
            {
                node.value = 0;
            }

            return outputs;
        }

        public void mutateWeights()
        {
            Random mathRandom = new Random();

            foreach (ConnectionGene connection in connectionGenes)
            {
                double seed = mathRandom.NextDouble();

                if (seed < 0.1)
                {
                    connection.weight = mathRandom.Next(-10000, 10000) / 10000;
                }
                else
                {
                    connection.weight += Helper.std0() / 10;
                }
            }
        }

        public void addConnection()
        {
            bool connectionFound = false;
            Random mathRandom = new Random();

            foreach (NodeGene node1 in Helper.Shuffle<NodeGene>(nodeGenes))
            {
                foreach (NodeGene node2 in Helper.Shuffle<NodeGene>(nodeGenes))
                {
                    if ((node1.geneType == NeatHelper.GeneType.Input && node2.geneType == NeatHelper.GeneType.Hidden) || (node1.geneType == NeatHelper.GeneType.Input && node2.geneType == NeatHelper.GeneType.Output) || (node1.geneType == NeatHelper.GeneType.Hidden && node2.geneType == NeatHelper.GeneType.Hidden) || (node1.geneType == NeatHelper.GeneType.Hidden && node2.geneType == NeatHelper.GeneType.Output))
                    {
                        if (!connectionFound && (node1.id != node2.id))
                        {
                            bool isConnection = connectionGenes.Any(gene => (gene.id == node1.id && gene.outId == node2.id) || (gene.id == node2.id && gene.outId == node1.id));

                            if (!isConnection)
                            {
                                ConnectionGene c;

                                if (node1.id > node2.id && node1.geneType == NeatHelper.GeneType.Hidden && node2.geneType == NeatHelper.GeneType.Hidden)
                                {
                                    c = new ConnectionGene()
                                    {
                                        innov = ++innovationNumber,
                                        id = node2.id,
                                        outId = node1.id,
                                        enabled = true,
                                        weight = mathRandom.Next(-10000, 10000) / 10000
                                    };
                                }
                                else
                                {
                                    c = new ConnectionGene()
                                    {
                                        innov = ++innovationNumber,
                                        id = node1.id,
                                        outId = node2.id,
                                        enabled = true,
                                        weight = mathRandom.Next(-10000, 10000) / 10000
                                    };
                                }
                                connectionGenes.Append(c);
                                connectionFound = true;
                            }
                        }
                    }
                }
            }
        }

        public void addNode()
        {
            Random mathRandom = new Random();
            ConnectionGene chosen = connectionGenes[(int)Math.Floor((double)(mathRandom.Next() * connectionGenes.Length))];
            if (chosen != null)
            {
                chosen.enabled = false;
                NodeGene newNode = new NodeGene()
                {
                    geneType = NeatHelper.GeneType.Hidden,
                    id = nodeGenes.Max(m => m.id) + 1
                };
                nodeGenes.Append(newNode);
                connectionGenes.Append(new ConnectionGene()
                {
                    innov = ++innovationNumber,
                    id = chosen.id,
                    outId = newNode.id,
                    enabled = true,
                    weight = mathRandom.Next(-10000, 10000) / 10000
                });
                connectionGenes.Append(new ConnectionGene()
                {
                    innov = ++innovationNumber,
                    id = newNode.id,
                    outId = chosen.outId,
                    enabled = true,
                    weight = mathRandom.Next(-10000, 10000) / 10000
                });
                storage = nodeGenes.OrderBy(n => n.id).ThenBy(n => n.geneType).ToArray();
            }
        }

        public NeuralNetwork crossover(NeuralNetwork otherNet)
        {
            NodeGene[] newNodeGenes = (NodeGene[])nodeGenes.Clone();
            ConnectionGene[] newConnectionGenes = new ConnectionGene[connectionGenes.Length];
            Random mathRandom = new Random();
            foreach (ConnectionGene gene in connectionGenes)
            {
                ConnectionGene otherGene = otherNet.connectionGenes.Where(g => g.innov == gene.innov).FirstOrDefault();
                if (otherGene != null)
                {
                    bool toEnable = true;
                    if (!gene.enabled || !otherGene.enabled)
                    {
                        if (mathRandom.Next() < 0.75)
                        {
                            toEnable = false;
                        }
                    }
                    if (mathRandom.Next() < 0.5)
                    {
                        otherGene.enabled = toEnable;
                        newConnectionGenes.Append(otherGene);
                    }
                    else
                    {
                        gene.enabled = toEnable;
                        newConnectionGenes.Append(gene);
                    }
                }
                else
                {
                    newConnectionGenes.Append(gene);
                }
            }
            return new NeuralNetwork(newNodeGenes, newConnectionGenes);
        }

        public void mutate()
        {
            Random mathRandom = new Random();

            if (mathRandom.Next() < 0.8)
            {
                this.mutateWeights();
            }
            if (mathRandom.Next() < 0.05)
            {
                this.addConnection();
            }
            if (mathRandom.Next() < 0.01)
            {
                this.addNode();
            }
        }

        public double weightDiff(NeuralNetwork otherNet)
        {
            int diff = 0;
            int matching = 0;

            foreach (ConnectionGene gene in connectionGenes)
            {
                foreach(ConnectionGene gene2 in otherNet.connectionGenes)
                {
                    if (gene.innov == gene2.innov)
                    {
                        matching++;
                        diff += (int)Math.Abs(gene.weight - gene2.weight);
                    }
                }
            }

            if (matching == 0)
            {
                return 100;
            }

            return diff / matching;
        }

        public double disjointAndExcess(NeuralNetwork otherNet)
        {
            int matching = connectionGenes.Where(c => otherNet.connectionGenes.Any(c2 => c2.innov == c.innov)).Count();
            return (connectionGenes.Length + otherNet.connectionGenes.Length) - 2 * (matching);
        }
    }
}
