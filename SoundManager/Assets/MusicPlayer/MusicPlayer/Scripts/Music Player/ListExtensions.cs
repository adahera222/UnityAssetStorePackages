using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
	public static void ShuffleContents<T>(this List<T> list)
	{
		var rand = new System.Random();
		for (int i = list.Count; i > 1; i--)
		{
			int position = rand.Next(i);
			var item = list[i - 1];
			list[i - 1] = list[position];
			list[position] = item;
		}	
	}
}