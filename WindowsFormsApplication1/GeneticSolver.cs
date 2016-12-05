using System;
using System.Collections;
using System.Collections.Generic;

namespace TSP
{
	public class GeneticSolver
	{
		private City[] initialCityArray;
		private int Size;

		private static int Generations = 100;

        //private List<GeneticChild> children; Don't need this if we just save the 4 children we want
		private GeneticChild best1Child;
        private GeneticChild best2Child;
        private GeneticChild rand1Child;
        private GeneticChild rand2Child;

        public GeneticSolver(ref City[] cities)
        {
            this.initialCityArray = cities;
            this.Size = cities.Length;

        }

        public GeneticSolver(ref City[] cities, GeneticChild b1, GeneticChild b2, GeneticChild r1, GeneticChild r2)
		{
            this.initialCityArray = cities;
			this.Size = cities.Length;
            this.best1Child = b1;
            this.best2Child = b2;
            this.rand1Child = r1;
            this.rand2Child = r2;

		}

        private GeneticChild randomSolver()
        {
            // initialize variables
            byte i, swap, temp, count = 0;
            GeneticChild child;
            byte[] perm = new byte[this.Size];
            Random rnd = new Random();
            do
            {
                // set the index values in order
                for (i = 0; i < perm.Length; i++)                                 // create a random permutation template
                    perm[i] = i;
                // swap around the index values
                for (i = 0; i < perm.Length; i++)
                {
                    swap = i;
                    while (swap == i)
                        swap = (byte)rnd.Next(0, this.Size);
                    temp = perm[i];
                    perm[i] = perm[swap];
                    perm[swap] = temp;
                }
                // create a child with the new city sequence
                child = new GeneticChild(perm, this.Size);
                if (child.valid)
                {
                    // if the child was valid, calculate a score
                    child.calcScore(this.initialCityArray);
                }
                // not sure if we should keep the count here.
                // count++;
                // if the child's score was infinity, keep trying
            } while (child.score == double.PositiveInfinity);                // until a valid route is found
            return child;
        }

		public void solve()
		{
			for (int generation = 0; generation < Generations; generation++)
			{
				// delete all other children
				// choose 4 best

				// cross the 4 best making 16 routes

				// take the 20 resulting paths then mutate each 20 times making 400
			}


			// return best path
		}


		public void initialRound()
		{

            // run 500 times and add to children, use the default algorithm in TSP
            int population = 500;
            int rand1, rand2;
            // save the two random children for genetic variation
            Random rnd = new Random();
            rand1 = rnd.Next(0, population);
            do
            {
                rand2 = rnd.Next(0, population);
            }
            while (rand1 == rand2);

            for (int i = 0; i < 500; i++)
			{
				// add to children
                
                if(i == rand1)
                {

                }
                if(i == rand2)
                {

                }


			}
		}


		public GeneticChild[] cross(GeneticChild childA, GeneticChild childB)
		{
			GeneticChild[] children = new GeneticChild[2];

			// cross children

			return children;
		}

		public GeneticChild[] mutate(GeneticChild child, int iterations)
		{
			GeneticChild[] children = new GeneticChild[2];

			for (int i = 0; i < iterations; i++)
			{
				// mutate the child by
				// swapping nodes then
				// add to children
			}

			return children;
		}

		public double updateCostForChild(GeneticChild child)
		{
			// update the child

			return 0.0;
		}


		public string getCost()
		{
			return "not implemented";
		}

		public string getTime()
		{
			return "-1";
		}

		public string getCount()
		{
			return "-1";
		}






		///// initial double matrix for reachability and calculating

		//public double[,] fillCitiesReducedCostMatrix()
		//{
		//    double[,] matrix = new double[Size, Size];

		//    for (int row = 0; row < Size; row++)
		//    {
		//        for (int col = 0; col < Size; col++)
		//        {
		//            City fromCity = initialCityArray[row];
		//            City toCity = initialCityArray[col];

		//            double cost = Double.PositiveInfinity;
		//            if (row != col)
		//            {
		//                cost = fromCity.costToGetTo(toCity);
		//            }

		//            matrix[col, row] = cost;
		//        }
		//    }
		//    return matrix;
		//}




		//public double reduceMatrix(ref double[,] matrix)
		//{
		//	double boundShavedOff = 0;
		//	boundShavedOff += rowReduce(ref matrix);
		//	boundShavedOff += colReduce(ref matrix);

		//	return boundShavedOff;
		//}

		//public double rowReduce(ref double[,] matrix)
		//{
		//	double boundShavedOff = 0;

		//	for (int row = 0; row < Size; row++)
		//	{
		//		double lowestValue = Double.PositiveInfinity;

		//		for (int col = 0; col < Size; col++)
		//		{
		//			double value = matrix[col, row];

		//			if (value < lowestValue)
		//			{
		//				lowestValue = value;
		//			}
		//		}

		//		if (!Double.IsPositiveInfinity(lowestValue))
		//		{
		//			boundShavedOff += lowestValue;
		//		}

		//		for (int col = 0; col < Size; col++)
		//		{
		//			double value = matrix[col, row];

		//			if (!Double.IsPositiveInfinity(value))
		//			{
		//				matrix[col, row] = value - lowestValue;
		//			}
		//		}
		//	}

		//	return boundShavedOff;
		//}

		//public double colReduce(ref double[,] matrix)
		//{
		//	double boundShavedOff = 0;

		//	for (int col = 0; col < Size; col++)
		//	{
		//		double lowestValue = Double.PositiveInfinity;

		//		for (int row = 0; row < Size; row++)
		//		{
		//			double value = matrix[col, row];

		//			if (value < lowestValue)
		//			{
		//				lowestValue = value;
		//			}
		//		}

		//		if (!Double.IsPositiveInfinity(lowestValue))
		//		{
		//			boundShavedOff += lowestValue;
		//		}

		//		for (int row = 0; row < Size; row++)
		//		{
		//			double value = matrix[col, row];

		//			if (!Double.IsPositiveInfinity(value))
		//			{
		//				matrix[col, row] = value - lowestValue;
		//			}
		//		}
		//	}
		//	return boundShavedOff;
		//}






	}
}
