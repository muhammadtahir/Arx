//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Extensions;
using System;
using System.Collections.Generic;

public static class IEnumerableExtensions
{
    public static IEnumerable<Tuple<T, T>> ToPairs<T>(this IEnumerable<T> source)
    {
        if (source != null)
        {
            var previous = default(T);
            
            using (var it = source.GetEnumerator())
            {
                if (it.MoveNext())
                    previous = it.Current;
                
                while (it.MoveNext())
                    yield return new Tuple<T, T>(previous, previous = it.Current);
            }
        }
    }
}

