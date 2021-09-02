﻿using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ObservableCollections.Internal
{
    internal ref struct ResizableArray<T>
    {
        T[]? array;
        int count;

        public ReadOnlySpan<T> Span => array.AsSpan(0, count);

        public ResizableArray(int initialCapacity)
        {
            array = ArrayPool<T>.Shared.Rent(initialCapacity);
            count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (array == null) Throw();
            if (array.Length == count)
            {
                EnsureCapacity();
            }
            array[count++] = item;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void EnsureCapacity()
        {
            var newArray = array.AsSpan().ToArray();
            ArrayPool<T>.Shared.Return(array!, RuntimeHelpersEx.IsReferenceOrContainsReferences<T>());
            array = newArray;
        }

        public void Dispose()
        {
            if (array != null)
            {
                ArrayPool<T>.Shared.Return(array, RuntimeHelpersEx.IsReferenceOrContainsReferences<T>());
                array = null;
            }
        }

        [DoesNotReturn]
        void Throw()
        {
            throw new ObjectDisposedException("ResizableArray");
        }
    }
}
