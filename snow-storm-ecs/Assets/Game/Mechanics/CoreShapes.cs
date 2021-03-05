using System;

namespace Game.Mechanics
{
    public static class CoreShapes
    {
        [Serializable]
        public readonly struct s<T1, T2>
        {
            readonly T1 _1;
            readonly T2 _2;
            
            public T1 c1<T>() where T : T1 => _1;
            public T2 c2<T>() where T : T2 => _2;
            
            public s(T1 _1, T2 _2)
            {
                this._1 = _1;
                this._2 = _2;
            }
            
            public static implicit operator T1(s<T1, T2> e) => e._1;
            public static implicit operator T2(s<T1, T2> e) => e._2;

            public static implicit operator s<T2, T1>(s<T1, T2> e) => new s<T2, T1>(e._2, e._1);
        }
        
        [Serializable]
        public readonly struct s<T1, T2, T3>
        {
            readonly T1 _1;
            readonly T2 _2;
            readonly T3 _3;
            
            public T1 c1<T>() where T : T1 => _1;
            public T2 c2<T>() where T : T2 => _2;
            public T3 c3<T>() where T : T3 => _3;

            public s(T1 _1, T2 _2, T3 _3)
            {
                this._1 = _1;
                this._2 = _2;
                this._3 = _3;
            }
            
            public static implicit operator T1(s<T1, T2, T3> e) => e._1;
            public static implicit operator T2(s<T1, T2, T3> e) => e._2;
            public static implicit operator T3(s<T1, T2, T3> e) => e._3;

            public static implicit operator s<T1, T2>(s<T1, T2, T3> e) => new s<T1, T2>(e._1, e._2);
            public static implicit operator s<T1, T3>(s<T1, T2, T3> e) => new s<T1, T3>(e._1, e._3);
            public static implicit operator s<T2, T1>(s<T1, T2, T3> e) => new s<T2, T1>(e._2, e._1);
            public static implicit operator s<T2, T3>(s<T1, T2, T3> e) => new s<T2, T3>(e._2, e._3);
            public static implicit operator s<T3, T1>(s<T1, T2, T3> e) => new s<T3, T1>(e._3, e._1);
            public static implicit operator s<T3, T2>(s<T1, T2, T3> e) => new s<T3, T2>(e._3, e._2);
            
            public static implicit operator s<T1, T3, T2>(s<T1, T2, T3> e) => new s<T1, T3, T2>(e._1, e._3, e._2);
            public static implicit operator s<T2, T1, T3>(s<T1, T2, T3> e) => new s<T2, T1, T3>(e._2, e._1, e._3);
            public static implicit operator s<T2, T3, T1>(s<T1, T2, T3> e) => new s<T2, T3, T1>(e._2, e._3, e._1);
            public static implicit operator s<T3, T1, T2>(s<T1, T2, T3> e) => new s<T3, T1, T2>(e._3, e._1, e._2);
            public static implicit operator s<T3, T2, T1>(s<T1, T2, T3> e) => new s<T3, T2, T1>(e._3, e._2, e._1);
        }
        
