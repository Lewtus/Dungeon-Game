using System;
using System.Collections.Generic;
using System.Text;

namespace gam
{
    static class RandomHelpers
    {
        public static T PickFromArray<T>(T[] options)
        {
            Random rnd = new Random();
            int index = rnd.Next(0, options.Length);
            return options[index];
        }

    }
}
