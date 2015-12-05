# SIMD
* .NET TPL(Task Parallel Library), 
* .NET hardware SIMD acceleration,
* C++ AMP

---
To use TPL, target framework must greater then 4.0.

To enable CPU-SIMD instruction, target framework must greater then 4.6, install Nuget package System.Numerics.Vectors

`PM> Install-Package System.Numerics.Vectors`, and run at x64 release build.

C++ AMP need `d3dcompiler_47.dll`, copy it from Windows SDK (above 8) OR DirectX SDK (Jun2010).

---
Test environment
* CPU: Intel Xeon E3-1230v2 (AVX only, no AVX2) with 4 physical core, HT enable
* GPU: Nvidia GTX580 3GB (driver 359.00)
* Windows 7 SP1
* Visual Studio 2015 Update 1

---
program output (time is approximate)

>Test data generated in 00:00:03.5

>Single thread basic add

>Single thread basic add in 00:02:15

>Threads count: 8

>Multi threads basic add

>Multi threads basic add in 00:00:21.5

>Current runtime: x64

>CPU SIMD HardwareAccelerated: True

>SIMD int batch size: 4

>Single thread SIMD add

>Single thread SIMD add in 00:00:15.1

>Multi threads SIMD add

>Multi threads SIMD add in 00:00:03.8

>GPU SIMD add

>GPU SIMD add in 00:00:01.8

