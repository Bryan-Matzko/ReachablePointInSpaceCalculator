using System;
using System.Collections.Generic;
using System.Linq;

namespace ReachablePointInSpace
{
    class MainClass
    {
        // Set this flag to false to avoid duplicates within tolerance
        // NOTE: This will slow down execution
        private static readonly bool runFast = false;

        //Other variables
        private static readonly IEnumerable<Point> points = new List<Point>();
        private static IEnumerable<FinalAngles> possibleSolutions;
        private static readonly double[] lengths = new double[3];
        private static readonly double[] point = new double[3];

        static void Main(string[] args)
        {
            GetSystemParameters();

            //Run the simulation for the provided point
            InitializeMatrix();
            possibleSolutions = MatrixSolver.RunPointSimulation(point[0], point[1], point[2]);

            //Display Solutions
            if (possibleSolutions.Any())
            {
                PrintSln();
            }

            //Wait for user to terminate program
            Console.WriteLine("\nHit Enter to Exit");
            Console.ReadLine();
        }

        private static void GetSystemParameters()
        {
            // Read in params from the user
            for (int i = 0; i < 3; i++)
            {
                Console.Write($"Length {i + 1} :");
                String Result = Console.ReadLine();

                while (!double.TryParse(Result, out lengths[i]))
                {
                    Console.Write($"Not a valid number, try again. \nLength {i + 1} :");
                    Result = Console.ReadLine();
                }
            }

            char[] xyz = { 'x', 'y', 'z' };
            for (int i = 0; i < 3; i++)
            {
                Console.Write($"{xyz[i]} value :");
                String Result = Console.ReadLine();

                while (!double.TryParse(Result, out point[i]))
                {
                    Console.Write($"Not a valid number, try again. \n{xyz[i]} value :");
                    Result = Console.ReadLine();
                }
            }
        }

        private static void InitializeMatrix()
        {
            MatrixSolver.lengths = lengths;
            MatrixSolver.SetRadius();
            MatrixSolver.runFastFlag = runFast;
        }

        private static void PrintSln()
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
    }
}
