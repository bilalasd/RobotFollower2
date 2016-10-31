using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFollowerWPF2
{
    public class Command
    {
        public Command(actions initialAction, double initialMagnitude, double initialAngle, double initialRotation)
        {
            action = initialAction;
            magnitude = initialMagnitude;
            angle = initialAngle;
            rotation = initialRotation;
        }

        public Command(actions initialAction)
        {
            action = initialAction;
            magnitude = 0;
            angle = 0;
        }

        public enum actions
        {
            FORWARD,
            BACK,
            TURN_LEFT,
            TURN_RIGHT,
            STOP
        };

        public double magnitude = 0;
        public double angle = 0;
        public double rotation = 0;
        public actions action;


    }
}
