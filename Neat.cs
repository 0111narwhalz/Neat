using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
	static class Neat
	{
		//process parameters
		static int popSize;
		static int inputCount;
		static int outputCount;
		static float c1; //compatibility distance coefficients
		static float c2;
		static float dt; //threshold compatibility distance
		static float nocross; //chance of mutation only, without crossing
		static float deletion; //chance of gene being deleted if absent in either parent
		static float interspecies; //chance of interspecies mating
		static float pointMut; //chance of point mutation being a small change, otherwise new random value
		static float mutation; //chance of genome being selected for mass point mutation
		static float edgeMut; //chance of new edge
		static float nodeMut; //chance of new node
		
		//working data
		static int generation;
		static uint nodeCounter; //ID of next node to be created
		static Genome[] population;
		static List<List<int>> species;
		static List<float> speciesFitness;
		static int[] speciesOffspring;
		static List<Genome> speciesMarkers;
		static Dictionary<uint, uint> splitList;
		
		static Random rng;
		
		public static void Initialize()
		{
			//*
			popSize = 150;
			inputCount = 3;
			outputCount = 1;
			c1 = 1f;
			c2 = .4f;
			dt = 3f;
			nocross = .25f;
			deletion = 0;//.75f;
			interspecies = .001f;
			pointMut = .9f;
			mutation = .8f;
			edgeMut = .03f;
			nodeMut = .05f;
			//*/
			
			rng = new Random();
			
			Console.WriteLine("Seeding new population");
			Seed();
		}
		
		static void Seed()
		{
			generation = 0;
			nodeCounter = (uint)(inputCount + outputCount);
			population = new Genome[popSize];
			species = new List<List<int>>();
			speciesFitness = new List<float>();
			speciesMarkers = new List<Genome>();
			splitList = new Dictionary<uint, uint>();
			for(int i = 0; i < popSize; i++)
			{
				population[i] = SeedGenome();
			}
			Console.WriteLine("Speciating new population");
			Speciate();
			Console.WriteLine("Assessing new population");
			Assess();
			Report();
		}
		
		static Genome SeedGenome()
		{
			Genome output = new Genome(inputCount, outputCount);
			//Create fully-connected perceptron
			//Note that this may not be the minimal case for large numbers of inputs or outputs
			for(uint i = 0; i < inputCount; i++)
			{
				for(uint j = 0; j < outputCount; j++)
				{
					output.contents.Add(Genome.Pair(i,(uint)(j+inputCount)), (float)(rng.NextDouble()*2 - 1));
				}
			}
			return output;
		}
		
		public static void DoGeneration()
		{
			Console.WriteLine("Provisioning offspring");
			Provision();
			Console.WriteLine("Breeding to quota");
			Breed();
			Console.WriteLine("Speciating population");
			Speciate();
			Console.WriteLine("Assessing");
			Assess();
			Report();
		}
		
		static void Provision()
		{
			speciesOffspring = new int[species.Count];
			int offspringRemaining = popSize;
			//Account for champions
			Console.WriteLine("Reserving positions for champions");
			for(int i = 0; i < species.Count; i++)
			{
				if(species[i].Count >= 5)
				{
					speciesOffspring[i] = 1;
					offspringRemaining--;
					continue;
				}
				speciesOffspring[i] = 0;
			}
			
			Console.WriteLine("Distributing the rest: {0}", offspringRemaining);
			float sumFitness = 0;
			for(int i = 0; i < species.Count; i++)
			{
				sumFitness += speciesFitness[i] * species[i].Count;
			}
			float totalOverSum = (float)offspringRemaining / sumFitness;
			for(int i = 0; i < species.Count; i++)
			{
				speciesOffspring[i] += (int)(totalOverSum * speciesFitness[i] * species[i].Count);
				Console.WriteLine("Allocated {0} offspring to species {1}", speciesOffspring[i], i);
				offspringRemaining -= (int)(totalOverSum * speciesFitness[i] * species[i].Count);
			}
			if(offspringRemaining < 0)
			{
				Console.WriteLine("Overallocated offspring!");
			}
			Console.WriteLine("Filling {0} unused seats", offspringRemaining);
			while(offspringRemaining > 0)
			{
				speciesOffspring[rng.Next(species.Count)]++;
				offspringRemaining--;
			}
		}
		
		static void Breed()
		{
			int popIndex = 0;
			int offspring;
			Genome[] newPop = new Genome[popSize];
			int parent1;
			int parent2;
			Genome child;
			for(int i = 0; i < species.Count; i++)
			{
				offspring = speciesOffspring[i];
				if(species[i].Count >= 5)
				{
					newPop[popIndex] = population[species[i][0]].Copy();
					popIndex++;
					offspring--;
				}
				for(; offspring > 0; offspring--)
				{
					parent1 = species[i][offspring % species[i].Count];
					parent2 = species[i][(offspring + 1) % species[i].Count];
					if(rng.NextDouble() < nocross)
					{
						child = MutatePoints(population[parent1]);
					}else
					{
						if(rng.NextDouble() < interspecies)
						{
							parent2 = rng.Next(popSize);
						}
						child = Cross(population[parent1], population[parent2]);
						if(rng.NextDouble() < mutation)
						{
							child = MutatePoints(child);
						}
						if(rng.NextDouble() < edgeMut)
						{
							child = AddEdge(child);
						}
						if(rng.NextDouble() < nodeMut)
						{
							child = AddNode(child, ref splitList);
						}
					}
					newPop[popIndex] = child;
					popIndex++;
				}
				//splitList.Clear();
			}
			population = newPop;
			generation++;
		}
		
		static void Report()
		{
			Console.WriteLine("Generation {0}", generation);
			for(int i = 0; i < species.Count; i++)
			{
				Console.WriteLine("Species {0} with {1} members: {2}", i, species[i].Count, speciesFitness[i]);
			}
		}
		
		static void Assess()
		{
			for(int i = 0; i < popSize; i++)
			{
				population[i].fitness = Assess(population[i]);
			}
			speciesFitness.Clear();
			float spfit;
			for(int i = 0; i < species.Count; i++)
			{
				spfit = 0;
				for(int j = 0; j < species[i].Count; j++)
				{
					spfit += population[species[i][j]].fitness;
				}
				speciesFitness.Add(spfit / species[i].Count);
				species[i].Sort((genome1, genome2) => population[genome1].fitness.CompareTo(population[genome2].fitness));
			}
		}
		
		static float Assess(Genome subject)
		{
			Network subjectNet = subject.Compile();
			float error = 0;
			subjectNet.SetInputs(new float[3]{1,0,0});
			subjectNet.Iterate();
			error += (float)Math.Abs(subjectNet.GetOutputs()[0]);
			subjectNet.SetInputs(new float[3]{1,0,1});
			subjectNet.Iterate();
			error += (float)Math.Abs(1-subjectNet.GetOutputs()[0]);
			subjectNet.SetInputs(new float[3]{1,1,0});
			subjectNet.Iterate();
			error += (float)Math.Abs(1-subjectNet.GetOutputs()[0]);
			subjectNet.SetInputs(new float[3]{1,1,1});
			subjectNet.Iterate();
			error += (float)Math.Abs(subjectNet.GetOutputs()[0]);
			float fitness = 4-error;
			return fitness * fitness;
		}
		
		static Genome Cross(Genome parent1, Genome parent2) //parent1 should be the more fit
		{
			Genome result = new Genome(inputCount, outputCount);
			foreach(KeyValuePair<uint, float> gene in parent1.contents)
			{
				if(parent2.contents.ContainsKey(gene.Key))
				{
					result.contents.Add(gene.Key, rng.NextDouble() < .5 ? gene.Value : parent2.contents[gene.Key]);
				}else
				{
					if(rng.NextDouble() > deletion)
					{
						result.contents.Add(gene.Key, gene.Value);
					}
				}
			}
			return result;
		}
		
		static Genome MutatePoints(Genome subject)
		{
			Genome result = subject.Copy();
			foreach(KeyValuePair<uint, float> gene in subject.contents)
			{
				if(rng.NextDouble() < pointMut)
				{
					result.contents[gene.Key] += (float)(rng.NextDouble()-1);
				}else
				{
					result.contents[gene.Key] = (float)(rng.NextDouble()*2 - 1);
				}
			}
			return result;
		}
		
		static Genome AddEdge(Genome subject)
		{
			Genome result = subject.Copy();
			Dictionary<uint, Dictionary<uint, float>> adjacence = result.Link(result.Resolve());
			uint from = (uint)rng.Next(adjacence.Count);
			uint to = (uint)(rng.Next(Math.Max(adjacence.Count - inputCount, outputCount)) + inputCount);
			uint i = 0;
			foreach(KeyValuePair<uint, Dictionary<uint, float>> node in adjacence)
			{
				if(from == i)
				{
					from = node.Key;
					break;
				}
				i++;
			}
			i = 0;
			foreach(KeyValuePair<uint, Dictionary<uint, float>> node in adjacence)
			{
				if(to == i)
				{
					to = node.Key;
					break;
				}
				i++;
			}
			result.contents[Genome.Pair(from, to)] = (float)(rng.NextDouble()*2-1);
			return result;
		}
		
		static Genome AddNode(Genome subject, ref Dictionary<uint, uint> sList)
		{
			Genome result = subject.Copy();
			int target = rng.Next(result.contents.Count);
			int i = 0;
			uint[] oldLink;
			foreach(KeyValuePair<uint, float> gene in result.contents)
			{
				if(i == target)
				{
					if(!sList.ContainsKey(gene.Key))
					{
						sList.Add(gene.Key, nodeCounter);
						nodeCounter++;
					}
					Console.WriteLine("Adding node {0}", sList[gene.Key]);
					oldLink = Genome.PairInverse(gene.Key);
					//Create new link from selected gene's from, to new node
					result.contents[Genome.Pair(oldLink[0], sList[gene.Key])] = (float)(rng.NextDouble()*2-1);
					//Create new link from new node, to selected gene's to
					result.contents[Genome.Pair(sList[gene.Key], oldLink[1])] = (float)(rng.NextDouble()*2-1);
					//Disable selected gene
					result.contents.Remove(gene.Key);
					break;
				}
				i++;
			}
			return result;
		}
		
		static double CompatDistance(Genome subject1, Genome subject2)
		{
			float w = 0;
			int matches = 0;
			int misses = 0;
			int n;
			if(subject1.contents.Count > subject2.contents.Count)
			{
				n = subject1.contents.Count;
			}else
			{
				n = subject2.contents.Count;
			}
			foreach(KeyValuePair<uint, float> gene in subject1.contents)
			{
				if(subject2.contents.ContainsKey(gene.Key))
				{
					matches++;
					w += gene.Value - subject2.contents[gene.Key];
					continue;
				}
				misses++;
			}
			foreach(KeyValuePair<uint, float> gene in subject2.contents)
			{
				if(!subject1.contents.ContainsKey(gene.Key))
				{
					misses++;
				}
			}
			
			w /= matches;
			return (misses * c1 / n) + c2 * w;
		}
		
		static void Speciate()
		{
			bool found;
			species.Clear();
			for(int i = 0; i < popSize; i++)
			{
				found = false;
				for(int j = 0; j < species.Count; j++)
				{
					if(speciesMarkers.Count == 0)
					{
						break;
					}
					if(CompatDistance(population[i], speciesMarkers[j]) < dt)
					{
						found = true;
						species[j].Add(i);
						break;
					}
				}
				if(!found)
				{
					species.Add(new List<int>());
					species[species.Count-1].Add(i);
					speciesMarkers.Add(population[i]);
				}
			}
			speciesMarkers.Clear();
			for(int i = 0; i < species.Count; i++)
			{
				speciesMarkers.Add(population[species[i][0]]);
			}
		}
	}
}
