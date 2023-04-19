using System;
using System.Collections.Generic;

namespace graphsearch
{
    public class Graph<T>
    {
        //slovník na ukládání vrcholů a jejich sousedů
        public Dictionary<T, HashSet<T>> Neighbors { get; } = new Dictionary<T, HashSet<T>>();


        //konstruktor
        public Graph() { }
        public Graph(IEnumerable<T> vertices, IEnumerable<Tuple<T, T>> edges)
        {
            foreach (var vertex in vertices)
                AddVertex(vertex);

            foreach (var edge in edges)
                AddEdge(edge);
        }

        //přidání vrcholu
        public void AddVertex(T vertex)
        {
            Neighbors[vertex] = new HashSet<T>();
        }

        //přidání sousedů vrcholu
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
        public HashSet<T> BFS<T>(Graph<T> graph, T beginning)
        {
            //navštívené vrcholy
            HashSet<T> visited = new();

            //jestli je v grafu vůbec obsažen vrchol ze kterého chceme začit (pokud ne, "visited" se vrátí prázdné)
            if (!graph.Neighbors.ContainsKey(beginning))
                return visited;

            Queue<T> queue = new();
            //dáme si tam první vrchol kterým chceme začit
            queue.Enqueue(beginning);

            while (queue.Count > 0)
            {
                //první uložený vrchol ve frontě se uloží
                var vertex = queue.Dequeue();

                //pokud to dojelo tam kde to už bylo, tak pokračuje na další vrchol ve frontě
                if (visited.Contains(vertex))
                    continue;

                //přidání
                visited.Add(vertex);

                //vkládání do fronty vrcholů z leva do prava (BFS)
                foreach (var neighbor in graph.Neighbors[vertex])
                    if (!visited.Contains(neighbor))
                        queue.Enqueue(neighbor);
            }
            //to je snad jasný
            return visited;
        }

        public Func<T, IEnumerable<T>> BFSShortestPath<T>(Graph<T> graph, T beginning)
        {
            var previous = new Dictionary<T, T>();

            //nová fronta na vrcholi do které so vloží začáteční vrchol
            var queue = new Queue<T>();
            queue.Enqueue(beginning);

            while (queue.Count > 0)
            {
                //první uložený vrchol ve frontě se uloží
                var vertex = queue.Dequeue();
                //projde všechny sousedy vrcholu na kterém právě jsme
                foreach (var neighbor in graph.Neighbors[vertex])
                {
                    //zkontroluje jestli jsme se nedostali někam kde jsme už byli
                    if (previous.ContainsKey(neighbor))
                        continue;
                    //uloží to další vrchol a přidá ho i do fronty
                    previous[neighbor] = vertex;
                    queue.Enqueue(neighbor);
                }
            }

            //funkce na vyhledání nejkratší cesty z bodu A do bodu B
            Func<T, IEnumerable<T>> shortestPath = v => {
                //vytvoření cesty vrcholů
                var path = new List<T> { };

                var current = v;
                //to asi popíšu u tabule
                while (!current.Equals(beginning))
                {
                    path.Add(current);
                    current = previous[current];
                };

                path.Add(beginning);
                path.Reverse();

                return path;
            };

            return shortestPath;
        }

        public HashSet<T> DFS<T>(Graph<T> graph, T begining)
        {
            //navštívené vrcholy
            var visited = new HashSet<T>();

            //jestli je v grafu vůbec obsažen vrchol ze kterého chceme začit (pokud ne, "visited" se vrátí prázdné)
            if (!graph.Neighbors.ContainsKey(begining))
                return visited;

            //vytvoření stacku a vložení vrcholu odkud chceme vyhledávat
            Stack<T> stack = new();
            stack.Push(begining);

            while (stack.Count > 0)
            {
                //uloží tam posledně přidaný vrchol
                var vertex = stack.Pop();

                //pokud to dojelo tam kde to už bylo, tak pokračuje na další vrchol ve stacku
                if (visited.Contains(vertex))
                    continue;

                //přidá kde byl
                visited.Add(vertex);

                //vkládání do stacku vrcholů ze shora dolů (DFS)
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
            //seznam všech vrcholů a spojení
            int[] vertices = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12 };
            var edges = new[]{Tuple.Create(1,2), Tuple.Create(1,3),
                Tuple.Create(2,4), Tuple.Create(3,5), Tuple.Create(3,6),
                Tuple.Create(4,7), Tuple.Create(5,7), Tuple.Create(5,8),
                Tuple.Create(5,6), Tuple.Create(8,9), Tuple.Create(9,10), Tuple.Create(9,11)};

            //zhmotnění grafu a algoritmů
            Graph<int> graph = new(vertices, edges);
            Algorithms algorithms = new();

            //ukázka
            int beginingVertex = 1;
            var shortestPath = algorithms.BFSShortestPath(graph, beginingVertex);
            foreach (var vertex in vertices)
                Console.WriteLine("Nejkratší cesta k {0} je {1}", vertex, string.Join(", ", shortestPath(vertex)));

            Console.WriteLine(string.Join(", ", algorithms.DFS(graph, beginingVertex)));
            Console.WriteLine(string.Join(", ", algorithms.BFS(graph, beginingVertex)));
        }
    }
}
