using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanDifferences
{
    public partial class Form1 : Form
    {
        public Form1(Patient this_patient, string[] args)
        {
            InitializeComponent(this_patient,args);// args);
            //InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private bool IsSelectedItemNull(System.Windows.Forms.ComboBox c)
        {
            bool result = ( string.IsNullOrEmpty(c.Text) || (c.SelectedIndex==-1) );
            return result;
        }

        private bool AreDataSelected()
        {
            bool output;
            
            if (IsSelectedItemNull(this.comboBox1) || IsSelectedItemNull(this.comboBox2) || IsSelectedItemNull(this.comboBox3) || IsSelectedItemNull(this.comboBox4))  
            {
                output = false;
                MessageBox.Show("Please select a pair of plans to continue.");
            }
            else
            {
                output = true;
            }

            return output;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (AreDataSelected())
            {
                //get plan 1 and 2 for comparison
                this.textBox1.Clear();

                String crs1str = this.comboBox1.Text;
                String crs2str = this.comboBox2.Text;
                String pln1str = this.comboBox3.Text;
                String pln2str = this.comboBox4.Text;

                IEnumerable<Course> courses = DATA.this_patient.Courses;
                Course crs1 = courses.FirstOrDefault(Course => Course.Id == crs1str);
                Course crs2 = courses.FirstOrDefault(Course => Course.Id == crs2str);

                IEnumerable<PlanSetup> plans1 = crs1.PlanSetups;
                IEnumerable<PlanSetup> plans2 = crs2.PlanSetups;

                PlanSetup pln1 = plans1.FirstOrDefault(PlanSetup => PlanSetup.Id == pln1str);
                PlanSetup pln2 = plans2.FirstOrDefault(PlanSetup => PlanSetup.Id == pln2str);

                if (pln1 == null)
                {
                    String errormessage = "Error: " + crs1str + " \\ " + pln1str + " not found";
                    MessageBox.Show(errormessage);
                    return;
                }
                else if (string.Compare(pln1.PlanIntent, "Verification") >= 0)
                {
                    String errormessage = "Error: " + crs1str + " \\ " + pln1str + " is a verification plan. Please select a clinical plan to continue.";
                    MessageBox.Show(errormessage);
                    return;
                }

                if (pln2 == null)
                {
                    String errormessage = "Error: " + crs2str + " \\ " + pln2str + " not found";
                    MessageBox.Show(errormessage);
                    return;
                }
                else if (string.Compare(pln2.PlanIntent, "Verification") >= 0)
                {
                    String errormessage = "Error: " + crs2str + " \\ " + pln2str + " is a verification plan. Please select a clinical plan to continue.";
                    MessageBox.Show(errormessage);
                    return;
                }

                DATA.pln1 = null;
                DATA.pln2 = null;


                DATA.pln1 = pln1;
                DATA.pln2 = pln2;

                // given pln1 and pln2 as selected, process the differences between them. 

                // extract data from each plan
                PlanData p1 = ExtractPlanData(pln1);
                PlanData p2 = ExtractPlanData(pln2);
                String rt = "";
                try
                {
                    rt = ProcessPlanDifferences(p1, p2);
                }
                catch
                {
                    MessageBox.Show("Exception occured in method: ProcessPlanDifferences");
                    this.textBox1.Text = rt;
                    SetTextBoxColors(this.textBox1);

                    return;
                }



                this.textBox1.Text = rt;
                SetTextBoxColors(this.textBox1);

                // plot results next
                }

        }


        public RichTextBox SetTextBoxColors(RichTextBox rt)
        {
            int start_point = 0;
            while (start_point >= 0)
            {
                start_point = rt.Find("OK -", start_point, RichTextBoxFinds.MatchCase);
                if (start_point >= 0)
                {
                    //rt.SelectionFont = new Font("Times New Roman", 8, FontStyle.Bold);
                    rt.SelectionColor = Color.Blue;
                    start_point += 4;
                }
            }

            start_point = 0;
            while (start_point >= 0)
            {
                start_point = rt.Find("Check -", start_point, RichTextBoxFinds.MatchCase);
                if (start_point >= 0)
                {
                    //rt.SelectionFont = new Font("Times New Roman", 8, FontStyle.Bold);
                    rt.SelectionColor = Color.Red;
                    start_point += 7;
                }
            }
            return rt;
       
    }


        public int CompareMLCSequences(ControlPoint ctrl1, ControlPoint ctrl2)
        {
            int result = 0, i = 0, j = 0;
            int npairs = ctrl1.LeafPositions.Length/2;
            double epsilon = 1e-3;
            //double v1, v2, d;
            while (i < 2) // bank 1 and 2
            {
                j = 0;
                while (j<npairs)// number of leafs on each bank
                {
                    //v1 = ctrl1.LeafPositions[i, j];
                    //v2 = ctrl2.LeafPositions[i, j];
                    //d = Math.Abs(v1 - v2);
                    
                    if (Math.Abs(ctrl1.LeafPositions[i, j]- ctrl2.LeafPositions[i, j])>epsilon)
//                    if (d>epsilon)
                    {
                        result++; // this counts every difference between leaf pairs. 
                    }
                    j++;
                }
                i++;
            }
            return result;
        }

        public String ReturnCPParameterComparison(List<double> f1,List<double> f2 ,List<double> deltaf, string thestring,string DELIM)
        {
            String r = "";
            float epsilon = (float)0.001;
            List<Double> Uniquef1 = f1.Distinct().ToList();
            List<Double> Uniquef2 = f2.Distinct().ToList();

            if ((Uniquef1.Count() == 1) && (Uniquef2.Count() == 1))
            {
                if (Uniquef1[0] == Uniquef2[0])
                    r += "OK - All control points have the same " + thestring + ": (" + Math.Round(Uniquef1[0],3) + " vs " + Math.Round(Uniquef2[0], 3) + ")" + DELIM;
                else
                    r += ShowCPDifference(deltaf, thestring, epsilon, DELIM);
            }
            else
            {
                r += "Check - this field shows more than one " + thestring + " for all control points: (" + Uniquef1.ToList().ToString() + ") vs (" + Uniquef2.ToList().ToString() + ")" + DELIM;
            }
            return r;
        }
        public String CompareControlPoints(Beam b1, Beam b2, string DELIM)
        {// only do this if the number of control points are the same. 
            String r = "";
            float epsilon = (float)0.001;
            //List<double> x1jaws = new List<double>();
            //List<double> x2jaws = new List<double>();
            //List<double> y1jaws = new List<double>();
            //List<double> y2jaws = new List<double>();
            
            List<double> CollAngle1 = new List<double>();
            List<double> CollAngle2 = new List<double>();


            ControlPointCollection ctrl_colls1 = b1.ControlPoints;
            ControlPointCollection ctrl_colls2 = b2.ControlPoints;
            double MU1 = b1.Meterset.Value;
            double MU2 = b2.Meterset.Value;
            
            int nctrls = ctrl_colls1.Count();
            int MLCDiffTally = 0;
            List<double> DeltaMU = new List<double>(nctrls);
            List<double> DeltaCol = new List<double>(nctrls);
            List<double> CouchAngle1= new List<double>(nctrls);
            List<double> CouchAngle2 = new List<double>(nctrls);
            List<double> DeltaCouchAngle = new List<double>(nctrls);


            List<double> DeltaJawsX1 = new List<double>(nctrls);
            List<double> DeltaJawsX2 = new List<double>(nctrls);
            List<double> DeltaJawsY1 = new List<double>(nctrls);
            List<double> DeltaJawsY2 = new List<double>(nctrls);

            List<double> DeltaMUs = new List<double>(nctrls);

            List<double> CouchVrt1 = new List<double>(nctrls);
            List<double> CouchVrt2 = new List<double>(nctrls);
            List<double> DeltaCouchVrt = new List<double>(nctrls);

            List<double> CouchLng1 = new List<double>(nctrls);
            List<double> CouchLng2 = new List<double>(nctrls);
            List<double> DeltaCouchLng = new List<double>(nctrls);

            List<double> CouchLat1 = new List<double>(nctrls);
            List<double> CouchLat2 = new List<double>(nctrls);
            List<double> DeltaCouchLat = new List<double>(nctrls);

            for (int i = 1; i <= nctrls-1 ; i++)
            {
                ControlPoint ctrl1 = ctrl_colls1[i];
                ControlPoint ctrl2 = ctrl_colls2[i];
                double segMU1 = MU1 * (ctrl1.MetersetWeight - ctrl_colls1[i - 1].MetersetWeight);
                double segMU2 = MU2 * (ctrl2.MetersetWeight - ctrl_colls2[i - 1].MetersetWeight);

                DeltaMU.Add(segMU1 - segMU2);

                CollAngle1.Add(ctrl1.CollimatorAngle);
                CollAngle2.Add(ctrl2.CollimatorAngle);
                DeltaCol.Add(ctrl1.CollimatorAngle - ctrl2.CollimatorAngle);

                CouchAngle1.Add(ctrl1.PatientSupportAngle);
                CouchAngle2.Add(ctrl2.PatientSupportAngle);
                DeltaCouchAngle.Add(ctrl1.PatientSupportAngle- ctrl2.PatientSupportAngle);

                CouchVrt1.Add(ctrl1.TableTopVerticalPosition);
                CouchVrt2.Add(ctrl2.TableTopVerticalPosition);
                DeltaCouchVrt.Add(ctrl1.TableTopVerticalPosition - ctrl2.TableTopVerticalPosition);

                CouchLng1.Add(ctrl1.TableTopLongitudinalPosition);
                CouchLng2.Add(ctrl2.TableTopLongitudinalPosition);
                DeltaCouchLng.Add(ctrl1.TableTopLongitudinalPosition - ctrl2.TableTopLongitudinalPosition);

                CouchLat1.Add(ctrl1.TableTopLateralPosition);
                CouchLat2.Add(ctrl2.TableTopLateralPosition);
                DeltaCouchLat.Add(ctrl1.TableTopLateralPosition - ctrl2.TableTopLateralPosition);

                MLCDiffTally += CompareMLCSequences(ctrl1, ctrl2);

                //DeltaLeafs.Add(ctrl1.LeafPositions[0,0] - ctrl2.LeafPositions[0,0]);

                VRect<double> jaws1 = ctrl1.JawPositions;
                VRect<double> jaws2 = ctrl2.JawPositions;

                DeltaJawsX1.Add((jaws1.X1 - jaws2.X1)/10);
                DeltaJawsX2.Add((jaws1.X2 - jaws2.X2)/10);
                DeltaJawsY1.Add((jaws1.Y1 - jaws2.Y1)/10);
                DeltaJawsY2.Add((jaws1.Y2 - jaws2.Y2)/10);

                
            }
            r += ShowCPDifference(DeltaMU, "MU", epsilon, DELIM);

            r += ReturnCPParameterComparison(CollAngle1, CollAngle2, DeltaCol, "Collimator angle", DELIM);
            r += ReturnCPParameterComparison(CouchAngle1, CouchAngle2, DeltaCouchAngle, "Couch angle", DELIM);
            r += ReturnCPParameterComparison(CouchVrt1, CouchVrt2, DeltaCouchVrt, "Couch Vrt", DELIM);
            r += ReturnCPParameterComparison(CouchLng1, CouchLng2, DeltaCouchLng, "Couch Lng", DELIM);
            r += ReturnCPParameterComparison(CouchLat1, CouchLat2, DeltaCouchLat, "Couch Lat", DELIM);



            if (MLCDiffTally>0)
                r += "Check - Differences found among leaf positions: " + MLCDiffTally + " different leaf pairs found. " + DELIM;
            else
                r += "OK - All control points show the same leaf positions" + DELIM;
            
            r += ShowCPDifference(DeltaJawsX1, "X1 jaw", epsilon, DELIM);            
            r += ShowCPDifference(DeltaJawsX2, "X2 jaw", epsilon, DELIM);
            r += ShowCPDifference(DeltaJawsY1, "Y1 jaw", epsilon, DELIM);

            DELIM = (Environment.NewLine).PadRight(10);
            r += ShowCPDifference(DeltaJawsY2, "Y2 jaw", epsilon, DELIM);

            return r;
        }
        public String ShowCPDifference(List<Double> D, String label, float epsilon,string DELIM)
        {
            string r = "";
            List <Double> AbsD = D.Select(x => Math.Abs(x)).ToList();

            if (AbsD.Sum() > epsilon) // if the summation of absolute values is > 0, then there's a difference at least somwhere
                { // there is a difference between control point segments somewhere
                int ind = (AbsD.FindAll(x => x > epsilon)).Count();

                r += "Check - " + ind + " of " + D.Count() + " control points show having different " + label + " values. Max(Delta) = " + DoubleToString(AbsD.Max()) + DELIM;
            }
            else
            {
                r += "OK - All control points have the same " + label + "s " + DELIM;
            }

            return r;

        }

        public List<int> FindTxBeamIndexes(PlanSetup p1) // returns a list of length[number of beams] where list item is 0 if beam is a setup field, 1 otherwise. 
        {
            List<int> inds = new List<int>(p1.Beams.Count());
            int i = 0;
            while (i < p1.Beams.Count())
            {
                if (!(p1.Beams.ToList()[i].IsSetupField))                                    
                    inds.Add(i); // only add if not a setup field
                i++;
            }
            return inds;
        }

        public String CompareBeams(PlanSetup pln1,PlanSetup pln2,PlanData p1,PlanData p2)
        {
            string DELIM= (Environment.NewLine).PadRight(10);
            string r="";
            int f = 0;
            List<int> inds1 = FindTxBeamIndexes(pln1);
            List<int> inds2 = FindTxBeamIndexes(pln2);

            r += ReturnStringComparisonText(pln1.PrimaryReferencePoint.DailyDoseLimit.ToString(), pln2.PrimaryReferencePoint.DailyDoseLimit.ToString(), "The daily dose limits are", DELIM);
            r += ReturnStringComparisonText(pln1.PrimaryReferencePoint.SessionDoseLimit.ToString(), pln2.PrimaryReferencePoint.SessionDoseLimit.ToString(), "The session dose limits are", DELIM);
            r += ReturnStringComparisonText(pln1.PrimaryReferencePoint.TotalDoseLimit.ToString(), pln2.PrimaryReferencePoint.TotalDoseLimit.ToString(), "The total dose limits are", DELIM);
            r += ReturnStringComparisonText(pln1.PlanIntent, pln2.PlanIntent, "The plan intents are", DELIM);
            if (pln1.PredecessorPlan == null || pln2.PredecessorPlan == null)
            {
                if (pln1.PredecessorPlan == null && pln2.PredecessorPlan == null)
                    r += "OK - The predecessor plans are both not defined" + DELIM;
                else
                {
                    if (pln1.PredecessorPlan == null)
                        r += "Check - One plan has a predecessor plan: (null vs " + pln2.PredecessorPlan.Id + ")" + DELIM;
                    else
                        r += "Check - One plan has a predecessor plan: (" + pln1.PredecessorPlan.Id + ", null)" + DELIM;
                }
            }
            else
                r += ReturnStringComparisonText(pln1.PredecessorPlan.Id, pln2.PredecessorPlan.Id, "The predecessor plans are", DELIM);



            if (p1.NumberOfFields > 0)
            {
                while (f < p1.NumberOfFields) // Loops on the number of treatment fields. 
                {
                    DELIM = (Environment.NewLine).PadRight(20);

                    r += "Beam: " + (f + 1) + " of " + p1.NumberOfFields + " Tx fields: " + pln1.Beams.ToList()[inds1[f]].Id + " vs " + pln2.Beams.ToList()[inds2[f]].Id + DELIM;

                    Beam b1 = pln1.Beams.ToList()[inds1[f]];
                    Beam b2 = pln2.Beams.ToList()[inds2[f]];

                    r += ReturnStringComparisonText(b1.Id, b2.Id, "The field names are", DELIM);
                    r += ReturnStringComparisonText(Enum.GetName(typeof(MLCPlanType), b1.MLCPlanType), Enum.GetName(typeof(MLCPlanType), b2.MLCPlanType), "MLC Types are", DELIM);
                    r += ReturnStringComparisonText(b1.EnergyModeDisplayName, b2.EnergyModeDisplayName, "The field energies are", DELIM);
                    r += ReturnDoubleComparisonText(b1.DoseRate, b2.DoseRate, "The dose rates are", DELIM);
                    r += ReturnDoubleComparisonText(b1.PlannedSSD/10, b2.PlannedSSD/10, "The planned SSD's are", DELIM);
                    r += ReturnStringComparisonText(b1.TreatmentUnit.Id, b2.TreatmentUnit.Id, "The unit names are", DELIM);

                    r += ReturnDoubleComparisonText(b1.Meterset.Value, b2.Meterset.Value, "The beam MU's are", DELIM);

                    r += ReturnDoubleComparisonText(b1.NormalizationFactor, b2.NormalizationFactor, "Beam normalization factors are", DELIM);

                    r += ReturnStringComparisonText(b1.NormalizationMethod, b2.NormalizationMethod, "Beam normalization methods are", DELIM);

                    // compare bolus information                    
                    if (b1.Boluses.Any() || b2.Boluses.Any())
                        
                        if (b1.Boluses.Any() && b2.Boluses.Any()) // if they both have boluses: 
                            if (!(b1.Boluses.Count() == b2.Boluses.Count()))
                                r += "Check - Different number of boluses attached to beam (" + b1.Boluses.Count() + " vs " + b2.Boluses.Count() + ")" + DELIM;
                            else
                            {
                                r += ReturnStringComparisonText(b1.Boluses.FirstOrDefault().Id, b2.Boluses.FirstOrDefault().Id, "Bolus names are", DELIM);
                                r += ReturnDoubleComparisonText(b1.Boluses.FirstOrDefault().MaterialCTValue, b2.Boluses.FirstOrDefault().MaterialCTValue, "Bolus material HU values are", DELIM);
                            }
                        
                        else
                            r += "Check - Bolus information is different between beams (" + b1.Boluses.Any() + " vs " + b2.Boluses.Any() + ")" + DELIM;
                    else
                        r += "OK - Bolus information is the same between beams (" + b1.Boluses.Any() + " vs " + b2.Boluses.Any() + ")" + DELIM;


                    bool cflag = (b1.ControlPoints.Count() == b2.ControlPoints.Count());
                    r += ReturnStringComparisonText(b1.SetupTechnique.ToString(), b2.SetupTechnique.ToString(), "Beam setup techniques are", DELIM);

                    if (String.Equals(p1.EnergyMode, "Electron") && String.Equals(p2.EnergyMode, "Electron"))
                        r += ReturnStringComparisonText(b1.Applicator.Id, b2.Applicator.Id, "Electron applicators are", DELIM);
                    else
                    {

                        r += ReturnDoubleComparisonText(b1.DosimetricLeafGap, b2.DosimetricLeafGap, "DLG parameters are", DELIM);
                        if (b1.Wedges.Any() || b2.Wedges.Any())
                            if (b1.Wedges.Any() && b2.Wedges.Any()) // if they both have wedges                                                
                                r += ReturnStringComparisonText(b1.Wedges.FirstOrDefault().Id, b2.Wedges.FirstOrDefault().Id, "Wedge Id's are", DELIM);
                            else
                                r += "Check - wedge information is different between beams (" + b1.Wedges.Any() + " vs " + b2.Wedges.Any() + ")" + DELIM;
                        else
                            r += "OK - Wedge information is the same between beams (" + b1.Wedges.Any() + " vs " + b2.Boluses.Any() + ")" + DELIM;
                        if (b1.MLC == null || b2.MLC == null)
                        {
                            if (b1.MLC == null && b2.MLC == null)
                            { }//r += "OK - no MLC's noted for fields.";
                            else
                                r += "Check - mismatch noted for attached MLC's: ( " + (b1.MLC == null) + " vs " + (b2.MLC == null) + " )" + DELIM;
                        }
                        else
                            r += ReturnStringComparisonText(b1.MLC.Model, b2.MLC.Model, "MLC model is", DELIM);

                    }

                    r += ReturnDoubleComparisonText(b1.WeightFactor, b2.WeightFactor, "Beam weight factors are", DELIM);




                    if (String.Equals(Enum.GetName(typeof(MLCPlanType), b1.MLCPlanType), Enum.GetName(typeof(MLCPlanType), b2.MLCPlanType)))
                    {// MLC types are equal. Let's examine the MLC plan types
                        if (cflag)
                        {
                            switch ((int)b1.MLCPlanType) 
                            {
                                case 0://static 
                                    r += ReturnDoubleComparisonText(b1.ControlPoints.FirstOrDefault().GantryAngle, b2.ControlPoints.FirstOrDefault().GantryAngle, "Gantry Angles are", DELIM);
                                    break;
                                case 1:// DoseDynamic
                                    r += ReturnDoubleComparisonText(b1.ControlPoints.FirstOrDefault().GantryAngle, b2.ControlPoints.FirstOrDefault().GantryAngle, "Gantry Angles are", DELIM);
                                    break;
                                case 2://ArcDynamic
                                    {
                                        r += ReturnStringComparisonText(Enum.GetName(typeof(GantryDirection), b1.GantryDirection), Enum.GetName(typeof(GantryDirection), b2.GantryDirection), "Direction of gantry rotation is", DELIM);
                                        r += ReturnDoubleComparisonText(b1.ControlPoints[0].GantryAngle, b2.ControlPoints[0].GantryAngle, "Gantry start angles are", DELIM);
                                        r += ReturnDoubleComparisonText(b1.ControlPoints[b1.ControlPoints.Count() - 1].GantryAngle, b2.ControlPoints[b1.ControlPoints.Count() - 1].GantryAngle, "Gantry stop angles are", DELIM);
                                    }
                                    break;
                                case 3://VMAT
                                    {
                                        r += ReturnStringComparisonText(Enum.GetName(typeof(GantryDirection), b1.GantryDirection), Enum.GetName(typeof(GantryDirection), b2.GantryDirection), "Direction of gantry rotation is", DELIM);
                                        r += ReturnDoubleComparisonText(b1.ControlPoints[0].GantryAngle, b2.ControlPoints[0].GantryAngle, "Gantry start angles are", DELIM);
                                        r += ReturnDoubleComparisonText(b1.ControlPoints[b1.ControlPoints.Count() - 1].GantryAngle, b2.ControlPoints[b1.ControlPoints.Count() - 1].GantryAngle, "Gantry stop angles are", DELIM);
                                    }
                                    break;
                                default: //electron
                                    r += ReturnDoubleComparisonText(b1.ControlPoints.FirstOrDefault().GantryAngle, b2.ControlPoints.FirstOrDefault().GantryAngle, "Gantry Angles are", DELIM);
                                    break;

                            }
                            r += ReturnDoubleTripletComparisonText(p1.IsoX, p1.IsoY, p1.IsoZ, p2.IsoX, p2.IsoY, p2.IsoZ, "Isocenter shifts are", DELIM);
                            r += CompareControlPoints(b1, b2, DELIM);
                        }
                        else
                        {
                            DELIM = (Environment.NewLine).PadRight(10);
                            r += "Check - The number of control points are different. Skipping control point analysis." + DELIM;
                        }
                    }
                    f++;
                }
            }
            else
                r += "Ok - no treatment fields detected" + DELIM;
            return r;
        }

        public string ProcessPlanDifferences(PlanData p1, PlanData p2)
        {
            string o = "";
            
            //string DELIM = Environment.NewLine;
            string DELIM = (Environment.NewLine).PadRight(10);

            o += "Dose metrics:" + DELIM;
            o += ReturnDoubleComparisonText(p1.PlanNormalization, p2.PlanNormalization, "Dose normalizations are", DELIM);
            if (p1.DoseIsNull || p2.DoseIsNull)
                o += "Check - at least one dose volume is null. Dose metrics will not be processed." + Environment.NewLine;
            else
            {
                o += ReturnDoubleComparisonText(p1.DoseMax3D, p2.DoseMax3D, "DoseMax3D values are", DELIM);                                
                o += ReturnDoubleTripletComparisonText(p1.DoseResX, p1.DoseResY, p1.DoseResZ, p2.DoseResX, p2.DoseResY, p2.DoseResZ, "Dose resolution values are", DELIM);

                o += ReturnStringComparisonText(p1.DoseAlgorithm, p2.DoseAlgorithm, "Dose Algorithms are", DELIM);
                o += ReturnStringComparisonText(p1.TargetVolume, p2.TargetVolume, "Dose Target Volumes are", DELIM);
                o += ReturnDoubleComparisonText(p1.PlanNormalization, p2.PlanNormalization, "Dose normalizations are", DELIM);
                o += ReturnStringComparisonText(p1.PlanOrientation, p2.PlanOrientation, "Plan orientations are", DELIM);
                o += ReturnStringComparisonText(p1.TargetVolume, p2.TargetVolume, "Dose Target Volumes are", DELIM);
                o += ReturnStringComparisonText(p1.PrimaryRefPoint, p2.PrimaryRefPoint, "Primary reference points are", DELIM);
                o += ReturnDoubleComparisonText(p1.TotalDose, p2.TotalDose, "The total doses are", DELIM);
                o += ReturnDoubleComparisonText(p1.NumberOfFractions, p2.NumberOfFractions, "Number of fractions are", DELIM);
                o += ReturnDoubleComparisonText(p1.FractionDose, p2.FractionDose, "Fractional doses are", Environment.NewLine);                                

            }

            //Field information
            o += "Field information:"+ DELIM;           
            o += ReturnDoubleComparisonText(p1.NumberOfFields, p2.NumberOfFields, "The number of Tx fields are", "");

            if ( (p1.NumberOfFields == p2.NumberOfFields) && (p1.NumberOfFields>0 || p2.NumberOfFields > 0 ))
            {
                o += " - Entering detailed beam analysis: " + DELIM;
                o += CompareBeams(DATA.pln1, DATA.pln2, p1,p2);
            }
            else
                o += " - Skipping detailed beam analysis." + DELIM;

            o += ReturnDoubleComparisonText(p1.TotalMu, p2.TotalMu, "Total MU's are", DELIM);            
            o += ReturnStringComparisonText(p1.ToleranceTable, p2.ToleranceTable, "The Tolerance types are", DELIM);
            o += ReturnBooleanComparisonText(p1.UseJawTracking, p2.UseJawTracking, "The jaw tracking settings are", DELIM);
            o += ReturnBooleanComparisonText(p1.UseGating, p2.UseGating, "The gating settings are", Environment.NewLine);            

            o += "Image information:" + DELIM;
            o += ReturnStringComparisonText(p1.ImageDate, p2.ImageDate, "Image dates are", DELIM);
            o += ReturnStringComparisonText(p1.ImageId, p2.ImageId, "Image Id's are", DELIM);
            o += ReturnDoubleComparisonText(p1.NumberOfImages, p2.NumberOfImages, "Number of images are", DELIM);
            o += ReturnStringComparisonText(p1.ContourId, p2.ContourId, "Contour Id's are", DELIM);
            o += ReturnDoubleTripletComparisonText(p1.ImageResX, p1.ImageResY, p1.ImageResZ, p2.ImageResX, p2.ImageResY, p2.ImageResZ, "Image resolution is", Environment.NewLine);

            o += "Course information:" + DELIM;
            o += ReturnStringComparisonText(p1.CourseId, p2.CourseId, "Course Id's are", DELIM);
            o += ReturnStringComparisonText(p1.RxSite, p2.RxSite,"Prescribed sites are", DELIM);
            o += ReturnStringComparisonText(p1.RxTargets, p2.RxTargets,"Prescribed targets are", DELIM);
            o += ReturnStringComparisonText(p1.RxTechnique, p2.RxTechnique,"Prescribed techniques are", DELIM);
            o += ReturnStringComparisonText(p1.RxSequence, p2.RxSequence,"Rx sequences are", DELIM); 
            o += ReturnStringComparisonText(p1.RxNotes, p2.RxNotes,"Rx notes are", DELIM);
            o += ReturnStringComparisonText(p1.RxGating, p2.RxGating, "Gating Rx values are", DELIM); 
            o += ReturnStringComparisonText(p1.DiagnosisCode, p2.DiagnosisCode, "Diagnosis Codes are", DELIM);

            return o;
        }

        public string ReturnDoubleComparisonText(Double A, Double B, String thestring, string DELIM)
        {            
            string o;

            if (Double.IsNaN(A) || Double.IsNaN(B))
            {
                if (Double.IsNaN(A) && Double.IsNaN(B))
                {
                    o = "OK - " + thestring + " are both not defined ";
                }
                else
                    o = "Check - one of " + thestring + " are not defined ";
            }
            else if (Math.Abs(A - B) > 0.001)
            {
                o = "Check - " + thestring + " different ";
            }
            else 
            {
                o = "OK - " + thestring + " the same ";
            }
            o+="(" + DoubleToString(A) + " vs " + DoubleToString(B) + ")" + DELIM;
            return o;
        }

        public string ReturnDoubleTripletComparisonText(Double A1, Double A2, Double A3, Double B1, Double B2, Double B3, String thestring, string DELIM)
        {
            string o;
            if (Math.Abs(A1 - B1) > 0.01 || Math.Abs(A2 - B2) > 0.01 || Math.Abs(A3 - B3) > 0.01)
                o = "Check - "+ thestring + " different ";
            else
                o = "OK - " + thestring + " the same ";

            o+="(" + DoubleToString(A1) + ", " + DoubleToString(A2) + ", " + DoubleToString(A3) + ") vs (" + DoubleToString(B1) + ", " + DoubleToString(B2) + ", " + DoubleToString(B3) + ") " + DELIM;
            return o;
        }


        public string ReturnEmptyStringPhrase(String A)
        {
            String B = "";
            if (String.IsNullOrEmpty(A))
                B = "<--Emtpy-->";
            else
                B = A;
            return B;
        }
        
        public string ReturnStringComparisonText(String A, String B, String thestring, string DELIM)
        {
            string o;

            if (!(String.IsNullOrEmpty(A) && (String.IsNullOrEmpty(B))))
            {
                if (String.Equals(A, B))
                {
                    o = "OK - " + thestring + " the same ";
                }
                else
                {
                    o = "Check - " + thestring + " different ";                    
                }
                o += "(" + ReturnEmptyStringPhrase(A) + " vs " + ReturnEmptyStringPhrase(B) + ")" + DELIM;
            }
            else if ((String.IsNullOrEmpty(A)) && (String.IsNullOrEmpty(B)))
            {
                o = "OK - " + thestring + " both not defined" + DELIM;
            }
            else
            {
                o = "Check - at least one of " + thestring + " not defined";
                o += "(" + A + " vs " + B + ")" + DELIM;
            }            

            return o;
        }

        public string ReturnBooleanComparisonText(bool A, bool B, String thestring, string DELIM)
        {
            string o;
            if ( (A && B) || ( (!A) && (!B) ) )
            {
                o = "OK - " + thestring + " the same ";
            }
            else
            {

                o = "Check - " + thestring + " different ";
            }

            o += "(" + A + " vs " + B + ")" + DELIM;

            return o;
        }
        public string DoubleToString(Double A)
        {
            String O;
            O = Math.Round(A, 3).ToString();
            return O;
        }

        public string[] DoubleToString3(Double A, Double B, Double C)
        {
            String[] O = new string[3];

            O[0]=Math.Round(A, 2).ToString();
            O[1] = Math.Round(B, 2).ToString();
            O[2] = Math.Round(C, 2).ToString();

            return O;
        }

        public PlanData ExtractPlanData(PlanSetup pln)
        {
 ///////////////// Not original code by Amir, implemented from ExportPlanData (Yildirim Mutaf). Although, slight adjustments have been made. 
            p1.ImageDate = pln.StructureSet.Image.CreationDateTime.Value.ToString("g");
            p1.ImageId = pln.StructureSet.Image.Id;
            p1.NumberOfImages = pln.StructureSet.Image.ZSize;
            p1.ImageResX = pln.StructureSet.Image.XRes;
            p1.ImageResY = pln.StructureSet.Image.YRes;
            p1.ImageResZ = pln.StructureSet.Image.ZRes;
            p1.ContourId = pln.StructureSet.Id;

            // RX INFORMATION
            RTPrescription rx = pln.RTPrescription;
            p1.PhysicianFullName = "NA";
            p1.RxSite = "NA";
            p1.RxTechnique = "NA";
            p1.RxSequence = "NA";
            p1.RxNotes = "";
            p1.RxTargets = "";

            p1.Energy = "";

            if (rx != null)
            {
                p1.PhysicianFullName = rx.HistoryUserDisplayName;
                p1.RxSite = rx.Site;
                p1.RxTechnique = rx.Technique;
                p1.RxSequence = rx.PhaseType;
                p1.RxNotes = rx.Notes;
                p1.RxGating = rx.Gating;

                IEnumerable<RTPrescriptionTarget> targets = rx.Targets;
                foreach (RTPrescriptionTarget trgt in targets)
                    p1.RxTargets += trgt.TargetId + "; ";


                //IEnumerable<string> ens = rx.Energies;
                //foreach (string en in ens)
                //    DATA.Energy += en + "; ";
            }
            else
            {
                // rx is null. set a flag to capture this. 
            }
            p1.PlanOrientation = Enum.GetName(typeof(PatientOrientation), pln.TreatmentOrientation);
            p1.UseGating = pln.UseGating;


            // FIELD INFORMATION
            IEnumerable<Beam> bms = pln.Beams;
            int nbms = 0;
            double Total_MU = 0.0;
            //string enEnergyMode = "";
            double iso_X = 0.0, iso_Y = 0.0, iso_Z = 0.0;

            p1.MLCType = "";
            p1.ToleranceTable = "";
            p1.BolusId = "";
            p1.MachineId = "";

            // bool SRS = false;
            p1.UseCouchKick = false;
            p1.UseJawTracking = false;
            List<string> energies = new List<string>();

            foreach (Beam b in bms)
            {
                if (!b.IsSetupField)
                {
                    nbms++;
                    energies.Add(b.EnergyModeDisplayName);

                    Total_MU += b.Meterset.Value;

                    p1.MLCType = Enum.GetName(typeof(MLCPlanType), b.MLCPlanType);
                    p1.MachineId = b.TreatmentUnit.Id;
                    p1.ToleranceTable = b.ToleranceTableLabel;


                    List<double> x1jaws = new List<double>();
                    List<double> x2jaws = new List<double>();
                    List<double> y1jaws = new List<double>();
                    List<double> y2jaws = new List<double>();
                    // this is field specific

                    ControlPointCollection ctrl_colls = b.ControlPoints;
                    foreach (ControlPoint ctrl in ctrl_colls)
                    {
                        VRect<double> jaws = ctrl.JawPositions;
                        x1jaws.Add(jaws.X1);
                        x2jaws.Add(jaws.X2);
                        y1jaws.Add(jaws.Y1);
                        y2jaws.Add(jaws.Y2);
                    }
                    // this is field specific
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
                        p1.UseJawTracking = true;
                                                            
                    if (!p1.UseCouchKick && b.ControlPoints.First().PatientSupportAngle != 0.0)
                        p1.UseCouchKick = true;

                    // this is field specific
                    double delta_X = (b.IsocenterPosition.x - pln.StructureSet.Image.UserOrigin.x) / 10.0;
                    double delta_Y = (b.IsocenterPosition.y - pln.StructureSet.Image.UserOrigin.y) / 10.0;
                    double delta_Z = (b.IsocenterPosition.z - pln.StructureSet.Image.UserOrigin.z) / 10.0;
                    iso_X = delta_X;
                    iso_Y = delta_Y;
                    iso_Z = delta_Z;

                    if (b.Boluses.Any()) // this is field specific
                        p1.BolusId = b.Boluses.First().Id;
                }
            }

            foreach (var en in energies.Distinct())
                p1.Energy += en + "; ";

            if (p1.Energy.IndexOf('E') >= 0)
                p1.EnergyMode = "Electron";
            else if (p1.Energy.IndexOf('X') >= 0)
                p1.EnergyMode = "Photon";
            else
                p1.EnergyMode = "Unknown";

            p1.NumberOfFields = nbms;

            p1.TotalMu = Total_MU;
            p1.IsoX = iso_X;
            p1.IsoY = iso_Y;
            p1.IsoZ = iso_Z;

            p1.IsoXstr = Math.Round(iso_X,2).ToString();
            p1.IsoYstr = Math.Round(iso_Y, 2).ToString(); 
            p1.IsoZstr = Math.Round(iso_Z, 2).ToString(); 

            p1.UseShifts = false;
            if (Math.Abs(iso_X) > 0.009 || Math.Abs(iso_Y) > 0.009 || Math.Abs(iso_Z) > 0.009)
                p1.UseShifts = true;

            p1.DoseAlgorithm = "";
            p1.DoseGridSizeCM = "";
            p1.DoseMax3D = 0.0;
            p1.DoseResX = 0.0;
            p1.DoseResY = 0.0;
            p1.DoseResZ = 0.0;

            p1.TargetVolume = pln.TargetVolumeID;
            p1.NumberOfFractions = pln.NumberOfFractions.Value;
            p1.FractionDose = pln.PlannedDosePerFraction.Dose;
            p1.TotalDose = pln.TotalDose.Dose;
            p1.PlanNormalization = pln.PlanNormalizationValue;
            p1.PrimaryRefPoint = pln.PrimaryReferencePoint.Id;

            Dose dose = pln.Dose;
            if (dose != null)
            {
                p1.DoseMax3D = dose.DoseMax3D.Dose;
                p1.DoseResX = dose.XRes;
                p1.DoseResY = dose.YRes;
                p1.DoseResZ = dose.ZRes;
                
                switch (p1.EnergyMode)
                {
                    case "Photon":
                        p1.DoseAlgorithm = pln.PhotonCalculationModel;
                        break;
                    case "Electron":
                        p1.DoseAlgorithm = pln.ElectronCalculationModel;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                p1.DoseIsNull = true;
            }


            return p1;

        }

        public PlanData p1;

        public struct PlanData
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


            public Beam beams;
            public string PlanOrientation;
            public bool UseGating;
            public int NumberOfFields;
            public string MachineId;
            public string EnergyMode;
            public string Energy;
            public double TotalMu;
            public double IsoX, IsoY, IsoZ;
            public string IsoXstr, IsoYstr, IsoZstr;
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
            public bool DoseIsNull; 

            public bool HasComment;
            public string Comment;



        }



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            DATA.SelectedCourseIndex = this.comboBox2.SelectedIndex;
            this.comboBox4.Text = "-----";
            this.comboBox4.Items.Clear();
            
            
            GetPlansList();
            this.comboBox4.Items.AddRange(DATA.PlansList);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
   
}
