using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
	public class GeneticChild
	{
		public HashSet<UInt16> nodes;
		public double bound;

		public GeneticChild()
		{

		}



    public int CompareTo(GeneticChild otherChild)
    {
      if (otherChild == null) return 1;

      if (otherChild != null)
      {
        return this.bound.CompareTo(otherChild.bound);
      }
      else
      {
        throw new ArgumentException("Object is not a GeneticChild");
      }
    }

	}

	public class GeneticChildComparer : IComparer<GeneticChild>
  {
    public int Compare(GeneticChild a, GeneticChild b)
    {
      return a.CompareTo(b);
    }
  }
}
