using System.Collections.Generic;

namespace HeroServer
{
    public class GenValuesList
    {
        public List<GenValues> GenValues { get; set; }

        public GenValuesList(int count)
        {
            GenValues = new List<GenValues>(count);
        }

        public GenValuesList(List<GenValues> genValues)
        {
            GenValues = genValues;
        }

        public GenValues this[int idx]
        {
            get { return GenValues[idx]; }
        }

        public void Add(GenValues genValues)
        {
            GenValues.Add(genValues);
        }
    }
}
