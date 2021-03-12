using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;       //Add Microsoft Excel Object Library Under References-> COM tab
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ExportPlanData
{
    public partial class ExportPlanData : Form
    {
        public DataBlock DATA;

        public ExportPlanData(string patient_id, string course_id, string plan_id, string file_path)
        {
            DATA.PatientId = patient_id;
            DATA.CourseId = course_id;
            DATA.PlanId = plan_id;
            DATA.ExportFilePath = file_path;
            InitializeComponent();
            InitializeData();
        }

        public void InitializeData()
        {
            Info_PatientID.Text = DATA.PatientId;
            Info_PlanID.Text = DATA.CourseId + " \\ " + DATA.PlanId;
            TBox_AllData.Text = "";
            TBox_Comments.Text = "";
            RichTBox_CheckData.Text = "";
        }


        private void Button_Extract_Click(object sender, EventArgs e)
        {
            InitializeData();
            var app = VMS.TPS.Common.Model.API.Application.CreateApplication();
            Patient this_patient = app.OpenPatientById(DATA.PatientId);

            if (this_patient != null)
            {
                IEnumerable<Course> courses = this_patient.Courses;
                Course crs = courses.FirstOrDefault(Course => Course.Id == DATA.CourseId);

                if (crs != null)
                {
                    IEnumerable<Diagnosis> diags = crs.Diagnoses;
                    string DiagCode = "NA";
                    if (diags.Any())
                    {
                        DiagCode = diags.First().Code;
                    }
                    DATA.DiagnosisCode = DiagCode;


                    IEnumerable<PlanSetup> plans = crs.PlanSetups;
                    PlanSetup pln = plans.FirstOrDefault(PlanSetup => PlanSetup.Id == DATA.PlanId);
                    if (pln != null)
                    {
                        // CT Image Data
                        DATA.ImageDate = pln.StructureSet.Image.CreationDateTime.Value.ToString("g");
                        DATA.ImageId = pln.StructureSet.Image.Id;
                        DATA.NumberOfImages = pln.StructureSet.Image.ZSize;
                        DATA.ImageResX = pln.StructureSet.Image.XRes;
                        DATA.ImageResY = pln.StructureSet.Image.YRes;
                        DATA.ImageResZ = pln.StructureSet.Image.ZRes;

                        //foreach (Structure s in pln.StructureSet.Structures)
                        //{
                        //    if (s.DicomType == "EXTERNAL")
                        //    {
                        //        MessageBox.Show("Found structure => " + s.Id);     
                        //    }
                        //}

                        DATA.ContourId = pln.StructureSet.Id;

                        //ENTRY INFORMATION
                        DATA.EntryDateTime = DateTime.Now.ToString("g");
                        DATA.EnteredBy = app.CurrentUser.Name;

                        // PATIENT INFORMATION
                        DATA.PatientFirstName = this_patient.FirstName.ToString().Substring(0, 1);
                        DATA.PatientLastName = this_patient.LastName.ToString().Substring(0, 1);
                        DATA.PatientId = this_patient.Id.ToString();

                        // USER INFORMATION
                        DATA.PlannerFullName = pln.PlanningApproverDisplayName;

                        // APPROVAL DATE
                        IEnumerable<ApprovalHistoryEntry> pln_approvals = pln.ApprovalHistory;
                        foreach (var apstat in pln_approvals)
                        {
                            if (apstat.ApprovalStatus == PlanSetupApprovalStatus.Reviewed)
                            {
                                DATA.PlanReviewDate = apstat.ApprovalDateTime.ToString("g");
                                DATA.PlanReviewBy = apstat.UserDisplayName;
                            }
                        }



                        // RX INFORMATION
                        RTPrescription rx = pln.RTPrescription;
                        DATA.PhysicianFullName = "NA";
                        DATA.RxSite = "NA";
                        DATA.RxTechnique = "NA";
                        DATA.RxSequence = "NA";
                        DATA.RxNotes = "";
                        DATA.RxTargets = "";

                        DATA.Energy = "";
                        //string EnergyType = "Photon";
                        //string energies = "";

                        if (rx != null)
                        {
                            DATA.PhysicianFullName = rx.HistoryUserDisplayName;
                            DATA.RxSite = rx.Site;
                            DATA.RxTechnique = rx.Technique;
                            DATA.RxSequence = rx.PhaseType;
                            DATA.RxNotes = rx.Notes;
                            DATA.RxGating = rx.Gating;

                            IEnumerable<RTPrescriptionTarget> targets = rx.Targets;
                            foreach (RTPrescriptionTarget trgt in targets)
                                DATA.RxTargets += trgt.TargetId + "; ";


                            //IEnumerable<string> ens = rx.Energies;
                            //foreach (string en in ens)
                            //    DATA.Energy += en + "; ";
                        }

                        // DATA.EnergyMode = EnergyType;


                        DATA.PlanOrientation = Enum.GetName(typeof(PatientOrientation), pln.TreatmentOrientation);
                        DATA.UseGating = pln.UseGating;
                        //DATA.UseJawTracking = pln.OptimizationSetup.UseJawTracking;


                        // FIELD INFORMATION
                        IEnumerable<Beam> bms = pln.Beams;
                        int nbms = 0;
                        double Total_MU = 0.0;
                        //string enEnergyMode = "";
                        double iso_X = 0.0, iso_Y = 0.0, iso_Z = 0.0;

                        DATA.MLCType = "";
                        DATA.ToleranceTable = "";
                        DATA.BolusId = "";
                        DATA.MachineId = "";

                        // bool SRS = false;
                        DATA.UseCouchKick = false;
                        DATA.UseJawTracking = false;
                        List<string> energies = new List<string>();
                        foreach (Beam b in bms)
                        {
                            if (!b.IsSetupField)
                            {
                                nbms++;
                                energies.Add(b.EnergyModeDisplayName);

                                Total_MU += b.Meterset.Value;

                                DATA.MLCType = Enum.GetName(typeof(MLCPlanType), b.MLCPlanType);
                                DATA.MachineId = b.TreatmentUnit.Id;
                                DATA.ToleranceTable = b.ToleranceTableLabel;


                                List<double> x1jaws = new List<double>();
                                List<double> x2jaws = new List<double>();
                                List<double> y1jaws = new List<double>();
                                List<double> y2jaws = new List<double>();


                                ControlPointCollection ctrl_colls = b.ControlPoints;
                                //IEnumerator<ControlPoint> b_ctrl_pts = ctrl_colls.GetEnumerator();
                                foreach (ControlPoint ctrl in ctrl_colls)
                                {
                                    VRect<double> jaws = ctrl.JawPositions;
                                    x1jaws.Add(jaws.X1);
                                    x2jaws.Add(jaws.X2);
                                    y1jaws.Add(jaws.Y1);
                                    y2jaws.Add(jaws.Y2);
                                }

                                double meanX1 = x1jaws.Average();
                                double meanX2 = x2jaws.Average();
                                double meanY1 = y1jaws.Average();
                                double meanY2 = y2jaws.Average();

                                double minX1 = x1jaws.Min();
                                double minX2 = x2jaws.Min();
                                double minY1 = y1jaws.Min();
                                double minY2 = y2jaws.Min();

                                double maxX1 = x1jaws.Max();
                                double maxX2 = x2jaws.Max();
                                double maxY1 = y1jaws.Max();
                                double maxY2 = y2jaws.Max();

                                if (Math.Abs(maxX1 - minX1) > 0.01 || Math.Abs(maxX2 - minX2) > 0.01 || Math.Abs(maxY1 - minY1) > 0.01 || Math.Abs(maxY2 - minY2) > 0.01)
                                    DATA.UseJawTracking = true;


                                // if (b.Technique.ToString().Contains("SRS"))
                                //    isSRS = true;

                                if (!DATA.UseCouchKick && b.ControlPoints.First().PatientSupportAngle != 0.0)
                                    DATA.UseCouchKick = true;

                                double delta_X = (b.IsocenterPosition.x - pln.StructureSet.Image.UserOrigin.x) / 10.0;
                                double delta_Y = (b.IsocenterPosition.y - pln.StructureSet.Image.UserOrigin.y) / 10.0;
                                double delta_Z = (b.IsocenterPosition.z - pln.StructureSet.Image.UserOrigin.z) / 10.0;
                                iso_X = delta_X;
                                iso_Y = delta_Y;
                                iso_Z = delta_Z;

                                if (b.Boluses.Any())
                                    DATA.BolusId = b.Boluses.First().Id;
                            }
                        }

                        foreach (var en in energies.Distinct())
                            DATA.Energy += en + "; ";

                        if (DATA.Energy.IndexOf('E') >= 0)
                            DATA.EnergyMode = "Electron";
                        else if (DATA.Energy.IndexOf('X') >= 0)
                            DATA.EnergyMode = "Photon";
                        else
                            DATA.EnergyMode = "Unknown";

                        DATA.NumberOfFields = nbms;

                        DATA.TotalMu = Total_MU;
                        DATA.IsoX = iso_X;
                        DATA.IsoY = iso_Y;
                        DATA.IsoZ = iso_Z;

                        DATA.UseShifts = false;
                        if (Math.Abs(iso_X) > 0.009 || Math.Abs(iso_Y) > 0.009 || Math.Abs(iso_Z) > 0.009)
                            DATA.UseShifts = true;

                        DATA.DoseAlgorithm = "";
                        DATA.DoseGridSizeCM = "";
                        DATA.DoseMax3D = 0.0;
                        DATA.DoseResX = 0.0;
                        DATA.DoseResY = 0.0;
                        DATA.DoseResZ = 0.0;

                        DATA.TargetVolume = pln.TargetVolumeID;
                        DATA.NumberOfFractions = pln.NumberOfFractions.Value;
                        DATA.FractionDose = pln.PlannedDosePerFraction.Dose;
                        DATA.TotalDose = pln.TotalDose.Dose;
                        DATA.PlanNormalization = pln.PlanNormalizationValue;
                        DATA.PrimaryRefPoint = pln.PrimaryReferencePoint.Id;

                        Dose dose = pln.Dose;
                        if (dose != null)
                        {
                            DATA.DoseMax3D = dose.DoseMax3D.Dose;
                            DATA.DoseResX = dose.XRes;
                            DATA.DoseResY = dose.YRes;
                            DATA.DoseResZ = dose.ZRes;
                            switch (DATA.EnergyMode)
                            {
                                case "Photon":
                                    DATA.DoseAlgorithm = pln.PhotonCalculationModel;
                                    break;
                                case "Electron":
                                    DATA.DoseAlgorithm = pln.ElectronCalculationModel;
                                    break;
                                default:
                                    break;
                            }
                        }


                        ReportData();

                        PlanCheck(pln);

                        app.ClosePatient();
                    }
                    else
                        MessageBox.Show("Plan has not been found!");
                }
                else
                    MessageBox.Show("Course has not been found!");
            }
            else
                MessageBox.Show("Patient has not been found!");

            app.Dispose();
        }


        private void ReportData()
        {
            string DELIM = Environment.NewLine;

            //ENTRY INFORMATION
            TBox_AllData.Text += ("Entry Date:").PadRight(20) + DATA.EntryDateTime + DELIM;
            TBox_AllData.Text += ("Entered By:").PadRight(20) + DATA.EnteredBy + DELIM;

            // PATIENT INFORMATION
            TBox_AllData.Text += DELIM;
            TBox_AllData.Text += ("Patient Name:").PadRight(20) + DATA.PatientLastName + ", " + DATA.PatientFirstName + DELIM;
            TBox_AllData.Text += ("Patient Id:").PadRight(20) + DATA.PatientId + DELIM;
            TBox_AllData.Text += ("Planned By:").PadRight(20) + DATA.PlannerFullName + DELIM;
            TBox_AllData.Text += ("Plan Review Date:").PadRight(20) + DATA.PlanReviewDate + DELIM;
            TBox_AllData.Text += ("Plan Review By:").PadRight(20) + DATA.PlanReviewBy + DELIM;
            TBox_AllData.Text += ("Physician:").PadRight(20) + DATA.PhysicianFullName + DELIM;

            // IMAGE INFORMATION
            // TBox_AllData.Text += ("NumberOfRegistrations:").PadRight(20) + DATA.NumberOfRegistrations.ToString() + DELIM;
            TBox_AllData.Text += DELIM;
            TBox_AllData.Text += ("Imaging Date:").PadRight(20) + DATA.ImageDate + DELIM;
            TBox_AllData.Text += ("Image Id:").PadRight(20) + DATA.ImageId + DELIM;
            TBox_AllData.Text += ("Contour Id:").PadRight(20) + DATA.ContourId + DELIM;
            TBox_AllData.Text += ("Number Of Images:").PadRight(20) + DATA.NumberOfImages.ToString() + DELIM;
            TBox_AllData.Text += ("Image Res X:").PadRight(20) + DATA.ImageResX.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Image Res Y:").PadRight(20) + DATA.ImageResY.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Image Res Z:").PadRight(20) + DATA.ImageResZ.ToString("F1") + DELIM;

            // COURSE INFORMATION
            TBox_AllData.Text += DELIM;
            TBox_AllData.Text += ("Course Id:").PadRight(20) + DATA.CourseId + DELIM;
            TBox_AllData.Text += ("Rx Site:").PadRight(20) + DATA.RxSite + DELIM;
            TBox_AllData.Text += ("Rx Targets:").PadRight(20) + DATA.RxTargets + DELIM;
            TBox_AllData.Text += ("Rx Technique:").PadRight(20) + DATA.RxTechnique + DELIM;
            TBox_AllData.Text += ("Rx Sequence:").PadRight(20) + DATA.RxSequence + DELIM;
            TBox_AllData.Text += ("Rx Notes:").PadRight(20) + DATA.RxNotes + DELIM;
            TBox_AllData.Text += ("Rx Gating:").PadRight(20) + DATA.RxGating + DELIM;
            TBox_AllData.Text += ("Diagnosis Code:").PadRight(20) + DATA.DiagnosisCode + DELIM;

            // PLAN INFORMATION
            TBox_AllData.Text += DELIM;
            TBox_AllData.Text += ("Plan Id:").PadRight(20) + DATA.PlanId + DELIM;
            TBox_AllData.Text += ("Orientation:").PadRight(20) + DATA.PlanOrientation + DELIM;
            TBox_AllData.Text += ("Couch Kicks:").PadRight(20) + DATA.UseCouchKick.ToString() + DELIM;
            TBox_AllData.Text += ("MLC Type:").PadRight(20) + DATA.MLCType + DELIM;
            TBox_AllData.Text += ("Jaw Tracking:").PadRight(20) + DATA.UseJawTracking.ToString() + DELIM;
            TBox_AllData.Text += ("Bolus Id:").PadRight(20) + DATA.BolusId + DELIM;
            TBox_AllData.Text += ("Tolerance Table:").PadRight(20) + DATA.ToleranceTable + DELIM;
            TBox_AllData.Text += ("Use Gating:").PadRight(20) + DATA.UseGating + DELIM;
            TBox_AllData.Text += ("Number of Fields:").PadRight(20) + DATA.NumberOfFields + DELIM;
            TBox_AllData.Text += ("Machine Id:").PadRight(20) + DATA.MachineId + DELIM;
            TBox_AllData.Text += ("Modality:").PadRight(20) + DATA.EnergyMode + DELIM;
            TBox_AllData.Text += ("Energies:").PadRight(20) + DATA.Energy + DELIM;
            TBox_AllData.Text += ("Total MU:").PadRight(20) + DATA.TotalMu.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Iso X:").PadRight(20) + DATA.IsoX.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Iso Y:").PadRight(20) + DATA.IsoY.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Iso Z:").PadRight(20) + DATA.IsoZ.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Shifts:").PadRight(20) + DATA.UseShifts.ToString() + DELIM;

            // DOSE INFORMATION
            TBox_AllData.Text += DELIM;
            TBox_AllData.Text += ("Number of Fxs:").PadRight(20) + DATA.NumberOfFractions.ToString() + DELIM;
            TBox_AllData.Text += ("Fx Dose:").PadRight(20) + DATA.FractionDose.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Total Dose:").PadRight(20) + DATA.TotalDose.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Target Volume:").PadRight(20) + DATA.TargetVolume + DELIM;
            TBox_AllData.Text += ("Primary Ref Pt:").PadRight(20) + DATA.PrimaryRefPoint + DELIM;
            TBox_AllData.Text += ("Plan Norm:").PadRight(20) + DATA.PlanNormalization.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Calc Algo:").PadRight(20) + DATA.DoseAlgorithm + DELIM;
            TBox_AllData.Text += ("DMax in 3D:").PadRight(20) + DATA.DoseMax3D.ToString("F1") + " %" + DELIM;
            TBox_AllData.Text += ("Dose Res X:").PadRight(20) + DATA.DoseResX.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Dose Res Y:").PadRight(20) + DATA.DoseResY.ToString("F1") + DELIM;
            TBox_AllData.Text += ("Dose Res Z:").PadRight(20) + DATA.DoseResZ.ToString("F1") + DELIM;
            // TBox_AllData.Text += ("DoseGridSizeCM:").PadRight(20) + DATA.DoseGridSizeCM + DELIM;

        }






        private void PlanCheck(PlanSetup pln)
        {
            string DELIM = Environment.NewLine;

            bool isBreast = false;
            bool isLeftSide = false;
            bool isRightSide = false;
            bool isProstate = false;
            bool isLung = false;
            bool isSBRT = false;

            RTPrescription rx = pln.RTPrescription;

            if (rx != null)
            {
                if (!String.IsNullOrEmpty(rx.Site))
                {

                    if (rx.Site.IndexOf("Breast", StringComparison.OrdinalIgnoreCase) >= 0)
                        isBreast = true;
                    else if (rx.Site.IndexOf("Chest Wall", StringComparison.OrdinalIgnoreCase) >= 0)
                        isBreast = true;

                    if (rx.Site.IndexOf("Lung", StringComparison.OrdinalIgnoreCase) >= 0)
                        isLung = true;

                    if (rx.Technique.IndexOf("SRT", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        isSBRT = true;
                    }


                    if (rx.Site.IndexOf("Prostate", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (rx.Site.IndexOf("Fossa", StringComparison.OrdinalIgnoreCase) < 0 &&
                            rx.Site.IndexOf("Bed", StringComparison.OrdinalIgnoreCase) < 0)
                            isProstate = true;
                    }
                    else if (pln.Course.Diagnoses.Count() > 0 && pln.Course.Diagnoses.FirstOrDefault().ClinicalDescription.IndexOf("Prostate", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (pln.Course.Diagnoses.FirstOrDefault().ClinicalDescription.IndexOf("Fossa", StringComparison.OrdinalIgnoreCase) < 0 &&
                            pln.Course.Diagnoses.FirstOrDefault().ClinicalDescription.IndexOf("Bed", StringComparison.OrdinalIgnoreCase) < 0)
                            isProstate = true;
                    }

                    if (rx.Site.IndexOf("Left", StringComparison.OrdinalIgnoreCase) >= 0)
                        isLeftSide = true;
                    else if (rx.Site.IndexOf("Right", StringComparison.OrdinalIgnoreCase) >= 0)
                        isRightSide = true;
                }
            }


            RichTBox_CheckData.SelectionColor = System.Drawing.Color.Red;

            string Header = "CATEGORY - General";
            RichTBox_CheckData.Text += Header + DELIM;

            if (pln.Course.Id.Contains('_') || pln.Course.Id.Contains('&') || pln.Course.Id.Contains('\\') || pln.Course.Id.Contains('/') || pln.Course.Id.Contains('%') || pln.Course.Id.Contains('@'))
                RichTBox_CheckData.Text += "CHECK - Course id (" + pln.Course.Id + ") contains special characters." + DELIM;
            else
                RichTBox_CheckData.Text += "OK - Course id (" + pln.Course.Id + ") does not use special characters." + DELIM;


            if (pln.Id.Contains('_') || pln.Id.Contains('&') || pln.Id.Contains('\\') || pln.Id.Contains('/') || pln.Id.Contains('%') || pln.Id.Contains('@'))
                RichTBox_CheckData.Text += "CHECK - Plan id (" + pln.Id + ") contains special characters." + DELIM;
            else
                RichTBox_CheckData.Text += "OK - Plan id (" + pln.Id + ") does not use special characters." + DELIM;

            bool found_verification_plan = false;
            foreach (Course course in pln.Course.Patient.Courses)
            {
                if (!course.Id.Contains(pln.Course.Id))
                {
                    if (course.CompletedDateTime == null)
                        RichTBox_CheckData.Text += "CHECK - Another active course is found (Id : " + course.Id + "). " + DELIM;
                }

                if (course.CompletedDateTime == null)
                {
                    IEnumerable<PlanSetup> active_plans = course.PlanSetups;
                    foreach (PlanSetup p in active_plans)
                    {
                        if (string.Compare(p.PlanIntent, "Verification") >= 0)
                        {
                            if (p.VerifiedPlan != null)
                            {
                                if (!string.IsNullOrEmpty(p.VerifiedPlan.UID))
                                {
                                    if (string.Compare(p.VerifiedPlan.UID, pln.UID) >= 0)
                                    {
                                        // MessageBox.Show(p.Id + "\t" + p.VerifiedPlan.Id + "\t" + p.VerifiedPlan.UID);
                                        // A verification plan was found in an active course for the plan we're checking.
                                        found_verification_plan = true;

                                        RichTBox_CheckData.Text += "OK - A verification plan " + p.Id + " under " + course.Id + " course was found for the plan." + DELIM;
                                        if (p.UseGating)
                                            RichTBox_CheckData.Text += "\tCHECK - Verification plan should not have gating turned on." + DELIM;
                                        if (p.ApprovalStatus != PlanSetupApprovalStatus.PlanningApproved)
                                            RichTBox_CheckData.Text += "\tCHECK - Verification plan status should only be PlanApproved (plan status = " + p.ApprovalStatus + ")." + DELIM;


                                        bool hasCouchVert = true;
                                        bool hasCouchLong = true;
                                        bool hasCouchLat = true;
                                        bool hasCouchRotZero = true;
                                        // bool isSAD = true;
                                        foreach (Beam b in p.Beams)
                                        {
                                            hasCouchVert &= !Double.IsNaN(b.ControlPoints.FirstOrDefault().TableTopVerticalPosition);
                                            hasCouchLong &= !Double.IsNaN(b.ControlPoints.FirstOrDefault().TableTopLongitudinalPosition);
                                            hasCouchLat &= !Double.IsNaN(b.ControlPoints.FirstOrDefault().TableTopLateralPosition);
                                            hasCouchRotZero &= b.ControlPoints.FirstOrDefault().PatientSupportAngle == 0;
                                            // isSAD &= b.SSD == 100;
                                            // RichTBox_CheckData.Text += "SSD - " + b.SSD + DELIM;
                                        }

                                        if (!hasCouchVert || !hasCouchLong || !hasCouchLat)
                                            RichTBox_CheckData.Text += "\tCHECK - Verification plan beams do not have couch values." + DELIM;

                                        // if (!isSAD)
                                        //    RichTBox_CheckData.Text += "\tCHECK - Verification plan is not planned at SAD = 100 cm." + DELIM;

                                        if (!hasCouchRotZero)
                                            RichTBox_CheckData.Text += "\tCHECK - Verification plan has non-zero couch angle (should be zero)." + DELIM;

                                    }
                                }
                            }
                        }


                    }
                }
            }

            // Display Course and Diagnosis information:
            if (pln.Course.Diagnoses.Count() != 0 && rx != null)
            {
                RichTBox_CheckData.Text += "OK - Course " + pln.Course.Id + " Diagnosis: " + pln.Course.Diagnoses.FirstOrDefault().Code +
                     pln.Course.Diagnoses.FirstOrDefault().ClinicalDescription + DELIM;
                RichTBox_CheckData.Text += "\tRX Site: " + rx.Site + DELIM;

                if (pln.Course.Diagnoses.FirstOrDefault().ClinicalDescription.IndexOf("Left", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (isLeftSide)
                        RichTBox_CheckData.Text += "OK - Diagnosis and RX site laterality match (Left Side)." + DELIM;
                    else if (isRightSide)
                        RichTBox_CheckData.Text += "CHECK - Diagnosis laterality (Left) does not match RX site (Right)." + DELIM;
                    else
                        RichTBox_CheckData.Text += "CHECK - Diagnosis laterality (Left) is not indicated in RX site." + DELIM;
                }
                else if (pln.Course.Diagnoses.FirstOrDefault().ClinicalDescription.IndexOf("Right", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (isRightSide)
                        RichTBox_CheckData.Text += "OK - Diagnosis and RX site laterality match (Right Side)." + DELIM;
                    else if (isLeftSide)
                        RichTBox_CheckData.Text += "CHECK - Diagnosis laterality (Right) does not match RX site (Left)." + DELIM;
                    else
                        RichTBox_CheckData.Text += "CHECK - Diagnosis laterality (Right) is not indicated in RX site." + DELIM;
                }
                else
                {
                    if (isRightSide || isLeftSide)
                        RichTBox_CheckData.Text += "CHECK - RX site laterality is not indicated in Diagnosis." + DELIM;
                    else
                        RichTBox_CheckData.Text += "OK - Diagnosis or RX site does not specify laterality." + DELIM;
                }
            }
            else
            {
                RichTBox_CheckData.Text += "CHECK - Course " + pln.Course.Id + " does not have diagnoses or RX is not attached." + DELIM +
                    "\t RX vs. Diagnosis laterality check cannot be performed." + DELIM;
            }




            // Display CT-Device (for e-density match) Information: 
            StructureSet structset = pln.StructureSet;
            string ImagingDevice = structset.Image.Series.ImagingDeviceId;

            if (ImagingDevice.Contains("DUB_Confidence"))
                RichTBox_CheckData.Text += "OK - DUB_Confindence imaging device was selected." + DELIM;
            else
                RichTBox_CheckData.Text += "CHECK - " + ImagingDevice + " imaging device was selected." + DELIM;



            // Find User Origin: 
            VVector UserOrigin = structset.Image.UserOrigin;
            double origin_Xcm = UserOrigin.x / 10.0;
            double origin_Ycm = UserOrigin.y / 10.0;
            double origin_Zcm = UserOrigin.z / 10.0;

            // RichTBox_CheckData.Text += "\t User Origin:\t DICOM (" +
            //     origin_Xcm.ToString("F2") + "cm, " + origin_Ycm.ToString("F2") + "cm, " + origin_Zcm.ToString("F2") + "cm)" + DELIM;



            bool pacemaker_found = false;
            bool markers_found = false;

            bool tattoo_found = false;
            double tattoo_Xcm = 0.0, tattoo_Ycm = 0.0, tattoo_Zcm = 0.0;

            double body_center_Xcm = 0.0;

            foreach (Structure ss in structset.Structures)
            {
                if (ss.Id.Contains("Tattoo"))
                {
                    tattoo_found = true;
                    tattoo_Xcm = ss.CenterPoint.x / 10.0;
                    tattoo_Ycm = ss.CenterPoint.y / 10.0;
                    tattoo_Zcm = ss.CenterPoint.z / 10.0;
                }
                if (ss.Id.IndexOf("pacemaker", StringComparison.OrdinalIgnoreCase) >= 0 || ss.Id.IndexOf("cied", StringComparison.OrdinalIgnoreCase) >= 0)
                    pacemaker_found = true;

                if (ss.Id.IndexOf("marker", StringComparison.OrdinalIgnoreCase) >= 0 || ss.Id.IndexOf("seed", StringComparison.OrdinalIgnoreCase) >= 0)
                    markers_found = true;

                if (ss.Id.IndexOf("body", StringComparison.OrdinalIgnoreCase) >= 0)
                    body_center_Xcm = (ss.CenterPoint.x - pln.StructureSet.Image.UserOrigin.x) / 10.0;
            }

            if (tattoo_found)
            {
                RichTBox_CheckData.Text += "OK - Tattoo found at:" + DELIM +
                        "\t INFO - DICOM (" +
                        tattoo_Xcm.ToString("F2") + "cm, " + tattoo_Ycm.ToString("F2") + "cm, " + tattoo_Zcm.ToString("F2") + "cm)" + DELIM;

                bool all_same = Math.Abs(tattoo_Xcm - origin_Xcm) < 0.001 && Math.Abs(tattoo_Ycm - origin_Ycm) < 0.001 && Math.Abs(tattoo_Zcm - origin_Zcm) < 0.001;
                if (all_same)
                    RichTBox_CheckData.Text += "OK - User Origin and Tattoo are SAME..." + DELIM;
                else
                    RichTBox_CheckData.Text += "CHECK - User origin is NOT SAME as Tattoo point!" + DELIM;
            }
            else
                RichTBox_CheckData.Text += "CHECK - Tattoo marker is not found!" + DELIM;


            if (isProstate)
            {
                if (markers_found)
                    RichTBox_CheckData.Text += "OK - Fiducial markers are found (Prostate case). " + DELIM;
                else
                    RichTBox_CheckData.Text += "CHECK - Fiducial markers are not found (Prostate case)." + DELIM;
            }


            if (isLung && isSBRT)
            {
                if (!pln.UseGating)
                    RichTBox_CheckData.Text += "CHECK - Lung SBRT plan is detected but Use Gating option is not enabled." + DELIM;
                else
                    RichTBox_CheckData.Text += "OK - Lung SBRT plan is detected and Use Gating option is turned on." + DELIM;
            }


            // APPROVALS
            IEnumerable<ApprovalHistoryEntry> pln_approvals = pln.ApprovalHistory;

            RichTBox_CheckData.Text += DELIM + "CATEGORY - Approvals" + DELIM;
            foreach (var apstat in pln_approvals)
            {
                RichTBox_CheckData.Text += "  " + apstat.ApprovalStatus.ToString().PadRight(20) + "\t" + apstat.ApprovalDateTime + " by " + apstat.UserDisplayName + DELIM;
                //if (apstat.ApprovalStatus == PlanSetupApprovalStatus.Reviewed)
                //    RichTBox_CheckData.Text += "\tOK - Plan Reviewed by " + apstat.UserDisplayName + DELIM;
                //else
                //    RichTBox_CheckData.Text += "\tCHECK - Plan not reviewed." + DELIM;
            }




            // RX INFORMATION

            string Rx_energies = "";

            IEnumerable<Beam> bms = pln.Beams;
            int nbms = 0;
            double iso_X = 0.0, iso_Y = 0.0, iso_Z = 0.0;


            Header = "CATEGORY - Prescription";
            RichTBox_CheckData.Text += DELIM + Header + DELIM;
            RichTBox_CheckData.Find(Header, RichTextBoxFinds.MatchCase);
            if (rx != null)
            {
                if (string.IsNullOrEmpty(rx.Name) || rx.Name.Contains("None"))
                    RichTBox_CheckData.Text += "\tCHECK - RX name is not available." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tINFO - Name:\t\t" + rx.Name + DELIM;



                if (rx.Status.IndexOf("draft", StringComparison.OrdinalIgnoreCase) >= 0)
                    RichTBox_CheckData.Text += "\tCHECK - RX is in DRAFT status. " + DELIM;
                else
                    RichTBox_CheckData.Text += "\tINFO - Status:\t\t" + rx.Status + DELIM;


                if (string.IsNullOrEmpty(rx.Site) || rx.Site.Contains("None"))
                    RichTBox_CheckData.Text += "\tCHECK - Site information is not available." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tINFO - Site:\t\t" + rx.Site + DELIM;



                if (string.IsNullOrEmpty(rx.PhaseType) || rx.PhaseType.Contains("None"))
                    RichTBox_CheckData.Text += "\tCHECK - Phase information is not available." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tINFO - Phase:\t\t" + rx.PhaseType + DELIM;



                string rx_targets = "";
                IEnumerable<RTPrescriptionTarget> targets = rx.Targets;
                foreach (RTPrescriptionTarget trgt in targets)
                    rx_targets += trgt.TargetId + "; ";

                if (string.IsNullOrEmpty(rx.Technique))
                    RichTBox_CheckData.Text += "\tCHECK - No targets are specified." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tINFO - Targets:\t\t" + rx_targets + DELIM;



                if (rx.NumberOfFractions != null)
                {
                    if (rx.NumberOfFractions == pln.NumberOfFractions)
                        RichTBox_CheckData.Text += "\tOK - No of Fxs:\t\t" + rx.NumberOfFractions + DELIM;
                    else
                        RichTBox_CheckData.Text += "\tCHECK - Plan and RX Number of fractions does not match: " + pln.NumberOfFractions + " vs " + rx.NumberOfFractions + DELIM;
                }
                else
                    RichTBox_CheckData.Text += "\tCHECK - RX Number of fractions is not valid." + DELIM;


                double Rx_dosePerFraction = 0.0;
                if (targets.Count() > 0)
                    Rx_dosePerFraction = targets.FirstOrDefault().DosePerFraction.Dose;

                if (Rx_dosePerFraction != 0)
                {
                    if (Rx_dosePerFraction == pln.DosePerFraction.Dose)
                        RichTBox_CheckData.Text += "\tOK - Dose per Fx:\t\t" + Rx_dosePerFraction.ToString("F1") + " cGy " + DELIM;
                    else
                        RichTBox_CheckData.Text += "\tCHECK - Plan and RX dose per fractions do not match: " + Rx_dosePerFraction + " vs " + pln.DosePerFraction.Dose + " cGy " + DELIM;
                }
                else
                    RichTBox_CheckData.Text += "\tCHECK - RX dose per fraction is not valid." + DELIM;




                if (string.IsNullOrEmpty(rx.Technique) || rx.Technique.Contains("None"))
                    RichTBox_CheckData.Text += "\tCHECK - Technique information is not available." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tINFO - Technique:\t\t" + rx.Technique + DELIM;



                IEnumerable<string> ens = rx.Energies;
                foreach (string en in ens)
                    Rx_energies += en.Replace(" ", "-") + "; ";

                if (string.IsNullOrEmpty(Rx_energies))
                    RichTBox_CheckData.Text += "\tCHECK - No beam energies are defined." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tINFO - Energies:\t\t" + Rx_energies + DELIM;



                bool Rx_useGating = false;
                if (!string.IsNullOrEmpty(rx.Gating) && rx.Gating.IndexOf("None", StringComparison.OrdinalIgnoreCase) < 0)
                    Rx_useGating = true;


                if (pln.UseGating == Rx_useGating)
                    RichTBox_CheckData.Text += "\tOK - Plan and RX gating options match (Use Gating: " + Rx_useGating + ")." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tCHECK - Plan and RX gating options do not match (Plan: " + pln.UseGating + " vs RX = " + Rx_useGating + ") " + DELIM;


                if (string.IsNullOrEmpty(rx.Notes))
                    RichTBox_CheckData.Text += "\tINFO - Notes:\t\t" + DELIM;
                else
                    RichTBox_CheckData.Text += "\tINFO - Notes:\t\t" + rx.Notes + DELIM;

            }
            else
                RichTBox_CheckData.Text += "\tCHECK - Prescription is not valid or attached." + DELIM;


            // FIELD INFORMATION

            Header = "CATEGORY - Fields";
            RichTBox_CheckData.Text += DELIM + Header + DELIM;
            RichTBox_CheckData.Find(Header, RichTextBoxFinds.MatchCase);

            bool found_setup_LLAT = false;
            bool found_setup_RLAT = false;
            bool found_setup_AP = false;
            bool found_setup_PA = false;
            bool found_setup_CBCT = false;

            // bool found_setup_LAO = false;
            // bool found_setup_RAO = false;

            foreach (Beam b in bms)
            {
                if (b.IsSetupField)
                {
                    if (b.Id.IndexOf("CBCT", StringComparison.OrdinalIgnoreCase) >= 0)
                        found_setup_CBCT = true;

                    else if (b.Id.IndexOf("AP", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        found_setup_AP = true;
                        switch (pln.TreatmentOrientation)
                        {
                            case PatientOrientation.HeadFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 0)
                                    RichTBox_CheckData.Text += "\tCHECK - AP setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.HeadFirstProne:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 180)
                                    RichTBox_CheckData.Text += "\tCHECK - AP setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.FeetFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 0)
                                    RichTBox_CheckData.Text += "\tCHECK - AP setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.FeetFirstProne:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 180)
                                    RichTBox_CheckData.Text += "\tCHECK - AP setup field is found but confirm gantry angle." + DELIM;
                                break;
                            default:
                                break;
                        } // switch statement
                    }

                    else if (b.Id.IndexOf("PA", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        found_setup_PA = true;
                        switch (pln.TreatmentOrientation)
                        {
                            case PatientOrientation.HeadFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 180)
                                    RichTBox_CheckData.Text += "\tCHECK - PA setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.HeadFirstProne:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 0)
                                    RichTBox_CheckData.Text += "\tCHECK - PA setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.FeetFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 180)
                                    RichTBox_CheckData.Text += "\tCHECK - PA setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.FeetFirstProne:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 0)
                                    RichTBox_CheckData.Text += "\tCHECK - PA setup field is found but confirm gantry angle." + DELIM;
                                break;
                            default:
                                break;
                        } // switch statement
                    }

                    else if (b.Id.IndexOf("LLAT", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                b.Id.IndexOf("L LAT", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                b.Id.IndexOf("LT LAT", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                b.Id.IndexOf("LTLAT", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        found_setup_LLAT = true;
                        switch (pln.TreatmentOrientation)
                        {
                            case PatientOrientation.HeadFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 90)
                                    RichTBox_CheckData.Text += "\tCHECK - LLAT setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.HeadFirstProne:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 270)
                                    RichTBox_CheckData.Text += "\tCHECK - LLAT setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.FeetFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 270)
                                    RichTBox_CheckData.Text += "\tCHECK - LLAT setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.FeetFirstProne:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 90)
                                    RichTBox_CheckData.Text += "\tCHECK - LLAT setup field is found but confirm gantry angle." + DELIM;
                                break;
                            default:
                                break;
                        } // switch statement

                    }

                    else if (b.Id.IndexOf("RLAT", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                b.Id.IndexOf("R LAT", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                b.Id.IndexOf("RT LAT", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                b.Id.IndexOf("RTLAT", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        found_setup_RLAT = true;
                        switch (pln.TreatmentOrientation)
                        {
                            case PatientOrientation.HeadFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 270)
                                    RichTBox_CheckData.Text += "\tCHECK - RLAT setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.HeadFirstProne:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 90)
                                    RichTBox_CheckData.Text += "\tCHECK - RLAT setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.FeetFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 90)
                                    RichTBox_CheckData.Text += "\tCHECK - RLAT setup field is found but confirm gantry angle." + DELIM;
                                break;
                            case PatientOrientation.FeetFirstProne:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 270)
                                    RichTBox_CheckData.Text += "\tCHECK - RLAT setup field is found but confirm gantry angle." + DELIM;
                                break;
                            default:
                                break;
                        } // switch statement
                    }


                    else if (b.Id.IndexOf("RAO", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        //found_setup_RAO = true;
                        switch (pln.TreatmentOrientation)
                        {
                            case PatientOrientation.HeadFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 315)
                                    RichTBox_CheckData.Text += "\tCHECK - RAO setup field is found but confirm gantry angle." + DELIM;
                                break;
                            default:
                                break;
                        } // switch statement
                    }


                    else if (b.Id.IndexOf("LAO", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        //found_setup_LAO = true;
                        switch (pln.TreatmentOrientation)
                        {
                            case PatientOrientation.HeadFirstSupine:
                                if (b.ControlPoints.FirstOrDefault().GantryAngle != 45)
                                    RichTBox_CheckData.Text += "\tCHECK - LAO setup field is found but confirm gantry angle." + DELIM;
                                break;
                            default:
                                break;
                        } // switch statement
                    }


                } // if setup field

            } // beam for loop



            if (found_setup_CBCT && found_setup_AP && found_setup_PA && found_setup_RLAT && found_setup_LLAT)
                RichTBox_CheckData.Text += "\tOK - Standard 5 setup fields are found. " + DELIM;
            else
            {
                if (!found_setup_CBCT)
                    RichTBox_CheckData.Text += "\tCHECK - CBCT Setup field is not found. " + DELIM;
                if (!found_setup_AP)
                    RichTBox_CheckData.Text += "\tCHECK - AP Setup field is not found. " + DELIM;
                if (!found_setup_PA)
                    RichTBox_CheckData.Text += "\tCHECK - PA Setup field is not found. " + DELIM;
                if (!found_setup_RLAT)
                    RichTBox_CheckData.Text += "\tCHECK - RLAT Setup field is not found. " + DELIM;
                if (!found_setup_LLAT)
                    RichTBox_CheckData.Text += "\tCHECK - LLAT Setup field is not found. " + DELIM;
            }

            //if (isProstate && markers_found)
            //{
            //    if(found_setup_RAO && found_setup_LAO)
            //        RichTBox_CheckData.Text += "\tOK - RAO/LAO Setup fields are found for Prostate plan with fiducials. " + DELIM;
            //    else
            //        RichTBox_CheckData.Text += "\tCHECK - RAO/LAO Setup fields are not found for Prostate plan with fiducials. " + DELIM;
            //}




            foreach (Beam b in bms)
            {
                if (!b.IsSetupField)
                {
                    nbms++;
                    RichTBox_CheckData.Text += "Treatment Field: " + b.Id;
                    Technique tech = b.Technique;
                    if (tech.ToString().Contains("ARC"))
                    {
                        RichTBox_CheckData.Text += " (" + b.GantryDirection + " " + b.ControlPoints.First().GantryAngle + " - " + b.ControlPoints.Last().GantryAngle +
                            ", Couch = " + b.ControlPoints.First().PatientSupportAngle + ")" + DELIM;

                        switch (b.GantryDirection)
                        {
                            case GantryDirection.CounterClockwise:
                                if (b.Id.IndexOf("CCW", StringComparison.OrdinalIgnoreCase) < 0)
                                    RichTBox_CheckData.Text += "\tCHECK - Counter Clockwise arc field with ID mismatch." + DELIM;
                                break;
                            case GantryDirection.Clockwise:
                                if (b.Id.IndexOf("CW", StringComparison.OrdinalIgnoreCase) < 0 || b.Id.IndexOf("CCW", StringComparison.OrdinalIgnoreCase) >= 0)
                                    RichTBox_CheckData.Text += "\tCHECK - Clockwise arc field with ID mismatch." + DELIM;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        RichTBox_CheckData.Text += " (Gantry = " + b.ControlPoints.First().GantryAngle +
                            ", Couch = " + b.ControlPoints.First().PatientSupportAngle + ")" + DELIM;





                    if (isBreast)
                    {
                        if (isLeftSide && b.Id[0] == 'R')
                            RichTBox_CheckData.Text += "\tCHECK - Left Breast treatment with field id starting R." + DELIM;
                        if (isRightSide && b.Id[0] == 'L')
                            RichTBox_CheckData.Text += "\tCHECK - Right Breast treatment with field id starting L." + DELIM;
                    }


                    List<double> x1jaws = new List<double>();
                    List<double> x2jaws = new List<double>();
                    List<double> y1jaws = new List<double>();
                    List<double> y2jaws = new List<double>();

                    ControlPointCollection ctrl_colls = b.ControlPoints;
                    double MU = b.Meterset.Value;
                    int small_coll = 0;
                    double small_weight = 0;
                    int nctrls = ctrl_colls.Count();
                    for (int i = 1; i < nctrls - 1; i++)
                    {
                        ControlPoint ctrl = ctrl_colls[i];
                        double segMU = MU * (ctrl.MetersetWeight - ctrl_colls[i - 1].MetersetWeight);

                        VRect<double> jaws = ctrl.JawPositions;
                        x1jaws.Add(jaws.X1);
                        x2jaws.Add(jaws.X2);
                        double segX = Math.Abs(jaws.X2 - jaws.X1) / 10.0; // convert to CM

                        y1jaws.Add(jaws.Y1);
                        y2jaws.Add(jaws.Y2);
                        double segY = Math.Abs(jaws.Y2 - jaws.Y1) / 10.0; // convert to CM
                        if (segX < 2.0 || segY < 2.0)
                        {
                            small_coll++;
                            small_weight += segMU;
                        }
                    }



                    if (Math.Abs(b.ControlPoints.FirstOrDefault().GantryAngle - 180.0) < 0.1)
                    {
                        double offsetXcm = body_center_Xcm - (b.IsocenterPosition.x - pln.StructureSet.Image.UserOrigin.x) / 10.0;
                        // RichTBox_CheckData.Text += "\tINFO: " + body_center_Xcm.ToString("F1") + "\t" + isoXcm.ToString("F1") + DELIM;
                        switch (pln.TreatmentOrientation)
                        {
                            case PatientOrientation.HeadFirstSupine:
                                if (offsetXcm > 7.0)
                                    RichTBox_CheckData.Text += "\tCHECK - Gantry 180deg field with isocenter on the right side (HFS). Check for 180E. " + DELIM;
                                break;
                            case PatientOrientation.HeadFirstProne:
                                if (offsetXcm < -7.0)
                                    RichTBox_CheckData.Text += "\tCHECK - Gantry 180deg field with isocenter on the left side (HFP). Check for 180E. " + DELIM;
                                break;
                            case PatientOrientation.FeetFirstSupine:
                                if (offsetXcm < -7.0)
                                    RichTBox_CheckData.Text += "\tCHECK - Gantry 180deg field with isocenter on the left side (FFS). Check for 180E. " + DELIM;
                                break;
                            case PatientOrientation.FeetFirstProne:
                                if (offsetXcm > 7.0)
                                    RichTBox_CheckData.Text += "\tCHECK - Gantry 180deg field with isocenter on the right side (FFP). Check for 180E. " + DELIM;
                                break;
                            default:
                                break;
                        } // switch statement

                    }




                    if (small_coll == 0)
                        RichTBox_CheckData.Text += "\tOK - All collimator positions > 2cm. " + DELIM;
                    else
                        RichTBox_CheckData.Text += "\tCHECK - Beam has " + small_coll + " of " + ctrl_colls.Count() + " segments with net jaw size < 2cm (" +
                            (small_weight * 100 / MU).ToString("F1") + " % of field MUs) " + DELIM;

                    double delta_X = (b.IsocenterPosition.x - pln.StructureSet.Image.UserOrigin.x) / 10.0;
                    double delta_Y = (b.IsocenterPosition.y - pln.StructureSet.Image.UserOrigin.y) / 10.0;
                    double delta_Z = (b.IsocenterPosition.z - pln.StructureSet.Image.UserOrigin.z) / 10.0;
                    iso_X = delta_X;
                    iso_Y = delta_Y;
                    iso_Z = delta_Z;

                    if (Rx_energies.Length != 0)
                    {
                        if (Rx_energies.Contains(b.EnergyModeDisplayName))
                            RichTBox_CheckData.Text += "\tOK - " + b.EnergyModeDisplayName + " energy is indicated in RX. " + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tCHECK - " + b.EnergyModeDisplayName + " energy is not found in RX. " + DELIM;
                    }
                    else
                        RichTBox_CheckData.Text += "\tCHECK - RX does not have valid beam energies." + DELIM;



                    if (b.EnergyModeDisplayName.Contains("2.5X-FFF"))
                    {
                        RichTBox_CheckData.Text += "\tCHECK - " + b.EnergyModeDisplayName + " energy should not be used for treatment fields." + DELIM;
                    }
                    else if (b.EnergyModeDisplayName.Contains("10X-FFF"))
                    {
                        if (b.DoseRate == 2400)
                            RichTBox_CheckData.Text += "\tOK - " + b.EnergyModeDisplayName + " energy with DR = " + b.DoseRate.ToString() + " MU/min" + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tCHECK - " + b.EnergyModeDisplayName + " energy with DR = " + b.DoseRate.ToString() + " MU/min" + DELIM;

                    }
                    else if (b.EnergyModeDisplayName.Contains("6X-FFF"))
                    {
                        if (b.DoseRate == 1400)
                            RichTBox_CheckData.Text += "\tOK - " + b.EnergyModeDisplayName + " energy with DR = " + b.DoseRate.ToString() + " MU/min" + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tCHECK - " + b.EnergyModeDisplayName + " energy with DR = " + b.DoseRate.ToString() + " MU/min" + DELIM;
                    }
                    else if (b.EnergyModeDisplayName.Contains("E"))
                    {
                        if (b.DoseRate == 1000)
                            RichTBox_CheckData.Text += "\tOK - " + b.EnergyModeDisplayName + " energy with DR = " + b.DoseRate.ToString() + " MU/min" + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tCHECK - " + b.EnergyModeDisplayName + " energy with DR = " + b.DoseRate.ToString() + " MU/min" + DELIM;
                    }
                    else
                    {
                        if (b.DoseRate == 600)
                            RichTBox_CheckData.Text += "\tOK - " + b.EnergyModeDisplayName + " energy with DR = " + b.DoseRate.ToString() + " MU/min" + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tCHECK - " + b.EnergyModeDisplayName + " energy with DR = " + b.DoseRate.ToString() + " MU/min" + DELIM;
                    }


                    if (pacemaker_found)
                    {
                        if (b.EnergyModeDisplayName.Contains("6X"))
                            RichTBox_CheckData.Text += "\tOK - Pacemaker contour found, beam energy of " + b.EnergyModeDisplayName + " is allowed. " + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tCHECK - Pacemaker contour found, confirm energy selection of " + b.EnergyModeDisplayName + ". " + DELIM;
                    }


                    IEnumerable<FieldReferencePoint> refpts = b.FieldReferencePoints;
                    foreach (FieldReferencePoint refpt in refpts)
                        if (Double.IsNaN(refpt.FieldDose.Dose))
                            RichTBox_CheckData.Text += "\tCHECK - Field is not contributing dose to a reference point (" + refpt.ReferencePoint.Id + "). " + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tOK - Field is contributing dose to " + refpt.ReferencePoint.Id + " reference point." + DELIM;



                    if (b.MLCPlanType > MLCPlanType.Static && b.ControlPoints.Count > 10)
                    {
                        if (found_verification_plan)
                            RichTBox_CheckData.Text += "\tOK - A dynamic beam is detected and QA plan has been created." + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tCHECK - A QA plan was not found." + DELIM;

                    }

                }

                else // SETUP FIELDS:
                {
                    RichTBox_CheckData.Text += " Setup Field: " + b.Id + "  (Gantry = " + b.ControlPoints.First().GantryAngle + ")" + DELIM;

                    if (b.EnergyModeDisplayName.Contains("2.5X-FFF") && b.DoseRate == 60)
                        RichTBox_CheckData.Text += "\tOK - 2.5X-FFF setup-field energy & 60 MU/min dose-rate are used." + DELIM;
                    else
                    {
                        RichTBox_CheckData.Text += "\tCHECK - Verify if setup field energy and dose rate are appropriate." + DELIM
                            + "\t\t Energy= " + b.EnergyModeDisplayName + "\t DR= " + b.DoseRate.ToString("d") + DELIM;
                    }
                }


                if (Math.Abs(b.SSD - b.PlannedSSD) < 0.1)
                    RichTBox_CheckData.Text += "\tOK - Planned SSD and Calculated SSD are same (" + (b.SSD / 10.0).ToString("F1") + ")." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tCHECK - Planned SSD (" + (b.PlannedSSD / 10.0).ToString("F1") + ") and Calculated SSD (" +
                        (b.SSD / 10.0).ToString("F1") + ") are different." + DELIM;





                /// This section checks if the couch values are present (not empty): 
                bool Good_CouchPositionVert = !Double.IsNaN(b.ControlPoints.FirstOrDefault().TableTopVerticalPosition);
                bool Good_CouchPositionLong = !Double.IsNaN(b.ControlPoints.FirstOrDefault().TableTopLongitudinalPosition);
                bool Good_CouchPositionLat = !Double.IsNaN(b.ControlPoints.FirstOrDefault().TableTopLateralPosition);

                if (Good_CouchPositionLat && Good_CouchPositionVert && Good_CouchPositionLong)
                    RichTBox_CheckData.Text += "\tOK - All couch vert/long/lat parameters are entered." + DELIM;
                else
                    RichTBox_CheckData.Text += "\tCHECK - Couch values are not entered for all vert/long/lat parameters (defaults are -15.0 / 150.0 / 0.0)." + DELIM;




                /// This section checks if the couch parameters are default values (if present)
                double default_couchvert_inCM = -15.0;
                double default_couchlong_inCM = 150.0;
                double default_couchlat_inCM = 0.0;

                if (Good_CouchPositionVert)
                    Good_CouchPositionVert = Math.Abs(b.ControlPoints.FirstOrDefault().TableTopVerticalPosition / 10.0 - default_couchvert_inCM) < 0.01;
                if (Good_CouchPositionLong)
                    Good_CouchPositionLong = Math.Abs(b.ControlPoints.FirstOrDefault().TableTopLongitudinalPosition / 10.0 - default_couchlong_inCM) < 0.01;
                if (Good_CouchPositionLat)
                    Good_CouchPositionLat = Math.Abs(b.ControlPoints.FirstOrDefault().TableTopLateralPosition / 10.0 - default_couchlat_inCM) < 0.01;

                if (Good_CouchPositionLat && Good_CouchPositionVert && Good_CouchPositionLong)
                    RichTBox_CheckData.Text += "\tOK - All couch vert/long/lat parameters are consistent with default values (-15.0 / 150.0 / 0.0). " + DELIM;
                else
                    RichTBox_CheckData.Text += "\tCHECK - Some couch parameters vert/long/lat are different from default values (-15.0 / 150.0 / 0.0). " + DELIM;




                if (b.Boluses.Any())
                {
                    if (rx != null)
                    {
                        if (string.IsNullOrEmpty(rx.BolusThickness) || rx.BolusThickness.Contains("None"))
                            RichTBox_CheckData.Text += "\tCHECK - Bolus used but RX does not indicate bolus." + DELIM;
                        else
                            RichTBox_CheckData.Text += "\tOK - Field and RX indicates bolus usage." + DELIM +
                                                        "\t\tINFO - Bolus: Rx = " + pln.RTPrescription.BolusThickness + " vs Plan = " + b.Boluses.FirstOrDefault().Id + DELIM;
                    }
                    else
                        RichTBox_CheckData.Text += "\tCHECK - Bolus used but RX check cannot be performed -- RX is not attached." + DELIM;
                }

                if (pln.UseGating && isBreast)
                {
                    if (!string.IsNullOrEmpty(b.Name))
                        RichTBox_CheckData.Text += "\tOK - Gated Breast treatment and field name is valid: " + b.Name + DELIM;
                    else
                        RichTBox_CheckData.Text += "\tCHECK - Gated Breast treatment (e.g. DIBH) but field name is EMPTY." + DELIM;
                }

                RichTBox_CheckData.Text += DELIM;

            } // beam loop



            int start_point = 0;
            while (start_point >= 0)
            {
                start_point = RichTBox_CheckData.Find("OK -", start_point, RichTextBoxFinds.MatchCase);
                if (start_point >= 0)
                {
                    RichTBox_CheckData.SelectionFont = new Font("Times New Roman", 8, FontStyle.Bold);
                    RichTBox_CheckData.SelectionColor = Color.Blue;
                    start_point += 4;
                }
            }

            start_point = 0;
            while (start_point >= 0)
            {
                start_point = RichTBox_CheckData.Find("CHECK -", start_point, RichTextBoxFinds.MatchCase);
                if (start_point >= 0)
                {
                    RichTBox_CheckData.SelectionFont = new Font("Times New Roman", 8, FontStyle.Bold);
                    RichTBox_CheckData.SelectionColor = Color.Red;
                    start_point += 7;
                }
            }


            start_point = 0;
            while (start_point >= 0)
            {
                start_point = RichTBox_CheckData.Find("CATEGORY -", start_point, RichTextBoxFinds.MatchCase);
                if (start_point >= 0)
                {
                    RichTBox_CheckData.SelectionFont = new Font("Times New Roman", 8, FontStyle.Bold);
                    RichTBox_CheckData.SelectionColor = Color.Black;
                    start_point += 4;
                }
            }

            start_point = 0;
            while (start_point >= 0)
            {
                start_point = RichTBox_CheckData.Find("INFO -", start_point, RichTextBoxFinds.MatchCase);
                if (start_point >= 0)
                {
                    RichTBox_CheckData.SelectionFont = new Font("Times New Roman", 8, FontStyle.Bold);
                    RichTBox_CheckData.SelectionColor = Color.DarkGreen;
                    start_point += 4;
                }
            }



            return;
        }









        private void Button_Save_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(DATA.EntryDateTime))
            {
                MessageBox.Show("Plan data has not been extracted...");
                return;
            }

            if (File.Exists(DATA.ExportFilePath))
            {
                //Create COM Objects. Create a COM object for everything that is referenced
                Excel.Application xlApp = new Excel.Application
                {
                    Visible = false
                };

                try
                {
                    Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(DATA.ExportFilePath);
                    Excel._Worksheet xlWorksheet = xlWorkbook.Sheets["DATA"];

                    if (xlWorksheet != null)
                    {
                        Excel.Range xlRange = xlWorksheet.UsedRange;

                        int rowCount = xlRange.Rows.Count + 1;
                        //int colCount = xlRange.Columns.Count;


                        /// Keep the order of entries same as Excel columns:
                        int colCount = 1;
                        // ENTRY INFORMATION
                        xlWorksheet.Cells[rowCount, colCount++] = rowCount - 1;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.EntryDateTime;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.EnteredBy;

                        // PATIENT INFORMATION
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PatientLastName + ", " + DATA.PatientFirstName;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PatientId;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PlannerFullName;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PlanReviewDate;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PlanReviewBy;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PhysicianFullName;

                        // IMAGE INFORMATION
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.ImageDate;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.ImageId;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.ContourId;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.NumberOfImages.ToString();
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.ImageResX.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.ImageResY.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.ImageResZ.ToString("F1");

                        // COURSE INFORMATION
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.CourseId;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.RxSite;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.RxTargets;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.RxTechnique;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.RxSequence;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.RxNotes;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.RxGating;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.DiagnosisCode;

                        // PLAN INFORMATION
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PlanId;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PlanOrientation;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.UseCouchKick.ToString();
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.MLCType;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.UseJawTracking.ToString();
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.BolusId;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.ToleranceTable;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.UseGating;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.NumberOfFields;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.MachineId;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.EnergyMode;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.Energy;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.TotalMu.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.IsoX.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.IsoY.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.IsoZ.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.UseShifts.ToString();

                        // DOSE INFORMATION
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.NumberOfFractions.ToString();
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.FractionDose.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.TotalDose.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.TargetVolume;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PrimaryRefPoint;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.PlanNormalization.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.DoseAlgorithm;
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.DoseMax3D.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.DoseResX.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.DoseResY.ToString("F1");
                        xlWorksheet.Cells[rowCount, colCount++] = DATA.DoseResZ.ToString("F1");

                        // DOSE INFORMATION
                        xlWorksheet.Cells[rowCount, colCount++] = (!string.IsNullOrEmpty(TBox_Comments.Text)).ToString();
                        xlWorksheet.Cells[rowCount, colCount++] = TBox_Comments.Text;

                        xlWorkbook.Save();
                        xlWorkbook.Close();

                        MessageBox.Show("Save completed.");
                    }
                    else
                        MessageBox.Show("Can't find the WorkSheet.");
                    xlApp.Quit();
                }
                catch (Exception)
                {
                    MessageBox.Show("Can't access file...");
                }

            }
            else
                MessageBox.Show("Can not save: " + DATA.ExportFilePath);
        }

        private void Button_Exit_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Close Program?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }

    public struct DataBlock
    {
        public string ExportFilePath;

        public string EntryDateTime;
        public string EnteredBy;

        public string PatientFirstName;
        public string PatientLastName;
        public string PatientId;
        public string PlannerFullName;
        public string PlanReviewDate;
        public string PlanReviewBy;
        public string PhysicianFullName;


        public string ImageDate;
        public string ImageId;
        public string ContourId;
        public int NumberOfImages;
        public double ImageResX;
        public double ImageResY;
        public double ImageResZ;

        public string CourseId;
        public string PlanId;
        public string RxSite;
        public string RxTargets;
        public string RxTechnique;
        public string RxSequence;
        public string RxNotes;
        public string RxGating;
        public string DiagnosisCode;

        public string PlanOrientation;
        public bool UseGating;
        public int NumberOfFields;
        public string MachineId;
        public string EnergyMode;
        public string Energy;
        public double TotalMu;
        public double IsoX, IsoY, IsoZ;
        public bool UseShifts;
        public bool UseCouchKick;
        public string MLCType;
        public bool UseJawTracking;
        public string BolusId;
        public string ToleranceTable;

        public double FractionDose;
        public int NumberOfFractions;
        public double TotalDose;
        public string TargetVolume;
        public string PrimaryRefPoint;
        public double PlanNormalization;
        public string DoseAlgorithm;
        public double DoseMax3D;
        public double DoseResX;
        public double DoseResY;
        public double DoseResZ;
        public string DoseGridSizeCM;

        public bool HasComment;
        public string Comment;

    }
}
