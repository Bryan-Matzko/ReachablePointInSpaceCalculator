using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MTE204Project.Helpers;
using MTE204Project.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MTE204Project.Controllers
{
    public class DivergenceAppController : Controller
    {
        private const double EPSILON = 0.00000001;

        //Other variables
        private static readonly IEnumerable<Point> points = new List<Point>();
        private static readonly double[] point = new double[3];
        private static readonly bool _runFast = true;

        // GET: /<controller>/
        public IActionResult Index(
            double l1, double l2, double l3,
            double x, double y, double z)
        {
            ViewData["x"] = x;
            ViewData["y"] = y;
            ViewData["z"] = z;
            ViewData["l1"] = l1;
            ViewData["l2"] = l2;
            ViewData["l3"] = l3;
            ViewData["runFast"] = _runFast;

            InitializeMatrixSolver(new double[3] { l1, l2, l3 });

            if (Math.Abs(l1) > EPSILON &&
                Math.Abs(l2) > EPSILON &&
                Math.Abs(l3) > EPSILON)
                return View(MatrixSolver.RunPointSimulation(x, y, z));
            else
                return View(new List<FinalAngles>());
        }

        private static void InitializeMatrixSolver(double[] lengths)
        {
            MatrixSolver.lengths = lengths;
            MatrixSolver.SetRadius();
            MatrixSolver.runFastFlag = _runFast;
        }
    }
}
