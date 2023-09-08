namespace GoPlay
{
    public class Tuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            if (obj is Tuple<T1, T2> t)
            {
                if (t.Item1 == null && t.Item1 != null) return false;
                if (t.Item2 == null && t.Item2 != null) return false;
                
                if (t.Item1 != null && !t.Item1.Equals(Item1)) return false;
                if (t.Item2 != null && !t.Item2.Equals(Item2)) return false;

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode()*10 + Item2.GetHashCode();
        }
    }
}