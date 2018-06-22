using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra_s_Algorithm
{
    /// <summary>
    /// Ребро графа
    /// </summary>
    class Edge
    {
        public int v1, v2; //Номера вершин, инцидентных ребру
        public int weight; //вес ребра;

        public Edge(int v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}
