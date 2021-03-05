using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Game.Mechanics
{
    public static class CoreNativeArray
    {
        public static ref T get_ref<T>(this NativeArray<T> array, int index)
            where T : struct
        {
            // You might want to validate the index first, as the unsafe method won't do that.
            if (index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            return ref array.nocheck_ref(index);
        }
        
        public static unsafe ref T nocheck_ref<T>(this NativeArray<T> array, int index)
            where T : struct =>
            ref UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafePtr(), index);
    }
}