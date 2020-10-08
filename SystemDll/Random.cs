namespace SystemDll
{
    public static class Random
    {
        private static readonly System.Random rand = new System.Random();

        public static int Integer()
        {
            return rand.Next();
        }

        public static double Double()
        {
            return rand.NextDouble();
        }
    }
}
