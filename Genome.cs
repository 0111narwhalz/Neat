using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
	class Genome
	{
		public Dictionary<uint, float> contents;
		int inputCount;
		int outputCount;
		public float fitness;
		
		public Genome(int inputCount, int outputCount)
		{
			this.inputCount = inputCount;
			this.outputCount = outputCount;
			contents = new Dictionary<uint, float>();
		}
		
		public static uint Pair(uint from, uint to)
		=> (from + to) * (from + to + 1) / 2 + to;
		
		public static uint[] PairInverse(uint pairIndex)
		{
			uint[] output = new uint[2];
			uint w = (uint)(Math.Sqrt(8*pairIndex + 1) - 1)/2;
			uint t = (w * w + w) / 2;
			output[1] = pairIndex - t;
			output[0] = w - output[1];
			return output;
		}
		
		public Network Compile()
		=> Assemble(Link(Resolve()));
		
		public Tuple<uint, uint, float>[] Resolve()
		{
			//Console.WriteLine("Resolving genome into incidence table");
			Tuple<uint, uint, float>[] incidence = new Tuple<uint, uint, float>[contents.Count];
			int i = 0;
			foreach(KeyValuePair<uint, float> gene in contents)
			{
				uint[] pair = PairInverse(gene.Key);
				incidence[i] = new Tuple<uint, uint, float>(pair[0], pair[1], gene.Value);
				i++;
			}
			
			//PrintTable(incidence);
			
			return incidence;
		}
		
		public Dictionary<uint, Dictionary<uint, float>> Link(Tuple<uint, uint, float>[] incidence)
		{
			//Console.WriteLine("Linking incidence table into adjacence table");
			Dictionary<uint, Dictionary<uint, float>> adjacence = new Dictionary<uint, Dictionary<uint, float>>();
			for(int i = 0; i < incidence.Length; i++)
			{
				if(!adjacence.ContainsKey(incidence[i].Item2))
				{
					adjacence.Add(incidence[i].Item2, new Dictionary<uint, float>());
				}
				adjacence[incidence[i].Item2].Add(incidence[i].Item1, incidence[i].Item3);
			}
			
			//PrintTable(adjacence);
			
			return adjacence;
		}
		
		public Network Assemble(Dictionary<uint, Dictionary<uint, float>> adjacence)
		{
			//Console.WriteLine("Assembling adjacence table into network");
			//Console.WriteLine("Creating empty network");
			Network output = new Network(inputCount, outputCount);
			//Console.WriteLine("Creating neurons");
			foreach(KeyValuePair<uint, Dictionary<uint, float>> node in adjacence)
			{
				//Console.WriteLine(" NodeID = {0}", node.Key);
				uint[] from = new uint[node.Value.Count];
				float[] weight = new float[node.Value.Count];
				int i = 0;
				//Console.WriteLine(" Creating edges");
				foreach(KeyValuePair<uint, float> edge in node.Value)
				{
					//Console.WriteLine("  Creating edge from {0}", edge.Key);
					from[i] = edge.Key;
					weight[i] = edge.Value;
					i++;
				}
				//Console.WriteLine(" Adding node to network");
				output.neurons.Add(node.Key, new Neuron(from, weight));
			}
			//Console.WriteLine("Creating unlinked input neurons");
			for(uint i = 0; i < inputCount; i++)
			{
				if(!output.neurons.ContainsKey(i))
				{
					//Console.WriteLine(" Creating input: nodeID = {0}", i);
					output.neurons.Add(i, new Neuron(new uint[0], new float[0]));
				}
			}
			//Console.WriteLine("Network assembly complete");
			return output;
		}
		
		public Genome Copy()
		{
			Genome copy = new Genome(inputCount, outputCount);
			foreach(KeyValuePair<uint, float> gene in contents)
			{
				copy.contents.Add(gene.Key, gene.Value);
			}
			return copy;
		}
		
		public override string ToString()
		{
			string output = "";
			foreach(KeyValuePair<uint, float> gene in contents)
			{
				output += string.Format("{0},{1};", gene.Key, gene.Value);
			}
			return output;
		}
		
		static void PrintTable(Tuple<uint, uint, float>[] incidence)
		{
			Console.WriteLine("Printing incidence table");
			for(int i = 0; i < incidence.Length; i++)
			{
				Console.WriteLine(" {0}→{1},{2}", incidence[i].Item1, incidence[i].Item2, incidence[i].Item3);
			}
		}
		
		static void PrintTable(Dictionary<uint, Dictionary<uint, float>> adjacence)
		{
			Console.WriteLine("Printing adjacence table");
			foreach(KeyValuePair<uint, Dictionary<uint, float>> node in adjacence)
			{
				Console.WriteLine(" {0}←", node.Key);
				foreach(KeyValuePair<uint, float> edge in node.Value)
				{
					Console.WriteLine("   {0},{1}", edge.Key, edge.Value);
				}
			}
		}
	}
}
