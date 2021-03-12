using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;


namespace PlanDifferences
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()//string [] args)
        {
            try
            {
                using (var app = VMS.TPS.Common.Model.API.Application.CreateApplication())
                {
                    Execute(app);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                //Console.Read();
            }
        }
        public static void Execute(VMS.TPS.Common.Model.API.Application app)
        {
            string[] args = { "", "", "", "", "", "", "", "" };
            
            args[0] = "$P-901";
            
            Patient this_patient = app.OpenPatientById(args[0]);

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Form1(this_patient, args));
            app.ClosePatient();
            app.Dispose();

        }
    }
}
