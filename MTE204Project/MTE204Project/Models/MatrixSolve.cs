using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTE204Project.Models
{
    public class MatrixSolver
    {
        #region Variables
        private double[] lengths = new double[3];
        private double[] guesses = new double[3];

        private double[] f = new double[3];
        private double[,] jInv = new double[3, 3];

        private const double TOL = 0.00001, MAXITERATIONS = 150;
        private double endR, endZ, endPsi, inerRadius;

        public double innerRadius;
        #endregion

        #region Initialization
        public MatrixSolver(double[] lengths)
        {
            this.lengths = lengths;
            SetRadius(lengths);
            SetGuesses();
        }
        #endregion

        #region update F and J-Inverse
        private void UpdateF()
        {
            f[0] = lengths[0] * Math.Sin(guesses[0]) + lengths[1] * Math.Sin(guesses[1] + guesses[0])
                + lengths[2] * Math.Sin(endPsi) - endR;
            f[1] = lengths[0] * Math.Cos(guesses[0]) + lengths[1] * Math.Cos(guesses[0] + guesses[1])
                + lengths[2] * Math.Cos(endPsi) - endZ;
            f[2] = guesses[0] + guesses[1] + guesses[2] - endPsi;
        }

        private void UpdateJInv()
        {
            jInv[0, 0] = -Math.Sin(guesses[0] + guesses[1]) / (lengths[0] * Math.Cos(guesses[0] + guesses[1]) * Math.Sin(guesses[0]) - lengths[0] * Math.Sin(guesses[0] + guesses[1]) * Math.Cos(guesses[0]));
            jInv[0, 1] = -Math.Cos(guesses[0] + guesses[1]) / (lengths[0] * Math.Cos(guesses[0] + guesses[1]) * Math.Sin(guesses[0]) - lengths[0] * Math.Sin(guesses[0] + guesses[1]) * Math.Cos(guesses[0]));
            jInv[0, 2] = 0;
            jInv[1, 0] = (lengths[1] * Math.Sin(guesses[0] + guesses[1]) + lengths[0] * Math.Sin(guesses[0])) / (lengths[0] * lengths[1] * Math.Cos(guesses[0] + guesses[1]) * Math.Sin(guesses[0]) - lengths[0] * lengths[1] * Math.Sin(guesses[0] + guesses[1]) * Math.Cos(guesses[0]));
            jInv[1, 1] = (lengths[1] * Math.Cos(guesses[0] + guesses[1]) + lengths[0] * Math.Cos(guesses[0])) / (lengths[0] * lengths[1] * Math.Cos(guesses[0] + guesses[1]) * Math.Sin(guesses[0]) - lengths[0] * lengths[1] * Math.Sin(guesses[0] + guesses[1]) * Math.Cos(guesses[0]));
            jInv[1, 2] = 0;
            jInv[2, 0] = -Math.Sin(guesses[0]) / (lengths[1] * Math.Cos(guesses[0] + guesses[1]) * Math.Sin(guesses[0]) - lengths[1] * Math.Sin(guesses[0] + guesses[1]) * Math.Cos(guesses[0]));
            jInv[2, 1] = -Math.Cos(guesses[0]) / (lengths[1] * Math.Cos(guesses[0] + guesses[1]) * Math.Sin(guesses[0]) - lengths[1] * Math.Sin(guesses[0] + guesses[1]) * Math.Cos(guesses[0]));
            jInv[2, 2] = 1;
        }
        #endregion

        #region Solver
        public bool WillSolutionConverge(double finalR, double finalZ, double finalPsi)
        {
            SetGuesses();
            endR = finalR;
            endZ = finalZ;
            endPsi = finalPsi;

            double[] deltaGuess = new double[3] { 10, 10, 10 };
            int iterationCount = 0;
            while ((
                Math.Abs(deltaGuess[0]) > TOL ||
                Math.Abs(deltaGuess[1]) > TOL ||
                Math.Abs(deltaGuess[2]) > TOL)
                && iterationCount < MAXITERATIONS)
            {
                UpdateF();
                UpdateJInv();

                deltaGuess[0] = f[0] * jInv[0, 0] + f[1] * jInv[0, 1] + f[2] * jInv[0, 2];
                deltaGuess[1] = f[0] * jInv[1, 0] + f[1] * jInv[1, 1] + f[2] * jInv[1, 2];
                deltaGuess[2] = f[0] * jInv[2, 0] + f[1] * jInv[2, 1] + f[2] * jInv[2, 2];

                guesses[0] -= deltaGuess[0];
                guesses[1] -= deltaGuess[1];
                guesses[2] -= deltaGuess[2];

                iterationCount++;
            }

            return iterationCount != MAXITERATIONS;
        }
        #endregion

        #region Set function variables
        private void SetGuesses()
        {
            guesses[0] = 10 * Math.PI / 180;
            guesses[1] = 20 * Math.PI / 180;
            guesses[2] = 30 * Math.PI / 180;
        }
        private void SetLengths(double length1, double length2, double length3)
        {
            lengths[0] = length1;
            lengths[1] = length2;
            lengths[2] = length3;
        }
        private double SetPrincipleAngles(double x, double y)
        {
            if (x >= 0 && y == 0)
                return 0;
            else if (x < 0 && y == 0)
                return -180;
            else if (x == 0 && y >= 0)
                return 90;
            else if (x == 0 && y < 0)
                return -270;
            else
                return Math.Tan(y / x);
        }
        #endregion


        #region Run 360 Degrees
        public List<FinalAngles> RunPointSimulation(double endX, double endY, double endZ)
        {
            this.endR = Math.Sqrt(endX * endX + endY * endY);
            this.endZ = endZ;

            double radius = Math.Sqrt(endR * endR + endZ * endZ);
            if (radius < innerRadius || radius > OuterRadius())
            {
                return new List<FinalAngles>();
            }

            List<FinalAngles> possibleConfigurations = new List<FinalAngles>();
            List<double> wontWork = new List<double>();
            double principleAngle = SetPrincipleAngles(endX, endY);


            for (int angle = 0; angle < 360; angle++)
            {
                endPsi = angle * Math.PI / 180;
                if (WillSolutionConverge(this.endR, endZ, endPsi))
                    possibleConfigurations.Add(new FinalAngles(principleAngle, guesses[0], guesses[1], guesses[2], angle));
                else
                    wontWork.Add(endPsi);

                SetGuesses();
            }

            return possibleConfigurations;

        }
        #endregion

        #region Checking
        public void SetRadius(double[] arrayLengths)
        {
            Array.Sort(arrayLengths);
            if (arrayLengths[0] + arrayLengths[1] >= arrayLengths[2])
                this.inerRadius = 0;
            else
                this.inerRadius = arrayLengths[2] - arrayLengths[0] - arrayLengths[1];
        }

        public double OuterRadius()
        {
            return lengths[0] + lengths[1] + lengths[2];
        }

        #endregion
    }
}
