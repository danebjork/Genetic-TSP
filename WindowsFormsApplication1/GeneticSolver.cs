using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace TSP
{
	public class GeneticSolver
	{
		private City[] initialCityArray;
		private int Size;
        Random rnd;
		private static int Generations = 100;
        private int childNum;
        private int count;
        private double bssf;
        private byte[] bestGene;
        //private List<GeneticChild> children; Don't need this if we just save the 4 children we want
		private GeneticChild best1Child;
        private double b1Score;
        private GeneticChild best2Child;
        private double b2Score;
        private GeneticChild rand1Child;
        private int rand1Index;
        private GeneticChild rand2Child;
        private int rand2Index;
        private Stopwatch timer;

        private int iterations;

        public GeneticSolver(ref City[] cities)
        {
            count = 0;
            this.bssf = Double.PositiveInfinity;
            childNum = 0;
            this.initialCityArray = cities;
            this.Size = cities.Length;
            b1Score = Double.PositiveInfinity;
            b2Score = Double.PositiveInfinity;
            this.rnd = new Random();
            this.iterations = 100; // Change this as some function of size/depth

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
            byte i, swap, temp;
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
                    child.calcScore(ref this.initialCityArray);
                }
                // not sure if we should keep the count here.
                // count++;
                // if the child's score was infinity, keep trying
            } while (child.score == double.PositiveInfinity);                // until a valid route is found
            return child;
        }

		public void solve()
		{
            timer = new Stopwatch();

            timer.Start();
            initialRound();
			for (int generation = 0; generation < Generations; generation++)
			{
                //Console.WriteLine("GENERATION: {0}", generation);
                //Console.Write("SEQ 1: ");
                //Console.WriteLine(string.Join(",", best1Child.gene));
                //Console.WriteLine(best1Child.valid);
                //Console.Write("SEQ 2: ");
                //Console.WriteLine(string.Join(",", best2Child.gene));
                //Console.WriteLine(best2Child.valid);
                // delete all other children
                // choose 4 best
                b1Score = Double.PositiveInfinity;
                b2Score = Double.PositiveInfinity;
                GeneticChild parent1 = this.best1Child;
                GeneticChild parent2 = this.best2Child;
                GeneticChild parent3 = this.rand1Child;
                GeneticChild parent4 = this.rand2Child;
                rand1Index = rnd.Next(0, 500);
                rand2Index = rnd.Next(0, 500);
                childNum = 0;
                // cross each of the parents with each other
                cross(parent1, parent2);
                cross(parent1, parent3);
                cross(parent1, parent4);
                cross(parent2, parent3);
                cross(parent2, parent4);
                cross(parent3, parent4);
                // mutate each of the parents into children
                mutate(parent1);
                mutate(parent2);
                mutate(parent3);
                mutate(parent4);


                // cross the 4 best making 16 routes

                // take the 20 resulting paths then mutate each 20 times making 400
            }
            timer.Stop();

            //Console.WriteLine("BSSF: {0}", bssf);
			// return best path
		}


		public void initialRound()
		{

            // run 500 times and add to children, used a modified /TSP default algorithm.
            int population = 2000;
            childNum = 0;
            HashSet<double> test = new HashSet<double>();
            // save the two random children for genetic variation
            Random rnd = new Random();
            rand1Index = rnd.Next(0, population);
            do
            {
                rand2Index = rnd.Next(0, population);
            }
            while (rand1Index == rand2Index);
            while(b2Score == Double.PositiveInfinity || childNum < population)
            {
                // add to children
                GeneticChild temp = randomSolver();
                checkChild(temp);
			}
            //Console.WriteLine(string.Join(",", test));
            //Console.WriteLine("Best 1: {0}", best1Child.score);
            //Console.WriteLine("Best 2: {0}", best2Child.score);
            //Console.WriteLine("Rand 1: {0}", rand1Child.score);
            //Console.WriteLine("Rand 2: {0}", rand2Child.score);
        }


		public void cross(GeneticChild childA, GeneticChild childB)
		{
            //Console.WriteLine(string.Join(",", childA.gene));
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
                    rand1 = rnd.Next(0, newGeneB.Length);
                    // find the city in gene B as the end point
                    byte cityB = newGeneB[rand1];
                    // find the index of the end point
                    tempIndex = Array.IndexOf(newGeneA, cityB);

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
                checkChild(temp);


                // check if Gene B works
                temp = new GeneticChild(newGeneB, newGeneB.Length);
                checkChild(temp);
                
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
                checkChild(temp);

                //children[i] = temp;

            }

			//return children;
		}

        private void checkChild(GeneticChild temp)
        {
            if (temp.valid)
            {
                temp.calcScore(ref this.initialCityArray);
                childNum += 1;
                if (temp.score < b1Score)
                {
                    best2Child = best1Child;
                    best1Child = temp;
                    b2Score = b1Score;
                    b1Score = temp.score;
                    if (b1Score < bssf)
                    {
                        count += 1;
                        bssf = b1Score;
                        bestGene = best1Child.gene;
                    }
                }
                if (childNum == rand1Index)
                {
                    rand1Child = temp;
                }
                if (childNum == rand2Index)
                {
                    rand2Child = temp;
                }
            }
        }

		public double updateCostForChild(GeneticChild child)
		{
			// update the child

			return 0.0;
		}

        public byte[] getBestRoute()
        {
            return this.bestGene;
        }


		public string getCost()
		{
			return bssf.ToString();
		}

		public string getTime()
		{
			return timer.Elapsed.ToString();
        }

		public string getCount()
		{
			return this.count.ToString();
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
