using System;
namespace TSP
{
	public class GeneticSolver
	{
		private City[] initialCityArray;
		private int Size;

		private static int Generations = 100;


		private GeneticChild bestChild;

		public GeneticSolver(ref City[] cities)
		{
			this.initialCityArray = cities;
			this.Size = cities.Length;

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
			// run 500 times and add to children
			for (int i = 0; i < 500; i++)
			{
				// add to children

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




		/// initial double matrix for reachability and calculating

		public double[,] fillCitiesReducedCostMatrix()
		{
		    double[,] matrix = new double[Size, Size];

		    for (int row = 0; row < Size; row++)
		    {
		        for (int col = 0; col < Size; col++)
		        {
		            City fromCity = initialCityArray[row];
		            City toCity = initialCityArray[col];

		            double cost = Double.PositiveInfinity;
		            if (row != col)
		            {
		                cost = fromCity.costToGetTo(toCity);
		            }

		            matrix[col, row] = cost;
		        }
		    }
		    return matrix;
		}




		public double reduceMatrix(ref double[,] matrix)
		{
			double boundShavedOff = 0;
			boundShavedOff += rowReduce(ref matrix);
			boundShavedOff += colReduce(ref matrix);

			return boundShavedOff;
		}

		public double rowReduce(ref double[,] matrix)
		{
			double boundShavedOff = 0;

			for (int row = 0; row < Size; row++)
			{
				double lowestValue = Double.PositiveInfinity;

				for (int col = 0; col < Size; col++)
				{
					double value = matrix[col, row];

					if (value < lowestValue)
					{
						lowestValue = value;
					}
				}

				if (!Double.IsPositiveInfinity(lowestValue))
				{
					boundShavedOff += lowestValue;
				}

				for (int col = 0; col < Size; col++)
				{
					double value = matrix[col, row];

					if (!Double.IsPositiveInfinity(value))
					{
						matrix[col, row] = value - lowestValue;
					}
				}
			}

			return boundShavedOff;
		}

		public double colReduce(ref double[,] matrix)
		{
			double boundShavedOff = 0;

			for (int col = 0; col < Size; col++)
			{
				double lowestValue = Double.PositiveInfinity;

				for (int row = 0; row < Size; row++)
				{
					double value = matrix[col, row];

					if (value < lowestValue)
					{
						lowestValue = value;
					}
				}

				if (!Double.IsPositiveInfinity(lowestValue))
				{
					boundShavedOff += lowestValue;
				}

				for (int row = 0; row < Size; row++)
				{
					double value = matrix[col, row];

					if (!Double.IsPositiveInfinity(value))
					{
						matrix[col, row] = value - lowestValue;
					}
				}
			}
			return boundShavedOff;
		}






	}
}
