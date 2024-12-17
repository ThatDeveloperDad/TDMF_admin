using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ThatDeveloperDad.iFX.CollectionUtilities;

public class Reconciler
{
    /// <summary>
    /// Returns a subset of the left collection that is not present in the right collection.
    /// </summary>
    /// <typeparam name="T">The Type of the members of the collection.</typeparam>
    /// <param name="left">The list of Values that we expect to find.</param>
    /// <param name="right">The List of Values that we are searching within.</param>
    public static IEnumerable<T> GetMissingMembers<T>(IEnumerable<T> left,
        IEnumerable<T> right)
    {
        List<T> missingMembers = new();

        missingMembers = left.Except(right).ToList();

        return missingMembers;
    }
}
