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
		private static int Generations = 1400;
        private int childNum;
        private int count;
        private double bssf;
        private byte[] bestGene;
        private Double[] bestScores;
        private int[] randInts;
        private GeneticChild[] bestChildren;
        private GeneticChild[] randChildren;
        private GeneticChild[] parents;
        private Stopwatch timer;
        private int bestNum;
        private int randNum;

        private int iterations;

        public GeneticSolver(ref City[] cities)
        {
            count = 0;
            this.bssf = Double.PositiveInfinity;
            childNum = 0;
            this.initialCityArray = cities;
            this.Size = cities.Length;
            this.rnd = new Random();
            // This represents the number of children attempted to create in
            // each of 10 functions. If iterations = 50, then children is about 500.
            this.iterations = 20;
            this.bestNum = 4;
            this.randNum = 4;

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
                //Console.WriteLine("GENERATION: {0}, Last Best: {1}, score: {2}", generation, this.bestChildren[0].score, bssf);
                int makeRandom = 500;
                // choose 4 best
                childNum = 0;
                bestScores = new Double[bestNum];
                randInts = new int[randNum];
                parents = new GeneticChild[bestNum + randNum];
                for (int j = 0; j < bestNum; j++)
                {
                    parents[j] = this.bestChildren[j];
                }
                for (int j = 0; j < randNum; j++)
                {
                    parents[bestNum + j] = randChildren[j];
                }
                for (int j = 0; j < randInts.Length; j++)
                {
                    randInts[j] = rnd.Next(0, makeRandom);
                }
                for (int j = 0; j < bestScores.Length; j++)
                {
                    bestScores[j] = Double.PositiveInfinity;
                }
                this.bestChildren = new GeneticChild[bestNum];
                randChildren = new GeneticChild[bestNum];
                for(int i = 0; i < parents.Length; i++)
                {
                    for(int j = i+1; j < parents.Length; j++)
                    {
                        if(parents[i] != null && parents[j] != null)
                        cross(parents[i], parents[j]);
                    }
                }
                for(int i = 0; i < parents.Length; i++)
                {
                    if(parents[i] != null)
                    {
                        mutate(parents[i]);
                    }
                }
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
            int makeRandom = 300;
            childNum = 0;
            bestScores = new Double[bestNum];
            randInts = new int[randNum];
            for (int j = 0; j < randInts.Length; j++)
            {
                randInts[j] = rnd.Next(0, makeRandom);
            }
            for (int j = 0; j < bestScores.Length; j++)
            {
                bestScores[j] = Double.PositiveInfinity;
            }
            this.bestChildren = new GeneticChild[bestNum];
            randChildren = new GeneticChild[bestNum];
            while (bestScores[bestScores.Length-1] == Double.PositiveInfinity || childNum < population)
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
		}

        // randomly create an integer value representing the index value to mutate
        // mutate that value to a new value swapping with another value.
		public void mutate(GeneticChild child)
		{        
			for (int i = 0; i < this.iterations; i++)
			{
                // mutate the child by
                // swapping nodes then
                // add to children
                byte[] newGene = new byte[child.gene.Length];
                newGene = (byte[])child.gene.Clone();
                int rand1, rand2, randIter;
                randIter = rnd.Next(0, 10);
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
            }
		}

        private void checkChild(GeneticChild temp)
        {
            if (temp.valid)
            {
                temp.calcScore(ref this.initialCityArray);
                childNum += 1;
                shiftScores(temp, temp.score);
                checkRand(temp);
                if (bestScores[0] < bssf)
                {
                    count += 1;
                    bssf = bestScores[0];
                    bestGene = this.bestChildren[0].gene;
                }
            }
        }

        public void checkRand(GeneticChild node)
        {
            for(int i = 0; i < randInts.Length; i++)
            {
                if(childNum == randInts[i])
                {
                    randChildren[i] = node;
                }
            }
        }
        public void shiftScores(GeneticChild node, double score)
        {
            for(int i = 0; i < bestScores.Length; i++)
            {
                if (score < bestScores[i])
                {

                    double tempScore = bestScores[i];
                    bestScores[i] = score;
                    score = tempScore;

                    GeneticChild tempNode = this.bestChildren[i];
                    this.bestChildren[i] = node;
                    node = tempNode;
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
