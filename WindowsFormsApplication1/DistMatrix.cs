using System;
using System.Collections.Generic;
using System.Text;

namespace TSP
{

    class DistMatrix
    {
        // to help keep track of index values:
        // row = from
        // col = to
        // declare the class variables
        public double[] m;
        public int maxIndex;
        public List<int> visited;
        public int currNode;
        public int toNode, fromNode;
        public double b;
        // create a constructor for the root of the tree
        // time and space complexity O(n^2)
        public DistMatrix(City[] Cities)
        {
            // initilize the root
            // the root node is always the first node
            this.fromNode = 0;
            this.currNode = 0;
            // the width and height of the matrix m
            this.maxIndex = Cities.Length;
            this.visited = new List<int>();
            this.visited.Add(0);
            this.b = 0;
            this.m = new double[maxIndex * maxIndex];
            // fill in the values of the m matrix based on distances
            createMatrix(Cities);
            // update the matrix to have at least one 0 per row/column
            updateMatrix();
        }

        // creating a child node is O(n^2) for both time and space complexity
        public DistMatrix(int toNode, DistMatrix parent)
        {
            // take in the index from the parent
            this.maxIndex = parent.maxIndex;
            // add the cost of moving from the previous node to the current node
            this.b = parent.b + parent.m[parent.currNode * maxIndex + toNode];
            // create a new m matrix and copy over all the values
            this.m = new double[maxIndex * maxIndex];
            for(int i = 0; i < m.Length; i++)
            {
                this.m[i] = parent.m[i];
            }
            // create a new visited list and copy those values
            this.visited = new List<int>();
            for(int i = 0; i < parent.visited.Count; i++)
            {

                this.visited.Add(parent.visited[i]);
            }
            // update the visited list
            this.visited.Add(toNode);
            this.toNode = toNode;
            this.currNode = toNode;
            this.fromNode = parent.currNode;
            // mark infinity on the column for the node going to (the child)
            markToNode();
            // mark infinities on the row for the node coming from (the parent)
            markFromNode();
            // mark the awkward diagonal cell in the matrix as inifinity
            markFromToNode();
            // make sure there are still 0 values in row/column
            updateMatrix();

        }

        // this function returns a list of nodes that haven't been visited
        // O(n)
        public List<int> returnNodesLeft()
        {
            List<int> returnList = new List<int>();
            for(int i = 0; i < maxIndex; i++)
            {
                if(!this.visited.Contains(i))
                {
                    returnList.Add(i);
                }
            }
            return returnList;
        }

        // returns the next 0 value in the row that this node currently belongs in
        // O(n)
        public int returnNextNode()
        {
            for(int col = 0; col < this.maxIndex; col++)
            {
                if(col != currNode)
                {
                    int currIndex = this.fromNode * maxIndex + col;
                    if (this.m[currIndex] == 0)
                    {
                        return col;
                    }
                }
            }
            return 0;
        }

        // Puts infinity values on the column of the node coming to (child node)
        // O(n)
        private void markToNode()
        {
            for (int row = 0; row < this.maxIndex; row++)
            {
                int currIndex = row * maxIndex + this.toNode;
                this.m[currIndex] = Double.PositiveInfinity;
            }
        }

        // puts infinity values on the row representing the node coming from (parent node)
        // O(n)
        private void markFromNode()
        {
            for(int col = 0; col < this.maxIndex; col++)
            {
                int currIndex = this.fromNode * maxIndex + col;
                this.m[currIndex] = Double.PositiveInfinity;
            }
        }

        // puts an infinity value on the reflected cell as shown in the examples of the project
        // O(1)
        private void markFromToNode()
        {
            int currIndex = this.toNode * maxIndex + this.fromNode;
            this.m[currIndex] = Double.PositiveInfinity;
        }

        // creates a new matrix m, based on the distances from each city to each city
        // Complexity - O(n^2)
        private void createMatrix(City[] Cities)
        {
            for(int row = 0; row < Cities.Length; row++)
            {
                for(int col = 0; col < Cities.Length; col++)
                {
                    int currIndex = row * Cities.Length + col;
                    // put infinity values if trying to go to self.
                    if(row == col)
                    {
                        this.m[currIndex] = Double.PositiveInfinity;
                    }
                    else
                    {
                        this.m[currIndex] = Cities[row].costToGetTo(Cities[col]);
                    }
                }
            }
        }

        // updates rows and columns
        // O(2n^2) = O(n^2)
        private void updateMatrix()
        {
            updateRows();
            updateCols();
        }

        // finds the smallest value in the row and subtract it from 
        // all the other values in the row.
        // then add that subtracted value to the lower bound - b
        // Complexity - O(n^2)
        private void updateRows()
        {
            for (int row = 0; row < maxIndex; row++)
            {
                List<int> indexUpdates = new List<int>();
                double lowest = Double.PositiveInfinity;
                for (int col = 0; col < maxIndex; col++)
                {
                    int currIndex = col * maxIndex + row;
                    if ( !Double.IsPositiveInfinity(this.m[currIndex]))
                    {
                        if (this.m[currIndex] < lowest)
                        {
                            lowest = this.m[currIndex];
                        }
                        indexUpdates.Add(currIndex);
                    }
                }

                // don't add to the lower bound if the only value left is infinity
                if(!Double.IsPositiveInfinity(lowest))
                {
                    for (int i = 0; i < indexUpdates.Count; i++)
                    {
                        this.m[indexUpdates[i]] -= lowest;
                    }
                    this.b += lowest;
                }
            }
        }

        // checks each column if there is a zero value.
        // if not, then it will subtract the smallest value in the
        // column from all the other values in the column.
        // then it will add that small value to the lower bound - b
        // Complexity - O(n^2)
        private void updateCols()
        {
            for (int col = 0; col < maxIndex; col++)
            {
                List<int> indexUpdates = new List<int>();
                double lowest = Double.PositiveInfinity;
                for (int row = 0; row < maxIndex; row++)
                {
                    int currIndex = col * maxIndex + row;
                    if (!Double.IsPositiveInfinity(this.m[currIndex]))
                    {
                        if (this.m[currIndex] < lowest)
                        {
                            lowest = this.m[currIndex];
                        }
                        indexUpdates.Add(currIndex);
                    }
                }
                if (lowest > 0 && !Double.IsPositiveInfinity(lowest))
                {
                    for (int i = 0; i < indexUpdates.Count; i++)
                    {
                        this.m[indexUpdates[i]] -= lowest;
                    }
                    this.b += lowest;
                }

            }
        }

        // checks if the visited list is equal in size to the number
        // of total nodes in the tree. If it is equal, return true.
        // else, return false.
        // O(1)
        public bool finished()
        {
            if(this.visited.Count == maxIndex)
            {
                return true;
            }
            return false;
        }

        // prints out most of the data from the class.
        // used for some serious debugging
        public void printAll()
        {
            Console.WriteLine("VISITED: ");
            for (int i = 0; i < this.visited.Count; i++)
            {
                Console.Write(" {0} ", this.visited[i]);
            }
            Console.WriteLine("");
            for (int row = 0; row < maxIndex; row++)
            {
                for (int col = 0; col < maxIndex; col++)
                {
                    int currIndex = row * maxIndex + col;
                    Console.Write("{0}\t", this.m[currIndex]);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("b: {0}", this.b);
            Console.WriteLine("");

        }
        
    }
}

