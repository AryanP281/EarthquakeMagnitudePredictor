
/*******************Preprocessor Directives*****************/
#include "stdafx.h"
#include<random>
#include <time.h>
#include "Neuron.h"

/*******************Constructors And Destructors*****************/
Neuron::Neuron() : BIAS(1.0f)
{
}

Neuron::Neuron(short numOfInputs) : BIAS(1.0f)
{
	srand(time(NULL));
	//Initializing the weights
	for (short a = 0; a < numOfInputs + 1; ++a) //One extra weight for the bias
	{
		weights.push_back((float)rand() / (float)RAND_MAX);
	}

	//Initializing the learning rate
	this->learningRate = 0.001f;
}

Neuron::~Neuron()
{
}

/*******************Methods*****************/
void Neuron::Train(const std::vector<std::vector<float>>& inputs, const std::vector<float>& expected, short epoch)
{
	std::vector<std::pair<float, float>> mses;

	for (; learningRate < 1; learningRate += 0.001f)
	{
		float mse = 0.0f;
		float lastMse = 0.0f;

		while (true)
		{
			std::vector<float> meanErrors;
			for (int x = 0; x < weights.size(); ++x)
			{
				meanErrors.push_back(0.0F);
			}

			for (int b = 0; b < inputs.size(); ++b)
			{
				float guess = Output(inputs[b]);
				float t = expected[b];
				float error = t - guess;
				mse += error * error;

				meanErrors[0] += error * BIAS;
				for (int b2 = 0; b2 < inputs[b].size(); ++b2)
				{
					meanErrors[b2 + 1] += error * inputs[b][b2];
				}
			}

			for (int c = 0; c < weights.size(); ++c)
			{
				meanErrors[c] /= inputs.size();
			}

			//Adjusting the weights
			for (int d = 0; d < weights.size(); ++d)
			{
				weights[d] += learningRate * meanErrors[d];
			}

			mse /= 2 * inputs.size();
			if (mse == lastMse)
				break;
			lastMse = mse;
		}
		mses.push_back(std::pair<float, float>(learningRate, mse));
	}

	//Selecting an appropriate learning weight
	int lowestMseIndex = 0;
	for (int a = 0; a < mses.size(); ++a)
	{
		if (mses[lowestMseIndex].second > mses[a].second)
			lowestMseIndex = a;
	}
	learningRate = mses[lowestMseIndex].first;
}

float Neuron::Train(const std::vector<float>& inputs, float expected)
{
	float guess = Output(inputs);
	float error = expected - guess;

	//Adjusting the weights
	float errorByWeight = error * -BIAS;
	weights[0] -= learningRate * errorByWeight;
	for (int a = 1; a < weights.size(); ++a)
	{
		errorByWeight = error * -inputs[a - 1];
		weights[a] -= learningRate * errorByWeight;
	}

	return guess;
}

float Neuron::Output(const std::vector<float>& inputs)
{
	float sum = weights[0] * BIAS;
	for (int a = 1; a < weights.size(); ++a)
	{
		sum += weights[a] * inputs[a - 1];
	}

	return sum;
}

/*******************Operators*****************/
void Neuron::operator=(const Neuron& n)
{
	this->weights = n.weights;
	this->learningRate = n.learningRate;
}
