using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        Graphics graphics;
        Pen pen;
        Turtle turtle;
        BackgroundWorker mainWorker;
        Timer timer = new Timer();
        private Font font_Arial = new Font("Arial", 10);
        bool start;
        private int lastIndex;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap tmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            tmp.MakeTransparent();
            graphics = pictureBox1.CreateGraphics();
            turtle = new Turtle(this, pictureBox1.Size.Width / 2, pictureBox1.Size.Height / 2, graphics, pictureBox1);
            pen = new Pen(Color.Black, 1);

            mainWorker = new BackgroundWorker();
            mainWorker.DoWork += new DoWorkEventHandler(Update);

            timer.Tick += new EventHandler(timerHandler);

            listBox1.AllowDrop = true;

            mainWorker.RunWorkerAsync();
            listBox1.BeginUpdate();
            listBox1.Items.Add("REPEAT 5");
            listBox1.Items.Add("FORWARD 20");
            listBox1.Items.Add("LEFT 20");
            listBox1.Items.Add("REPEAT 6");
            listBox1.Items.Add("FORWARD 50");
            listBox1.Items.Add("LEFT 60");
            listBox1.Items.Add("ENDREPEAT");
            listBox1.Items.Add("ENDREPEAT");

            listBox1.EndUpdate();

            listBox1.SelectedIndex = 0;
            lastIndex = -1;

            if (lastIndex == listBox1.SelectedIndex)
                return;
            if (listBox1.SelectedIndex >= 0)
            {
                turtle.draw(pictureBox1, graphics, listBox1);
                lastIndex = listBox1.SelectedIndex;
            }
            label2.Text = "Stack: " + turtle.currentStack.ToString() + "\nInstruction: " + listBox1.SelectedIndex.ToString() + "\nLoop: " + Math.Max(0, turtle.getLoopsLeft());
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            graphics.DrawRectangle(pen, turtle.getPositonX(), turtle.getPositionY(), 40, 40);
            turtle.setPosition(turtle.getPositonX() + 1, turtle.getPositionY() + 1);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer.Stop();
            int interval = (1000 / (int)numericUpDown1.Value);
            timer.Interval = interval;
            timer.Start();
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            start = !start;
            if (start)
                btn_Start.Text = "Stop";
            else
                btn_Start.Text = "Start";

            Debug.WriteLine("tick");

            if (!mainWorker.IsBusy)
                mainWorker.RunWorkerAsync();

            int interval = (1000 / (int)numericUpDown1.Value);
            timer.Interval = interval;
            timer.Start();
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            int nextIndex = turtle.getListBoxIndex();
            if (nextIndex > 0)
            {
                listBox1.SelectedIndex = nextIndex;
                return;
            }

            if (turtle.getCommandIndex() >= listBox1.Items.Count - 1)
                turtle.setCommandIndex(0);
            else
                turtle.setCommandIndex(turtle.getCommandIndex() + 1);

            listBox1.SelectedIndex = turtle.getCommandIndex();


        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            if (turtle.getCommandIndex() <= 0)
                turtle.setCommandIndex(listBox1.Items.Count - 1);
            else
                turtle.setCommandIndex(turtle.getCommandIndex() - 1);
            listBox1.SelectedIndex = turtle.getCommandIndex();

        }
        private void timerHandler(Object sender, EventArgs e)
        {
            if (start)
            {
                if (turtle.getCommandIndex() >= listBox1.Items.Count - 1)
                    turtle.setCommandIndex(0);
                else
                    turtle.setCommandIndex(turtle.getCommandIndex() + 1);

                listBox1.Invoke((MethodInvoker)delegate
                {
                    listBox1.SelectedIndex = turtle.getCommandIndex();
                });
            }
        }

        private void Update(object sender, DoWorkEventArgs e)
        {

        }


        private int getValueFromListAtIndex(int index)
        {
            int valid = listBox1.Items[index].ToString().IndexOf(' ');

            if (index > 0 && index < listBox1.Items.Count && valid >= 1)
            {
                string listboxCommand = listBox1.SelectedItem.ToString().Substring(0, listBox1.SelectedItem.ToString().IndexOf(' '));
                if (listboxCommand == "START" || listboxCommand == "ENDREPEAT" || listboxCommand == "DRAW" || listboxCommand == "STOPDRAW")
                {
                    return -1;
                }
                int value = Int32.Parse(listBox1.Items[index].ToString().Substring((listBox1.Items[index].ToString().IndexOf(' ') + 1)));
                if (value < num_CommandValue.Minimum)
                    value = (int)num_CommandValue.Minimum;
                if (value > num_CommandValue.Maximum)
                    value = (int)num_CommandValue.Maximum;
                num_CommandValue.Value = value;
                return value;
            }
            return 0;
        }

        private void setValueFromListAtIndex(int value)
        {
            string commandString;
            if (listBox1.SelectedItem == null)
                return;
            commandString = listBox1.SelectedItem.ToString().Substring(0, (listBox1.SelectedItem.ToString().IndexOf(' ') + 1));
            if (commandString == "START " || commandString == "ENDREPEAT " || commandString == "DRAW " || commandString == "STOPDRAW ")
            {
                return;
            }
            commandString += value;
            listBox1.Items[listBox1.SelectedIndex] = commandString;
        }

        private string getCommandFromComboBox(int index)
        {
            string command = "";
            int valid = listBox1.Items[index].ToString().IndexOf(' ');

            if (index > 0 && index < listBox1.Items.Count && valid >= 1)
            {
                string listboxCommand = listBox1.SelectedItem.ToString().Substring(0, listBox1.SelectedItem.ToString().IndexOf(' '));
                int i = cb_Commands.FindString(listboxCommand);
                cb_Commands.SelectedIndex = i;
            }
            return command;
        }

        private void setCommandFromComboBox(string command, int index)
        {
            if (listBox1.Items[index].ToString() == "START ")
                return;

            if (command == "ENDREPEAT" || command == "DRAW" || command == "STOPDRAW")
            {
                listBox1.Items[index] = command + " ";
                return;
            }
            int value = getValueFromListAtIndex(index);
            if (value == -1)
                value = 0;
            listBox1.Items[index] = command + " " + value;

        }

        private void cb_Commands_SelectedIndexChanged(object sender, EventArgs e)
        {
            setCommandFromComboBox(cb_Commands.SelectedItem.ToString(), listBox1.SelectedIndex);
        }

        private void num_CommandValue_ValueChanged(object sender, EventArgs e)
        {
            setValueFromListAtIndex((int)num_CommandValue.Value);
        }

        private void btn_addCmd_Click(object sender, EventArgs e)
        {
            if (getValueFromListAtIndex(listBox1.SelectedIndex) == -1)
            {
                listBox1.Items.Insert(listBox1.SelectedIndex + 1, cb_Commands.SelectedItem + " ");
                return;
            }

            listBox1.Items.Insert(listBox1.SelectedIndex + 1, cb_Commands.SelectedItem + " " + num_CommandValue.Value);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            turtle.draw(pictureBox1, graphics, listBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            turtle.setAngle(turtle.getAngle() + 5);
            graphics.DrawString("Angle: " + turtle.getAngle().ToString(), font_Arial, Brushes.Black, new PointF(40, 40));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            turtle.setAngle(turtle.getAngle() - 5);
            graphics.DrawString("Angle: " + turtle.getAngle().ToString(), font_Arial, Brushes.Black, new PointF(40, 40));
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0 && listBox1.SelectedIndex != -1)
            {
                if (listBox1.SelectedItem.ToString() == "START ")
                    return;
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                if (listBox1.Items.Count > 0)
                {
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (lastIndex == listBox1.SelectedIndex)
                return;

            if (listBox1.SelectedIndex >= 0)
            {
                lastIndex = listBox1.SelectedIndex;
                getCommandFromComboBox(listBox1.SelectedIndex);
                getValueFromListAtIndex(listBox1.SelectedIndex);
                turtle.draw(pictureBox1, graphics, listBox1);

            }
            label2.Text = "Stack: " + turtle.currentStack.ToString() + "\nInstruction: " + listBox1.SelectedIndex.ToString() + "\nLoop: " + Math.Max(0, turtle.getLoopsLeft());
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            if (listBox1.SelectedIndex >= 0)
            {
                lastIndex = listBox1.SelectedIndex;
                getCommandFromComboBox(listBox1.SelectedIndex);
                getValueFromListAtIndex(listBox1.SelectedIndex);
                if (listBox1.SelectedItem.ToString() == "START ")
                {
                    turtle.reset();
                    label2.Text = "Stack: " + turtle.currentStack.ToString() + "\nInstruction: " + listBox1.SelectedIndex.ToString() + "\nLoop: " + Math.Max(0, turtle.getLoopsLeft());
                    graphics.Clear(pictureBox1.BackColor);
                }
            }
            listBox1.DoDragDrop(listBox1.SelectedItem, DragDropEffects.Move);
        }

        private void listBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            Point point = listBox1.PointToClient(new Point(e.X, e.Y));
            int index = listBox1.IndexFromPoint(point);
            if (index < 0) index = listBox1.Items.Count - 1;
            object data = listBox1.SelectedItem;
            if (data.ToString() == "START" || data.ToString() == "START " || index == 0)
                return;
            listBox1.Items.Remove(data);
            listBox1.Items.Insert(index, data);
            listBox1.SelectedIndex = index;
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {

            e.Effect = DragDropEffects.All;
        }

        private void btn_up_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && listBox1.Items.Count > 1)
            {
                if (listBox1.SelectedIndex == 0)
                {
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                    return;
                }
                listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
            }
        }

        private void btn_down_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && listBox1.Items.Count > 1)
            {
                if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
                {
                    listBox1.SelectedIndex = 0;
                    return;
                }
                listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
            }
        }

    }
}
