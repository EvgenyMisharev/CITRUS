using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace CITRUS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class MySQL_Test : IExternalCommand
    {
        //public class ApplicationContext : DbContext
        //{
        //    public DbSet<User> Users { get; set; }

        //    public ApplicationContext()
        //    {
        //        Database.EnsureCreated();
        //    }
        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        optionsBuilder.UseMySql(
        //            "server=localhost;user=root;password=lp254469444;database=revitdb;",
        //            new MySqlServerVersion(new Version(8, 0, 21))
        //        );
        //    }
        //}



        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {



            return Result.Succeeded;
        }
    }
}
