using System;

namespace MTE204Project.Models
{
    public class FinalAngles
    {
        //Stores all the final angles to be displayed 
        public double principleAngle, link1Angle, link2Angle, link3Angle, endPsi;

        public FinalAngles(double principleAngle, double link1Angle, double link2Angle, double link3Angle, double endPsi)
        {
            //Convert angles to degrees then ensure angle is between -360 and 360 
            this.link1Angle = Math.Round((link1Angle * 180 / Math.PI) % 360, 2);
            this.link2Angle = Math.Round((link2Angle * 180 / Math.PI) % 360, 2);
            this.link3Angle = Math.Round((link3Angle * 180 / Math.PI) % 360, 2);

            this.principleAngle = Math.Round(principleAngle,2);
            this.endPsi = endPsi;

            //If angles are negative make positive equivalent
            if (this.link1Angle < 0)
            {
                this.link1Angle += 360;
            }
            if (this.link2Angle < 0)
            {
                this.link2Angle += 360;
            }
            if (this.link3Angle < 0)
            {
                this.link3Angle += 360;
            }
        }
    }
}
