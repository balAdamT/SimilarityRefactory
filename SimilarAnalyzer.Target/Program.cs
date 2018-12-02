namespace SimilarAnalyzer.Target
{
    internal class Class
    {
        public Class C;
        public Class OtherC;
        public int Value;
        public Class GetClass() { return null; }
        public Class GetOtherClass() { return null; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Class c = new Class();
            Operation1(c, c);
            Operation2(c, c);

            var x = 2;
            if (x++ > 5 && x % 2 == 0)
                Operation1(c, c);
            else
            {
                Operation2(c,c);                
            }
            
            if (x++ > 5 && x % 2 == 0)
                Operation1(c, c);
            else
            {
                Operation2(c,c);                
            }
        }

        private static void Operation2(Class c1, Class c2)
        {
            int x = c1.C.Value;
            var y = c2.C.C;
            var d = c2?.C?.C;
            var e = c2.GetClass().C;
            var f = c2.C.C;
        }

         static void Operation3(Class c1, Class c2)
        {
            int x = c1.C.Value;
            var y = c2.C.C;
            var d = c2?.C?.C;
            var e = c2.GetClass().C;
            var f = c2.C.C;
        }

        private static void Operation1(Class c1, Class c2)
        {
            int x = c1.C.Value;
            var y = c2.C.C;
            var d = c2?.C?.C;
            var e = c2.GetClass().C;
            var f = c2.C.C;
        }
    }
}
