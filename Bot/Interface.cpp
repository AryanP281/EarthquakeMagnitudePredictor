/*******************Preprocesor Directives*****************/
#include "stdafx.h"
#include "Interface.h"

/*******************Functions*****************/
void InitializeBot(short numOfInputs)
{
	botInputCtr = numOfInputs;
	bot = Neuron(numOfInputs);
}

void BatchTraining(float* input, int batchSize,float* expected, short epoch)
{
	std::vector<std::vector<float>> inputs;
	inputs.push_back(std::vector<float>());
	for (int a = 0; a < (batchSize * botInputCtr); ++a)
	{
		if (a % botInputCtr == 0 && a != 0)
		{
			inputs.push_back(std::vector<float>());
			inputs[inputs.size() - 1].push_back(input[a]);
			continue;
		}
		inputs[inputs.size() - 1].push_back(input[a]);
	}

	std::vector<float> ts;
	for (int a = 0; a < batchSize; ++a)
	{
		ts.push_back(expected[a]);
	}

	bot.Train(inputs, ts, epoch);
}

float ExampleTraining(float* input, float expected)
{
	std::vector<float> inputs;
	for (short a = 0; a < botInputCtr; ++a)
	{
		inputs.push_back(input[a]);
	}

	return bot.Train(inputs, expected);
}

float GetOutput(float* input)
{
	std::vector<float> inputs;
	for (short a = 0; a < botInputCtr; ++a)
	{
		inputs.push_back(input[a]);
	}

	return bot.Output(inputs);
}
