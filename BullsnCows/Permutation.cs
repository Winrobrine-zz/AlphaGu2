using System.Collections.Generic;
using System.Linq;

namespace BullsnCows
{
    public static class Permutation
    {
        public static IEnumerable<string> Create(string script, int size)
        {
            if (size > 0)
            {
                foreach (string s in Create(script, size - 1))
                {
                    foreach (char n in script)
                    {
                        if (!s.Contains(n))
                            yield return s + n;
                    }
                }
            }
            else yield return "";
        }
    }
}
