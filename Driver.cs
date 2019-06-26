using System;
using System.Collections.Generic;

using NeuralNetwork;

class Driver
{
	public static void Main()
	{
		Console.WriteLine("Initializing");
		Neat.Initialize();
		Console.WriteLine("Initialization Complete");
		for(;;)
		{
			Neat.DoGeneration();
			Console.Read();
		}
		
		/*
		Console.WriteLine("Creating new genome");
		Genome genome01 = new Genome(3, 5);
		Console.WriteLine("Populating genome");
		uint[] geneList = new uint[10]{9,18,14,19,20,26,27,42,43,52};
		for(int i = 0; i < geneList.Length; i++)
		{
			genome01.contents.Add(geneList[i], 1);
		}
		Console.WriteLine(genome01);
		Console.WriteLine("Compiling network");
		Network net01 = genome01.Compile();
		Console.WriteLine("Network compilation complete");
		
		Console.WriteLine("Inspecting network");
		foreach(KeyValuePair<uint, Neuron> node in net01.neurons)
		{
			Console.WriteLine(" NodeID={0}", node.Key);
			Console.WriteLine(" Edges={0}", node.Value.from.Length);
			for(int i = 0; i < node.Value.from.Length; i++)
			{
				Console.WriteLine("  Weight={0}", node.Value.weight[i]);
			}
		}
		
		Console.WriteLine("Beginning neuron test cases");
		float[][] testInput01 = new float[8][];
			testInput01[0] = new float[3]{0,0,0};
			testInput01[1] = new float[3]{0,0,1};
			testInput01[2] = new float[3]{0,1,0};
			testInput01[3] = new float[3]{0,1,1};
			testInput01[4] = new float[3]{1,0,0};
			testInput01[5] = new float[3]{1,0,1};
			testInput01[6] = new float[3]{1,1,0};
			testInput01[7] = new float[3]{1,1,1};
		for(int i = 0; i < testInput01.Length; i++)
		{
			//Console.WriteLine("Flushing network");
			//net01.FlushStates();
			Console.WriteLine("Setting test input {0}", i);
			net01.SetInputs(testInput01[i]);
			Console.WriteLine("Calculating network state");
			net01.Iterate();
			Console.WriteLine("Reading network output");
			float[] outputs = net01.GetOutputs();
			for(int j = 0; j < 5; j++)
			{
				Console.Write(",{0}", outputs[j]);
			}
			Console.WriteLine();
		}
		//*/
		
		/*
		uint from;
		uint to;
		for(;;)
		{
			Console.Write("From: ");
			from = uint.Parse(Console.ReadLine());
			Console.Write("To: ");
			to = uint.Parse(Console.ReadLine());
			Console.WriteLine("Pair Index: {0}", Genome.Pair(from, to));
		}
		//*/
	}
}