        [Serializable]
        public readonly struct s<T1, T2, T3, T4>
        {
            readonly T1 _1;
            readonly T2 _2;
            readonly T3 _3;
            readonly T4 _4;
            
            public T1 c1<T>() where T : T1 => _1;
            public T2 c2<T>() where T : T2 => _2;
            public T3 c3<T>() where T : T3 => _3;
            public T4 c4<T>() where T : T4 => _4;

            public s(T1 _1, T2 _2, T3 _3, T4 _4)
            {
                this._1 = _1;
                this._2 = _2;
                this._3 = _3;
                this._4 = _4;
            }
            
            public static implicit operator T1(s<T1, T2, T3, T4> e) => e._1;
            public static implicit operator T2(s<T1, T2, T3, T4> e) => e._2;
            public static implicit operator T3(s<T1, T2, T3, T4> e) => e._3;
            public static implicit operator T4(s<T1, T2, T3, T4> e) => e._4;
            
            public static implicit operator s<T1, T2>(s<T1, T2, T3, T4> e) => new s<T1, T2>(e._1, e._2);
            public static implicit operator s<T1, T3>(s<T1, T2, T3, T4> e) => new s<T1, T3>(e._1, e._3);
            public static implicit operator s<T1, T4>(s<T1, T2, T3, T4> e) => new s<T1, T4>(e._1, e._4);
            public static implicit operator s<T2, T1>(s<T1, T2, T3, T4> e) => new s<T2, T1>(e._2, e._1);
            public static implicit operator s<T2, T3>(s<T1, T2, T3, T4> e) => new s<T2, T3>(e._2, e._3);
            public static implicit operator s<T2, T4>(s<T1, T2, T3, T4> e) => new s<T2, T4>(e._2, e._4);
            public static implicit operator s<T3, T1>(s<T1, T2, T3, T4> e) => new s<T3, T1>(e._3, e._1);
            public static implicit operator s<T3, T2>(s<T1, T2, T3, T4> e) => new s<T3, T2>(e._3, e._2);
            public static implicit operator s<T3, T4>(s<T1, T2, T3, T4> e) => new s<T3, T4>(e._3, e._4);
            public static implicit operator s<T4, T1>(s<T1, T2, T3, T4> e) => new s<T4, T1>(e._4, e._1);
            public static implicit operator s<T4, T2>(s<T1, T2, T3, T4> e) => new s<T4, T2>(e._4, e._2);
            public static implicit operator s<T4, T3>(s<T1, T2, T3, T4> e) => new s<T4, T3>(e._4, e._3);
            
            public static implicit operator s<T1, T2, T3>(s<T1, T2, T3, T4> e) => new s<T1, T2, T3>(e._1, e._2, e._3);
            public static implicit operator s<T1, T2, T4>(s<T1, T2, T3, T4> e) => new s<T1, T2, T4>(e._1, e._2, e._4);
            public static implicit operator s<T1, T3, T2>(s<T1, T2, T3, T4> e) => new s<T1, T3, T2>(e._1, e._3, e._2);
            public static implicit operator s<T1, T3, T4>(s<T1, T2, T3, T4> e) => new s<T1, T3, T4>(e._1, e._3, e._4);
            public static implicit operator s<T1, T4, T2>(s<T1, T2, T3, T4> e) => new s<T1, T4, T2>(e._1, e._4, e._2);
            public static implicit operator s<T1, T4, T3>(s<T1, T2, T3, T4> e) => new s<T1, T4, T3>(e._1, e._4, e._3);
            public static implicit operator s<T2, T1, T3>(s<T1, T2, T3, T4> e) => new s<T2, T1, T3>(e._2, e._1, e._3);
            public static implicit operator s<T2, T1, T4>(s<T1, T2, T3, T4> e) => new s<T2, T1, T4>(e._2, e._1, e._4);
            public static implicit operator s<T2, T3, T1>(s<T1, T2, T3, T4> e) => new s<T2, T3, T1>(e._2, e._3, e._1);
            public static implicit operator s<T2, T3, T4>(s<T1, T2, T3, T4> e) => new s<T2, T3, T4>(e._2, e._3, e._4);
            public static implicit operator s<T2, T4, T1>(s<T1, T2, T3, T4> e) => new s<T2, T4, T1>(e._2, e._4, e._1);
            public static implicit operator s<T2, T4, T3>(s<T1, T2, T3, T4> e) => new s<T2, T4, T3>(e._2, e._4, e._3);
            public static implicit operator s<T3, T1, T2>(s<T1, T2, T3, T4> e) => new s<T3, T1, T2>(e._3, e._1, e._2);
            public static implicit operator s<T3, T1, T4>(s<T1, T2, T3, T4> e) => new s<T3, T1, T4>(e._3, e._1, e._4);
            public static implicit operator s<T3, T2, T1>(s<T1, T2, T3, T4> e) => new s<T3, T2, T1>(e._3, e._2, e._1);
            public static implicit operator s<T3, T2, T4>(s<T1, T2, T3, T4> e) => new s<T3, T2, T4>(e._3, e._2, e._4);
            public static implicit operator s<T3, T4, T1>(s<T1, T2, T3, T4> e) => new s<T3, T4, T1>(e._3, e._4, e._1);
            public static implicit operator s<T3, T4, T2>(s<T1, T2, T3, T4> e) => new s<T3, T4, T2>(e._3, e._4, e._2);
            public static implicit operator s<T4, T1, T2>(s<T1, T2, T3, T4> e) => new s<T4, T1, T2>(e._4, e._1, e._2);
            public static implicit operator s<T4, T1, T3>(s<T1, T2, T3, T4> e) => new s<T4, T1, T3>(e._4, e._1, e._3);
            public static implicit operator s<T4, T2, T1>(s<T1, T2, T3, T4> e) => new s<T4, T2, T1>(e._4, e._2, e._1);
            public static implicit operator s<T4, T2, T3>(s<T1, T2, T3, T4> e) => new s<T4, T2, T3>(e._4, e._2, e._3);
            public static implicit operator s<T4, T3, T1>(s<T1, T2, T3, T4> e) => new s<T4, T3, T1>(e._4, e._3, e._1);
            public static implicit operator s<T4, T3, T2>(s<T1, T2, T3, T4> e) => new s<T4, T3, T2>(e._4, e._3, e._2);
            
            public static implicit operator s<T1, T2, T4, T3>(s<T1, T2, T3, T4> e) => new s<T1, T2, T4, T3>(e._1, e._2, e._4, e._3);
            public static implicit operator s<T1, T3, T2, T4>(s<T1, T2, T3, T4> e) => new s<T1, T3, T2, T4>(e._1, e._3, e._2, e._4);
            public static implicit operator s<T1, T3, T4, T2>(s<T1, T2, T3, T4> e) => new s<T1, T3, T4, T2>(e._1, e._3, e._4, e._2);
            public static implicit operator s<T1, T4, T2, T3>(s<T1, T2, T3, T4> e) => new s<T1, T4, T2, T3>(e._1, e._4, e._2, e._3);
            public static implicit operator s<T1, T4, T3, T2>(s<T1, T2, T3, T4> e) => new s<T1, T4, T3, T2>(e._1, e._4, e._3, e._2);
            public static implicit operator s<T2, T1, T3, T4>(s<T1, T2, T3, T4> e) => new s<T2, T1, T3, T4>(e._2, e._1, e._3, e._4);
            public static implicit operator s<T2, T1, T4, T3>(s<T1, T2, T3, T4> e) => new s<T2, T1, T4, T3>(e._2, e._1, e._4, e._3);
            public static implicit operator s<T2, T3, T1, T4>(s<T1, T2, T3, T4> e) => new s<T2, T3, T1, T4>(e._2, e._3, e._1, e._4);
            public static implicit operator s<T2, T3, T4, T1>(s<T1, T2, T3, T4> e) => new s<T2, T3, T4, T1>(e._2, e._3, e._4, e._1);
            public static implicit operator s<T2, T4, T1, T3>(s<T1, T2, T3, T4> e) => new s<T2, T4, T1, T3>(e._2, e._4, e._1, e._3);
            public static implicit operator s<T2, T4, T3, T1>(s<T1, T2, T3, T4> e) => new s<T2, T4, T3, T1>(e._2, e._4, e._3, e._1);
            public static implicit operator s<T3, T1, T2, T4>(s<T1, T2, T3, T4> e) => new s<T3, T1, T2, T4>(e._3, e._1, e._2, e._4);
            public static implicit operator s<T3, T1, T4, T2>(s<T1, T2, T3, T4> e) => new s<T3, T1, T4, T2>(e._3, e._1, e._4, e._2);
            public static implicit operator s<T3, T2, T1, T4>(s<T1, T2, T3, T4> e) => new s<T3, T2, T1, T4>(e._3, e._2, e._1, e._4);
            public static implicit operator s<T3, T2, T4, T1>(s<T1, T2, T3, T4> e) => new s<T3, T2, T4, T1>(e._3, e._2, e._4, e._1);
            public static implicit operator s<T3, T4, T1, T2>(s<T1, T2, T3, T4> e) => new s<T3, T4, T1, T2>(e._3, e._4, e._1, e._2);
            public static implicit operator s<T3, T4, T2, T1>(s<T1, T2, T3, T4> e) => new s<T3, T4, T2, T1>(e._3, e._4, e._2, e._1);
            public static implicit operator s<T4, T1, T2, T3>(s<T1, T2, T3, T4> e) => new s<T4, T1, T2, T3>(e._4, e._1, e._2, e._3);
            public static implicit operator s<T4, T1, T3, T2>(s<T1, T2, T3, T4> e) => new s<T4, T1, T3, T2>(e._4, e._1, e._3, e._2);
            public static implicit operator s<T4, T2, T1, T3>(s<T1, T2, T3, T4> e) => new s<T4, T2, T1, T3>(e._4, e._2, e._1, e._3);
            public static implicit operator s<T4, T2, T3, T1>(s<T1, T2, T3, T4> e) => new s<T4, T2, T3, T1>(e._4, e._2, e._3, e._1);
            public static implicit operator s<T4, T3, T1, T2>(s<T1, T2, T3, T4> e) => new s<T4, T3, T1, T2>(e._4, e._3, e._1, e._2);
            public static implicit operator s<T4, T3, T2, T1>(s<T1, T2, T3, T4> e) => new s<T4, T3, T2, T1>(e._4, e._3, e._2, e._1);
        }
        
