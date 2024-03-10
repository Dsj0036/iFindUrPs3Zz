using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Pair<T>
    {
        public void SetX(T x) { X = x; }
        public void SetY(T y) { Y = y; }
        public T X { get; set; }
        public T Y { get; set; }  
        /// <summary>
        /// Representa una pareja de valores.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Pair(T x, T y)
        {
            X = x;
            Y = y;
        }
    }
}
