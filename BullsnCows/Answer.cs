namespace BullsnCows
{
    public struct Answer
    {
        public int Bulls { get; private set; }
        public int Cows { get; set; }

        public Answer(int bulls, int cows)
        {
            Bulls = bulls;
            Cows = cows;
        }

        public override string ToString()
        {
            return string.Format("{0} Strike {1} Ball", Bulls, Cows);
        }
    }
}
