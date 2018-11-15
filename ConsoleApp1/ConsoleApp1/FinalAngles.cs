using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FinalAngles
    {
        //Stores all the final angles to be displayed 
        public double principleAngle, link1Angle, link2Angle, link3Angle;

        public FinalAngles(double principleAngle, double link1Angle, double link2Angle, double link3Angle)
        {
            //Convert angles to degrees then ensure angle is between -360 and 360 
            this.principleAngle = principleAngle;
            this.link1Angle = (link1Angle * 180 / Math.PI) % 360;
            this.link2Angle = (link2Angle * 180 / Math.PI) % 360;
            this.link3Angle = (link3Angle * 180 / Math.PI) % 360;

            //If angles are negative make positive equivalent
            if (this.link1Angle < 0)
                this.link1Angle += 360;
            if (this.link2Angle < 0)
                this.link2Angle += 360;
            if (this.link3Angle < 0)
                this.link3Angle += 360;
        }
    }
}
