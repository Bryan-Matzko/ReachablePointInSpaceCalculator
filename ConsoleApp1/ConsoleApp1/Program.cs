using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        //Set this flag to false to avoid duplicates within tolerance
        //This will slow down execution
        private static readonly bool runFast = false;

        //This is the main function that determined divergent points for 
        //A configuration described by assigned variables
        static void Main(string[] args)
        {
            //Declare variables to store lengths and points
            List<Points> points = new List<Points>();
            double[] lengths = new double[3];
            double[] point = new double[3];

            //Read in parameters from the console
            for(int i = 0; i < 3; i++)
            {
                Console.Write($"Length{i} ");
                double.TryParse(Console.ReadLine(), out lengths[i]);
            }

            Console.Write("x value ");
            double.TryParse(Console.ReadLine(), out point[0]);
            Console.Write("y value ");
            double.TryParse(Console.ReadLine(), out point[1]);
            Console.Write("z value ");
            double.TryParse(Console.ReadLine(), out point[2]);

            //Initialize Matrix Solver With Inputs
            MatrixSolver.lengths = lengths;
            MatrixSolver.SetRadius(lengths);
            MatrixSolver.runFastFlag = runFast;

            //Run the simulation for the provided point
            List<FinalAngles> possibleSolutions =
                MatrixSolver.RunPointSimulation(point[0], point[1], point[2]);

            //Display Solutions
            if(possibleSolutions.Count != 0)
            {
                possibleSolutions = possibleSolutions
                    .OrderBy(slns => slns.link1Angle)
                    .ThenBy(slns => slns.link2Angle)
                    .ThenBy(slns => slns.link3Angle)
                    .ToList();

                Console.WriteLine("Format: Link 1 angle, Link 2 angle, Link 3 angle \n");

                foreach (var solution in possibleSolutions)
                {
                    Console.WriteLine($"{solution.link1Angle}, {solution.link2Angle}, {solution.link3Angle}");
                }
            }
           
            //Wait for user to terminate program
            Console.WriteLine("\nHit Enter to Exit");
            Console.ReadLine();
        }
    }
}