        [Serializable]
        public readonly struct s<T1, T2, T3, T4, T5>
        {
            readonly T1 _1;
            readonly T2 _2;
            readonly T3 _3;
            readonly T4 _4;
            readonly T5 _5;
            
            public T1 c1<T>() where T : T1 => _1;
            public T2 c2<T>() where T : T2 => _2;
            public T3 c3<T>() where T : T3 => _3;
            public T4 c4<T>() where T : T4 => _4;
            public T5 c5<T>() where T : T5 => _5;

            public s(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5)
            {
                this._1 = _1;
                this._2 = _2;
                this._3 = _3;
                this._4 = _4;
                this._5 = _5;
            }
            
            public static implicit operator T1(s<T1, T2, T3, T4, T5> e) => e._1;
            public static implicit operator T2(s<T1, T2, T3, T4, T5> e) => e._2;
            public static implicit operator T3(s<T1, T2, T3, T4, T5> e) => e._3;
            public static implicit operator T4(s<T1, T2, T3, T4, T5> e) => e._4;
            public static implicit operator T5(s<T1, T2, T3, T4, T5> e) => e._5;
            
            public static implicit operator s<T1, T2>(s<T1, T2, T3, T4, T5> e) => new s<T1, T2>(e._1, e._2);
            public static implicit operator s<T1, T3>(s<T1, T2, T3, T4, T5> e) => new s<T1, T3>(e._1, e._3);
            public static implicit operator s<T1, T4>(s<T1, T2, T3, T4, T5> e) => new s<T1, T4>(e._1, e._4);
            public static implicit operator s<T2, T1>(s<T1, T2, T3, T4, T5> e) => new s<T2, T1>(e._2, e._1);
            public static implicit operator s<T2, T3>(s<T1, T2, T3, T4, T5> e) => new s<T2, T3>(e._2, e._3);
            public static implicit operator s<T2, T4>(s<T1, T2, T3, T4, T5> e) => new s<T2, T4>(e._2, e._4);
            public static implicit operator s<T3, T1>(s<T1, T2, T3, T4, T5> e) => new s<T3, T1>(e._3, e._1);
            public static implicit operator s<T3, T2>(s<T1, T2, T3, T4, T5> e) => new s<T3, T2>(e._3, e._2);
            public static implicit operator s<T3, T4>(s<T1, T2, T3, T4, T5> e) => new s<T3, T4>(e._3, e._4);
            public static implicit operator s<T4, T1>(s<T1, T2, T3, T4, T5> e) => new s<T4, T1>(e._4, e._1);
            public static implicit operator s<T4, T2>(s<T1, T2, T3, T4, T5> e) => new s<T4, T2>(e._4, e._2);
            public static implicit operator s<T4, T3>(s<T1, T2, T3, T4, T5> e) => new s<T4, T3>(e._4, e._3);
            
            public static implicit operator s<T1, T2, T3>(s<T1, T2, T3, T4, T5> e) => new s<T1, T2, T3>(e._1, e._2, e._3);
            public static implicit operator s<T1, T2, T4>(s<T1, T2, T3, T4, T5> e) => new s<T1, T2, T4>(e._1, e._2, e._4);
            public static implicit operator s<T1, T3, T2>(s<T1, T2, T3, T4, T5> e) => new s<T1, T3, T2>(e._1, e._3, e._2);
            public static implicit operator s<T1, T3, T4>(s<T1, T2, T3, T4, T5> e) => new s<T1, T3, T4>(e._1, e._3, e._4);
            public static implicit operator s<T1, T4, T2>(s<T1, T2, T3, T4, T5> e) => new s<T1, T4, T2>(e._1, e._4, e._2);
            public static implicit operator s<T1, T4, T3>(s<T1, T2, T3, T4, T5> e) => new s<T1, T4, T3>(e._1, e._4, e._3);
            public static implicit operator s<T2, T1, T3>(s<T1, T2, T3, T4, T5> e) => new s<T2, T1, T3>(e._2, e._1, e._3);
            public static implicit operator s<T2, T1, T4>(s<T1, T2, T3, T4, T5> e) => new s<T2, T1, T4>(e._2, e._1, e._4);
            public static implicit operator s<T2, T3, T1>(s<T1, T2, T3, T4, T5> e) => new s<T2, T3, T1>(e._2, e._3, e._1);
            public static implicit operator s<T2, T3, T4>(s<T1, T2, T3, T4, T5> e) => new s<T2, T3, T4>(e._2, e._3, e._4);
            public static implicit operator s<T2, T4, T1>(s<T1, T2, T3, T4, T5> e) => new s<T2, T4, T1>(e._2, e._4, e._1);
            public static implicit operator s<T2, T4, T3>(s<T1, T2, T3, T4, T5> e) => new s<T2, T4, T3>(e._2, e._4, e._3);
            public static implicit operator s<T3, T1, T2>(s<T1, T2, T3, T4, T5> e) => new s<T3, T1, T2>(e._3, e._1, e._2);
            public static implicit operator s<T3, T1, T4>(s<T1, T2, T3, T4, T5> e) => new s<T3, T1, T4>(e._3, e._1, e._4);
            public static implicit operator s<T3, T2, T1>(s<T1, T2, T3, T4, T5> e) => new s<T3, T2, T1>(e._3, e._2, e._1);
            public static implicit operator s<T3, T2, T4>(s<T1, T2, T3, T4, T5> e) => new s<T3, T2, T4>(e._3, e._2, e._4);
            public static implicit operator s<T3, T4, T1>(s<T1, T2, T3, T4, T5> e) => new s<T3, T4, T1>(e._3, e._4, e._1);
            public static implicit operator s<T3, T4, T2>(s<T1, T2, T3, T4, T5> e) => new s<T3, T4, T2>(e._3, e._4, e._2);
            public static implicit operator s<T4, T1, T2>(s<T1, T2, T3, T4, T5> e) => new s<T4, T1, T2>(e._4, e._1, e._2);
            public static implicit operator s<T4, T1, T3>(s<T1, T2, T3, T4, T5> e) => new s<T4, T1, T3>(e._4, e._1, e._3);
            public static implicit operator s<T4, T2, T1>(s<T1, T2, T3, T4, T5> e) => new s<T4, T2, T1>(e._4, e._2, e._1);
            public static implicit operator s<T4, T2, T3>(s<T1, T2, T3, T4, T5> e) => new s<T4, T2, T3>(e._4, e._2, e._3);
            public static implicit operator s<T4, T3, T1>(s<T1, T2, T3, T4, T5> e) => new s<T4, T3, T1>(e._4, e._3, e._1);
            public static implicit operator s<T4, T3, T2>(s<T1, T2, T3, T4, T5> e) => new s<T4, T3, T2>(e._4, e._3, e._2);
            
            public static implicit operator s<T1, T2, T4, T3>(s<T1, T2, T3, T4, T5> e) => new s<T1, T2, T4, T3>(e._1, e._2, e._4, e._3);
            public static implicit operator s<T1, T3, T2, T4>(s<T1, T2, T3, T4, T5> e) => new s<T1, T3, T2, T4>(e._1, e._3, e._2, e._4);
            public static implicit operator s<T1, T3, T4, T2>(s<T1, T2, T3, T4, T5> e) => new s<T1, T3, T4, T2>(e._1, e._3, e._4, e._2);
            public static implicit operator s<T1, T4, T2, T3>(s<T1, T2, T3, T4, T5> e) => new s<T1, T4, T2, T3>(e._1, e._4, e._2, e._3);
            public static implicit operator s<T1, T4, T3, T2>(s<T1, T2, T3, T4, T5> e) => new s<T1, T4, T3, T2>(e._1, e._4, e._3, e._2);
            public static implicit operator s<T2, T1, T3, T4>(s<T1, T2, T3, T4, T5> e) => new s<T2, T1, T3, T4>(e._2, e._1, e._3, e._4);
            public static implicit operator s<T2, T1, T4, T3>(s<T1, T2, T3, T4, T5> e) => new s<T2, T1, T4, T3>(e._2, e._1, e._4, e._3);
            public static implicit operator s<T2, T3, T1, T4>(s<T1, T2, T3, T4, T5> e) => new s<T2, T3, T1, T4>(e._2, e._3, e._1, e._4);
            public static implicit operator s<T2, T3, T4, T1>(s<T1, T2, T3, T4, T5> e) => new s<T2, T3, T4, T1>(e._2, e._3, e._4, e._1);
            public static implicit operator s<T2, T4, T1, T3>(s<T1, T2, T3, T4, T5> e) => new s<T2, T4, T1, T3>(e._2, e._4, e._1, e._3);
            public static implicit operator s<T2, T4, T3, T1>(s<T1, T2, T3, T4, T5> e) => new s<T2, T4, T3, T1>(e._2, e._4, e._3, e._1);
            public static implicit operator s<T3, T1, T2, T4>(s<T1, T2, T3, T4, T5> e) => new s<T3, T1, T2, T4>(e._3, e._1, e._2, e._4);
            public static implicit operator s<T3, T1, T4, T2>(s<T1, T2, T3, T4, T5> e) => new s<T3, T1, T4, T2>(e._3, e._1, e._4, e._2);
            public static implicit operator s<T3, T2, T1, T4>(s<T1, T2, T3, T4, T5> e) => new s<T3, T2, T1, T4>(e._3, e._2, e._1, e._4);
            public static implicit operator s<T3, T2, T4, T1>(s<T1, T2, T3, T4, T5> e) => new s<T3, T2, T4, T1>(e._3, e._2, e._4, e._1);
            public static implicit operator s<T3, T4, T1, T2>(s<T1, T2, T3, T4, T5> e) => new s<T3, T4, T1, T2>(e._3, e._4, e._1, e._2);
            public static implicit operator s<T3, T4, T2, T1>(s<T1, T2, T3, T4, T5> e) => new s<T3, T4, T2, T1>(e._3, e._4, e._2, e._1);
            public static implicit operator s<T4, T1, T2, T3>(s<T1, T2, T3, T4, T5> e) => new s<T4, T1, T2, T3>(e._4, e._1, e._2, e._3);
            public static implicit operator s<T4, T1, T3, T2>(s<T1, T2, T3, T4, T5> e) => new s<T4, T1, T3, T2>(e._4, e._1, e._3, e._2);
            public static implicit operator s<T4, T2, T1, T3>(s<T1, T2, T3, T4, T5> e) => new s<T4, T2, T1, T3>(e._4, e._2, e._1, e._3);
            public static implicit operator s<T4, T2, T3, T1>(s<T1, T2, T3, T4, T5> e) => new s<T4, T2, T3, T1>(e._4, e._2, e._3, e._1);
            public static implicit operator s<T4, T3, T1, T2>(s<T1, T2, T3, T4, T5> e) => new s<T4, T3, T1, T2>(e._4, e._3, e._1, e._2);
            public static implicit operator s<T4, T3, T2, T1>(s<T1, T2, T3, T4, T5> e) => new s<T4, T3, T2, T1>(e._4, e._3, e._2, e._1);
        }
        
        public static s<T1, T2> shape<T1, T2>(T1 _1, T2 _2) => new s<T1, T2>(_1, _2);
        public static s<T1, T2, T3> shape<T1, T2, T3>(T1 _1, T2 _2, T3 _3) => new s<T1, T2, T3>(_1, _2, _3);
        public static s<T1, T2, T3, T4> shape<T1, T2, T3, T4>(T1 _1, T2 _2, T3 _3, T4 _4) => new s<T1, T2, T3, T4>(_1, _2, _3, _4);
        public static s<T1, T2, T3, T4, T5> shape<T1, T2, T3, T4, T5>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5) => new s<T1, T2, T3, T4, T5>(_1, _2, _3, _4, _5);
    }
}