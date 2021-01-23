﻿//
//  Emoji.Wpf — Emoji support for WPF
//
//  Copyright © 2017—2021 Sam Hocevar <sam@hocevar.net>
//
//  This library is free software. It comes without any warranty, to
//  the extent permitted by applicable law. You can redistribute it
//  and/or modify it under the terms of the Do What the Fuck You Want
//  to Public License, Version 2, as published by the WTFPL Task Force.
//  See http://www.wtfpl.net/ for more details.
//

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Emoji.Wpf
{
    internal static class Extensions
    {
        /// <summary>
        /// Call a function on each element of a sequence
        /// </summary>
        internal static void ForAll<T>(this IEnumerable<T> elements, Action<T> fn)
        {
            foreach (var e in elements)
                fn(e);
        }

        /// <summary>
        /// Partition a sequence into chunks of fixed size
        /// </summary>
        internal static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> elements, int size)
            => new ChunkHelper<T>(elements, size);

        private sealed class ChunkHelper<T> : IEnumerable<IEnumerable<T>>
        {
            internal ChunkHelper(IEnumerable<T> elements, int size)
            {
                m_elements = elements;
                m_size = size;
            }

            public IEnumerator<IEnumerable<T>> GetEnumerator()
            {
                using (var enumerator = m_elements.GetEnumerator())
                {
                    m_has_more = enumerator.MoveNext();
                    while (m_has_more)
                        yield return GetNextBatch(enumerator).ToList();
                }
            }

            private IEnumerable<T> GetNextBatch(IEnumerator<T> enumerator)
            {
                for (int i = 0; i < m_size; ++i)
                {
                    yield return enumerator.Current;
                    m_has_more = enumerator.MoveNext();
                    if (!m_has_more)
                        yield break;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            private readonly IEnumerable<T> m_elements;
            private readonly int m_size;
            private bool m_has_more;
        }

        /// <summary>
        /// Convert a sequence into a hashset
        /// </summary>
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> elements,
                                                IEqualityComparer<T> comparer = null)
            => new HashSet<T>(elements, comparer);
    }
}
