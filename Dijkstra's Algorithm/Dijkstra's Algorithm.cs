using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra_s_Algorithm
{
    /// <summary>
    /// Класс для вычисления кратчайших путей на основе алгоритма Дейкстры
    /// </summary>
    public class Dijkstra
    {
        List<int> queue = new List<int>(); //Очередь для хранения непросмотренных вершин
        public Dijkstra(double[,] _adjacencyMatrix, int start, int s)
        {
            initial(start, s);
            while (queue.Count > 0)
            {
                int u = getNextVertex();
                for (int i = 0; i < s; i++)
                {
                    if (_adjacencyMatrix[u, i] > 0)
                    {
                        if (dist[i] > dist[u] + _adjacencyMatrix[u, i])
                        {
                            dist[i] = dist[u] + _adjacencyMatrix[u, i];
                        }
                    }
                }
            }
        }

        public double[] dist { get; set; } //расстояния

        /// <summary>
        /// Следующая вершина
        /// </summary>
        /// <returns></returns>
        int getNextVertex()
        {
            var min = double.PositiveInfinity; //бесконечность. Прямого пути через эти вершинамы не существует
            int vertex = -1;
            foreach (int value in queue)
            {
                if (dist[value] <= min)
                {
                    min = dist[value];
                    vertex = value;
                }
            }
            queue.Remove(vertex);
            return vertex;
        }

        /// <summary>
        /// Начальная инициализация
        /// </summary>
        /// <param name="s"></param>
        /// <param name="len"></param>
        public void initial(int s, int len)
        {
            dist = new double[len];
            for (int i = 0; i < len; i++)
            {
                dist[i] = double.PositiveInfinity;
                queue.Add(i);
            }
            dist[0] = 0;
        }
    }
}
