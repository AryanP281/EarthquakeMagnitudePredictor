#pragma once
/*******************Preprocessor Directives*****************/
#include "Neuron.h"

/*******************Functions*****************/
Neuron bot;
short botInputCtr;

/*******************Dll Functions*****************/
extern "C"
{
	_declspec(dllexport) void InitializeBot(short numOfInputs);
	_declspec(dllexport) void BatchTraining(float* input, int batchSize,float* expected, short epoch);
	_declspec(dllexport) float ExampleTraining(float* input, float expected);
	_declspec(dllexport) float GetOutput(float* input);
}
