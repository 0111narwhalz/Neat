using System;

namespace NeuralNetwork
{
	class Neuron
	{
		public uint[] from;
		public float[] weight;
		public float state;
		
		public Neuron(uint[] from, float[] weight)
		{
			this.from = from;
			this.weight = weight;
			state = 0;
		}
		
		public static float Activate(float stimulus)
		{
			/* Hyperbolic tangent
			float ex = (float)Math.Exp(stimulus);
			float enegx = 1 / ex;
			return (ex - enegx) / (ex + enegx);
			//*/
			
			//* Modified sigmoid, as described in NEAT paper
			float ex = (float)Math.Exp(stimulus * -4.9f);
			return 1 / (1 + ex);
			//*/
		}
	}
}
