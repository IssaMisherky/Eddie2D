namespace GOAPSystem
{
    public sealed class GOAPWorld
    {
        private static readonly GOAPWorld instance = new GOAPWorld();
        private static WorldStates states;

        static GOAPWorld()
        {
            states = new WorldStates();
        }

        private GOAPWorld()
        {
        }

        public static GOAPWorld Instance
        {
            get { return instance; }
        }

        public WorldStates getStates()
        {
            return states;
        }
    }
}