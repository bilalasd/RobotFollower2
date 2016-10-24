using System;
using System.Windows;
using System.Threading;
using Microsoft.Kinect;
using System.Collections;
using System.Collections.Generic;

namespace RobotFollowerWPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public float personZ = 0;
        public float personX = 0;
        public float personY = 0;
        public float needToMoveX = 0;
        public float needToMoveZ = 0;
        string commandText = "";
        public bool currentlyTracking = false;
        public enum commands { FORWARD, BACK, TURN_LEFT, TURN_RIGHT, STOP };
        double personMagnitude = 0;
        double personAngle = 0;

        public Queue<commands> commandQueue = new Queue<commands>();

        SerialCommunication arduino = new SerialCommunication();
        SkeletonHandler skeletonHandler;
        ColorHandler colorHandler;
        KinectSensor kinect;

        public MainWindow()
        {
            InitializeComponent();
            arduino.PortNameToDetect.Add("Arduino");
            new Thread(new ThreadStart(arduino.Connect)).Start();
            initialize();
            //new Thread(new ThreadStart(checkDistance)).Start();
        }

        public void UpdateTextBoxes()
        {
            while (true)
            {
                this.Dispatcher.BeginInvoke((Action)(delegate ()
                {
                    spineZ.Text = Math.Round(personZ, 3).ToString();
                    spineX.Text = Math.Round(personX, 3).ToString();
                    spineY.Text = Math.Round(personY, 3).ToString();
                    commandTextBox.Text = commandText;
                    personMagTextBox.Text = personMagnitude.ToString();
                    personAngleTextBox.Text = personAngle.ToString();
                    //Thread.Sleep(50);
                }));
                Thread.Sleep(10);
            }
        }

        public void Controller()
        {
            int missCounter = 0; //Counts the number of loops that the person has not being tracked
            commands lastCommand = commands.STOP;

            while (true)
            {
                if (personZ > 0) //check if the person is being tracked
                {
                    missCounter = 0;
                    if (personMagnitude < 1.3)
                    {
                        commandQueue.Enqueue(commands.BACK);
                        lastCommand = commands.BACK;
                    }
                    else if (personMagnitude < 1.75)
                    {
                        commandQueue.Enqueue(commands.STOP);
                        lastCommand = commands.STOP;
                        //commandText = commands.STOP.ToString();
                    }
                    else
                    {
                        if (personX < -.3)
                        {
                            commandQueue.Enqueue(commands.TURN_RIGHT);
                            lastCommand = commands.TURN_RIGHT;
                            //commandText = commands.TURN_RIGHT.ToString();
                        }
                        else if (personX > .3)
                        {
                            commandQueue.Enqueue(commands.TURN_LEFT);
                            lastCommand = commands.TURN_LEFT;
                            //commandText = commands.TURN_LEFT.ToString();
                        }
                        else
                        {
                            commandQueue.Enqueue(commands.FORWARD);
                            lastCommand = commands.FORWARD;
                            //commandText = commands.FORWARD.ToString();
                        }
                    }

                }
                else //if the person is not being tracked
                {
                    missCounter++;
                    if (missCounter > 20)
                    {
                        commandQueue.Enqueue(commands.STOP);
                    }
                    else
                    {
                        commandQueue.Enqueue(lastCommand);
                    }

                }
                Thread.Sleep(200);
            }

        }

        public void CommandExecutor()
        {
            //magnitude 0.35 -> 39.5 inches / 5 seconds -> 2.3 mph
            double constantMag = 0.5;

            // Thread.Sleep(500);
            while (true)
            {
                personAngle = Math.Atan2(personX, personZ) * (180 / Math.PI);
                personMagnitude = Math.Sqrt(Math.Pow(personX, 2) + Math.Pow(personZ, 2));
                double rotation = personAngle / 90;


                if (commandQueue.Count > 0)
                {
                    commands currentCommand = commandQueue.Dequeue();
                    commandText = currentCommand.ToString();
                    switch (currentCommand)
                    {
                        case commands.STOP:
                            arduino.SendString("STOP");
                            break;
                        case commands.FORWARD:
                            arduino.SendString("MOVE 0 " + constantMag.ToString() + " 0");
                            break;
                        case commands.BACK:
                            arduino.SendString("MOVE 0 -" + constantMag.ToString() + " 0");
                            break;
                        case commands.TURN_LEFT:
                            arduino.SendString("MOVE 0 " + constantMag.ToString() + " " + rotation);
                            break;
                        case commands.TURN_RIGHT:
                            arduino.SendString("MOVE 0 " + constantMag.ToString() + " " + rotation);
                            break;
                        default:
                            break;
                    }
                }
                Thread.Sleep(100);
            }
        }

        public void initialize()
        {
            //find the correct sensor
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.kinect = potentialSensor;
                    break;
                }
            }

            if (kinect != null)
            {
                skeletonHandler = new SkeletonHandler(kinect, this, skeleton_image);
                colorHandler = new ColorHandler(kinect, this, color_image);

                while (!arduino.currentlyConnected)
                {
                    Thread.Sleep(50);
                }

                new Thread(new ThreadStart(Controller)).Start();
                new Thread(new ThreadStart(UpdateTextBoxes)).Start();
                new Thread(new ThreadStart(CommandExecutor)).Start();
            }

        }
    }
}
