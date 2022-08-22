//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests
//{
//    internal class SimpleQueryTestsData : IEnumerable<object[]>
//    {
//        public IEnumerator<object[]> GetEnumerator()
//        {
//            yield return new object[] { new CodeFirst.TestDbContext(), false };
//            yield return new object[] { new CodeFirst.TestDbContext(), true };
//            yield return new object[] { new DatabaseFirst.TestDbContext(), false };
//            yield return new object[] { new DatabaseFirst.TestDbContext(), true };
//        }

//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//    }
//}
