using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {

        }

        public void Execute(ScriptContext context) //, System.Windows.Window window)
        {
            try
            {
                string patient_id = "\"" + context.Patient.Id.ToString() + "\"";
                string patient_name = "\"" + context.Patient.LastName.ToString() + ", " + context.Patient.FirstName.ToString() + "\""; 
                string course_id = "\"" + context.Course.Id.ToString() + "\"";
                string plan_id = "\"" + context.PlanSetup.Id.ToString() + "\"";

                var patientposition = context.StructureSet.Image.ImagingOrientation.ToString(); //HeadFirstSupine, HeadFirstProne, FeetFirstSupine, FeetFirstProne
                string ptpos = "Patient Position = " + patientposition;

                var UOx = context.Image.UserOrigin.x;
                var UOy = context.Image.UserOrigin.y;
                var UOz = context.Image.UserOrigin.z;

                string args = patient_name + " " + patient_id + " " + course_id + " " + plan_id + " " + ptpos + " " + UOx.string() + " " + UOy.string() + " " + UOz.string();

                //Process.Start(@"\\Cnmborocimg15\va_data$\ProgramData\Vision\PublishedScripts\ExportPlanData.exe", args);
                //Process.Start(@"\\C$\Users\S792357\source\repos\test_Wforms_app\bin\Release\test_Wforms_app.exe");//, args);
                Process.Start(@"\\cs.msds.kp.org\ncal\dsa\userdir01\S792357\Scripting\test_Wforms_app\test_Wforms_app.exe",args);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);// "Failed to start application.");
            }
        }
    }
}