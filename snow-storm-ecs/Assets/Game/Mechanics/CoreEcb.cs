using Unity.Entities;
using static Game.Mechanics.CoreShapes;
using static Game.Mechanics.CoreIndex;
using static Unity.Entities.EntityCommandBuffer;

namespace Game.Mechanics
{
    public static class CoreEcb
    {
        public struct has<T>: IComponentData where T: struct, IBufferElementData
        {
        }
         
        #region Parallel Writer
        
        public static ParallelWriter as_parallel(this in EntityCommandBuffer ecb) => ecb.AsParallelWriter();
        public static void destroy_all(this in EntityQuery q, in EntityCommandBuffer ecb) => ecb.DestroyEntitiesForEntityQuery(q);
        public static void remove_all<T>(this in EntityQuery q, in EntityCommandBuffer ecb) => ecb.RemoveComponentForEntityQuery<T>(q);
        public static void add_to_all<T>(this in EntityQuery q, in EntityCommandBuffer ecb) => ecb.AddComponentForEntityQuery<T>(q);
        
        public static Entity create(this in ParallelWriter writer, index sort_key) => writer.CreateEntity(sort_key.i);
        public static Entity instantiate(this in ParallelWriter writer, index sort_key, in Entity prefab) => writer.Instantiate(sort_key.i, prefab);
        public static void destroy(this in ParallelWriter writer, index sort_key, in Entity e) => writer.DestroyEntity(sort_key.i, e);
        public static void add<TData>(this in ParallelWriter writer, index sort_key, in Entity e) where TData : struct, IComponentData => writer.AddComponent<TData>(sort_key.i, e);
        public static void add<TData>(this in ParallelWriter writer, index sort_key, in Entity e, in TData d) where TData : struct, IComponentData => writer.AddComponent(sort_key.i, e, d);
        public static void add<TData1, TData2>(this in ParallelWriter writer, index sort_key, in Entity e, in TData1 d1, in TData2 d2) 
            where TData1: struct, IComponentData 
            where TData2: struct, IComponentData
        {
            writer.AddComponent(sort_key.i, e, d1);
            writer.AddComponent(sort_key.i, e, d2);
        }
        
        public static void set<TData>(this in ParallelWriter writer, index sort_key, in Entity e, in TData d) where TData : struct, IComponentData => writer.SetComponent(sort_key.i, e, d);
        public static void append<TData>(this in ParallelWriter writer, index sort_key, in Entity e, in TData d) where TData : struct, IBufferElementData => writer.AppendToBuffer(sort_key.i, e, d);
        public static void remove<TData>(this in ParallelWriter writer, index sort_key, in Entity e) where TData: struct, IComponentData => writer.RemoveComponent<TData>(sort_key.i, e);
        public static void add_buffer<TData>(this in ParallelWriter writer, index sort_key, in Entity e) where TData : struct, IBufferElementData => writer.AddBuffer<TData>(sort_key.i, e);
        
        public static void add_or_set<TData>(
            this in ParallelWriter writer, 
            index sort_key,
            in Entity e,
            ComponentDataFromEntity<TData> type, 
            TData data
        ) 
            where TData : struct, IComponentData
        {
            if (e.doesnt_have(type))
            {
                writer.add(sort_key, e, data);
            }
            else
            {
                writer.set(sort_key, e, data);
            }
        }

        public static void append_and_flag<TData>(this index sort_key, in Entity e, in TData d, in ParallelWriter writer) 
            where TData : struct, IBufferElementData
        {
            writer.add<has<TData>>(sort_key, e);
            writer.append(sort_key, e, d);
        }
        
        #endregion
        
        #region End Frame ECB

        public struct EndFrameEcb
        {
            public ParallelWriter writer;
            public EndFrameEcb(ParallelWriter writer) => this.writer = writer;
            
            public readonly Entity create(index sort_key) => writer.CreateEntity(sort_key.i);
            public readonly Entity instantiate(index sort_key, in Entity prefab) => writer.Instantiate(sort_key.i, prefab);
            public readonly void destroy(index sort_key, in Entity e) => writer.DestroyEntity(sort_key.i, e);
            public readonly void add<TData>(index sort_key, in Entity e) where TData : struct, IComponentData => writer.AddComponent<TData>(sort_key.i, e);
            public readonly void add<TData>(index sort_key, in Entity e, in TData d) where TData : struct, IComponentData => writer.AddComponent(sort_key.i, e, d);
            public readonly void add<TData1, TData2>(index sort_key, in Entity e, in TData1 d1, in TData2 d2) 
                where TData1: struct, IComponentData 
                where TData2: struct, IComponentData
            {
                writer.AddComponent(sort_key.i, e, d1);
                writer.AddComponent(sort_key.i, e, d2);
            }
            
            public readonly void set<TData>(index sort_key, in Entity e, in TData d) where TData : struct, IComponentData => writer.SetComponent(sort_key.i, e, d);
            public readonly void append<TData>(index sort_key, in Entity e, in TData d) where TData : struct, IBufferElementData => writer.AppendToBuffer(sort_key.i, e, d);
            public readonly void remove<TData>(index sort_key, in Entity e) where TData: struct, IComponentData => writer.RemoveComponent<TData>(sort_key.i, e);
            public readonly void add_buffer<TData>(index sort_key, in Entity e) where TData : struct, IBufferElementData => writer.AddBuffer<TData>(sort_key.i, e);
            
            public readonly void add_or_set<TData>(
                index sort_key, 
                in Entity e,
                ComponentDataFromEntity<TData> type,
                TData data
            ) 
                where TData : struct, IComponentData
            {
                if (e.doesnt_have(type))
                {
                    add(sort_key, e, data);
                }
                else
                {
                    set(sort_key, e, data);
                }
            }

            public readonly void append_and_flag<TData>(in Entity e, in TData d, index sort_key) 
                where TData : struct, IBufferElementData
            {
                add<has<TData>>(sort_key, e);
                append(sort_key, e, d);
            }
        }
        
        public static EndFrameEcb to_end_frame_ecb(this in EntityCommandBuffer ecb) => new EndFrameEcb(ecb.as_parallel());
        public static EndFrameEcb to_end_frame_ecb(this in ParallelWriter ecb) => new EndFrameEcb(ecb);
        
        #endregion

        #region Communications Ecb

        public struct CommEcb
        {
            public ParallelWriter writer;
            public CommEcb(ParallelWriter writer) => this.writer = writer;
            
            public readonly void add<TData>(in index sort_key, Entity e) where TData : struct, IComponentData => writer.AddComponent<TData>(sort_key.i, e);
            public readonly void add<TData>(in index sort_key, in Entity e, in TData d) where TData : struct, IComponentData => writer.AddComponent(sort_key.i, e, d);
            public readonly void append<TData>(in index sort_key, in Entity e, in TData d) where TData : struct, IBufferElementData => writer.AppendToBuffer(sort_key.i, e, d);

            public readonly void append_and_flag<TData>(in index sort_key, in Entity e, in TData d) 
                where TData : struct, IBufferElementData
            {
                add<has<TData>>(sort_key, e);
                append(sort_key, e, d);
            }
        }
        
        #endregion

        public struct GameplayEcbs
        {
            public EndFrameEcb end;
            public CommEcb comm;

            public GameplayEcbs(EndFrameEcb end, CommEcb comm)
            {
                this.end = end;
                this.comm = comm;
            }

            public static implicit operator EndFrameEcb(GameplayEcbs ecbs) => ecbs.end;
            public static implicit operator CommEcb(GameplayEcbs ecbs) => ecbs.comm;
        }
    }
}