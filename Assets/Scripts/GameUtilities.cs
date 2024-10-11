using UnityEngine;

public static class GameUtilities
{
    // Roll dice method accessible from anywhere
    public static int RollDice(int rolls, int sides, int discard = 0)
    {
        if (discard >= rolls)
        {
            Debug.LogError("Cannot discard more rolls than are being made!");
            return 0; // Or handle this error as you see fit
        }

        // Roll the dice and store the results
        int[] results = new int[rolls];
        for (int i = 0; i < rolls; i++)
        {
            results[i] = Random.Range(1, sides + 1);  // Rolls between 1 and sides
        }

        // Sort the results and discard the lowest 'discard' number of rolls
        System.Array.Sort(results);
        int sum = 0;
        for (int i = discard; i < rolls; i++)
        {
            sum += results[i];  // Sum only the non-discarded results
        }

        return sum;
    }
}