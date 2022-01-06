using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp2.Neat;

namespace ConsoleApp2
{
    internal class Main
    {
        static void MainFunction(string[] args)
        {
            Population pop = new Population(3, 1, 128);
            for (int i = 0; i < 300; i++)
            {
                foreach (NeuralNetwork net in pop.population)
                {
                    net.fitness = xorfitness(net);
                }
                pop.doGeneration();
            }
            foreach (NeuralNetwork net in pop.population)
            {
                net.fitness = xorfitness(net);
            }
            NeuralNetwork champ = pop.population.Where(p => p.fitness == pop.population.Max(f => f.fitness)).FirstOrDefault();
            Console.WriteLine(champ.feedForward(new double[] { 0, 0, 1 })[0]);
            Console.WriteLine(champ.feedForward(new double[] { 1, 0, 1 })[0]);
            Console.WriteLine(champ.feedForward(new double[] { 0, 1, 1 })[0]);
            Console.WriteLine(champ.feedForward(new double[] { 1, 1, 1 })[0]);
            Console.ReadKey();
        }

        public static double xorfitness(NeuralNetwork net)
        {
            double fitness = 0;

            fitness += 1 - net.feedForward(new double[] { 0, 0, 1 })[0];
            fitness += net.feedForward(new double[] { 1, 0, 1 })[0];
            fitness += net.feedForward(new double[] { 0, 1, 1 })[0];
            fitness += 1 - net.feedForward(new double[] { 1, 1, 1 })[0];
            return Math.Pow(Math.Max((fitness * 100 - 200), 1), 2);
        }
    }
}
