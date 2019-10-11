using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{

    public class Turtle
    {

        Graphics graphics;
        Pen pen, tPen;
        Bitmap turtleBitmap;
        Bitmap linesBitmap;
        Bitmap finalBitmap;

        PictureBox picBox;

        int positionX;
        int positionY;
        int startPosX;
        int startPosY;
        double angle;
        int commandIndex;
        private Form1 f;
        int size;
        bool drawing;
        public int frame;

        const int MAX_STACK = 128;
        public struct Stack
        {
            public int loopLeft;
            public int beginLoopIndex;
        };
        public Stack[] stack;
        public int currentStack;
        bool execute;

        public volatile bool callstackEvent = false;

        Point tleft, tfront, tright;
        Point[] trianglePoint;
        public Turtle(Form1 f, int px, int py, Graphics g, PictureBox picturebox)
        {
            this.f = f;
            picBox = picturebox;

            turtleBitmap = new Bitmap(picBox.Size.Width, picBox.Size.Height);
            linesBitmap = new Bitmap(picBox.Size.Width, picBox.Size.Height);
            finalBitmap = new Bitmap(picBox.Size.Width, picBox.Size.Height);
            graphics = g;

            pen = new Pen(Color.Black, 1);
            tPen = new Pen(Color.Green, 1);
            stack = new Stack[MAX_STACK];

            currentStack = 0;
            execute = false;
            frame = 0;
            drawing = true;
            size = 14;
            angle = 180;
            setPosition(px, py);
            startPosX = px;
            startPosY = py;
            double targetPosX = px;
            double targetPosY = py;
            tleft = new Point((int)targetPosX - size / 2, (int)targetPosY);
            tfront = new Point((int)targetPosX, (int)targetPosY + size / 2);
            tright = new Point((int)targetPosX + size / 2, (int)targetPosY);
            trianglePoint = new Point[] { tleft, tfront, tright };

            setAngle(angle);

            commandIndex = 0;


            reset();

        }

        public void reset()
        {
            for (int n = 0; n < MAX_STACK; n++)
            {
                stack[n].loopLeft = -2;
                stack[n].beginLoopIndex = -1;
            }
            execute = false;
            currentStack = 0;
            frame = 0;
            commandIndex = 0;
            angle = 180;
            setPosition(startPosX, startPosY);
            clear();
        }

        public void draw(PictureBox p, Graphics g, ListBox commandListBox)
        {
            if (callstackEvent)
                return;
            int index = commandListBox.SelectedIndex;

            if (commandListBox.Items.Count > 0 && index >= 0)
            {
                int args = commandListBox.Items[index].ToString().IndexOf(' ');

                if (index >= 0 && index < commandListBox.Items.Count)
                {
                    string listboxCommand = "";
                    if (args > 0)
                        listboxCommand = commandListBox.SelectedItem.ToString().Substring(0, commandListBox.SelectedItem.ToString().IndexOf(' '));
                    else
                        listboxCommand = commandListBox.SelectedItem.ToString();

                    switch (listboxCommand)
                    {
                        case "FORWARD":
                            int value = Int32.Parse(commandListBox.Items[index].ToString().Substring((commandListBox.Items[index].ToString().IndexOf(' ') + 1)));

                            double targetX = positionX + ((Math.Sin(angle * Math.PI / 180)) * value);
                            double targetY = positionY + ((Math.Cos(angle * Math.PI / 180)) * value);
                            int tX = (int)Math.Round(targetX);
                            int tY = (int)Math.Round(targetY);
                            if (drawing)
                            {
                                using (Graphics gr = Graphics.FromImage(linesBitmap))
                                {
                                    gr.DrawLine(pen, positionX, positionY, tX, tY);
                                }
                            }

                            setPosition(tX, tY);
                            break;
                        case "BACKWARD":
                            value = Int32.Parse(commandListBox.Items[index].ToString().Substring((commandListBox.Items[index].ToString().IndexOf(' ') + 1)));

                            targetX = positionX + ((Math.Sin(angle * Math.PI / 180)) * -value);
                            targetY = positionY + ((Math.Cos(angle * Math.PI / 180)) * -value);
                            tX = (int)Math.Round(targetX);
                            tY = (int)Math.Round(targetY);
                            if (drawing)
                            {
                                using (Graphics gr = Graphics.FromImage(linesBitmap))
                                {
                                    gr.DrawLine(pen, positionX, positionY, tX, tY);
                                }
                            }

                            setPosition(tX, tY);
                            break;
                        case "LEFT":
                            value = Int32.Parse(commandListBox.Items[index].ToString().Substring((commandListBox.Items[index].ToString().IndexOf(' ') + 1)));

                            angle += value;
                            angle = angle % 360;

                            setAngle(angle);
                            break;
                        case "RIGHT":
                            value = Int32.Parse(commandListBox.Items[index].ToString().Substring((commandListBox.Items[index].ToString().IndexOf(' ') + 1)));

                            angle -= value;
                            angle = angle % 360;

                            setAngle(angle);

                            break;
                        case "REPEAT":
                            value = Int32.Parse(commandListBox.Items[index].ToString().Substring((commandListBox.Items[index].ToString().IndexOf(' ') + 1)));
                            if (currentStack < MAX_STACK)
                            {
                                if (currentStack > 0)
                                {
                                    if (stack[currentStack - 1].beginLoopIndex != commandListBox.SelectedIndex && stack[currentStack - 1].loopLeft == -2)
                                    {
                                        stack[currentStack].loopLeft = value;
                                        stack[currentStack].beginLoopIndex = commandListBox.SelectedIndex;
                                        currentStack++;
                                    }
                                    else
                                    {
                                        stack[currentStack].loopLeft = value;
                                        stack[currentStack].beginLoopIndex = commandListBox.SelectedIndex;
                                        currentStack++;
                                    }
                                }
                                else
                                {
                                    if (stack[currentStack].beginLoopIndex != commandListBox.SelectedIndex && stack[currentStack].loopLeft == -2)
                                    {
                                        stack[currentStack].loopLeft = value;
                                        stack[currentStack].beginLoopIndex = commandListBox.SelectedIndex;
                                        currentStack++;
                                    }
                                }
                            }
                            break;
                        case "ENDREPEAT":
                            if (currentStack > 0)
                            {
                                if (stack[currentStack - 1].loopLeft > 1)
                                {
                                    stack[currentStack - 1].loopLeft -= 1;
                                    setCommandIndex(stack[currentStack - 1].beginLoopIndex + 1);
                                    execute = true;
                                    commandListBox.SelectedIndex = stack[currentStack - 1].beginLoopIndex + 1;
                                }
                                else
                                {
                                    stack[currentStack - 1].loopLeft = -2;
                                    stack[currentStack - 1].beginLoopIndex = -1;
                                    currentStack--;
                                }
                            }
                            break;
                        case "DRAW":
                            drawing = true;
                            break;
                        case "STOPDRAW":
                            drawing = false;
                            break;
                        case "START":
                            reset();
                            break;
                        default:
                            break;
                    }
                }

            }
            setAngle(angle);

            drawTurtle(picBox, g);
        }

        public void clear()
        {
            Graphics gr = Graphics.FromImage(finalBitmap);
            gr.Clear(Color.Transparent);
            gr = Graphics.FromImage(linesBitmap);
            gr.Clear(Color.Transparent);
            gr = Graphics.FromImage(turtleBitmap);
            gr.Clear(Color.Transparent);
        }

        public int getListBoxIndex()
        {
            bool call = false;
            if (currentStack > 0 && stack[currentStack - 1].loopLeft > 0 && execute == true)
            {
                call = true;
            }
            if (call)
            {
                execute = false;
                return stack[currentStack - 1].beginLoopIndex + 1;
            }
            return -1;
        }

        public void drawTurtle(PictureBox p, Graphics g)
        {
            using (Graphics gr = Graphics.FromImage(finalBitmap))
            {
                gr.Clear(p.BackColor);
                gr.DrawPolygon(tPen, trianglePoint);
                gr.DrawImage(linesBitmap, new Rectangle(0, 0, linesBitmap.Width, linesBitmap.Height));
                gr.DrawImage(turtleBitmap, new Rectangle(0, 0, turtleBitmap.Width, turtleBitmap.Height));
            }

            g.DrawImage(finalBitmap, new Rectangle(0, 0, finalBitmap.Width, finalBitmap.Height));
        }

        public void setPosition(int x, int y)
        {
            positionX = x;
            positionY = y;
        }

        public int getPositonX()
        {
            return positionX;
        }

        public int getPositionY()
        {
            return positionY;
        }

        public int getCommandIndex()
        {
            return commandIndex;
        }

        public void setCommandIndex(int index)
        {
            commandIndex = index;
        }


        private int getValueFromListAtIndex(int index, ListBox listBox1)
        {
            if (index > 0 && index < listBox1.Items.Count)
            {
                int value = Int32.Parse(listBox1.Items[index].ToString().Substring((listBox1.Items[index].ToString().IndexOf(' ') + 1)));
                //num_CommandValue.Value = value;
                return value;
            }
            return 0;
        }

        public void setAngle(double a)
        {
            angle = a % 360;

            double angleRadians = angle * (Math.PI / 180);
            double cosTheta = Math.Cos(angleRadians);
            double sinTheta = -Math.Sin(angleRadians);

            double targetPosX = positionX;
            double targetPosY = positionY;
            tleft = new Point((int)targetPosX - size / 2, (int)targetPosY);
            tfront = new Point((int)targetPosX, (int)targetPosY + size / 2);
            tright = new Point((int)targetPosX + size / 2, (int)targetPosY);

            int centerPointX = (int)(targetPosX);
            int centerPointY = (int)(targetPosY);

            int newX = (int)(cosTheta * (tleft.X - centerPointX) - sinTheta * (tleft.Y - centerPointY) + centerPointX);
            int newY = (int)(sinTheta * (tleft.X - centerPointX) + cosTheta * (tleft.Y - centerPointY) + centerPointY);

            tleft.X = newX;
            tleft.Y = newY;

            newX = (int)(cosTheta * (tright.X - centerPointX) - sinTheta * (tright.Y - centerPointY) + centerPointX);
            newY = (int)(sinTheta * (tright.X - centerPointX) + cosTheta * (tright.Y - centerPointY) + centerPointY);

            tright.X = newX;
            tright.Y = newY;

            newX = (int)(cosTheta * (tfront.X - centerPointX) - sinTheta * (tfront.Y - centerPointY) + centerPointX);
            newY = (int)(sinTheta * (tfront.X - centerPointX) + cosTheta * (tfront.Y - centerPointY) + centerPointY);

            tfront.X = newX;
            tfront.Y = newY;


            trianglePoint = new Point[] { tleft, tfront, tright };
            drawTurtle(picBox, graphics);
        }

        public double getAngle()
        {
            return angle;
        }

        public int getLoopsLeft()
        {
            if (currentStack > 0)
            {
                return stack[currentStack - 1].loopLeft;
            }
            return 0;
        }
    }



    enum COMMANDS
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        REPEAT,
        ENDREPEAT,
        DRAW,
        STOPDRAW,
    }



}
