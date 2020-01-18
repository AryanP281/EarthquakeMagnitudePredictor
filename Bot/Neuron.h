#pragma once

/*******************Preprocessor Directives*****************/
#include <vector>

/*******************Class*****************/
class Neuron
{
private:
	const float BIAS;
	float learningRate;
	std::vector<float> weights;

public:
	Neuron();
	Neuron(short numOfInputs);
	~Neuron();

	void Train(const std::vector<std::vector<float>>& inputs, const std::vector<float>& expected, short epoch = 4);
	float Train(const std::vector<float>& inputs, float expected);
	float Output(const std::vector<float>& inputs);

	void operator=(const Neuron& n);
};

