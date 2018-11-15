using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        /*
        //This is the main function that determined divergent points for 
        //A configuration described by assigned variables
        static void Main(string[] args)
        {
            List<Points> points = new List<Points>();
            double[] lengths = new double[] { 2, 2, 4 };
            double maxLength = lengths[0] + lengths[1] + lengths[2];
            MatrixSolver matrixSolver = new MatrixSolver(lengths);

            //Write function to detect convergence.
            double factor = 10; //Incremental factor
            for (double r = 0; r <= maxLength; r += 0.1)
            {
                for (double z = 0; z <= Math.Sqrt(maxLength * maxLength - r * r); z += 0.1)
                {
                    int psi = 0;
                    bool converges = false;

                    z = Math.Round(z * factor) / factor;
                    r = Math.Round(r * factor) / factor;

                    while (!converges && psi < 360)
                    {
                        converges = matrixSolver.WillSolutionConverge(r, z, psi * Math.PI / 180);
                        psi++;
                    }

                    if (!converges)
                    {
                        points.Add(new Points(r, z));
                        Console.WriteLine($"{r}, {z}");
                    }
                }
            }
        }
        */

        static void Main(string[] args)
        {
            //Declare variables to store lengths and points
            List<Points> points = new List<Points>();
            double[] lengths = new double[3];
            double[] point = new double[3];

            //Read in parameters from the console
            Console.Write("Length1 ");
            double.TryParse(Console.ReadLine(), out lengths[0]);
            Console.Write("Length2 ");
            double.TryParse(Console.ReadLine(), out lengths[1]);
            Console.Write("Length3 ");
            double.TryParse(Console.ReadLine(), out lengths[2]);
            Console.Write("x value ");
            double.TryParse(Console.ReadLine(), out point[0]);
            Console.Write("y value ");
            double.TryParse(Console.ReadLine(), out point[1]);
            Console.Write("z value ");
            double.TryParse(Console.ReadLine(), out point[2]);
            
            //Instantiate instance of the matrix solver class to allow
            //For the function access
            MatrixSolver matrixSolver = new MatrixSolver(lengths);

            //Run the simulation for the provided point
            List<FinalAngles> possibleSolutions =
                matrixSolver.RunPointSimulation(point[0], point[1], point[2]);

            //If there are solutions display them
            if(possibleSolutions.Count != 0)
            {
                Console.WriteLine($"Principle Angle: {possibleSolutions[0].principleAngle}");
                Console.WriteLine("Format: Link 1 angle, Link 2 angle, Link 3 angle");

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
