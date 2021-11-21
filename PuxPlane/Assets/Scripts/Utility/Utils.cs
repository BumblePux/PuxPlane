using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void SetToNext<T>(T[] array, ref T item, int direction)
    {
        int currentIndex = Array.IndexOf(array, item);

        if (direction == 0) // Left
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = array.Length - 1;
            }
        }
        else // Right
        {
            currentIndex++;
            if (currentIndex > array.Length - 1)
            {
                currentIndex = 0;
            }
        }

        item = array[currentIndex];
    }

    /// <summary>
    /// Maps a value from one range to another.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="fromLow">The lower bound of the value's current range.</param>
    /// <param name="fromHigh">The upper bound of the value's current range.</param>
    /// <param name="toLow">the lower bound of the value's target range.</param>
    /// <param name="toHigh">the upper bound of the value's target range.</param>
    /// <returns>The mapped value.</returns>
    public static float Map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return toLow + (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow);
    }
}
