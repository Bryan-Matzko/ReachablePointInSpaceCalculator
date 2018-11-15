using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MTE204Project.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MTE204Project.Controllers
{
    public class DivergenceAppController : Controller
    {
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
            double[] lengths = new double[] { l1, l2, l3 };
            MatrixSolver matrixSolver = new MatrixSolver(lengths);

            if (l1 != 0 && l2 != 0 && l3 != 0)
                return View(matrixSolver.RunPointSimulation(x, y, z));
            else
                return View(new List<FinalAngles>());
        }
    }
}
