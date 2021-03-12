
GetCoursesList(this_patient);
this.comboBox1.Items.AddRange(DATA.CourseList);
DATA.this_patient = this_patient;
this.comboBox2.Items.AddRange(DATA.CourseList);
this.PatientTxtbx.Text = this_patient.LastName + ", " + this_patient.FirstName + " ( " + this_patient.Id + " )";

private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)//, Patient this_patient)
{
    //GetCoursesList(this_patient);
    DATA.SelectedCourseIndex = this.comboBox2.SelectedIndex;
    GetPlansList();
    this.comboBox2.Items.AddRange(DATA.PlansList);

}


//GetCoursesList(this_patient);
//this.comboBox1.Items.AddRange(DATA.CourseList);

//DATA.this_patient = this_patient;
//this.comboBox2.Items.AddRange(DATA.CourseList);

public PlanData ExtractPlanData(PlanSetup pln)
{
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
    //p1.UseJawTracking = pln.OptimizationSetup.UseJawTracking;


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
                p1.UseJawTracking = true;


            // if (b.Technique.ToString().Contains("SRS"))
            //    isSRS = true;

            if (!p1.UseCouchKick && b.ControlPoints.First().PatientSupportAngle != 0.0)
                p1.UseCouchKick = true;

            double delta_X = (b.IsocenterPosition.x - pln.StructureSet.Image.UserOrigin.x) / 10.0;
            double delta_Y = (b.IsocenterPosition.y - pln.StructureSet.Image.UserOrigin.y) / 10.0;
            double delta_Z = (b.IsocenterPosition.z - pln.StructureSet.Image.UserOrigin.z) / 10.0;
            iso_X = delta_X;
            iso_Y = delta_Y;
            iso_Z = delta_Z;

            if (b.Boluses.Any())
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


    return p1;

}

//this.textBox1.Text = this_patient.LastName+ ", " + this_patient.FirstName+ " (" + this_patient.Id+ ")";

BolusId: ""
    Comment: null
    ContourId: "56x56x40new"
    CourseId: null
    DiagnosisCode: null
    DoseAlgorithm: "AAA_15603"
    DoseGridSizeCM: ""
    DoseIsNull: false
    DoseMax3D: 709.4128
    DoseResX: 2.5
    DoseResY: 2.5
    DoseResZ: 2.5
    Energy: "6X; 10X; 15X; 10X-FFF; "
    EnergyMode: "Photon"
    EnteredBy: null
    EntryDateTime: null
    ExportFilePath: null
    FractionDose: 3600.0549153842312
    HasComment: false
    ImageDate: "10/15/2018 4:46 PM"
    ImageId: "56x56x40new"
    ImageResX: 1.640625
    ImageResY: 1.640625
    ImageResZ: 2.5
    IsoX: -0.00011006353585862882
    IsoXstr: "0"
    IsoY: 5.0000000000001537
    IsoYstr: "5"
    IsoZ: -3.5527136788005009E-15
    IsoZstr: "0"
    MLCType: "NotDefined"
    MachineId: "TrueBeam"
    NumberOfFields: 8
    NumberOfFractions: 1
    NumberOfImages: 227
    PatientFirstName: null
    PatientId: null
    PatientLastName: null
    PhysicianFullName: "NA"
    PlanId: null
    PlanNormalization: 100
    PlanOrientation: "HeadFirstSupine"
    PlanReviewBy: null
    PlanReviewDate: null
    PlannerFullName: null
    PrimaryRefPoint: "d5"
    RxGating: null
    RxNotes: ""
    RxSequence: "NA"
    RxSite: "NA"
    RxTargets: ""
    RxTechnique: "NA"
    TargetVolume: ""
    ToleranceTable: "T1"
    TotalDose: 1000
    TotalMu: 7200
    UseCouchKick: false
    UseGating: false
    UseJawTracking: false
    UseShifts: true




    GetCoursesList(this_patient);
this.comboBox1.Items.AddRange(DATA.CourseList);
DATA.this_patient = this_patient;
this.comboBox2.Items.AddRange(DATA.CourseList);