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
        double personMagnitude = 0;
        double personAngle = 0;

        bool stop = false;

        public Queue<Command> commandQueue = new Queue<Command>();

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
            double rotation = 0;
            //magnitude 0.35 -> 39.5 inches / 5 seconds -> 2.3 mph
            const double CONSTANTSPEED = .75;

            int missCounter = 0; //Counts the number of loops that the person has not being tracked
            Command lastCommand = new Command(Command.actions.STOP);

            while (true)
            {
                stop = false;
                personAngle = Math.Atan2(personX, personZ) * (180 / Math.PI); //angle of the person in degrees
                personMagnitude = Math.Sqrt(Math.Pow(personX, 2) + Math.Pow(personZ, 2)); //distance from robot to person
                rotation = personAngle / 90.0;
                double speed = CONSTANTSPEED;

                if (personZ > 0) //check if the person is being tracked
                {
                    missCounter = 0;

                    //BACK
                    if (personMagnitude < 1.3)
                    {
                        Command newCommand = new Command(Command.actions.BACK, speed, 0, 0);
                        commandQueue.Enqueue(newCommand);
                        lastCommand = newCommand;
                    }

                    //STOP
                    else if (personMagnitude > 1.3 && personMagnitude < 1.75)
                    {
                        //TURN
                        Command newCommand;
                        if (personX < -0.3)
                        {
                            newCommand = new Command(Command.actions.TURN_RIGHT, speed * 2, 0, -1);
                            commandQueue.Enqueue(newCommand);
                            lastCommand = newCommand;
                        }
                        else if (personX > 0.3)
                        {
                            newCommand = new Command(Command.actions.TURN_LEFT, speed * 2, 0, 1);
                            commandQueue.Enqueue(newCommand);
                            lastCommand = newCommand;
                        }

                        //JUST STOP
                        else
                        {
                            newCommand = new Command(Command.actions.STOP);
                            stop = true;
                            if (lastCommand != newCommand)
                            {
                                commandQueue.Enqueue(newCommand);
                                lastCommand = newCommand;
                            }

                        }

                    }

                    //FORWARD
                    else if (personMagnitude > 1.75)
                    {
                        //TURN
                        Command newCommand;
                        if (personX < -0.3)
                        {
                            newCommand = new Command(Command.actions.TURN_RIGHT, speed, 0, rotation);
                            commandQueue.Enqueue(newCommand);
                            lastCommand = newCommand;
                        }
                        else if (personX > 0.3)
                        {
                            newCommand = new Command(Command.actions.TURN_LEFT, speed, 0, rotation);
                            commandQueue.Enqueue(newCommand);
                            lastCommand = newCommand;
                        }

                        //JUST FORWARD
                        else
                        {
                            newCommand = new Command(Command.actions.FORWARD, speed, 0, 0);
                            commandQueue.Enqueue(newCommand);
                            lastCommand = newCommand;
                        }
                    }
                }
                else //if the person is not being tracked
                {
                    missCounter++;
                    if (missCounter > 30)
                    {
                        commandQueue.Enqueue(new Command(Command.actions.STOP));
                        stop = true;
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
            // Thread.Sleep(500);
            while (true)
            {
                if (stop)
                {
                    commandQueue.Clear();
                    commandQueue.Enqueue(new Command(Command.actions.STOP, 0, 0, 0));
                }
                if (commandQueue.Count > 0)
                {
                    Command currentCommand = commandQueue.Dequeue();
                    switch (currentCommand.action)
                    {
                        case Command.actions.FORWARD:
                            arduino.SendString("MOVE 0 " + currentCommand.magnitude + " 0");
                            break;
                        case Command.actions.BACK:
                            arduino.SendString("MOVE 0 -" + currentCommand.magnitude + " 0");
                            break;
                        case Command.actions.TURN_LEFT:
                            arduino.SendString("MOVE 0 " + currentCommand.magnitude + " " + currentCommand.rotation);
                            break;
                        case Command.actions.TURN_RIGHT:
                            arduino.SendString("MOVE 0 " + currentCommand.magnitude + " " + currentCommand.rotation);
                            break;
                        case Command.actions.STOP:
                            arduino.SendString("STOP");
                            break;
                        default:
                            arduino.SendString("STOP");
                            break;
                    }
                }
                Thread.Sleep(200);
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
