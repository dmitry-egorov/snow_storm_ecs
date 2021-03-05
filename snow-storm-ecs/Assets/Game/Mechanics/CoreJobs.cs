using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Mechanics
{
    public static class CoreJobs
    {
        public static JobHandle merged_with(this JobHandle h1, JobHandle h2) => JobHandle.CombineDependencies(h1, h2);

        #region Entity Input
        
        public struct EntityRead
        {
            [ReadOnly] public EntityTypeHandle type;
        }
        
        #endregion
        #region Read

        public struct Read<T1>
            where T1 : struct, IComponentData 
        {
            [ReadOnly] public ComponentTypeHandle<T1> type1;
        }

        public struct Read<T1, T2>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        {
            [ReadOnly] public ComponentTypeHandle<T1> type1;
            [ReadOnly] public ComponentTypeHandle<T2> type2;
        }

        public struct Read<T1, T2, T3>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData 
        {
            [ReadOnly] public ComponentTypeHandle<T1> type1;
            [ReadOnly] public ComponentTypeHandle<T2> type2;
            [ReadOnly] public ComponentTypeHandle<T3> type3;
        }

        public struct Read<T1, T2, T3, T4>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData 
            where T4 : struct, IComponentData 
        {
            [ReadOnly] public ComponentTypeHandle<T1> type1;
            [ReadOnly] public ComponentTypeHandle<T2> type2;
            [ReadOnly] public ComponentTypeHandle<T3> type3;
            [ReadOnly] public ComponentTypeHandle<T4> type4;
        }
        
        public struct Arrays<T1, T2>
            where T1 : struct 
            where T2 : struct
        {
            [ReadOnly] public NativeArray<T1> arr1;
            [ReadOnly] public NativeArray<T2> arr2;

            public Arrays(
                NativeArray<T1> arr1, 
                NativeArray<T2> arr2 
            )
            {
                this.arr1 = arr1;
                this.arr2 = arr2;
            }
            
            public Arrays<TN1, T2> reinterpret<TN1>() 
                where TN1 : struct 
                => new Arrays<TN1, T2>(arr1.Reinterpret<TN1>(), arr2);

            public Arrays<TN1, TN2> reinterpret<TN1, TN2>() 
                where TN1 : struct 
                where TN2 : struct
                => new Arrays<TN1, TN2>(arr1.Reinterpret<TN1>(), arr2.Reinterpret<TN2>());
            
            public void Deconstruct(out NativeArray<T1> arr1, out NativeArray<T2> arr2)
            {
                arr1 = this.arr1;
                arr2 = this.arr2;
            }
        }
        
        public struct Arrays<T1, T2, T3>
            where T1 : struct 
            where T2 : struct
            where T3 : struct 
        {
            [ReadOnly] public NativeArray<T1> arr1;
            [ReadOnly] public NativeArray<T2> arr2;
            [ReadOnly] public NativeArray<T3> arr3;

            public Arrays(
                NativeArray<T1> arr1, 
                NativeArray<T2> arr2, 
                NativeArray<T3> arr3
            )
            {
                this.arr1 = arr1;
                this.arr2 = arr2;
                this.arr3 = arr3;
            }
            
            public Arrays<TN1, T2, T3> reinterpret<TN1>() 
                where TN1 : struct 
                => new Arrays<TN1, T2, T3>(arr1.Reinterpret<TN1>(), arr2, arr3);

            public Arrays<TN1, TN2, T3> reinterpret<TN1, TN2>() 
                where TN1 : struct 
                where TN2 : struct
                => new Arrays<TN1, TN2, T3>(arr1.Reinterpret<TN1>(), arr2.Reinterpret<TN2>(), arr3);
            
            public Arrays<TN1, TN2, TN3> reinterpret<TN1, TN2, TN3>() 
                where TN1 : struct 
                where TN2 : struct
                where TN3 : struct 
                => new Arrays<TN1, TN2, TN3>(arr1.Reinterpret<TN1>(), arr2.Reinterpret<TN2>(), arr3.Reinterpret<TN3>());

            public void Deconstruct(out NativeArray<T1> arr1, out NativeArray<T2> arr2, out NativeArray<T3> arr3)
            {
                arr1 = this.arr1;
                arr2 = this.arr2;
                arr3 = this.arr3;
            }
        }
        
        public struct Arrays<T1, T2, T3, T4>
            where T1 : struct 
            where T2 : struct
            where T3 : struct 
            where T4 : struct 
        {
            [ReadOnly] public NativeArray<T1> arr1;
            [ReadOnly] public NativeArray<T2> arr2;
            [ReadOnly] public NativeArray<T3> arr3;
            [ReadOnly] public NativeArray<T4> arr4;

            public Arrays(
                NativeArray<T1> arr1, 
                NativeArray<T2> arr2, 
                NativeArray<T3> arr3,
                NativeArray<T4> arr4
            )
            {
                this.arr1 = arr1;
                this.arr2 = arr2;
                this.arr3 = arr3;
                this.arr4 = arr4;
            }
            
            public Arrays<TN1, T2, T3, T4> reinterpret<TN1>() 
                where TN1 : struct 
            => new Arrays<TN1, T2, T3, T4>(arr1.Reinterpret<TN1>(), arr2, arr3, arr4);

            public Arrays<TN1, TN2, T3, T4> reinterpret<TN1, TN2>() 
                where TN1 : struct 
                where TN2 : struct
            => new Arrays<TN1, TN2, T3, T4>(arr1.Reinterpret<TN1>(), arr2.Reinterpret<TN2>(), arr3, arr4);
            
            public Arrays<TN1, TN2, TN3, T4> reinterpret<TN1, TN2, TN3>() 
                where TN1 : struct 
                where TN2 : struct
                where TN3 : struct 
            => new Arrays<TN1, TN2, TN3, T4>(arr1.Reinterpret<TN1>(), arr2.Reinterpret<TN2>(), arr3.Reinterpret<TN3>(), arr4);
            
            public Arrays<TN1, TN2, TN3, TN4> reinterpret<TN1, TN2, TN3, TN4>() 
                where TN1 : struct 
                where TN2 : struct
                where TN3 : struct 
                where TN4 : struct 
            => new Arrays<TN1, TN2, TN3, TN4>(arr1.Reinterpret<TN1>(), arr2.Reinterpret<TN2>(), arr3.Reinterpret<TN3>(), arr4.Reinterpret<TN4>());

            public void Deconstruct(out NativeArray<T1> arr1, out NativeArray<T2> arr2, out NativeArray<T3> arr3, out NativeArray<T4> arr4)
            {
                arr1 = this.arr1;
                arr2 = this.arr2;
                arr3 = this.arr3;
                arr4 = this.arr4;
            }
        }
        
        public static NativeArray<T1> get_data_of<T1>(
            this ArchetypeChunk chunk, 
            Read<T1> read
        ) 
            where T1 : struct, IComponentData 
        => chunk.get_data_of(read.type1);
        
        public static Arrays<T1, T2> get_data_of<T1, T2>(
            this ArchetypeChunk chunk, 
            Read<T1, T2> read
        ) 
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        => new Arrays<T1, T2>(
              chunk.get_data_of(read.type1)
            , chunk.get_data_of(read.type2)
        );
        
        public static Arrays<T1, T2, T3> get_data_of<T1, T2, T3>(
            this ArchetypeChunk chunk, 
            Read<T1, T2, T3> read
        ) 
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData 
        => new Arrays<T1, T2, T3>(
              chunk.get_data_of(read.type1)
            , chunk.get_data_of(read.type2)
            , chunk.get_data_of(read.type3)
        );
        
        public static Arrays<T1, T2, T3, T4> get_data_of<T1, T2, T3, T4>(
            this ArchetypeChunk chunk, 
            Read<T1, T2, T3, T4> read
        ) 
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData
            where T4 : struct, IComponentData 
        => new Arrays<T1, T2, T3, T4>(
              chunk.get_data_of(read.type1)
            , chunk.get_data_of(read.type2)
            , chunk.get_data_of(read.type3)
            , chunk.get_data_of(read.type4)
        );
        
        public static Arrays<T1, T2, Entity> get_data_of<T1, T2>(
            this ArchetypeChunk chunk,
            Read<T1, T2> read,
            EntityRead er
        ) 
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            => new Arrays<T1, T2, Entity>(
                  chunk.get_data_of(read.type1)
                , chunk.get_data_of(read.type2)
                , chunk.get_data_of(er)

            );

        #endregion
        
        #region Global Read

        public struct GlobalRead<T1>
            where T1 : struct, IComponentData 
        {
            [ReadOnly] public ComponentDataFromEntity<T1> type;

            public void Deconstruct(out ComponentDataFromEntity<T1> type1) => type1 = this.type;
        }

        public struct GlobalRead<T1, T2>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        {
            [ReadOnly] public ComponentDataFromEntity<T1> type1;
            [ReadOnly] public ComponentDataFromEntity<T2> type2;

            public void Deconstruct(out ComponentDataFromEntity<T1> type1, out ComponentDataFromEntity<T2> type2)
            {
                type1 = this.type1;
                type2 = this.type2;
            }
        }

        public struct GlobalRead<T1, T2, T3>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData 
        {
            [ReadOnly] public ComponentDataFromEntity<T1> type1;
            [ReadOnly] public ComponentDataFromEntity<T2> type2;
            [ReadOnly] public ComponentDataFromEntity<T3> type3;

            public void Deconstruct(
                out ComponentDataFromEntity<T1> type1, 
                out ComponentDataFromEntity<T2> type2, 
                out ComponentDataFromEntity<T3> type3
            )
            {
                type1 = this.type1;
                type2 = this.type2;
                type3 = this.type3;
            }
        }

        public struct GlobalRead<T1, T2, T3, T4>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData 
            where T4 : struct, IComponentData 
        {
            [ReadOnly] public ComponentDataFromEntity<T1> type1;
            [ReadOnly] public ComponentDataFromEntity<T2> type2;
            [ReadOnly] public ComponentDataFromEntity<T3> type3;
            [ReadOnly] public ComponentDataFromEntity<T4> type4;

            public void Deconstruct(
                out ComponentDataFromEntity<T1> type1, 
                out ComponentDataFromEntity<T2> type2, 
                out ComponentDataFromEntity<T3> type3, 
                out ComponentDataFromEntity<T4> type4
            )
            {
                type1 = this.type1;
                type2 = this.type2;
                type3 = this.type3;
                type4 = this.type4;
            }
        }

        #endregion

        #region Buffer Global Read

        public struct BufferGlobalRead<T1>
            where T1 : struct, IBufferElementData 
        {
            [ReadOnly] public BufferFromEntity<T1> type;

            public void Deconstruct(out BufferFromEntity<T1> type1) => type1 = this.type;
        }

        public struct BufferGlobalRead<T1, T2>
            where T1 : struct, IBufferElementData 
            where T2 : struct, IBufferElementData
        {
            [ReadOnly] public BufferFromEntity<T1> type1;
            [ReadOnly] public BufferFromEntity<T2> type2;

            public void Deconstruct(out BufferFromEntity<T1> type1, out BufferFromEntity<T2> type2)
            {
                type1 = this.type1;
                type2 = this.type2;
            }
        }

        #endregion
        
        #region Buffer Global Write

        public struct BufferGlobalWrite<T1>
            where T1 : struct, IBufferElementData 
        {
            public BufferFromEntity<T1> type;

            public void Deconstruct(out BufferFromEntity<T1> type1) => type1 = this.type;
        }

        public struct BufferGlobalWrite<T1, T2>
            where T1 : struct, IBufferElementData 
            where T2 : struct, IBufferElementData
        {
            public BufferFromEntity<T1> type1;
            public BufferFromEntity<T2> type2;

            public void Deconstruct(out BufferFromEntity<T1> type1, out BufferFromEntity<T2> type2)
            {
                type1 = this.type1;
                type2 = this.type2;
            }
        }

        #endregion
        
        #region Optional Input

        public struct Optional<T1>
            where T1 : struct, IComponentData 
        {
            [ReadOnly] public ComponentTypeHandle<T1> type1;
        }

        public struct Optional<T1, T2>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        {
            [ReadOnly] public ComponentTypeHandle<T1> type1;
            [ReadOnly] public ComponentTypeHandle<T2> type2;
        }
        
        public static bool has<T1>(
            this ArchetypeChunk chunk, 
            Optional<T1> input
        ) 
            where T1 : struct, IComponentData 
        => chunk.has(input.type1);
        
        public static bool has<T1>(
            this ArchetypeChunk chunk, 
            Optional<T1> input,
            out NativeArray<T1> arr1
        ) 
            where T1 : struct, IComponentData 
        => chunk.has(input.type1, out arr1);
        
        public static bool has<T1, T2>(
            this ArchetypeChunk chunk, 
            Optional<T1, T2> input
        ) 
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData 
        => chunk.has(input.type1) && chunk.has(input.type2);
        
        public static bool has<T1, T2>(
            this ArchetypeChunk chunk
            , Optional<T1, T2> input
            , out NativeArray<T1> arr1
            , out NativeArray<T2> arr2
        )
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        {
            arr2 = default;
            return chunk.has(input.type1, out arr1) && chunk.has(input.type2, out arr2);
        }

        #endregion
        
        #region Output

        public struct Write<T1>
            where T1 : struct, IComponentData 
        {
            public ComponentTypeHandle<T1> type1;
        }

        public struct Write<T1, T2>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        {
            public ComponentTypeHandle<T1> type1;
            public ComponentTypeHandle<T2> type2;
        }

        public struct Write<T1, T2, T3>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData 
        {
            public ComponentTypeHandle<T1> type1;
            public ComponentTypeHandle<T2> type2;
            public ComponentTypeHandle<T3> type3;
        }

        public struct Write<T1, T2, T3, T4>
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData 
            where T4 : struct, IComponentData 
        {
            public ComponentTypeHandle<T1> type1;
            public ComponentTypeHandle<T2> type2;
            public ComponentTypeHandle<T3> type3;
            public ComponentTypeHandle<T4> type4;
        }
        
        public static NativeArray<T1> get_data_of<T1>(
            this ArchetypeChunk chunk, 
            Write<T1> write
        ) 
            where T1 : struct, IComponentData 
        => chunk.get_data_of(write.type1);
        
        public static Arrays<T1, T2> get_data_of<T1, T2>(
            this ArchetypeChunk chunk, 
            Write<T1, T2> write
        ) 
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        => new Arrays<T1, T2>(
            chunk.get_data_of(write.type1)
            , chunk.get_data_of(write.type2)
        );
        
        public static Arrays<T1, T2, T3> get_data_of<T1, T2, T3>(
            this ArchetypeChunk chunk, 
            Write<T1, T2, T3> write
        ) 
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData 
        => new Arrays<T1, T2, T3>(
            chunk.get_data_of(write.type1)
            , chunk.get_data_of(write.type2)
            , chunk.get_data_of(write.type3)
        );
        
        public static Arrays<T1, T2, T3, T4> get_data_of<T1, T2, T3, T4>(
            this ArchetypeChunk chunk, 
            Write<T1, T2, T3, T4> write
        ) 
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData
            where T4 : struct, IComponentData 
        => new Arrays<T1, T2, T3, T4>(
            chunk.get_data_of(write.type1)
            , chunk.get_data_of(write.type2)
            , chunk.get_data_of(write.type3)
            , chunk.get_data_of(write.type4)
        );
        
        public static Arrays<T1, Entity> get_data_of<T1>(
            this ArchetypeChunk chunk, 
            Write<T1> write,
            EntityRead er
        ) 
            where T1 : struct, IComponentData 
        => new Arrays<T1, Entity>(
            chunk.get_data_of(write.type1), 
            chunk.get_data_of(er)
        );
        
        #endregion
        
        #region Buffer Output
        
        public struct BufferWrite<T1>
            where T1 : struct, IBufferElementData 
        {
            public BufferTypeHandle<T1> type1;
        }

        public struct BufferWrite<T1, T2>
            where T1 : struct, IBufferElementData 
            where T2 : struct, IBufferElementData
        {
            public BufferTypeHandle<T1> type1;
            public BufferTypeHandle<T2> type2;
        }
        
        public static BufferAccessor<T1> get_data_of<T1>(
            this ArchetypeChunk chunk, 
            BufferWrite<T1> write
        ) 
            where T1 : struct, IBufferElementData 
        => chunk.get_accessor_of(write.type1);
        
        public static (BufferAccessor<T1>, BufferAccessor<T2>) get_data_of<T1, T2>(
            this ArchetypeChunk chunk, 
            BufferWrite<T1, T2> write
        ) 
            where T1 : struct, IBufferElementData 
            where T2 : struct, IBufferElementData
            => (
                chunk.get_accessor_of(write.type1)
                , chunk.get_accessor_of(write.type2)
            );
        
        #endregion
    }
}