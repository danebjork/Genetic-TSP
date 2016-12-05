using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP
{
  public class HeapPriorityQueue
  {
	SortedSet<GeneticChild> nodes;

    double currentLowerBound;

    public int maxNumberOfStates;

    public HeapPriorityQueue()
    {
			this.nodes = new SortedSet<GeneticChild>(new GeneticChildComparer());
      this.currentLowerBound = Double.PositiveInfinity;

      this.maxNumberOfStates = 0;
    }

    // O(log(V))
    public bool insert(GeneticChild node)
    {
      if (node.score < currentLowerBound)
      {
        nodes.Add(node);

        if (nodes.Count > maxNumberOfStates)
        {
          maxNumberOfStates = nodes.Count;
        }

        return true;
      }
      return false;
    }

    // O(log(V))
    public GeneticChild decreaseKey()
    {
      return nodes.First();
    }

    public int count()
    {
      return nodes.Count;
    }

    // O(log(V))
    public GeneticChild deleteMin()
    {
      GeneticChild v = decreaseKey();
      bool removed = nodes.Remove(v);

      if (!removed)
      {
        System.Console.WriteLine("deleteMin failed");
      }
      return v;
    }

    public bool removeNode(GeneticChild v)
    {
      return nodes.Remove(v);
    }

    // O(1)
    public bool isEmpty()
    {
      return (nodes.Count == 0);
    }

      public int prune(double boundToPrune)
      {
        int beforeCount = nodes.Count;
      currentLowerBound = boundToPrune;
      nodes.RemoveWhere(ShouldRemoveNode);

      return beforeCount - nodes.Count;
      }

    public bool ShouldRemoveNode(GeneticChild node)
    {
      return node.score >= currentLowerBound;
    }

  }

  internal class SortedIndex
  {
    public double Comparable { get; set; }
    public int Index { get; set; }
  }

  internal class SortedIndexComparar : IComparer<SortedIndex>
  {
    public int Compare(SortedIndex x, SortedIndex y)
    {
      return x.Comparable.CompareTo(y.Comparable);
    }
  }
}
