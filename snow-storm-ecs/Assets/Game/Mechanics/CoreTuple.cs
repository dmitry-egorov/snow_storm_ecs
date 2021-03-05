using System;

namespace Game.Mechanics
{
    public static class CoreTuple
    {
        public static T1 _1<T1, T2>(this (T1,T2) t) => t.Item1;
        public static T2 _2<T1, T2>(this (T1,T2) t) => t.Item2;
        
        public static T1 _1<T1, T2, T3>(this (T1,T2,T3) t) => t.Item1;
        public static T2 _2<T1, T2, T3>(this (T1,T2,T3) t) => t.Item2;
        public static T3 _3<T1, T2, T3>(this (T1,T2,T3) t) => t.Item3;
        
        public static T1 _1<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => t.Item1;
        public static T2 _2<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => t.Item2;
        public static T3 _3<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => t.Item3;
        public static T4 _4<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => t.Item4;
        
        public static (T1,T2) _12<T1, T2, T3>(this (T1,T2,T3) t) => (t.Item1, t.Item2);
        public static (T1, T3) _13<T1, T2, T3>(this (T1,T2,T3) t) => (t.Item1, t.Item3);
        public static (T2, T3) _23<T1, T2, T3>(this (T1,T2,T3) t) => (t.Item2, t.Item3);
        
        public static (T1,T2) _12<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => (t.Item1, t.Item2);
        public static (T2,T3) _23<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => (t.Item2, t.Item3);
        public static (T3,T4) _34<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => (t.Item3, t.Item4);
        public static (T1,T2,T3) _123<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => (t.Item1, t.Item2, t.Item3);
        public static (T1, T2, T4) _124<T1, T2, T3, T4>(this (T1,T2,T3,T4) t) => (t.Item1, t.Item2, t.Item4);

    }
}