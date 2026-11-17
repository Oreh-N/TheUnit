using System;
using UnityEngine;
using UnityEngine.Profiling;

class MemoryMonitor : MonoBehaviour
{
	const long MemoryLimitBytes = 1024 * 1024 * 1500L;  // 1.5 GB
	bool _quit = false;
	float _checkTime = 0.5f;
	float _timer = 0;


	private void Update()
	{
		_timer += Time.deltaTime;

		if (_timer >= _checkTime)
		{
			Check();
			_timer = 0;
		}
	}

	void Check()
	{
		long usedMemory = GC.GetTotalMemory(false);
		long totalAllocated = Profiler.GetTotalAllocatedMemoryLong();

		if (usedMemory > MemoryLimitBytes)
		{   // Objects overflow
			Debug.Log("Heap has reached memory limit. Exiting.");
			_quit = true;
		}
		if (totalAllocated > MemoryLimitBytes)
		{
			Debug.Log($"Memory limit exceeded: {totalAllocated / (1024 * 1024)} MB. Exiting.");
			_quit = true;
		}

		if (_quit) ForceClose();
	}

	void ForceClose()
	{
		Application.Quit();
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}
