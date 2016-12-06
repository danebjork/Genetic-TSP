using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
	public class GeneticSolver
	{
		private City[] initialCityArray;
		private int Size;
        Random rnd;
		private static int Generations = 10;
        private int childNum;

        //private List<GeneticChild> children; Don't need this if we just save the 4 children we want
		private GeneticChild best1Child;
        private double b1Score;
        private GeneticChild best2Child;
        private double b2Score;
        private GeneticChild rand1Child;
        private int rand1Index;
        private GeneticChild rand2Child;
        private int rand2Index;

        private int iterations;

        public GeneticSolver(ref City[] cities)
        {
            childNum = 0;
            this.initialCityArray = cities;
            this.Size = cities.Length;
            b1Score = Double.PositiveInfinity;
            b2Score = Double.PositiveInfinity;
            this.rnd = new Random();
            this.iterations = 50; // Change this as some function of size/depth

        }

        // Don't know if we'll need this anymore
  //      public GeneticSolver(ref City[] cities, GeneticChild b1, GeneticChild b2, GeneticChild r1, GeneticChild r2)
		//{
  //          this.initialCityArray = cities;
		//	this.Size = cities.Length;
  //          this.best1Child = b1;
  //          this.best2Child = b2;
  //          this.rand1Child = r1;
  //          this.rand2Child = r2;

		//}

        private GeneticChild randomSolver()
        {
            // initialize variables
            byte i, swap, temp, count = 0;
            GeneticChild child;
            byte[] perm = new byte[this.Size];
            
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
            initialRound();
			for (int generation = 0; generation < Generations; generation++)
			{
                Console.WriteLine("GENERATION: {0}", generation);
                Console.Write("SEQ 1: ");
                Console.WriteLine(string.Join(",", best1Child.gene));
                Console.WriteLine(best1Child.valid);
                Console.Write("SEQ 2: ");
                Console.WriteLine(string.Join(",", best2Child.gene));
                Console.WriteLine(best2Child.valid);
                // delete all other children
                // choose 4 best
                b1Score = Double.PositiveInfinity;
                b2Score = Double.PositiveInfinity;
                GeneticChild parent1 = this.best1Child;
                GeneticChild parent2 = this.best2Child;
                GeneticChild parent3 = this.rand1Child;
                GeneticChild praent4 = this.rand2Child;
                rand1Index = rnd.Next(0, 1000);
                rand2Index = rnd.Next(0, 1000);
                childNum = 0;
                // cross each of the parents with each other
                cross(parent1, parent2);
                //cross(best1Child, rand1Child);
                //cross(best1Child, rand2Child);
                //cross(best2Child, rand1Child);
                //cross(best2Child, rand2Child);
                //cross(rand1Child, rand2Child);
                // mutate each of the parents into children
                //mutate(best1Child);
                //mutate(best2Child);
                //mutate(rand1Child);
                //mutate(rand2Child);


                // cross the 4 best making 16 routes

                // take the 20 resulting paths then mutate each 20 times making 400
            }


			// return best path
		}


		public void initialRound()
		{

            // run 500 times and add to children, used a modified /TSP default algorithm.
            int population = 1000;
            int rand1, rand2;
            HashSet<double> test = new HashSet<double>();
            // save the two random children for genetic variation
            Random rnd = new Random();
            rand1 = rnd.Next(0, population);
            do
            {
                rand2 = rnd.Next(0, population);
            }
            while (rand1 == rand2);
            int i = 0;
            while(b2Score == Double.PositiveInfinity || i < population)
            {
                // add to children
                GeneticChild temp = randomSolver();
                test.Add(temp.score);
                if (temp.valid)
                {
                    if (temp.score < b1Score)
                    {
                        b2Score = b1Score;
                        b1Score = temp.score;
                        best2Child = best1Child;
                        best1Child = temp;

                    }
                    if (i == rand1)
                    {
                        Console.WriteLine(temp.score);
                        rand1Child = temp;
                    }
                    else if (i == rand2)
                    {
                        Console.WriteLine(temp.score);
                        rand2Child = temp;
                    }
                    i++;
                }


			}
            //Console.WriteLine(string.Join(",", test));
            //Console.WriteLine("Best 1: {0}", best1Child.score);
            //Console.WriteLine("Best 2: {0}", best2Child.score);
            //Console.WriteLine("Rand 1: {0}", rand1Child.score);
            //Console.WriteLine("Rand 2: {0}", rand2Child.score);
        }


		public void cross(GeneticChild childA, GeneticChild childB)
		{
            Console.WriteLine(string.Join(",", childA.gene));
			GeneticChild[] children = new GeneticChild[2];

            for(int i = 0; i < this.iterations; i++)
            {
                byte[] newGeneA = new byte[childA.gene.Length];
                byte[] newGeneB = new byte[childB.gene.Length];
                // copy the arrays
                for (int j = 0; j < childA.gene.Length; j++)
                {
                    newGeneA[j] = childA.gene[j];
                }
                for (int j = 0; j < childB.gene.Length; j++)
                {
                    newGeneB[j] = childB.gene[j];
                }

                int rand1, tempIndex;
                do
                {
                    rand1 = rnd.Next(0, newGeneA.Length);
                    // find the city in gene B as the end point
                    byte city = newGeneB[rand1];
                    // find the index of the end point
                    tempIndex = Array.IndexOf(newGeneA, city);
                    if(tempIndex == -1)
                    {
                        var groups = new HashSet<byte>(newGeneA);
                        Console.WriteLine("G: {0}", groups.Count());
                        Console.WriteLine(string.Join(",", newGeneA));
                        Console.WriteLine(childA.valid);
                        Console.WriteLine(childA.gene.Length);
                    }
                }
                while (tempIndex == rand1);


                if(tempIndex > rand1)
                {
                    for (int crossing = rand1; crossing <= tempIndex; crossing++)
                    {
                        byte city = newGeneB[crossing];
                        newGeneB[crossing] = newGeneA[crossing];
                        newGeneA[crossing] = city;
                    }
                }
                else if(tempIndex < rand1)
                {
                    for (int crossing = tempIndex; crossing <= rand1; crossing++)
                    {
                        byte city = newGeneB[crossing];
                        newGeneB[crossing] = newGeneA[crossing];
                        newGeneA[crossing] = city;
                    }
                }

                // check if Gene A Works
                GeneticChild temp = new GeneticChild(newGeneA, newGeneA.Length);
                if (temp.valid)
                {
                    childNum += 1;
                    if (temp.score < b1Score)
                    {
                        Console.WriteLine("Cross A");
                        best2Child = best1Child;
                        best1Child = temp;
                        Console.WriteLine(string.Join(",", temp.gene));
                        b1Score = temp.score;

                    }
                    if (childNum == rand1Index)
                    {
                        rand1Child = temp;
                    }
                    if (childNum == rand2Index)
                    {
                        rand1Child = temp;
                    }
                }


                // check if Gene B works
                temp = new GeneticChild(newGeneB, newGeneB.Length);
                if (temp.valid)
                {
                    childNum += 1;
                    if (temp.score < b1Score)
                    {
                        Console.WriteLine("Cross B");
                        Console.WriteLine(string.Join(",", temp.gene));
                        best2Child = best1Child;
                        best1Child = temp;
                        b1Score = temp.score;

                    }
                    if (childNum == rand1Index)
                    {
                        rand1Child = temp;
                    }
                    if (childNum == rand2Index)
                    {
                        rand1Child = temp;
                    }
                }
            }
			// cross children

			//return children;
		}

        // randomly create an integer value representing the index value to mutate
        // mutate that value to a new value swapping with another value.
		public void mutate(GeneticChild child)
		{
			//GeneticChild[] children = new GeneticChild[iterations];            
			for (int i = 0; i < this.iterations; i++)
			{
                // mutate the child by
                // swapping nodes then
                // add to children
                byte[] newGene = new byte[child.gene.Length];
                int rand1, rand2;
                do
                {
                    rand1 = rnd.Next(0, child.gene.Length);
                    rand2 = rnd.Next(0, child.gene.Length);
                }
                while (rand2 == rand1);

                // get the city at that index
                newGene = (byte[])child.gene.Clone();
                byte city = newGene[rand1];
                newGene[rand1] = newGene[rand2];
                newGene[rand2] = city;
                GeneticChild temp = new GeneticChild(newGene, newGene.Length);
                if (temp.valid)
                {
                    childNum += 1;
                    if (temp.score < b1Score)
                    {
                        best2Child = best1Child;
                        best1Child = temp;
                        b1Score = temp.score;
                    }
                    if (childNum == rand1Index)
                    {
                        rand1Child = temp;
                    }
                    if (childNum == rand2Index)
                    {
                        rand1Child = temp;
                    }
                }

                //children[i] = temp;

            }

			//return children;
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
