// CppAMP.cpp : 定義 DLL 應用程式的匯出函式。
//

#include "stdafx.h"
#include "amp.h"

using namespace concurrency;

extern "C" __declspec(dllexport) void __stdcall GPUAdd(int data[], int dataSize, int additions[], int additionsCount)
{
	array_view<int> avData(dataSize, data);
	array_view<int> avAdditions(additionsCount, additions);
	parallel_for_each(avData.extent, [=](index<1> idx) restrict(amp)
	{
		for (int i = 0; i < additionsCount; i++)
		{
			avData[idx] += avAdditions[i];
		}
	});
	avData.synchronize();
}
