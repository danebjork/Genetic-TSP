using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;


namespace TSP
{

    class ProblemAndSolver
    {

        private class TSPSolution
        {
            /// <summary>
            /// we use the representation [cityB,cityA,cityC] 
            /// to mean that cityB is the first city in the solution, cityA is the second, cityC is the third 
            /// and the edge from cityC to cityB is the final edge in the path.  
            /// You are, of course, free to use a different representation if it would be more convenient or efficient 
            /// for your data structure(s) and search algorithm. 
            /// </summary>
            public ArrayList
                Route;

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="iroute">a (hopefully) valid tour</param>
            public TSPSolution(ArrayList iroute)
            {
                Route = new ArrayList(iroute);
            }

            /// <summary>
            /// Compute the cost of the current route.  
            /// Note: This does not check that the route is complete.
            /// It assumes that the route passes from the last city back to the first city. 
            /// </summary>
            /// <returns></returns>
            public double costOfRoute()
            {
                // go through each edge in the route and add up the cost. 
                int x;
                City here;
                double cost = 0D;

                for (x = 0; x < Route.Count - 1; x++)
                {
                    here = Route[x] as City;
                    cost += here.costToGetTo(Route[x + 1] as City);
                }

                // go from the last city to the first. 
                here = Route[Route.Count - 1] as City;
                cost += here.costToGetTo(Route[0] as City);
                return cost;
            }
        }

        #region Private members 

        /// <summary>
        /// Default number of cities (unused -- to set defaults, change the values in the GUI form)
        /// </summary>
        // (This is no longer used -- to set default values, edit the form directly.  Open Form1.cs,
        // click on the Problem Size text box, go to the Properties window (lower right corner), 
        // and change the "Text" value.)
        private const int DEFAULT_SIZE = 25;

        /// <summary>
        /// Default time limit (unused -- to set defaults, change the values in the GUI form)
        /// </summary>
        // (This is no longer used -- to set default values, edit the form directly.  Open Form1.cs,
        // click on the Time text box, go to the Properties window (lower right corner), 
        // and change the "Text" value.)
        private const int TIME_LIMIT = 60;        //in seconds

        private const int CITY_ICON_SIZE = 5;


        // For normal and hard modes:
        // hard mode only
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;

        /// <summary>
        /// the cities in the current problem.
        /// </summary>
        private City[] Cities;
        /// <summary>
        /// a route through the current problem, useful as a temporary variable. 
        /// </summary>
        private ArrayList Route;
        /// <summary>
        /// best solution so far. 
        /// </summary>
        private TSPSolution bssf; 

        /// <summary>
        /// how to color various things. 
        /// </summary>
        private Brush cityBrushStartStyle;
        private Brush cityBrushStyle;
        private Pen routePenStyle;


        /// <summary>
        /// keep track of the seed value so that the same sequence of problems can be 
        /// regenerated next time the generator is run. 
        /// </summary>
        private int _seed;
        /// <summary>
        /// number of cities to include in a problem. 
        /// </summary>
        private int _size;

        /// <summary>
        /// Difficulty level
        /// </summary>
        private HardMode.Modes _mode;

        /// <summary>
        /// random number generator. 
        /// </summary>
        private Random rnd;

        /// <summary>
        /// time limit in milliseconds for state space search
        /// can be used by any solver method to truncate the search and return the BSSF
        /// </summary>
        private int time_limit;
        #endregion

        #region Public members

        /// <summary>
        /// These three constants are used for convenience/clarity in populating and accessing the results array that is passed back to the calling Form
        /// </summary>
        public const int COST = 0;           
        public const int TIME = 1;
        public const int COUNT = 2;
        
        public int Size
        {
            get { return _size; }
        }

        public int Seed
        {
            get { return _seed; }
        }
        #endregion

