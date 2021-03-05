using Unity.Entities;

namespace Game.Mechanics
{
    public static class DynamicBufferAsStack
    {
        public static bool try_pop<T>(this DynamicBuffer<T> db, out T item) where T : struct
        {
            if (db.Length == 0)
            {
                item = default;
                return false;
            }

            item = db.pop();
            return true;
        }
        
        public static T pop<T>(this DynamicBuffer<T> db) where T : struct
        {
            var last_index = db.Length - 1;
            var item = db[last_index];
            db.RemoveAtSwapBack(last_index);
            return item;
        }
    }
}