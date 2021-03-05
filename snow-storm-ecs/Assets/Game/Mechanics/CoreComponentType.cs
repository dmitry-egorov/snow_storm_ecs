using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static Game.Mechanics.CoreShapes;
using static Game.Mechanics.CoreJobs;

namespace Game.Mechanics
{
    public static class CoreComponentType
    {
        public static ComponentType read<T>() => ComponentType.ReadOnly<T>();
        public static ComponentType read_write<T>() => ComponentType.ReadWrite<T>();
        public static ComponentType exclude<T>() => ComponentType.Exclude<T>();

        public static ComponentType[] try_compose(this ComponentType[] a1, params ComponentType[] a2)
        {
            var list = new List<ComponentType>();
            list.AddRange(a1);
            list.AddRange(a2);
            return list.ToArray();
        }
        
        public static NativeArray<Entity> get_data_of(this ArchetypeChunk chunk, EntityRead eth) =>
            chunk.GetNativeArray(eth.type);

        public static bool has<T>(this ArchetypeChunk chunk, ComponentTypeHandle<T> cth)
            where T : struct, IComponentData =>
            chunk.Has(cth);

        public static bool has<T>(this ArchetypeChunk chunk, ComponentTypeHandle<T> cth, out NativeArray<T> arr) 
            where T : struct, IComponentData
        {
            if (chunk.has(cth))
            {
                arr = chunk.get_data_of(cth);
                return true;
            }
            
            arr = default;
            return false;
        }
        
        public static NativeArray<T> get_data_of<T>(this ArchetypeChunk chunk, ComponentTypeHandle<T> cth) 
            where T : struct, IComponentData =>
            chunk.GetNativeArray(cth);

        public static BufferAccessor<T> get_accessor_of<T>(this ArchetypeChunk chunk, BufferTypeHandle<T> cth) 
            where T : struct, IBufferElementData =>
            chunk.GetBufferAccessor(cth);
        
        public static bool has<T>(this Entity e, ComponentDataFromEntity<T> data) where T : struct, IComponentData
            => data.HasComponent(e);
        
        public static bool has<T>(this Entity e, BufferFromEntity<T> data) where T : struct, IBufferElementData 
            => data.HasComponent(e);
        
        public static bool has<T>(this Entity e, ComponentDataFromEntity<T> data, out T value)
            where T : struct, IComponentData
        {
            if (e.has(data))
            {
                value = e.get(data);
                return true;
            }

            value = default;
            return false;
        }
        
        public static bool has<T>(this Entity e, BufferFromEntity<T> data, out DynamicBuffer<T> value)
            where T : struct, IBufferElementData
        {
            if (e.has(data))
            {
                value = e.get(data);
                return true;
            }

            value = default;
            return false;
        }
        
        public static bool doesnt_have<T>(this Entity e, ComponentDataFromEntity<T> data) where T : struct, IComponentData
            => !data.HasComponent(e);
        
        public static T get<T>(this Entity e, ComponentDataFromEntity<T> data) where T : struct, IComponentData
            => data[e];
        
        public static DynamicBuffer<T> get<T>(this Entity e, BufferFromEntity<T> data) where T : struct, IBufferElementData 
            => data[e];

        public static bool must_have<T>(this Entity e, ComponentDataFromEntity<T> data)
            where T : struct, IComponentData
        {
            Debug.Assert(e.has(data));
            return true;
        }

        public static bool has<T1, T2>(
            this Entity e,
            ComponentDataFromEntity<T1> data1,
            ComponentDataFromEntity<T2> data2
        )
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData 
        =>
            e.has(data1) && e.has(data2);

        public static bool has<T1, T2>(
            this Entity e,
            ComponentDataFromEntity<T1> data1,
            ComponentDataFromEntity<T2> data2,
            out T1 value1
        )
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData 
        =>
            e.has(data1, out value1) && e.has(data2);

