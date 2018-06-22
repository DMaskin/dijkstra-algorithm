using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace Dijkstra_s_Algorithm 
{
    public partial class Form1 : Form
    {
        int click = 0; //счётчик для подсказок вначале
        byte graphOrientation; //Если орграф, то 1, если нет – 0;
        DrawGraph G;
        List<Vertex> V;
        List<Edge> E;
        double[,] AMatrix; //матрица смежности
        int[,] IMatrix; //матрица инцидентности     
        Graphics gr;
        int selected1; //выбранные вершины, для соединения линиями
        int selected2;

        public Form1()
        {
            InitializeComponent();
            V = new List<Vertex>();
            G = new DrawGraph(SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height);
            E = new List<Edge>();
            sheet.Image = G.GetBitmap(); 
            richTextBox1.Location = new Point(this.ClientSize.Width / 3 + 30, 80);
            richTextBox1.Size = new Size(this.ClientSize.Width / 3 - 60, this.ClientSize.Height / 3 + 50);
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            richTextBox1.Text = "\nНажмите \"далее\" для отображения подсказок: ";
            selectButton.Visible = false;
            drawVertexButton.Visible = false;
            drawEdgeButton.Visible = false;
            deleteButton.Visible = false;
            buttonInc.Visible = false;
            buttonAdj.Visible = false;
            Solve.Visible = false;
            listBoxMatrix.Visible = false;
            DeleteToolStripMenuItem.Visible = false;
            SaveToolStripMenuItem.Visible = false;
            about.Visible = false;
            gr = CreateGraphics();
        }

        /// <summary>
        /// Кнопка - выбрать вершину
        /// </summary>
        private void selectButton_Click(object sender, EventArgs e)
        {
            selectButton.Enabled = false;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E, graphOrientation);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
        }

        /// <summary>
        /// Кнопка - рисовать вершину
        /// </summary>
        private void drawVertexButton_Click(object sender, EventArgs e)
        {
            drawVertexButton.Enabled = false;
            selectButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E, graphOrientation);
            sheet.Image = G.GetBitmap();
        }

        /// <summary>
        /// Кнопка - рисовать ребро
        /// </summary>
        private void drawEdgeButton_Click(object sender, EventArgs e)
        {
            drawEdgeButton.Enabled = false;
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            deleteButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E, graphOrientation);
            sheet.Image = G.GetBitmap();
            selected1 = -1;
            selected2 = -1;
        }

        /// <summary>
        /// Кнопка - удалить элемент
        /// </summary>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            deleteButton.Enabled = false;
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            G.clearSheet();
            G.drawALLGraph(V, E, graphOrientation);
            sheet.Image = G.GetBitmap();          
        }

        /// <summary>
        /// Кнопка - матрица смежности
        /// </summary>
        private void buttonAdj_Click(object sender, EventArgs e)
        {
            createAdjAndOut();
        }

        /// <summary>
        /// Кнопка - матрица инцидентности 
        /// </summary>
        private void buttonInc_Click(object sender, EventArgs e)
        {
            createIncAndOut();
        }

        /// <summary>
        /// Любой клик по области рисования 
        /// </summary>
        private void sheet_MouseClick(object sender, MouseEventArgs e)
        {
            //нажата кнопка "выбрать вершину", ищем степень вершины
            if (selectButton.Enabled == false)
            {
                for (int i = 0; i < V.Count; i++)
                {
                    if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                    {
                        if (selected1 != -1)
                        {
                            selected1 = -1;
                            G.clearSheet();
                            G.drawALLGraph(V, E, graphOrientation);
                            sheet.Image = G.GetBitmap();
                        }
                        if (selected1 == -1)
                        {
                            G.drawSelectedVertex(V[i].x, V[i].y);
                            selected1 = i;
                            sheet.Image = G.GetBitmap();
                            createAdjAndOut();
                            listBoxMatrix.Items.Clear();
                            double degree = 0;
                            if (graphOrientation != 1)
                            {
                                for (int j = 0; j < V.Count; j++)
                                    if (AMatrix[selected1, j] != 0)
                                        //degree += AMatrix[selected1, j];
                                        degree += 1;
                                listBoxMatrix.Items.Add("Степень вершины №" + (selected1 + 1) + " равна " + degree);
                            }
                            else
                            {
                                double degreeIn = 0;
                                for (int j = 0; j < V.Count; j++)
                                {
                                    if (AMatrix[selected1, j] != 0)
                                        degree += 1;
                                    if (AMatrix[j, selected1] != 0)
                                        degreeIn += 1;
                                }
                                listBoxMatrix.Items.Add("Cтепень входа вершины №" + (selected1 + 1) + " равна " + degreeIn);
                                listBoxMatrix.Items.Add("Cтепень исхода вершины №" + (selected1 + 1) + " равна " + degree);                               
                            }
                            break;
                        }
                    }
                }
            }
            //нажата кнопка "рисовать вершину"
            if (drawVertexButton.Enabled == false)
            {
                V.Add(new Vertex(e.X, e.Y));
                G.drawVertex(e.X, e.Y, V.Count.ToString());
                sheet.Image = G.GetBitmap();
            }
            //нажата кнопка "рисовать ребро"
            if (drawEdgeButton.Enabled == false)
            {
                if (e.Button == MouseButtons.Left)
                {
                    for (int i = 0; i < V.Count; i++)
                    {
                        if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                        {
                            if (selected1 == -1)
                            {                            
                                G.drawSelectedVertex(V[i].x, V[i].y);
                                selected1 = i;
                                sheet.Image = G.GetBitmap();
                                break;
                            }
                            if (selected2 == -1)
                            {                               
                                G.drawSelectedVertex(V[i].x, V[i].y);                              
                                selected2 = i;
                                if (selected1 == selected2)
                                {
                                    MessageBox.Show("Петли не допускаются", "Ошибка", MessageBoxButtons.OK);
                                    drawEdgeButton.Enabled = false;
                                    selectButton.Enabled = true;
                                    drawVertexButton.Enabled = true;
                                    deleteButton.Enabled = true;
                                    G.clearSheet();
                                    G.drawALLGraph(V, E, graphOrientation);
                                    sheet.Image = G.GetBitmap();
                                    selected1 = -1;
                                    selected2 = -1;
                                    break;
                                }
                                E.Add(new Edge(selected1, selected2));
                                Point pointf = new Point((V[selected1].x + V[selected2].x) / 2 + 50, (V[selected1].y + V[selected2].y) / 2);
                                FormAddWeight inputWeight = new FormAddWeight();
                                inputWeight.ShowDialog();
                                E[E.Count - 1].weight = inputWeight.value;
                                G.drawEdge(V[selected1], V[selected2], E[E.Count - 1], E, E.Count - 1, graphOrientation);
                                selected1 = -1;
                                selected2 = -1;
                                sheet.Image = G.GetBitmap();
                                break;
                            }
                        }
                    }
                }
                if (e.Button == MouseButtons.Right)
                {
                    if ((selected1 != -1) &&
                        (Math.Pow((V[selected1].x - e.X), 2) + Math.Pow((V[selected1].y - e.Y), 2) <= G.R * G.R))
                    {
                        G.drawVertex(V[selected1].x, V[selected1].y, (selected1 + 1).ToString());
                        selected1 = -1;
                        sheet.Image = G.GetBitmap();
                    }
                }
            }
            //нажата кнопка "удалить элемент"
            if (deleteButton.Enabled == false)
            {
                bool flag = false; //удалили ли что-нибудь по ЭТОМУ клику
                //ищем, возможно была нажата вершина
                for (int i = 0; i < V.Count; i++)
                {
                    if (Math.Pow((V[i].x - e.X), 2) + Math.Pow((V[i].y - e.Y), 2) <= G.R * G.R)
                    {
                        for (int j = 0; j < E.Count; j++)
                        {
                            if ((E[j].v1 == i) || (E[j].v2 == i))
                            {
                                E.RemoveAt(j);
                                j--;
                            }
                            else
                            {
                                if (E[j].v1 > i) E[j].v1--;
                                if (E[j].v2 > i) E[j].v2--;
                            }
                        }
                        V.RemoveAt(i);
                        flag = true;
                        break;
                    }
                }
                //ищем, возможно было нажато ребро
                if (!flag)
                {
                    for (int i = 0; i < E.Count; i++)
                    {
                        if (((e.X - V[E[i].v1].x) * (V[E[i].v2].y - V[E[i].v1].y) / (V[E[i].v2].x - V[E[i].v1].x) + V[E[i].v1].y) <= (e.Y + 4) &&
                            ((e.X - V[E[i].v1].x) * (V[E[i].v2].y - V[E[i].v1].y) / (V[E[i].v2].x - V[E[i].v1].x) + V[E[i].v1].y) >= (e.Y - 4))
                        {
                            if ((V[E[i].v1].x <= V[E[i].v2].x && V[E[i].v1].x <= e.X && e.X <= V[E[i].v2].x) ||
                                (V[E[i].v1].x >= V[E[i].v2].x && V[E[i].v1].x >= e.X && e.X >= V[E[i].v2].x))
                            {
                                E.RemoveAt(i);
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                //если что-то было удалено, то обновляем граф на экране
                if (flag)
                {
                    G.clearSheet();
                    G.drawALLGraph(V, E, graphOrientation);
                    sheet.Image = G.GetBitmap();
                }
            }
        }

        /// <summary>
        /// Cоздание матрицы смежности и вывод в ListBox 
        /// </summary>
        private void createAdjAndOut()
        {
            AMatrix = new double[V.Count, V.Count];
            if (graphOrientation == 1)
                G.fillAdjacencyMatrixForOriented(V.Count, E, AMatrix);
            else
                G.fillAdjacencyMatrixForNoNOriented(V.Count, E, AMatrix);
            listBoxMatrix.Items.Clear();
            string sOut = "    ";
            for (int i = 0; i < V.Count; i++)
                sOut += (i + 1) + " ";
            listBoxMatrix.Items.Add(sOut);
            for (int i = 0; i < V.Count; i++)
            {
                sOut = (i + 1) + " | ";
                for (int j = 0; j < V.Count; j++)
                    sOut += AMatrix[i, j] + " ";
                listBoxMatrix.Items.Add(sOut);
            }
        }

        /// <summary>
        /// Создани матрицы инцидентности и вывод в ListBox
        /// </summary>
        private void createIncAndOut()
        {
            if (E.Count > 0)
            {
                IMatrix = new int[V.Count, E.Count];
                if (graphOrientation == 1)
                    G.fillIncidenceMatrixForOriented(V.Count, E, IMatrix);
                else
                    G.fillIncidenceMatrixForNoNOriented(V.Count, E, IMatrix);
                listBoxMatrix.Items.Clear();
                string sOut = "    ";
                for (int i = 0; i < E.Count; i++)
                    sOut += E[i].weight.ToString() + " ";
                listBoxMatrix.Items.Add(sOut);
                for (int i = 0; i < V.Count; i++)
                {
                    sOut = (i + 1) + " | ";
                    for (int j = 0; j < E.Count; j++)
                        sOut += IMatrix[i, j] + " ";
                    listBoxMatrix.Items.Add(sOut);
                }
            }
            else
                listBoxMatrix.Items.Clear();
        }

        /// <summary>
        /// О программе
        /// </summary>
        private void about_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Приложение для создания графов и определения кратчайшего пути с помощью алгоритма Дейкстры. \nАвтор: \tМаскин Дмитрий", "О программе: ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Кнопка нахождения кратчайший путей (алг. Дейкстры)
        /// </summary>
        private void Solve_Click(object sender, EventArgs e)
        {
            listBoxMatrix.Items.Clear();
            int startingVertex = 0;
            try
            {
                Dijkstra pool = new Dijkstra(AMatrix, startingVertex, V.Count); //Алгоритм(матрица смежности, начальная вершина, конечная вершина);
                var item = pool.dist;
                for (int i = 0; i < item.Length; i++)
                {
                    listBoxMatrix.Items.Add("От " + (startingVertex + 1) + " до вершины " + (i + 1) + " длина пути: = " + item[i]);
                }
            }
            catch
            {
                listBoxMatrix.Items.Add("Матрица смежности не создана");
            }
        }

        /// <summary>
        /// Пункт меню "удалить"
        /// </summary>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectButton.Enabled = true;
            drawVertexButton.Enabled = true;
            drawEdgeButton.Enabled = true;
            deleteButton.Enabled = true;
            const string message = "Вы действительно хотите полностью удалить граф?";
            const string caption = "Удаление";
            var MBSave = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (MBSave == DialogResult.Yes)
            {
                V.Clear();
                E.Clear();
                G.clearSheet();
                sheet.Image = G.GetBitmap();
            }
        }

        /// <summary>
        /// Пункт меню "Сохранить"
        /// </summary>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Image != null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                savedialog.OverwritePrompt = true;
                savedialog.CheckPathExists = true;
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        sheet.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormHello formHello = new FormHello();
            formHello.ShowDialog();
            graphOrientation = formHello.Orientation;
            ToolTip t = new ToolTip();
            t.SetToolTip(deleteButton,"Удалить вершину/ребро");
            t.SetToolTip(drawEdgeButton, "Нарисовать ребро");
            t.SetToolTip(drawVertexButton, "Нарисовать вершину");
            t.SetToolTip(selectButton, "Определить степень вершины");
            t.SetToolTip(buttonAdj, "Построить матрицу смежности");
            t.SetToolTip(buttonInc, "Построить матрицу инциденции");
            t.SetToolTip(Solve, "Найти кратчайший путь от 1-й вершины до всех остальных");
        }

        /// <summary>
        /// Кнопка "дальше" (подсказки)
        /// </summary>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            click++;
            switch (click)
            {
                case 1:
                    selectButton.Visible = true;
                    selectButton.Enabled = false;
                    richTextBox1.Text = "\nКнопка ниже служит для курсора и отображения степени вершины в окне справа: ";               
                    gr.DrawRectangle(new Pen(Color.Red, 10), selectButton.Location.X, selectButton.Location.Y, selectButton.Width, selectButton.Height);
                    break;
                case 2:
                    gr.DrawRectangle(new Pen(Form1.DefaultBackColor, 10), selectButton.Location.X, selectButton.Location.Y, selectButton.Width, selectButton.Height);
                    gr.DrawRectangle(new Pen(Color.Red, 10), listBoxMatrix.Location.X, listBoxMatrix.Location.Y, listBoxMatrix.Width, listBoxMatrix.Height);
                    richTextBox1.Text = "\nОкно справа служит для отображения степени вершины, отображения матрицы смежности, матрицы инцидентности, а также для отображения кратчайших путей до каждой вершины:";
                    listBoxMatrix.Visible = true;
                    break;
                case 3:
                    gr.DrawRectangle(new Pen(Form1.DefaultBackColor, 15), listBoxMatrix.Location.X, listBoxMatrix.Location.Y, listBoxMatrix.Width, listBoxMatrix.Height);
                    drawVertexButton.Visible = true;
                    drawVertexButton.Enabled = false;
                    richTextBox1.Text = "\nКнопка ниже служит для создания вершин графа: ";                  
                    gr.DrawRectangle(new Pen(Color.Red, 10), drawVertexButton.Location.X, drawVertexButton.Location.Y, drawVertexButton.Width, drawVertexButton.Height);
                    break;
                case 4:
                    gr.DrawRectangle(new Pen(Form1.DefaultBackColor, 10), drawVertexButton.Location.X, drawVertexButton.Location.Y, drawVertexButton.Width, drawVertexButton.Height);
                    drawEdgeButton.Visible = true;
                    drawEdgeButton.Enabled = false;
                    richTextBox1.Text = "\nКнопка ниже служит для создания ребер графа: ";
                    gr.DrawRectangle(new Pen(Color.Red, 10), drawEdgeButton.Location.X, drawEdgeButton.Location.Y, drawEdgeButton.Width, drawEdgeButton.Height);
                    break;
                case 5:
                    gr.DrawRectangle(new Pen(Form1.DefaultBackColor, 10), drawEdgeButton.Location.X, drawEdgeButton.Location.Y, drawEdgeButton.Width, drawEdgeButton.Height);
                    deleteButton.Visible = true;
                    deleteButton.Enabled = false;
                    richTextBox1.Text = "\nКнопка ниже служит для удаления вершин/ребер графа: ";
                    gr.DrawRectangle(new Pen(Color.Red, 10), deleteButton.Location.X, deleteButton.Location.Y, deleteButton.Width, deleteButton.Height);
                    break;
                case 6:
                    gr.DrawRectangle(new Pen(Form1.DefaultBackColor, 10), deleteButton.Location.X, deleteButton.Location.Y, deleteButton.Width, deleteButton.Height);
                    buttonAdj.Visible = true;
                    buttonAdj.Enabled = false;
                    richTextBox1.Text = "\nКнопка cбоку служит для построения матрицы смежности графа: ";
                    gr.DrawRectangle(new Pen(Color.Red, 10), buttonAdj.Location.X, buttonAdj.Location.Y, buttonAdj.Width, buttonAdj.Height);
                    break;
                case 7:
                    gr.DrawRectangle(new Pen(Form1.DefaultBackColor, 10), buttonAdj.Location.X, buttonAdj.Location.Y, buttonAdj.Width, buttonAdj.Height);
                    buttonInc.Visible = true;
                    buttonInc.Enabled = false;
                    richTextBox1.Text = "\nКнопка cбоку служит для построения матрицы инцидентности графа: ";
                    gr.DrawRectangle(new Pen(Color.Red, 10), buttonInc.Location.X, buttonInc.Location.Y, buttonInc.Width, buttonInc.Height);
                    break;
                case 8:
                    gr.DrawRectangle(new Pen(Form1.DefaultBackColor, 10), buttonInc.Location.X, buttonInc.Location.Y, buttonInc.Width, buttonInc.Height);
                    Solve.Visible = true;
                    Solve.Enabled = false;
                    richTextBox1.Text = "\nКнопка \"Solve\" в правом нижнем углу служит для нахождения кратчайших путей от первой вершины до всех остальных. \nРезультаты выводятся в окно, которое расположено над кнопкой: ";
                    gr.DrawRectangle(new Pen(Color.Red, 10), Solve.Location.X, Solve.Location.Y, Solve.Width, Solve.Height);
                    break;
                case 9:
                    gr.DrawRectangle(new Pen(Form1.DefaultBackColor, 10), Solve.Location.X, Solve.Location.Y, Solve.Width, Solve.Height);
                    richTextBox1.Text = "\nВ левом верхнем углу расположены две кнопки: для удаления всего графа и очистки области построения; \nдля сохранения графа в виде изображения на диск. \nПомимо этого в правом верхнем углу есть кнопка помощи для информации о создателе приложения: ";
                    DeleteToolStripMenuItem.Visible = true;
                    DeleteToolStripMenuItem.Enabled = false;
                    SaveToolStripMenuItem.Visible = true;
                    SaveToolStripMenuItem.Enabled = false;
                    about.Visible = true;
                    about.Enabled = false;
                    break;
                case 10:
                    richTextBox1.Dispose();
                    buttonNext.Dispose();
                    selectButton.Enabled = true;
                    drawVertexButton.Enabled = true;
                    drawEdgeButton.Enabled = true;
                    deleteButton.Enabled = true;
                    buttonAdj.Enabled = true;
                    buttonInc.Enabled = true;
                    Solve.Enabled = true;
                    DeleteToolStripMenuItem.Enabled = true;
                    SaveToolStripMenuItem.Enabled = true;
                    about.Enabled = true;
                    break;
            }
        }
    }
}
