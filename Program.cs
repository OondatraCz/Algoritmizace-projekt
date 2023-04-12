using System;
using System.Collections.Generic;

namespace graphsearch
{
    public class Graph<T>
    {
        public Dictionary<T, HashSet<T>> Neighbors { get; } = new Dictionary<T, HashSet<T>>();

        public Graph() { }
        public Graph(IEnumerable<T> vertices, IEnumerable<Tuple<T, T>> edges)
        {
            foreach (var vertex in vertices)
                AddVertex(vertex);

            foreach (var edge in edges)
                AddEdge(edge);
        }

        public void AddVertex(T vertex)
        {
            Neighbors[vertex] = new HashSet<T>();
        }

        public void AddEdge(Tuple<T, T> edge)
        {
            if (Neighbors.ContainsKey(edge.Item1) && Neighbors.ContainsKey(edge.Item2))
            {
                Neighbors[edge.Item1].Add(edge.Item2);
                Neighbors[edge.Item2].Add(edge.Item1);
            }
        }
    }

    public class Algorithms
    {
        public HashSet<T> BFS<T>(Graph<T> graph, T start, Action<T> preVisit = null)
        {
            var visited = new HashSet<T>();

            if (!graph.Neighbors.ContainsKey(start))
                return visited;

            var queue = new Queue<T>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                if (visited.Contains(vertex))
                    continue;

                if (preVisit != null)
                    preVisit(vertex);

                visited.Add(vertex);

                foreach (var neighbor in graph.Neighbors[vertex])
                    if (!visited.Contains(neighbor))
                        queue.Enqueue(neighbor);
            }

            return visited;
        }

        public Func<T, IEnumerable<T>> BFSShortestPath<T>(Graph<T> graph, T start)
        {
            var previous = new Dictionary<T, T>();

            var queue = new Queue<T>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();
                foreach (var neighbor in graph.Neighbors[vertex])
                {
                    if (previous.ContainsKey(neighbor))
                        continue;

                    previous[neighbor] = vertex;
                    queue.Enqueue(neighbor);
                }
            }

            Func<T, IEnumerable<T>> shortestPath = v => {
                var path = new List<T> { };

                var current = v;
                while (!current.Equals(start))
                {
                    path.Add(current);
                    current = previous[current];
                };

                path.Add(start);
                path.Reverse();

                return path;
            };

            return shortestPath;
        }

        public HashSet<T> DFS<T>(Graph<T> graph, T start)
        {
            var visited = new HashSet<T>();

            if (!graph.Neighbors.ContainsKey(start))
                return visited;

            var stack = new Stack<T>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var vertex = stack.Pop();

                if (visited.Contains(vertex))
                    continue;

                visited.Add(vertex);

                foreach (var neighbor in graph.Neighbors[vertex])
                    if (!visited.Contains(neighbor))
                        stack.Push(neighbor);
            }

            return visited;
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var vertices = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            var edges = new[]{Tuple.Create(1,2), Tuple.Create(1,3),
                Tuple.Create(2,4), Tuple.Create(3,5), Tuple.Create(3,6),
                Tuple.Create(4,7), Tuple.Create(5,7), Tuple.Create(5,8),
                Tuple.Create(5,6), Tuple.Create(8,9), Tuple.Create(9,10), Tuple.Create(9,11)};

            var graph = new Graph<int>(vertices, edges);
            var algorithms = new Algorithms();

            var startVertex = 1;
            var shortestPath = algorithms.BFSShortestPath(graph, startVertex);
            foreach (var vertex in vertices)
                Console.WriteLine("Nejkratší cesta k {0} je {1}", vertex, string.Join(", ", shortestPath(vertex)));

            Console.WriteLine(string.Join(", ", algorithms.DFS(graph, startVertex)));
            Console.WriteLine(string.Join(", ", algorithms.BFS(graph, startVertex)));
        }
    }
}
