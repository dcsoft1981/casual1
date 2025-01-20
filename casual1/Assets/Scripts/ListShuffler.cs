using System;
using System.Collections.Generic;

public class ListShuffler
{
	public static void ShuffleList(List<int> list, int seed)
	{
		Random random = new Random(seed);
		int n = list.Count;
		for (int i = n - 1; i > 0; i--)
		{
			int j = random.Next(0, i + 1);
			int temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
	}
}
