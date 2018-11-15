using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class MatrixSolver
    {
        #region Variables
        //Stores all variables corresponding to the class
        private double[] lengths = new double[3];
        private double[] guesses = new double[3];

        private double[] f = new double[3];
        private double[,] jInv = new double[3, 3];

        private const double TOL = 0.00001, MAXITERATIONS = 150;
        private double endR, endZ, endPsi, inerRadius;

        public double innerRadius;
        #endregion

        #region Initialization
        //Set lengths when matrix is created and set radius and guesses
        public MatrixSolver(double[] lengths)
        {
            this.lengths = lengths;
            SetRadius(lengths);
            SetGuesses();
        }
        #endregion

        #region update F and J-Inverse
        //Array representing F from the stored guesses by class
        private void UpdateF()
        {
            f[0] = lengths[0] * Math.Sin(guesses[0]) + lengths[1] * Math.Sin(guesses[1] + guesses[0])
                + lengths[2] * Math.Sin(endPsi) - endR;
            f[1] = lengths[0] * Math.Cos(guesses[0]) + lengths[1] * Math.Cos(guesses[0] + guesses[1])
                + lengths[2] * Math.Cos(endPsi) - endZ;
            f[2] = guesses[0] + guesses[1] + guesses[2] - endPsi;
        }

        //Update the Jacobian inverse array from the stored guesses by class
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
        //Checks if solution will converge
        public bool WillSolutionConverge(double finalR, double finalZ, double finalPsi)
        {
            //reset guesses and set some of the determined class variabls
            SetGuesses();
            endR = finalR;
            endZ = finalZ;
            endPsi = finalPsi;

            double[] deltaGuess = new double[3] { 10, 10, 10 };
            int iterationCount = 0;
            //While any of the delta guesses is greater than a set out tolerance
            while ((
                Math.Abs(deltaGuess[0]) > TOL ||
                Math.Abs(deltaGuess[1]) > TOL ||
                Math.Abs(deltaGuess[2]) > TOL)
                && iterationCount < MAXITERATIONS)
            {
                //Update F and Jacobian Inverse with the new guess
                UpdateF();
                UpdateJInv();

                //Determine the new delta guess
                deltaGuess[0] = f[0] * jInv[0, 0] + f[1] * jInv[0, 1] + f[2] * jInv[0, 2];
                deltaGuess[1] = f[0] * jInv[1, 0] + f[1] * jInv[1, 1] + f[2] * jInv[1, 2];
                deltaGuess[2] = f[0] * jInv[2, 0] + f[1] * jInv[2, 1] + f[2] * jInv[2, 2];

                //Set the new guess
                guesses[0] -= deltaGuess[0];
                guesses[1] -= deltaGuess[1];
                guesses[2] -= deltaGuess[2];

                //Increase iteration as while loop relies on it
                iterationCount++;
            }

            //If the iteration count isn't at it's max, a solution was found
            //Return true if solution found
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
            //Sets principle angles at initiation. If an x or y is 0 to start
            //Special cases need to be handled
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
            //Calculate and set class variables
            this.endR = Math.Sqrt(endX * endX + endY * endY);
            this.endZ = endZ;

            //Determine distance of the requested point to origin.
            double radius = Math.Sqrt(this.endR * this.endR + this.endZ * this.endZ);
            //If the point is outside of the bounds notify user and return empty list
            if (radius < innerRadius || radius > OuterRadius())
            {
                Console.WriteLine("No iterations run, point is inside a divergence zone");
                return new List<FinalAngles>();
            }

            //Declare variables to store  diverging and non diverging points
            List<FinalAngles> possibleConfigurations = new List<FinalAngles>();
            //Store the principle angle
            double principleAngle = SetPrincipleAngles(endX, endY);

            //Iterate all 360 degrees
            for (int angle = 0; angle < 360; angle++)
            {
                //Degrees to radians
                endPsi = angle * Math.PI / 180;

                //If converges add results to return variable 
                if (WillSolutionConverge(this.endR, endZ, endPsi))
                    possibleConfigurations.Add(new FinalAngles(principleAngle, guesses[0], guesses[1], guesses[2]));

                //Reset the guesses
                SetGuesses();
            }

            return possibleConfigurations;

        }
        #endregion

        #region Checking
        public void SetRadius(double[] arrayLengths)
        {
            //Sorte the array from lowest to highest lengths
            Array.Sort(arrayLengths);
            //If no diverging points set inner radius to 0 otherwise set the inner radius
            if (arrayLengths[0] + arrayLengths[1] >= arrayLengths[2])
                this.inerRadius = 0;
            else
                this.inerRadius = arrayLengths[2] - arrayLengths[0] - arrayLengths[1];
        }

        public double OuterRadius()
        {
            //Determine the outer radius
            return lengths[0] + lengths[1] + lengths[2];
        }

        #endregion
    }
}
