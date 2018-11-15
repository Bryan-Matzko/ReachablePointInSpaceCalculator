using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTE204Project.Models
{
    public class FinalAngles
    {
        public double principleAngle, link1Angle, link2Angle, link3Angle, endPsi;

        public FinalAngles(double principleAngle, double link1Angle, double link2Angle, double link3Angle, double endPsi)
        {
            this.principleAngle = principleAngle;
            this.endPsi = endPsi;
            this.link1Angle = (link1Angle * 180 / Math.PI)%360;
            this.link2Angle = (link2Angle * 180 / Math.PI)%360;
            this.link3Angle = (link3Angle * 180 / Math.PI)%360;

            if (this.link1Angle < 0)
                this.link1Angle += 360;
            if (this.link2Angle < 0)
                this.link2Angle += 360;
            if (this.link3Angle < 0)
                this.link3Angle += 360;
        }
    }
}
