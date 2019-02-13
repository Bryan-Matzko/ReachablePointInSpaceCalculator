using System;
using System.Collections.Generic;
using System.Linq;

namespace ReachablePointInSpace
{
    public static class MatrixSolver
    {
        #region Variables
        private readonly static double[] guesses = new double[3];
        private readonly static double[] f = new double[3];
        private readonly static double[,] jInv = new double[3, 3];

        private static double endR, endZ, endPsi, inerRadius;

        private const double TOL = 0.00001;
        private const double ANGLETOL = 0.75;
        private const double ABSVALUETOL = 0.00000001;
        private const int MAXITERATIONS = 150;

        public static double innerRadius;
        public static double[] lengths = new double[3];

        public static bool runFastFlag;
        #endregion

        #region update F and J-Inverse
        //Array representing F from the stored guesses by class
        private static void UpdateF()
        {
            f[0] = (lengths[0] * Math.Sin(guesses[0])) + (lengths[1] * Math.Sin(guesses[1] + guesses[0]))
                + (lengths[2] * Math.Sin(endPsi)) - endR;
            f[1] = (lengths[0] * Math.Cos(guesses[0])) + (lengths[1] * Math.Cos(guesses[0] + guesses[1]))
                + (lengths[2] * Math.Cos(endPsi)) - endZ;
            f[2] = guesses[0] + guesses[1] + guesses[2] - endPsi;
        }

        //Update the Jacobian inverse array from the stored guesses by class
        private static void UpdateJInv()
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
        public static bool WillSolutionConverge(double finalR, double finalZ, double finalPsi)
        {
            //reset guesses and set some of the determined class variabls
            SetGuesses();
            endR = finalR;
            endZ = finalZ;
            endPsi = finalPsi;

            double[] deltaGuess = { 10, 10, 10 };
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
        public static void SetGuesses()
        {
            guesses[0] = 10 * Math.PI / 180;
            guesses[1] = 20 * Math.PI / 180;
            guesses[2] = 30 * Math.PI / 180;
        }

        private static void SetLengths(double length1, double length2, double length3)
        {
            lengths[0] = length1;
            lengths[1] = length2;
            lengths[2] = length3;
        }

        private static double SetPrincipleAngles(double x, double y)
        {
            //Sets principle angles at initiation. If an x or y is 0 to start
            //Special cases need to be handled
            if (x >= 0 && Math.Abs(y) < ABSVALUETOL)
            {
                return 0;
            }
            else if (x < 0 && Math.Abs(y) < ABSVALUETOL)
            {
                return -180;
            }
            else if (Math.Abs(x) < ABSVALUETOL && y >= 0)
            {
                return 90;
            }
            else if (Math.Abs(x) < ABSVALUETOL && y < 0)
            {
                return -270;
            }
            else
            {
                return Math.Tan(y / x);
            }
        }
        #endregion


        #region Run 360 Degrees
        public static IEnumerable<FinalAngles> RunPointSimulation(double endXIn, double endYIn, double endZIn)
        {
            //Calculate and set class variables
            endR = Math.Sqrt(endXIn * endXIn + endYIn * endYIn);
            endZ = endZIn;

            //Determine distance of the requested point to origin.
            double radius = Math.Sqrt(endR * endR + endZ * endZ);
            //If the point is outside of the bounds notify user and return empty list
            if (radius < innerRadius || radius > OuterRadius())
            {
                Console.WriteLine("No iterations run, point is inside a divergence zone");
                return new List<FinalAngles>();
            }

            //Declare variables to store  diverging and non diverging points
            List<FinalAngles> possibleConfigurations = new List<FinalAngles>();
            Console.WriteLine($"Principle Angle: {SetPrincipleAngles(endXIn, endYIn)}");

            //Iterate all 360 degrees
            for (int angle = 0; angle < 360; angle++)
            {
                //Degrees to radians
                endPsi = angle * Math.PI / 180;

                if (WillSolutionConverge(endR, endZ, endPsi))
                {
                    //Create a configuration after rounding
                    var config = new FinalAngles(guesses[0], guesses[1], guesses[2]);

                    //If we are doing a blind run (don't care about duplicates)
                    if (runFastFlag)
                    {
                        possibleConfigurations.Add(config);
                    }
                    //Otherwise remove duplicates that have all angles within the tolerance of a
                    //same configuration. Assume if criteria is met, then duplicates
                    else if (possibleConfigurations.All(configOption =>
                        !
                    (configOption.link1Angle - config.link1Angle < ANGLETOL &&
                        configOption.link2Angle - config.link2Angle < ANGLETOL &&
                        configOption.link3Angle - config.link3Angle < ANGLETOL)))
                    {
                        possibleConfigurations.Add(config);
                    }
                }

                //Reset the guesses
                SetGuesses();
            }

            return possibleConfigurations;
        }
        #endregion

        #region Checking
        public static void SetRadius()
        {
            //Sorte the array from lowest to highest lengths
            Array.Sort(lengths);
            //If no diverging points set inner radius to 0 otherwise set the inner radius
            if (lengths[0] + lengths[1] >= lengths[2])
            {
                inerRadius = 0;
            }
            else
            {
                inerRadius = lengths[2] - lengths[0] - lengths[1];
            }

            SetGuesses();
        }

        public static double OuterRadius()
        {
            //Determine the outer radius
            return lengths[0] + lengths[1] + lengths[2];
        }

        #endregion
    }
}
