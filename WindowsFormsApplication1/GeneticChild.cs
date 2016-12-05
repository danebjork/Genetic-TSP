using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
	public class GeneticChild
	{
		public Byte[] gene;
		public double score;
        public bool valid;

		public GeneticChild(Byte[] gene, int cityNum)
		{
            this.gene = gene;
            validateGene(cityNum);
		}


    public void calcScore(City[] Cities)
        {
            this.score = 0;
            // go through each city and add the cost to the score
            for(int i = 0; i < Cities.Length-1; i++)
            {
                score += Cities[this.gene[i]].costToGetTo(Cities[this.gene[i + 1]]);
            }
            score += Cities[this.gene[this.gene.Length - 1]].costToGetTo(Cities[this.gene[0]]);
        }
    private void validateGene(int cityNum)
        {
            // check if the numbers of cities in the array equal the number of cities
            // that are required in the solution
            if(this.gene.Length != cityNum)
            {
                this.valid = false;
            }
            // if they are equal, check for duplicates
            else
            {
                if (this.gene.Distinct().Count() != this.gene.Length)
                {
                    this.valid = false;
                }
            }
            // otherwise, this is a valid city sequence
            this.valid = true;
        }


    public int CompareTo(GeneticChild otherChild)
    {
      if (otherChild == null) return 1;

      if (otherChild != null)
      {
        return this.score.CompareTo(otherChild.score);
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