        #region Constructors
        public ProblemAndSolver()
        {
            this._seed = 1; 
            rnd = new Random(1);
            this._size = DEFAULT_SIZE;
            this.time_limit = TIME_LIMIT * 1000;                  // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }

        public ProblemAndSolver(int seed)
        {
            this._seed = seed;
            rnd = new Random(seed);
            this._size = DEFAULT_SIZE;
            this.time_limit = TIME_LIMIT * 1000;                  // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }

        public ProblemAndSolver(int seed, int size)
        {
            this._seed = seed;
            this._size = size;
            rnd = new Random(seed);
            this.time_limit = TIME_LIMIT * 1000;                        // TIME_LIMIT is in seconds, but timer wants it in milliseconds

            this.resetData();
        }
        public ProblemAndSolver(int seed, int size, int time)
        {
            this._seed = seed;
            this._size = size;
            rnd = new Random(seed);
            this.time_limit = time*1000;                        // time is entered in the GUI in seconds, but timer wants it in milliseconds

            this.resetData();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Reset the problem instance.
        /// </summary>
        private void resetData()
        {

            Cities = new City[_size];
            Route = new ArrayList(_size);
            bssf = null;

            if (_mode == HardMode.Modes.Easy)
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble());
            }
            else // Medium and hard
            {
                for (int i = 0; i < _size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble() * City.MAX_ELEVATION);
            }

            HardMode mm = new HardMode(this._mode, this.rnd, Cities);
            if (_mode == HardMode.Modes.Hard)
            {
                int edgesToRemove = (int)(_size * FRACTION_OF_PATHS_TO_REMOVE);
                mm.removePaths(edgesToRemove);
            }
            City.setModeManager(mm);

            cityBrushStyle = new SolidBrush(Color.Black);
            cityBrushStartStyle = new SolidBrush(Color.Red);
            routePenStyle = new Pen(Color.Blue,1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// make a new problem with the given size.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode)
        {
            this._size = size;
            this._mode = mode;
            resetData();
        }

        /// <summary>
        /// make a new problem with the given size, now including timelimit paremeter that was added to form.
        /// </summary>
        /// <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode, int timelimit)
        {
            this._size = size;
            this._mode = mode;
            this.time_limit = timelimit*1000;                                   //convert seconds to milliseconds
            resetData();
        }

        /// <summary>
        /// return a copy of the cities in this problem. 
        /// </summary>
        /// <returns>array of cities</returns>
        public City[] GetCities()
        {
            City[] retCities = new City[Cities.Length];
            Array.Copy(Cities, retCities, Cities.Length);
            return retCities;
        }

        /// <summary>
        /// draw the cities in the problem.  if the bssf member is defined, then
        /// draw that too. 
        /// </summary>
        /// <param name="g">where to draw the stuff</param>
        public void Draw(Graphics g)
        {
            float width  = g.VisibleClipBounds.Width-45F;
            float height = g.VisibleClipBounds.Height-45F;
            Font labelFont = new Font("Arial", 10);

            // Draw lines
            if (bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[bssf.Route.Count];
                int index = 0;
                foreach (City c in bssf.Route)
                {
                    if (index < bssf.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    ps[index++] = new Point((int)(c.X * width) + CITY_ICON_SIZE / 2, (int)(c.Y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)Cities[0].X * width - 1, (float)Cities[0].Y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in Cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.X * width, (float)c.Y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }

        }

        /// <summary>
        ///  return the cost of the best solution so far. 
        /// </summary>
        /// <returns></returns>
        public double costOfBssf ()
        {
            if (bssf != null)
                return (bssf.costOfRoute());
            else
                return -1D; 
        }

        /// <summary>
        /// This is the entry point for the default solver
        /// which just finds a valid random tour 
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] defaultSolveProblem()
        {
            int i, swap, temp, count=0;
            string[] results = new string[3];
            int[] perm = new int[Cities.Length];
            Route = new ArrayList();
            Random rnd = new Random();
            Stopwatch timer = new Stopwatch();

            timer.Start();

            do
            {
                for (i = 0; i < perm.Length; i++)                                 // create a random permutation template
                    perm[i] = i;
                for (i = 0; i < perm.Length; i++)
                {
                    swap = i;
                    while (swap == i)
                        swap = rnd.Next(0, Cities.Length);
                    temp = perm[i];
                    perm[i] = perm[swap];
                    perm[swap] = temp;
                }
                Route.Clear();
                for (i = 0; i < Cities.Length; i++)                            // Now build the route using the random permutation 
                {
                    Route.Add(Cities[perm[i]]);
                }
                bssf = new TSPSolution(Route);
                count++;
            } while (costOfBssf() == double.PositiveInfinity);                // until a valid route is found
            timer.Stop();

            results[COST] = costOfBssf().ToString();                          // load results array
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = count.ToString();

            return results;
        }

        // takes in a list of nodes from a DistMatrix
        // and returns an ArrayList of Cities as a route.
        // O(n)
        public ArrayList buildRoute(List<int> nodeList)
        {
            ArrayList returnRoute = new ArrayList();
            for (int i = 0; i < nodeList.Count; i++)
            {
                returnRoute.Add(Cities[nodeList[i]]);
            }
            return returnRoute;
        }

        /// <summary>
        /// performs a Branch and Bound search of the state space of partial tours
        /// stops when time limit expires and uses BSSF as solution
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        // This algorithm implements a greedy approach, 10,000 random attempts, and the branch and bound approach with pruning of leaf
        // nodes that have values grater than the current BSSF.
        // O = greedy + 10,000*random + branch and bound
        // = O(n^3 + 10000*n + (2^n)*(n^2) = O((n^2)*(2^n))
        public string[] bBSolveProblem()
        {
            // initialize the variables
            int count = 0;
            string[] results = new string[3];
            int[] perm = new int[Cities.Length];
            Route = new ArrayList();
            Stopwatch timer = new Stopwatch();
            int maxStates = 0;
            int totalStates = 0;
            int statesPruned = 0;
            // start the timer
            timer.Start();
            // start BSSF as positive infinity;
            double currSol = Double.PositiveInfinity;
            // see if the BSSF updates with the greedy algorithm
            // Finding greedy solution = O(n^3)
            double greedySol = Convert.ToDouble(greedySolveProblem()[0]);
            if(greedySol < currSol)
            {
                currSol = greedySol;
                count += 1;
            }
            // attempt updating the BSSF with 10,000 random solutions
            // finding random solution = O(n)
            for (int cycleRand = 0; cycleRand < 10000; cycleRand++)
            {
                double tempSol = Convert.ToInt32(defaultSolveProblem()[0]);
                if (tempSol < currSol)
                {
                    currSol = tempSol;
                    count += 1;
                }
            }
            // create the List of nodes
            List<DistMatrix> children = new List<DistMatrix>();
            // initialize the first node in the list (root node)
            children.Add(new DistMatrix(Cities));

            // while there are nodes in the node list
            while (children.Count > 0)
            {
                // check if the timer has passed 60 seconds
                timer.Stop();
                long time = timer.ElapsedMilliseconds / 1000;
                if (time >= 60)
                {
                    // if it has passed 60 seconds, break out of the while loop
                    break;
                }
                timer.Start();

                // Find the next node - O(n)
                if (maxStates < children.Count)
                {
                    maxStates = children.Count;
                }
                // collect the variables to find the best child node
                DistMatrix currNode = children[0];
                double best = children[0].b;
                int numVisited = children[0].visited.Count;
                // for each child node, check if it is optimal
                for (int i = 0; i < children.Count; i++)
                {
                    double value = children[i].b;
                    int visited = children[i].visited.Count;
                    double diffValue = value - best;
                    int diffVisit = visited - numVisited;
                    // used this little algorithm to try and balance breadth and depth
                    if (value < best * (1 + (diffVisit / .75)))
                    {
                        currNode = children[i];
                        best = children[i].b;
                        numVisited = children[i].visited.Count;
                    }
                }
                // remove that node from the list of possiblities
                children.RemoveAt(children.IndexOf(currNode));
                // if the child node has not reached the end
                if (!currNode.finished())
                {
                    // get the list of nodes that haven't been visited
                    List<int> nextChildren;
                    nextChildren = currNode.returnNodesLeft();
                    // create a child object for each of those nodes - O(n^2) happening 2^n times
                    // for the worst case scenario where each leaf must be reached in the tree.
                    // This gives a O((n^2)*(2^n))
                    for (int i = 0; i < nextChildren.Count; i++)
                    {
                        DistMatrix child = new DistMatrix(nextChildren[i], currNode);
                        children.Add(new DistMatrix(nextChildren[i], currNode));
                        totalStates += 1;
                    }
                }
                // if the node is a leaf, check if it's better than the current BSSF
                //O(n) to check each one, with O(n) to check if any need removing after
                // an update on the BSSF.
                else
                {
                    if (currNode.b < currSol)
                    {
                        // if it is better, replace the current BSSF
                        ArrayList tempRoute = buildRoute(currNode.visited);
                        TSPSolution test = new TSPSolution(tempRoute);
                        if (test.costOfRoute() < currSol)
                        {
                            Route.Clear();
                            Route = buildRoute(currNode.visited);
                            bssf = null;
                            bssf = new TSPSolution(Route);
                            currSol = costOfBssf();
                            count += 1;
                        }
                        // filter any children that has a b greater than the current bssf.
                        // remove those children in hopes of speeding up the algorithm

                    }
                }
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].b > currSol)
                    {
                        children.RemoveAt(i);
                        statesPruned += 1;
                        i--;
                    }
                }

            }

            // Stop the time and report the results.
            timer.Stop();
            results[COST] = costOfBssf().ToString(); // load results array
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = count.ToString();
            Console.WriteLine("Max States: {0}, Total States: {1}, Total Pruned: {2}", maxStates, totalStates, statesPruned);
            return results;
        }

        // creates a greedy path given a starting node
        // complexity - O(n^2) - cycles through each node for each node
        // and selects the smallest path in that list.
        public ArrayList findGreedyPath(int node)
        {
            // create return route
            ArrayList retRoute = new ArrayList();
            retRoute.Add(Cities[node]);
            // while the route isn't finished
            while(retRoute.Count < Cities.Length)
            {
                // find the current best node to go to
                double bestDistance = Double.PositiveInfinity;
                int bestNode = -1;
                for(int i = 0; i < Cities.Length; i++)
                {
                    if(!retRoute.Contains(Cities[i]))
                    {
                        double dist = (retRoute[retRoute.Count - 1] as City).costToGetTo(Cities[i]);
                        if(dist < bestDistance)
                        {
                            bestDistance = dist;
                            bestNode = i;
                        }
                    }
                }
                // go to that best node
                if(bestNode != -1)
                {
                    retRoute.Add(Cities[bestNode]);
                }
                // if there were infinities, return a null path
                else
                {
                    return null;
                }
            }
            return retRoute;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // These additional solver methods will be implemented as part of the group project.
        ////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// finds the greedy tour starting from each city and keeps the best (valid) one
        /// </summary>
        /// <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        // O(n^3) - goes through each city for each city, with each city as a root node - n*n*n
        public string[] greedySolveProblem()
        {
            // initialize the vairables
            int count = 0;
            string[] results = new string[3];
            int[] perm = new int[Cities.Length];
            Route = new ArrayList();
            double currSol = Double.PositiveInfinity;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            // for each city as a starting node, find the best greedy solution
            // greedy for one node - O(n^2), doing that for each city as a root:
            // O(n^3)
            for(int i = 0; i < Cities.Length; i++)
            {
                ArrayList newRoute = findGreedyPath(i);
                TSPSolution greedyRoute = new TSPSolution(newRoute);
                double temp = costOfBssf();
                if(temp < currSol)
                {
                    currSol = temp;
                    Route = newRoute;
                    bssf = greedyRoute;
                }
            }
            timer.Stop();

            results[COST] = costOfBssf().ToString();                          // load results array
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = count.ToString();

            return results;
        }

        public string[] fancySolveProblem()
        {
            string[] results = new string[3];

            // TODO: Add your implementation for your advanced solver here.

            results[COST] = "not implemented";    // load results into array here, replacing these dummy values
            results[TIME] = "-1";
            results[COUNT] = "-1";

            return results;
        }
        #endregion
    }
}
