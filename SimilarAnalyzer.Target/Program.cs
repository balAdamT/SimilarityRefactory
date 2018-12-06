namespace SimilarAnalyzer.Target
{
    internal class Person
    {
        public Person Father;
        public Person Mother;
        public Person Sister;
        public Person Brother;
        public int Age;
        public Person[] GetGrandMothers() { return new[] { Mother.Mother, Father.Mother }; }
        public Person[] GetGrandFathers() { return new[] { Father.Father, Mother.Father }; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var c = new Person();
            Operation1(c, c);
            Operation2(c, c);

            var x = 2;
            if (x++ > 5 && x % 2 == 0)
                Operation1(c, c);
            else
            {
                Operation2(c, c);
            }

            if (x++ > 5 && x % 2 == 0)

                Operation1(c, c);
            else
            {

                Operation2(c, c);

            }
        }

        private static void Operation2(Person c1, Person c2)
        {
            var x = c1.Father.Age;
            var y = c2.Father.Father;
            var d = c2?.Father?.Father;


            var e = c2.GetGrandMothers();
            var f = c2.Father.Father;
        }




        private static void Operation3(Person rightPerson, Person leftPerson)
        {
            var x = rightPerson.Father.Age;
            var y = leftPerson.Father.Father;
            var d = leftPerson?.Father?.Father;
            var e = leftPerson.GetGrandMothers();
            var f = leftPerson.Father.Father;
        }

        private static void Operation1(Person c1, Person c2)
        {
            var x = c1.Father.Age;
            var y = c2.Father.Father;
            var d = c2?.Father?.Father;
            var e = c2.GetGrandFathers();
            var f = c2.Father.Father;
        }
    }
}