        public static bool has<T1, T2>(
            this Entity e,
            ComponentDataFromEntity<T1> data1,
            ComponentDataFromEntity<T2> data2,
            out T1 value1,
            out T2 value2
        )
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
        {
            value2 = default;
            return e.has(data1, out value1) && e.has(data2, out value2);
        }
        
        public static bool try_compose<T1, T2>(
            this Entity e,
            ComponentDataFromEntity<T1> data1,
            ComponentDataFromEntity<T2> data2,
            out s<Entity,T1,T2> value
        )
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
        {
            if (e.has(data1, out var _1) && e.has(data2, out var _2))
            {
                value = shape(e, _1, _2);
                return true;
            }

            value = default;
            return false;
        }
        
        public static bool try_compose<T1>(
            this Entity e,
            ComponentDataFromEntity<T1> data1,
            out s<Entity,T1> value
        )
            where T1 : struct, IComponentData
        {
            if (e.has(data1, out var _1))
            {
                value = shape(e, _1);
                return true;
            }

            value = default;
            return false;
        }
        
        public static bool try_compose<T1, T2, T3>(
            this Entity e,
            ComponentDataFromEntity<T1> data1,
            ComponentDataFromEntity<T2> data2,
            Exclude<T3> exclude3,
            out s<Entity, T1, T2, n<T3>> value
        )
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData
        {
            if (e.has(data1, out var _1) && e.has(data2, out var _2) && e.doesnt_have(exclude3.type))
            {
                value = shape(e, _1, _2, default(n<T3>));
                return true;
            }

            value = default;
            return false;
        }
        
        public static bool try_compose<T1, T2, T3, T4>(
            this Entity e,
            ComponentDataFromEntity<T1> data1,
            ComponentDataFromEntity<T2> data2,
            Exclude<T3> exclude3,
            Exclude<T4> exclude4,
            out s<Entity, T1, T2, n<T3>, n<T4>> value
        )
            where T1 : struct, IComponentData
            where T2 : struct, IComponentData
            where T3 : struct, IComponentData
            where T4 : struct, IComponentData
        {
            if (e.has(data1, out var _1) && e.has(data2, out var _2) && e.doesnt_have(exclude3.type) && e.doesnt_have(exclude4.type))
            {
                value = shape(e, _1, _2, default(n<T3>), default(n<T4>));
                return true;
            }

            value = default;
            return false;
        }
        
        public static Exclude<T> not<T>(ComponentDataFromEntity<T> data) where T : struct, IComponentData => new Exclude<T>(data);

        public readonly struct Exclude<T> 
            where T : struct, IComponentData
        {
            public readonly ComponentDataFromEntity<T> type;
            public Exclude(ComponentDataFromEntity<T> type) => this.type = type;

            public static implicit operator ComponentDataFromEntity<T>(Exclude<T> e) => e.type;
        }

        public readonly struct n<T> { }

        public static bool doesnt_have<T>(this Entity e, ComponentDataFromEntity<T> data, out T value)
            where T : struct, IComponentData =>
            !has(e, data, out value);
        
        public static bool doesnt_have<T1, T2>(
            this Entity e
            , ComponentDataFromEntity<T1> d1
            , ComponentDataFromEntity<T2> d2
            , out T1 v1
            , out T2 v2
        )
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        {
            v2 = default;
            return doesnt_have(e, d1, out v1) && doesnt_have(e, d2, out v2);
        }
        
        public static bool doesnt_have<T1, T2>(
            this Entity e
            , ComponentDataFromEntity<T1> d1
            , ComponentDataFromEntity<T2> d2
            , out T1 v1
        )
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        {
            return doesnt_have(e, d1, out v1) && doesnt_have(e, d2);
        }
        
        public static bool doesnt_have<T1, T2>(
            this Entity e
            , ComponentDataFromEntity<T1> d1
            , ComponentDataFromEntity<T2> d2
        )
            where T1 : struct, IComponentData 
            where T2 : struct, IComponentData
        {
            return doesnt_have(e, d1) && doesnt_have(e, d2);
        }
    }
}