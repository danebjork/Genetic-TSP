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
		private static int Generations = 200;
        private int childNum;
        private int count;
        private double bssf;
        private byte[] bestGene;
        //private List<GeneticChild> children; Don't need this if we just save the 4 children we want
		private GeneticChild best1Child;
        private double b1Score;
        private GeneticChild best2Child;
        private double b2Score;
        private GeneticChild best3Child;
        private double b3Score;
        private GeneticChild best4Child;
        private double b4Score;
        private GeneticChild best5Child;
        private double b5Score;
        private GeneticChild rand1Child;
        private int rand1Index;
        private GeneticChild rand2Child;
        private int rand2Index;
        private GeneticChild rand3Child;
        private int rand3Index;
        private GeneticChild rand4Child;
        private int rand4Index;
        private GeneticChild rand5Child;
        private int rand5Index;
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
            b3Score = Double.PositiveInfinity;
            b4Score = Double.PositiveInfinity;
            b5Score = Double.PositiveInfinity;
            this.rnd = new Random();
            // This represents the number of children attempted to create in
            // each of 10 functions. If iterations = 50, then children is about 500.
            this.iterations = 20; 

        }

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
                Console.WriteLine("GENERATION: {0}, score: {1}", generation, bssf);
                int makeRandom = 300;
                // choose 4 best
                b1Score = Double.PositiveInfinity;
                b2Score = Double.PositiveInfinity;
                b3Score = Double.PositiveInfinity;
                b4Score = Double.PositiveInfinity;
                b5Score = Double.PositiveInfinity;
                GeneticChild parent1 = this.best1Child;
                GeneticChild parent2 = this.best2Child;
                GeneticChild parent3 = this.best3Child;
                GeneticChild parent4 = this.best4Child;
                GeneticChild parent5 = this.best5Child;
                GeneticChild parent6 = this.rand1Child;
                GeneticChild parent7 = this.rand2Child;
                GeneticChild parent8 = this.rand3Child;
                GeneticChild parent9 = this.rand4Child;
                GeneticChild parent10 = this.rand5Child;
                rand1Index = rnd.Next(0, makeRandom);
                rand2Index = rnd.Next(0, makeRandom);
                rand3Index = rnd.Next(0, makeRandom);
                rand4Index = rnd.Next(0, makeRandom);
                rand5Index = rnd.Next(0, makeRandom);
                childNum = 0;
                // cross each of the parents with each other
                // cross 1
                cross(parent1, parent2);
                cross(parent1, parent3);
                cross(parent1, parent4);
                cross(parent1, parent5);
                cross(parent1, parent6);
                cross(parent1, parent7);
                cross(parent1, parent8);
                cross(parent1, parent9);
                cross(parent1, parent10);
                // cross 2
                cross(parent2, parent3);
                cross(parent2, parent4);
                cross(parent2, parent5);
                cross(parent2, parent6);
                cross(parent2, parent7);
                cross(parent2, parent8);
                cross(parent2, parent9);
                cross(parent2, parent10);
                //cross 3
                cross(parent3, parent4);
                cross(parent3, parent5);
                cross(parent3, parent6);
                cross(parent3, parent7);
                cross(parent3, parent8);
                cross(parent3, parent9);
                cross(parent3, parent10);
                //cross 4
                cross(parent4, parent5);
                cross(parent4, parent6);
                cross(parent4, parent7);
                cross(parent4, parent8);
                cross(parent4, parent9);
                cross(parent4, parent10);
                //cross 5
                cross(parent5, parent6);
                cross(parent5, parent7);
                cross(parent5, parent8);
                cross(parent5, parent9);
                cross(parent5, parent10);
                //cross 6
                cross(parent6, parent7);
                cross(parent6, parent8);
                cross(parent6, parent9);
                cross(parent6, parent10);
                //cross 7
                cross(parent7, parent8);
                cross(parent7, parent9);
                cross(parent7, parent10);
                //cross 8
                cross(parent8, parent9);
                cross(parent8, parent10);
                //cross 9 and 10
                cross(parent9, parent10);

                // mutate each of the parents into children
                mutate(parent1);
                mutate(parent2);
                mutate(parent3);
                mutate(parent4);
                mutate(parent5);
                mutate(parent6);
                mutate(parent7);
                mutate(parent8);
                mutate(parent9);
                mutate(parent10);
            }
            timer.Stop();
		}


		public void initialRound()
		{

            // run 500 times and add to children, used a modified /TSP default algorithm.
            int population = 2000;
            childNum = 0;
            HashSet<double> test = new HashSet<double>();
            // save the two random children for genetic variation
            Random rnd = new Random();
            rand1Index = rnd.Next(0, population/2);
            rand2Index = rnd.Next(0, population/2);
            rand3Index = rnd.Next(0, population/2);
            rand4Index = rnd.Next(0, population/2);
            rand5Index = rnd.Next(0, population/2);
            while (b5Score == Double.PositiveInfinity || childNum < population)
            {
                // add to children
                GeneticChild temp = randomSolver();
                checkChild(temp);
                Console.WriteLine(b5Score);
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
                newGene = (byte[])child.gene.Clone();
                int rand1, rand2, randIter;
                randIter = rnd.Next(1, 10);
                for(int j = 0; j < randIter; j++)
                {
                    do
                    {
                        rand1 = rnd.Next(0, child.gene.Length);
                        rand2 = rnd.Next(0, child.gene.Length);
                    }
                    while (rand2 == rand1);

                    // get the city at that index
                    byte city = newGene[rand1];
                    newGene[rand1] = newGene[rand2];
                    newGene[rand2] = city;
                }
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
                    best5Child = best4Child;
                    best4Child = best3Child;
                    best3Child = best2Child;
                    best2Child = best1Child;
                    best1Child = temp;
                    b5Score = b4Score;
                    b4Score = b3Score;
                    b3Score = b2Score;
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
                if (childNum == rand3Index)
                {
                    rand3Child = temp;
                }
                if (childNum == rand4Index)
                {
                    rand4Child = temp;
                }
                if (childNum == rand5Index)
                {
                    rand5Child = temp;
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
