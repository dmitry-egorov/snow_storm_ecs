namespace Game.Mechanics
{
    public static class CoreIndex
    {
        public readonly struct index
        {
            public readonly int i;

            public index(int i) => this.i = i;
            public static implicit operator index(int value) => new index(value);
        }
    }
}