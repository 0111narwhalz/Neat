using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
	class Network
	{
		public Dictionary<uint, Neuron> neurons;
		int inputCount;
		int outputCount;
		
		public Network(int input, int output)
		{
			inputCount = input;
			outputCount = output;
			neurons = new Dictionary<uint, Neuron>();
		}
		
		public void SetInputs(float[] inputs)
		{
			if(inputs.Length < inputCount)
			{
				return;
			}
			for(uint i = 0; i < inputCount; i++)
			{
				neurons[i].state = inputs[i];
			}
		}
		
		public float[] GetOutputs()
		{
			float[] outputs = new float[outputCount];
			for(uint i = 0; i < outputCount; i++)
			{
				outputs[i] = neurons[(uint)inputCount + i].state;
			}
			return outputs;
		}
		
		public void FlushStates()
		{
			foreach(KeyValuePair<uint, Neuron> node in neurons)
			{
				node.Value.state = 0;
			}
		}
		
		public void Iterate()
		{
			Dictionary<uint, float> newState = new Dictionary<uint, float>();
			foreach(KeyValuePair<uint, Neuron> node in neurons)
			{
				float aggregate = 0;
				for(int i = 0; i < node.Value.from.Length; i++)
				{
					aggregate += neurons[node.Value.from[i]].state * node.Value.weight[i];
				}
				newState.Add(node.Key, Neuron.Activate(aggregate));
			}
			foreach(KeyValuePair<uint, float> state in newState)
			{
				neurons[state.Key].state = state.Value;
			}
		}
	}
}
