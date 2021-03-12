using System;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
namespace MultiPatientExecutable
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                using (Application app = Application.CreateApplication())
                {
                    Execute(app);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                Console.Read();
            }
        }
        public static void Execute(Application app)
        {
            //Console.Read();
            foreach (var summary in app.PatientSummaries)
            {
                Patient patient = app.OpenPatient(summary);
                // Code here
                IEnumerable<PlanSetup> isDoseOk =
                patient.Courses.SelectMany(e => e.PlanSetups).Where(e => e.IsDoseValid);
                string message = string.Join("\n", isDoseOk.Select(e => e.Id));
                Console.WriteLine(message);
                Console.ReadLine();
                app.ClosePatient();
            }
        }
    }
}