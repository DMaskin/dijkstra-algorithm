using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Dijkstra_s_Algorithm
{
    /// <summary>
    /// Отрисовка графа
    /// </summary>
    class DrawGraph
    {
        Bitmap bitmap;
        Pen blackPen;
        Pen redPen;
        Pen darkGoldPen;
        Graphics gr;
        Font fo;
        Brush br;
        PointF point;
        TextBox inputWeightTB;
        public int R = 20; //радиус окружности вершины. Первоначальный вариант: 20

        public DrawGraph(int width, int height)
        {
            bitmap = new Bitmap(width, height);
            gr = Graphics.FromImage(bitmap);
            clearSheet();
            blackPen = new Pen(Color.Black);
            blackPen.Width = 2;
            redPen = new Pen(Color.Red);
            redPen.Width = 2;
            darkGoldPen = new Pen(Color.DarkGoldenrod);
            darkGoldPen.Width = 2;
            fo = new Font("Arial", 15);
            br = Brushes.Black;
            inputWeightTB = new TextBox();
        }

        public Bitmap GetBitmap()
        {
            return bitmap;
        }

        public void clearSheet()
        {
            gr.Clear(Color.White);
        }

        /// <summary>
        /// Рисовать вершину
        /// </summary>
        public void drawVertex(int x, int y, string number)
        {
            gr.FillEllipse(Brushes.White, (x - R), (y - R), 2 * R, 2 * R);
            gr.DrawEllipse(blackPen, (x - R), (y - R), 2 * R, 2 * R);
            point = new PointF(x - 9, y - 9);
            gr.DrawString(number, fo, br, point);
        }

        /// <summary>
        /// Подкраска вершины при её выделении
        /// </summary>
        public void drawSelectedVertex(int x, int y)
        {
            gr.DrawEllipse(redPen, (x - R), (y - R), 2 * R, 2 * R);
        }

        /// <summary>
        /// Отрисовка ребра
        /// </summary>
        public void drawEdge(Vertex V1, Vertex V2, Edge edge, List<Edge> Edges, int numberE, byte orientation)
        {
            Point Start = new Point(V1.x, V1.y);
            Point End = new Point(V2.x, V2.y);
            if (orientation == 1)
                DrawArrow(Start, End, darkGoldPen);
            else
                gr.DrawLine(darkGoldPen, Start, End);
            double ugol = Math.Atan2(V1.x - V2.x - R, V1.y - V2.y - R);
            point = new PointF((V1.x + V2.x) / 2, (V1.y + V2.y) / 2);
            gr.DrawString(edge.weight.ToString(), fo, br, point);
            drawVertex(V1.x, V1.y, (edge.v1 + 1).ToString());
            drawVertex(V2.x, V2.y, (edge.v2 + 1).ToString());
        }

        /// <summary>
        /// Отрисовка всего графа
        /// </summary>
        public void drawALLGraph(List<Vertex> V, List<Edge> E, byte orientation)
        {
            //рисуем ребра
            for (int i = 0; i < E.Count; i++)
            {
                Point Start = new Point(V[E[i].v1].x, V[E[i].v1].y);
                Point End = new Point(V[E[i].v2].x, V[E[i].v2].y);
                if (orientation == 1)
                    DrawArrow(Start, End, darkGoldPen);
                else
                    gr.DrawLine(darkGoldPen, Start, End);
                point = new PointF((V[E[i].v1].x + V[E[i].v2].x) / 2, (V[E[i].v1].y + V[E[i].v2].y) / 2);
                gr.DrawString(E[i].weight.ToString(), fo, br, point);
            }
            //рисуем вершины
            for (int i = 0; i < V.Count; i++)
            {
                drawVertex(V[i].x, V[i].y, (i + 1).ToString());
            }
        }

        /// <summary>
        /// Заполнение матрицы смежности для ориентированного графа
        /// </summary>
        public void fillAdjacencyMatrixForOriented(int numberV, List<Edge> E, double[,] matrix)
        {
            for (int i = 0; i < numberV; i++)
                for (int j = 0; j < numberV; j++)
                    matrix[i, j] = 0;
            for (int i = 0; i < E.Count; i++)
                matrix[E[i].v1, E[i].v2] = E[i].weight;
        }

        /// <summary>
        /// Заполнение матрицы инцидентности для ориентированного графа
        /// </summary>
        public void fillIncidenceMatrixForOriented(int numberV, List<Edge> E, int[,] matrix)
        {
            for (int i = 0; i < numberV; i++)
                for (int j = 0; j < E.Count; j++)
                    matrix[i, j] = 0;
            for (int i = 0; i < E.Count; i++)
            {
                matrix[E[i].v1, i] = E[i].weight;
                matrix[E[i].v2, i] = -1;
            }
        }

        /// <summary>
        /// Заполнение матрицы смежности для неориентированного графа
        /// </summary>
        public void fillAdjacencyMatrixForNoNOriented(int numberV, List<Edge> E, double[,] matrix)
        {
            for (int i = 0; i < numberV; i++)
                for (int j = 0; j < numberV; j++)
                    matrix[i, j] = 0;
            for (int i = 0; i < E.Count; i++)
            {
                matrix[E[i].v1, E[i].v2] = E[i].weight;
                matrix[E[i].v2, E[i].v1] = E[i].weight;
            }
        }

        /// <summary>
        /// Заполнение матрицы инцидентности для неориентированного графа
        /// </summary>
        public void fillIncidenceMatrixForNoNOriented(int numberV, List<Edge> E, int[,] matrix)
        {
            for (int i = 0; i < numberV; i++)
                for (int j = 0; j < E.Count; j++)
                    matrix[i, j] = 0;
            for (int i = 0; i < E.Count; i++)
            {
                matrix[E[i].v1, i] = E[i].weight;
                matrix[E[i].v2, i] = E[i].weight;
            }
        }

        /// <summary>
        /// Отрисовка ребра со стрелкой (для ориентированного графа)
        /// </summary>
        void DrawArrow(Point Start, Point end, Pen pen)
        {
            AdjustableArrowCap bigArrow = new AdjustableArrowCap(R - 5, R - 5); 
            pen.CustomEndCap = bigArrow;
            gr.DrawLine(pen, Start.X, Start.Y, end.X, end.Y);
        }
    }
}
