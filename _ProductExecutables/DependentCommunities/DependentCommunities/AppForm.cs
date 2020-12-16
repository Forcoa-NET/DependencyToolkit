using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeightedNetwork;
using NetworkModels;
using ZonesToCommunities;
using System.Globalization;
using GmlNetwork;

namespace DependencyZones
{
    public partial class AppForm : Form
    {
        private static string VERSION = "13.0";
        private CultureInfo invC = CultureInfo.InvariantCulture;

        //private static string GROUND_TRUTH_NAME = null;
        //private static string GROUND_TRUTH_NAME = @"C:\Users\kudelka\Desktop\ELISKA\Networks\_bigSNAP\__ground truth\com-dblp.top5000.cmty.csv";
        //private static string GROUND_TRUTH_NAME = @"C:\Users\kudelka\Desktop\ELISKA\Networks\_bigSNAP\__ground truth\com-amazon.top5000.cmty.csv";
        //private static string GROUND_TRUTH_NAME = @"C:\Users\kudelka\Desktop\ELISKA\Networks\_bigSNAP\__ground truth\com-youtube.top5000.cmty.csv";
        //private static string GROUND_TRUTH_NAME = @"C:\Users\kudelka\Desktop\ELISKA\Networks\_bigSNAP\__ground truth\com-lj.top5000.cmty.csv";
        //private static string GROUND_TRUTH_NAME = @"C:\Users\kudelka\Desktop\ELISKA\Networks\_bigSNAP\__ground truth\community email-Eu-core.csv";
        //private static string GROUND_TRUTH_NAME = @"C:\Users\kudelka\Desktop\ELISKA\Networks\_LFR Model\_ground-truth\community Bio.csv";
        //private static string GROUND_TRUTH_NAME = @"C:\Users\kudelka\Desktop\ELISKA\Networks\_LFR Model\_ground-truth\community Coll.csv";
        //private static string GROUND_TRUTH_NAME = @"C:\Users\kudelka\Desktop\ELISKA\Networks\#extra\_ground-truth\community anobii.csv";

        private TimeSpan DependencyMatrixTime;
        private TimeSpan EgoZoneDetectionTime;
        private int Covers;
        private int Overlaps;

        static bool ASSERT = false;
        static bool DETECTION_TIME_MESSAGE = true;

        static double NETWORK_DEPENDENCY_THRESHOLD = 0.5;
        static double GROUP_DEPENDENCY_THRESHOLD = 0;
        static int ZONE_MIN_SIZE = 1;
        static int ZONE_MAX_SIZE = 0;
        static DependencyZone.DuplicityFilter ZONE_DUPLICITY_FILTER = DependencyZone.DuplicityFilter.MultiEgo;
        static WeightedNetwork.DependencyZones.ProminencyFilter PROMINENCY_FILTER = WeightedNetwork.DependencyZones.ProminencyFilter.AllProminents;
        static ZoneGroups.GroupingStrategy ZONE_GROUPING_STRATEGY = ZoneGroups.GroupingStrategy.DependentOuter;

        int MIN_OVERLAPPING_ZONES = 3;
        int MIN_INTRAOVERLAP = 10;

        private Statistics statistics;

        static Random RANDOM = new Random();

        static string GRAY = "'211,211,211'";
        static string RED = "'255,0,0'";
        static string GREEN = "'0,204,0'";
        static string BLUE = "'0,0,255'";
        static string MAGENTA = "'238,130,238'";//violet
        static string YELLOW = "'255,215,0'";
        //static string ORANGE = "'255,165,0'";
        static string CYAN = "'0,191,255'";

        private string fileName = string.Empty;
        private string groundTruthName = string.Empty;
        private string PATH = string.Empty;
        //private bool englishMode;

        private Network fullNetwork = null;
        private Network network = null;

        private WeightedNetwork.DependencyZones zones = null;
        private ZoneGroups zoneGroups = null;
        private ZoneToCommunities zoneToCommunities = null;
        private Dictionary<IDependencyGroup, ZoneToCommunities.ZoneFitResult> ZoneFitResults = null;

        public AppForm()
        {
            InitializeComponent();
        }

        private void AppForm_Load(object sender, EventArgs e)
        {
            this.UkazkaButton.Visible = true; // ukazka volani
            this.DependencyClosureButton.Visible = true; // analyza triadic closure

            this.Text = "Ego-zones detector [version " + VERSION + "]";

            WeightedNetwork.Vertex.ASSERT = ASSERT;

            this.ProminencyFilterComboBox.DataSource = Enum.GetValues(typeof(WeightedNetwork.DependencyZones.ProminencyFilter));
            this.ProminencyFilterComboBox.SelectedIndex = Convert.ToInt32(WeightedNetwork.DependencyZones.ProminencyFilter.None);
            this.GroupingStrategyComboBox.DataSource = Enum.GetValues(typeof(ZoneGroups.GroupingStrategy));
            this.GroupingStrategyComboBox.SelectedIndex = Convert.ToInt32(ZoneGroups.GroupingStrategy.Outer);
            this.DuplicityFilterComboBox.DataSource = Enum.GetValues(typeof(DependencyZone.DuplicityFilter));
            this.DuplicityFilterComboBox.SelectedIndex = Convert.ToInt32(DependencyZone.DuplicityFilter.MultiEgo);

            this.NetDepNumericUpDown.Value = Convert.ToDecimal(0.5);
            this.MinSizeNumericUpDown.Value = Convert.ToDecimal(1);
            this.MaxSizeNumericUpDown.Value = Convert.ToDecimal(0);

            this.OuterZoneCheckBox.Checked = true;
            this.LargeScaleCheckBox.Checked = true;
            this.NetworkTypeComboBox.SelectedIndex = 2;

            this.VerticesTextBox.Text = "1000";
            this.ParameterTextBox.Text = string.Empty;

            this.GdfCheckBox.Checked = true;
            this.GdfCheckBox.Checked = false;
            this.GdfMinNumericUpDown.Minimum = 1;
            this.GdfMinNumericUpDown.Maximum = 999;
            this.GdfMinNumericUpDown.Value = 1;

            this.DepEdgeCheckBox.Checked = true;

            this.ProgressLabel.Text = "Network does not exist, click \"Read file\"";
        }

        private void ReadFileButton_Click(object sender, EventArgs e)
        {
            this.OpenFileDialog.Filter = "CSV files (*.csv)|*.csv|GML files(*.gml)|*.gml";

            if (OpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.setLoadingProgress();

                this.fileName = OpenFileDialog.FileName;
                PathTextBox.Text = this.fileName;

                if (System.IO.Path.GetExtension(this.fileName).ToLower() == ".gml")
                {
                    GmlToNetwork gmlNet = new GmlToNetwork(this.fileName);
                    this.fullNetwork = gmlNet.Network;
                }
                else
                {
                    this.fullNetwork = this.readNetwork();
                }

                this.zones = null;
                this.zoneGroups = null;
                this.zoneToCommunities = null;
                this.ZoneFitResults = null;
                this.PATH = string.Empty;

                this.setNetworkProgress();
            }

            
            //// test maximalni velikosti komunity
            //Communities ccc = new Communities(GROUND_TRUTH_NAME, this.fullNetwork); 

        }

        private void DetectZonesButton_Click(object sender, EventArgs e)
        {
            if (this.fullNetwork == null)
            {
                MessageBox.Show("Network does not exist...");
                return;
            }

            if (this.NetworkTypeComboBox.SelectedIndex == 0)
            {
                this.network = this.fullNetwork.GetLargestComponentNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 1)
            {
                this.network = this.fullNetwork.GetNoOutliersNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 2)
            {
                this.network = this.fullNetwork;
            }

            Enum.TryParse<WeightedNetwork.DependencyZones.ProminencyFilter>(this.ProminencyFilterComboBox.SelectedValue.ToString(), out PROMINENCY_FILTER);
            Enum.TryParse<DependencyZone.DuplicityFilter>(this.DuplicityFilterComboBox.SelectedValue.ToString(), out ZONE_DUPLICITY_FILTER);
            Enum.TryParse<ZoneGroups.GroupingStrategy>(this.GroupingStrategyComboBox.SelectedValue.ToString(), out ZONE_GROUPING_STRATEGY);

            NETWORK_DEPENDENCY_THRESHOLD = Convert.ToDouble(this.NetDepNumericUpDown.Value);
            GROUP_DEPENDENCY_THRESHOLD = Convert.ToDouble(this.GroupDepNumericUpDown.Value);
            ZONE_MIN_SIZE = Convert.ToInt32(this.MinSizeNumericUpDown.Value);
            ZONE_MAX_SIZE = Convert.ToInt32(this.MaxSizeNumericUpDown.Value);

            this.setDependencyMatrixProgress();

            System.Diagnostics.Stopwatch stopWatch;
            stopWatch = System.Diagnostics.Stopwatch.StartNew();
            network.DependencyThreshold = NETWORK_DEPENDENCY_THRESHOLD;

            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(this.network.Edges, options, de =>
            {
                de.VertexA.IsDependentOn(de.VertexB);
                de.VertexB.IsDependentOn(de.VertexA);
           });
            stopWatch.Stop();
            this.DependencyMatrixTime = stopWatch.Elapsed;

            Network.NetDependency netDependency = this.network.GetNetDependency();
            DialogResult result = MessageBox.Show("NetDep: " + netDependency.NetDep + Environment.NewLine + 
                "AvgDepOn: " + netDependency.AvgDependentOn + Environment.NewLine + 
                "MaxDepOn: " + netDependency.MaxDependentOn + Environment.NewLine +
                "Estimated size of large zone/group: " + netDependency.EstimatedLargeZoneSize + Environment.NewLine + Environment.NewLine +
                "Continue?", "Network Dependency", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.No)
            {
                this.setNetworkProgress();
                return;
            }


            stopWatch = System.Diagnostics.Stopwatch.StartNew();
            int minSize = 1;
            if (ZONE_MIN_SIZE > 0)
            {
                minSize = ZONE_MIN_SIZE;
            }
            int maxSize = this.network.Vertices.Count;
            if (ZONE_MAX_SIZE > 0)
            {
                maxSize = ZONE_MAX_SIZE;
            }

            this.setZoneDetectionProgress(false);

            this.zones = new WeightedNetwork.DependencyZones(false, this.network, minSize, maxSize, this.OuterZoneCheckBox.Checked,
                ZONE_DUPLICITY_FILTER, PROMINENCY_FILTER, GROUP_DEPENDENCY_THRESHOLD);
            stopWatch.Stop();
            this.EgoZoneDetectionTime = stopWatch.Elapsed;

            if (DETECTION_TIME_MESSAGE)
            {
                result = this.EgoZoneMesssage();
                if (result == DialogResult.No)
                {
                    this.setNetworkProgress();
                    return;
                }
            }

            this.setZoneDetectionProgress(true);

            this.zones.CompleteAndFilterDuplicities();
            this.zones.CalculateMemberships();

            ///////////////////////////
            this.statistics = new Statistics(this.zones);
            ///////////////////////////

            string name = System.IO.Path.GetFileNameWithoutExtension(this.fileName);

            DateTime dt = DateTime.Now;
            string code = dt.ToString(" yyyy-MM-dd HH-mm-ss");
            name += code;

            string folderName = System.IO.Path.GetDirectoryName(this.fileName);
            this.PATH = System.IO.Path.Combine(folderName, name);

            System.IO.Directory.CreateDirectory(this.PATH);

            if (this.SuperSubCheckBox.Checked)
            {
                this.setSuperSubOverlapsProgress();
                this.zones.CalculateRelationships();
            }

            if (this.SuperSubCheckBox.Checked)
            {
                if (!this.LargeScaleCheckBox.Checked)
                {
                    this.saveSuperSubOverlappingZones();
                }

                this.setCoversOverlapsProgress();
                this.saveCoversOverlaps();
            }

            string outputFile;
            bool isGml = (this.fileName.IndexOf(".gml") > 0);
            if (this.fileName.IndexOf(".gml") > 0)
            {
                name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");
                outputFile = System.IO.Path.Combine(this.PATH, "edges " + name);

                this.saveFullNetwork(outputFile, true, false, false);
            }

            outputFile = System.IO.Path.Combine(this.PATH, "edges_dep " + name);
            if (!isGml)
            {
                outputFile += ".csv";
            }
            this.saveFullNetwork(outputFile, true, true, false);

            outputFile = System.IO.Path.Combine(this.PATH, "edges_dep_directed " + name);
            if (!isGml)
            {
                outputFile += ".csv";
            }
            this.saveFullNetwork(outputFile, true, true, true);

            this.saveSummary();
            this.saveStatistics();

            result = MessageBox.Show("Zone grouping follows. Continue?...", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                this.setZoneGroupingProgress(false);
                bool nonZonesOnly = true;// this.CommunityDetectionCheckBox.Checked;
                this.zoneGroups = new ZoneGroups(false, zones, ZONE_GROUPING_STRATEGY, nonZonesOnly, GROUP_DEPENDENCY_THRESHOLD);

                this.setZoneGroupingProgress(true);
                this.zoneGroups.CompleteAndFilterDuplicities();
            }

            if (this.GdfCheckBox.Checked)
            {
                this.setGdfFilesProgress();
                this.saveGdfFiles();
            }

            if (this.CommunityDetectionCheckBox.Checked)
            {
                this.setCommunitiesProgress();
                List<IDependencyGroup> allGroups = new List<IDependencyGroup>(this.zones.AllZones);
                if (this.zoneGroups != null)
                {
                    foreach(ZoneGroup g in this.zoneGroups.AllGroups)
                    {
                        if (!g.IsZone)
                        {
                            allGroups.Add(g);
                        }
                    }
                }

                this.zoneToCommunities = new ZoneToCommunities(allGroups, this.network);
                if (string.IsNullOrEmpty(this.groundTruthName))
                {
                    this.zoneToCommunities.Detect();
                }
                else
                {
                    this.zoneToCommunities.Detect(this.groundTruthName);
                }


                this.setCommunitiesProcessingProgress();
                //this.calculateMccResults();
                this.ZoneFitResults = zoneToCommunities.GetZoneFitResults();
            }

            this.setZoneResultsProgress();
            this.saveZones();

            if (this.zoneGroups != null)
            {
                this.setZoneGroupsSavingProgress();
                this.saveZoneGroups();
            }

            if (this.CommunityDetectionCheckBox.Checked)
            {
                this.saveCommunities();
            }

            this.setNetworkZonesProgress();
            System.Diagnostics.Process.Start(this.PATH);
        }

        private void setLoadingProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Network is loading...");

            this.ProgressLabel.Text = sb.ToString();
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setNetworkProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Network has ");
            sb.Append(this.fullNetwork.Vertices.Count + " vertices and ");
            sb.Append(this.fullNetwork.Edges.Count + " edges");

            this.ProgressLabel.Text = sb.ToString();
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setNetworkZonesProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.network.Vertices.Count + " vertices, ");
            sb.Append(this.network.Edges.Count + " edges, ");
            sb.Append(this.zones.AllZones.Count + " zones, ");
            if (this.zoneGroups != null)
            {
                sb.Append(this.zoneGroups.AllGroups.Count + " groups");
            }

            this.ProgressLabel.Text = sb.ToString().Trim(',');
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setZoneDetectionProgress(bool filtering)
        {
            string procedure = "Detecting";
            if (filtering)
            {
                procedure = "Completing and filtering";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(procedure);
            sb.Append(" zones...");

            this.ProgressLabel.Text = sb.ToString();
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setZoneGroupingProgress(bool filtering)
        {
            string procedure = "Detecting";
            if (filtering)
            {
                procedure = "Completing and filtering";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(procedure);
            sb.Append(" zone groups...");

            this.ProgressLabel.Text = sb.ToString();
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setDependencyMatrixProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Dependency matrix calculation...");

            this.ProgressLabel.Text = sb.ToString();
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setCommunitiesProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Detecting Communities...");

            this.ProgressLabel.Text = sb.ToString();
            //ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setCommunitiesProcessingProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Processing Communities Against Zones...");

            this.ProgressLabel.Text = sb.ToString();
            //ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setZoneResultsProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Processing and saving results for ");
            sb.Append(this.zones.AllZones.Count + " zones...");

            this.ProgressLabel.Text = sb.ToString();

            ZonesProgressBar.Minimum = 0;
            ZonesProgressBar.Maximum = this.zones.AllZones.Count + this.network.Vertices.Count + 2;
            ZonesProgressBar.Step = 1;
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setZoneGroupsSavingProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Saving results for ");
            sb.Append(this.zoneGroups.AllGroups.Count + " zone groups...");

            this.ProgressLabel.Text = sb.ToString();

            ZonesProgressBar.Minimum = 0;
            ZonesProgressBar.Maximum = this.zoneGroups.AllGroups.Count;
            ZonesProgressBar.Step = 1;
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setGdfFilesProgress()
        {
            StringBuilder sb = new StringBuilder();
            int n = this.zones.AllZones.Count;
            if (this.zoneGroups != null)
            {
                n += this.zoneGroups.AllGroups.Count;
            }
            sb.Append("Processing " + n);
            sb.Append(" Gephi (GDF) files...");

            this.ProgressLabel.Text = sb.ToString();

            ZonesProgressBar.Minimum = 0;
            ZonesProgressBar.Maximum = this.zones.AllZones.Count;
            ZonesProgressBar.Step = 1;
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setSuperSubOverlapsProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Processing super, sub and overlaps of ");
            sb.Append(this.zones.AllZones.Count + " zones...");

            this.ProgressLabel.Text = sb.ToString();

            ZonesProgressBar.Minimum = 0;
            ZonesProgressBar.Maximum = this.zones.AllZones.Count;
            ZonesProgressBar.Step = 1;
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void setCoversOverlapsProgress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Processing sub-zones in overlaps for ");
            sb.Append(this.zones.AllZones.Count + " zones...");

            this.ProgressLabel.Text = sb.ToString();

            ZonesProgressBar.Minimum = 0;
            ZonesProgressBar.Maximum = 3;
            ZonesProgressBar.Step = 1;
            ZonesProgressBar.Value = 0;

            Application.DoEvents();
        }

        private void saveSummary()
        {
            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");
            name = name.Replace(".csv", ".txt");

            string outputFile = System.IO.Path.Combine(this.PATH, "summary " + name);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);

            StringBuilder sb = new StringBuilder();
            sb.Append("Vertices: " + this.network.Vertices.Count + Environment.NewLine);
            sb.Append("Edges: " + this.network.Edges.Count + Environment.NewLine);
            sb.Append("Zones: " + this.zones.AllZones.Count + " [filtered: " + ZONE_DUPLICITY_FILTER + "]" + Environment.NewLine);

            sb.Append(Environment.NewLine);

            sb.Append("Dependency matrix time: " + this.DependencyMatrixTime.TotalSeconds + Environment.NewLine);
            sb.Append("EgoZone detection time: " + this.EgoZoneDetectionTime.TotalSeconds + Environment.NewLine);

            sb.Append(Environment.NewLine);

            if (this.zoneToCommunities != null)
            {
                sb.Append("Communities: " + this.zoneToCommunities.Communities.AllCommunities.Count + Environment.NewLine);
                sb.Append("Modularity: " + this.zoneToCommunities.Communities.Modularity + Environment.NewLine);
            }

            sb.Append(Environment.NewLine);

            if (this.SuperSubCheckBox.Checked)
            {
                sb.Append("Super-zones: " + this.Covers + Environment.NewLine);
                sb.Append("Overlaps: " + this.Overlaps + Environment.NewLine);
            }

            sw.Write(sb.ToString());
            sw.Close();
        }

        private DialogResult EgoZoneMesssage()
        {
            DialogResult result = MessageBox.Show("Vertices: " + this.network.Vertices.Count + Environment.NewLine +
                "Edges: " + this.network.Edges.Count + Environment.NewLine +
                "Zones: " + this.zones.AllZones.Count + Environment.NewLine +
                "Dependency matrix time: " + this.DependencyMatrixTime.TotalSeconds + Environment.NewLine +
                "EgoZone detection time: " + this.EgoZoneDetectionTime.TotalSeconds + Environment.NewLine + Environment.NewLine +
                "Continue?",
                "Stopwatch...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            return result;
        }

        private Network readNetwork()
        {
            if (string.IsNullOrEmpty(this.fileName))
            {
                return null;
            }

            Network newNetwork = new Network(false);

            System.IO.StreamReader sr = new System.IO.StreamReader(this.fileName);
            string s = sr.ReadLine();
            //this.setLanguageMode(s);
            sr.Close();

            char[] sep = { ',', '\t', ' ' };

            char commaSep = ',';
            bool isSepDetected = false;

            sr = new System.IO.StreamReader(this.fileName);
            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();

                if (!isSepDetected)
                {
                    if (s.Contains(sep[0]) || s.Contains(sep[1]) || s.Contains(sep[2]))
                    {
                        isSepDetected = true;
                    }
                    else if (s.Contains(commaSep)) //prave anglicke CSV
                    {
                        s = s.Replace(',', ';');
                        isSepDetected = true;
                    }
                }

                string[] line = s.Split(sep);

                int idA = Convert.ToInt32(line[0]);
                int idB = Convert.ToInt32(line[1]);
                Vertex vA;
                if (!newNetwork.Vertices.TryGetValue(idA, out vA))
                {
                    vA = new Vertex(newNetwork, idA, Convert.ToString(idA), 1);
                    newNetwork.Vertices.Add(idA, vA);
                }
                Vertex vB;
                if (!newNetwork.Vertices.TryGetValue(idB, out vB))
                {
                    vB = new Vertex(newNetwork, idB, Convert.ToString(idB), 1);
                    newNetwork.Vertices.Add(idB, vB);
                }

                double w = 1;
                if (line.Length == 3)
                {
                    w = double.Parse(line[2], invC);
                }

                newNetwork.CreateEdge(vA, vB, w);
            }

            sr.Close();

            return newNetwork;
        }

        //private void setLanguageMode(string s)
        //{
        //    this.englishMode = !s.Contains(';');
        //}

        private void saveZones()
        {
            if (this.zones.AllZones.Count == 0)
            {
                return;
            }

            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");

            string outputFile = System.IO.Path.Combine(this.PATH, "zones " + name);
            string outputFileEI = System.IO.Path.Combine(this.PATH, "zones_extra_info " + name);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);
            System.IO.StreamWriter swEI = new System.IO.StreamWriter(outputFileEI);

            string sep = ";";

            swEI.Write("NUMBER;CATEGORY;SIZE;EGOS;LIAISONS;CO-LIAISONS;BASIC-MODULARITY;INNER-MODULARITY;DENSITY;ZxC_MATTHEWS_CC;COMMUNITY_ID;COMMUNITY_COUNT;COMMUNITY_OVERLAP;EMBEDDEDNESS;SUPER;SUB;OVERLAP;DEPENDENCY;QUALITY;EGO-EXP-RATIO");

            swEI.WriteLine();

            StringBuilder sb = new StringBuilder();
            StringBuilder sbEI = new StringBuilder();

            foreach (DependencyZone zone in this.zones.AllZones)
            {
                ZonesProgressBar.PerformStep();

                /////////////////////////////////////////////
                this.statistics.SumZoneSize += zone.AllVertices.Count;
                this.statistics.MaxZoneSize = Math.Max(this.statistics.MaxZoneSize, zone.AllVertices.Count);
                this.statistics.SumInnerZoneSize += zone.InnerZone.Count;
                this.statistics.MaxInnerZoneSize = Math.Max(this.statistics.MaxInnerZoneSize, zone.InnerZone.Count);
                this.statistics.SumOuterZoneSize += zone.OuterZone.Count;
                this.statistics.MaxOuterZoneSize = Math.Max(this.statistics.MaxOuterZoneSize, zone.OuterZone.Count);
                if (zone.Egos.Count > 1)
                {
                    this.statistics.MultiEgoZonesCount += 1;
                    this.statistics.SumMultiEgoZoneEgos += zone.Egos.Count;
                    this.statistics.MaxMultiEgoZoneEgos = Math.Max(this.statistics.MaxMultiEgoZoneEgos, zone.Egos.Count);
                }
                if (this.zones.Relationships.Count > 0 && this.zones.Relationships[zone].AlternativeZones.Count > 0)
                {
                    this.statistics.AltZonesCount += 1;
                }
                if (zone.AllVertices.Count == 1)
                {
                    this.statistics.TrivialZonesCount += 1;
                }
                if (zone.AllVertices.Count == 2)
                {
                    this.statistics.DyadZonesCount += 1;
                }
                if (zone.AllVertices.Count == 3)
                {
                    this.statistics.TriadZonesCount += 1;
                }
                /////////////////////////////////////////////


                sb.Append(zone.Id + sep + zone.AllVertices.Count);
                sbEI.Append(zone.Id + sep + this.getCategoryText(zone) + sep + zone.AllVertices.Count);

                StringBuilder sbTmp;

                if (zone.Egos.Count > 0)
                {
                    sbEI.Append(sep);
                    sbTmp = new StringBuilder();
                    foreach (Vertex ego in zone.Egos)
                    {
                        sbTmp.Append(ego.Id + "*");
                    }
                    sbEI.Append(sbTmp.ToString().Trim('*'));
                }
                else
                {
                    sbEI.Append(sep);
                }

                if (zone.Liaisons.Count > 0)
                {
                    sbEI.Append(sep);
                    sbTmp = new StringBuilder();
                    foreach (Vertex liaison in zone.Liaisons)
                    {
                        sbTmp.Append(liaison.Id + "*");
                    }
                    sbEI.Append(sbTmp.ToString().Trim('*'));
                }
                else
                {
                    sbEI.Append(sep);
                }

                if (zone.CoLiaisons.Count > 0)
                {
                    sbEI.Append(sep);
                    sbTmp = new StringBuilder();
                    foreach (Vertex coLiaison in zone.CoLiaisons)
                    {
                        sbTmp.Append(coLiaison.Id + "*");
                    }
                    sbEI.Append(sbTmp.ToString().Trim('*'));
                }
                else
                {
                    sbEI.Append(sep);
                }

                DependencyZone.ZoneModularity zoneModularity = zone.Modularity;
                string basicModularity = zoneModularity.BasicValue.ToString();
                string innerModularity = zoneModularity.InnerValue.ToString();
                string density = Network.GetDensity(zone.AllVertices).ToString();
                //string modularity = zoneModularity.ZoneValue.ToString();
                //string intraDependency = string.Empty;
                //if (!this.LargeScaleCheckBox.Checked)
                //{
                //    intraDependency = zone.GetIntraDependency().ToString();

                bool commaMode = false; // this.englishMode;
                if (commaMode)
                {
                    sep = ",";
                    basicModularity = basicModularity.Replace(',', '.');
                    innerModularity = innerModularity.Replace(',', '.');
                    density = density.Replace(',', '.');
                    //modularity = modularity.Replace(',', '.');
                    //intraDependency = intraDependency.Replace(',', '.');
                }
                sbEI.Append(sep + basicModularity + sep + innerModularity + sep + density);
                //sbEI.Append(sep + modularity + sep + innerModularity + sep + intraDependency);

                string FIT = string.Empty;
                string ID = string.Empty;
                string COUNT = string.Empty;
                string OVERLAP = string.Empty;
                if (this.CommunityDetectionCheckBox.Checked)
                {
                    ZoneToCommunities.ZoneFitResult fit = this.ZoneFitResults[zone];
                    FIT = fit.Value.ToString();
                    ID = fit.CommunityId.ToString();
                    COUNT = fit.CommunityCount.ToString();
                    OVERLAP = fit.OverlapCount.ToString();
                }
                double embed = Network.GetEmbeddedness(zone.AllVertices);
                sbEI.Append(sep + FIT + sep + ID + sep + COUNT + sep + OVERLAP + sep + embed + sep);

                /////////////////////////////////////////////
                if (zone.AllVertices.Count > 0)
                {
                    this.statistics.SumEmbeddedness += embed;
                    this.statistics.NonTrivialEmbeddednessCount += 1;
                }
                /////////////////////////////////////////////

                string super = string.Empty;
                string sub = string.Empty;
                string overlap = string.Empty;
                if (this.zones.Relationships.Count > 0)
                {
                    int sum = this.zones.Relationships[zone].SuperZones.Values.Count;
                    super = sum.ToString();

                    sum = this.zones.Relationships[zone].SubZones.Values.Count;
                    sub = sum.ToString();

                    sum = this.zones.Relationships[zone].OverlappingZones.Values.Count;
                    overlap = sum.ToString();

                    /////////////////////////////////////////////
                    this.statistics.SumSubZonesCount += this.zones.Relationships[zone].SubZones.Values.Count;
                    this.statistics.MaxSubZonesCount = Math.Max(this.statistics.MaxSubZonesCount, this.zones.Relationships[zone].SubZones.Values.Count);
                    this.statistics.SumOverlappingZonesCount += this.zones.Relationships[zone].OverlappingZones.Values.Count;
                    this.statistics.MaxOverlappingZonesCount = Math.Max(this.statistics.MaxOverlappingZonesCount, this.zones.Relationships[zone].OverlappingZones.Values.Count);
                    if (this.zones.Relationships[zone].GetCategory() == ZoneCategory.TopZone)
                    {
                        this.statistics.TopZonesCount += 1;
                    }
                    else if (this.zones.Relationships[zone].GetCategory() == ZoneCategory.InnerZone)
                    {
                        this.statistics.InnerZonesCount += 1;
                    }
                    else if (this.zones.Relationships.Count > 0 && this.zones.Relationships[zone].GetCategory() == ZoneCategory.BottomZone)
                    {
                        this.statistics.BottomZonesCount += 1;
                    }
                    else 
                    {
                        this.statistics.IndependentZonesCount += 1;
                    }
                    /////////////////////////////////////////////

                }
                sbEI.Append(super + sep);
                sbEI.Append(sub + sep);
                sbEI.Append(overlap);

                if (!this.LargeScaleCheckBox.Checked)
                {
                    foreach (Vertex v in zone.AllVertices)
                    {
                        sb.Append(sep + v.Id);
                    }
                }

                double dep = zone.Dependency.DependencyScore;
                sbEI.Append(sep + dep);
                sbEI.Append(sep + zone.GetQuality());
                sbEI.Append(sep + zone.GetEgoExpansionRatio());

                sw.WriteLine(sb.ToString());
                swEI.WriteLine(sbEI.ToString());

                sb.Clear();
                sbEI.Clear();
            }

            sw.Close();
            swEI.Close();

            outputFile = System.IO.Path.Combine(this.PATH, "vertices " + name);
            this.saveVertices(outputFile);

            if (!this.LargeScaleCheckBox.Checked)
            {
                string outputFileNames = System.IO.Path.Combine(this.PATH, "names_zones " + name);
                this.saveNames(outputFileNames);

                string gdfName = System.IO.Path.GetFileNameWithoutExtension(this.fileName);
                outputFile = System.IO.Path.Combine(this.PATH, "dependency_network " + gdfName + ".gdf");
                ZonesProgressBar.PerformStep();
                this.saveGdfDependencyNetwork(outputFile, DependencyEdgeWeight.GeometricMean, false);

                gdfName = System.IO.Path.GetFileNameWithoutExtension(this.fileName);
                outputFile = System.IO.Path.Combine(this.PATH, "degree_network " + gdfName + ".gdf");
                ZonesProgressBar.PerformStep();
                this.saveGdfDependencyNetwork(outputFile, DependencyEdgeWeight.Unweighted, true);

                outputFile = System.IO.Path.Combine(this.PATH, "directed_dependency_network " + gdfName + ".gdf");
                ZonesProgressBar.PerformStep();
                this.saveGdfDirectedDependencyNetwork(outputFile);

                outputFile = System.IO.Path.Combine(this.PATH, "prominency_network " + gdfName + ".gdf");
                ZonesProgressBar.PerformStep();
                this.saveGdfOverlapsNetwork(outputFile);
            }

            //outputFile = System.IO.Path.Combine(path, "reduced_edges " + name);
            //this.saveDependentEdges(outputFile);
        }

        private void saveZoneGroups()
        {
            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");
            string outputFile = System.IO.Path.Combine(this.PATH, "zoneGroups " + name);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);
            char sep = ';';

            sw.WriteLine("ID;Zone;Zones;Size;InnerSize;OuterSize;Embeddedness;Dependency;Quality;EgoExpRatio;Vertices");
            foreach (ZoneGroup group in this.zoneGroups.AllGroups)
            {
                ZonesProgressBar.PerformStep();

                sw.Write(group.Id.ToString() + sep);
                sw.Write(group.IsZone.ToString() + sep);
                sw.Write(group.Zones.Count.ToString() + sep);
                sw.Write(group.AllVertices.Count.ToString() + sep);
                sw.Write(group.InnerZone.Count.ToString() + sep);
                sw.Write(group.OuterZone.Count.ToString() + sep);
                sw.Write(group.GetEmbeddedness().ToString() + sep);
                sw.Write(group.Dependency.DependencyScore.ToString() + sep);
                sw.Write(group.GetQuality().ToString() + sep);
                sw.Write(group.GetEgoExpansionRatio().ToString() + sep);
                StringBuilder sb = new StringBuilder();
                //foreach (Vertex v in group.AllVertices)
                //{
                //    sb.Append(v.Id.ToString() + sep);
                //}
                sw.WriteLine(sb.ToString().Trim(sep));
            }

            sw.Close();
        }

        private void saveNames(string filename)
        {
            System.IO.StreamWriter swNames = new System.IO.StreamWriter(filename);

            string sep = ";";

            foreach (DependencyZone zone in this.zones.AllZones)
            {
                swNames.Write(zone.Id + sep + zone.AllVertices.Count);

                StringBuilder sb = new StringBuilder();
                foreach (string n in zone.GetNames())
                {
                    swNames.Write(sep + n);
                }

                swNames.WriteLine();
            }

            swNames.Close();
        }

        private string getCategoryText(IDependencyGroup group)
        {
            if (this.zones.Relationships.Count == 0)
            {
                return string.Empty;
            }

            string categoryText = string.Empty;
            if (group.GetType() == typeof(DependencyZone))
            {
                DependencyZone zone = (DependencyZone)group;
                ZoneCategory category = this.zones.Relationships[zone].GetCategory();

                if (category == ZoneCategory.TopZone)
                {
                    categoryText = "TOP";
                }
                else if (category == ZoneCategory.BottomZone)
                {
                    categoryText = "BOTTOM";
                }
                else if (category == ZoneCategory.InnerZone)
                {
                    categoryText = "INTER";
                }
                else if (category == ZoneCategory.OutlyingZone)
                {
                    categoryText = "OUTLYING";
                }
                else if (category == ZoneCategory.IndependentZone)
                {
                    categoryText = "INDEPENDENT";
                }
                else if (category == ZoneCategory.OverlappingZone) //zatim nenastalo
                {
                    categoryText = "OVERLAPPING";
                }
            }
            else
            {
                categoryText = "GROUP_OF_ZONES";
            }

            return categoryText;
        }

        private void saveGdfFiles()
        {
            List<IDependencyGroup> groups = new List<IDependencyGroup>(this.zones.AllZones);
            if (this.zoneGroups != null)
            {
                groups.AddRange(this.zoneGroups.AllGroups);
            }

            foreach (IDependencyGroup group in groups)
            {
                ZonesProgressBar.PerformStep();

                if (group.AllVertices.Count >= this.GdfMinNumericUpDown.Value)
                {
                    string gdfName = System.IO.Path.GetFileNameWithoutExtension(this.fileName);

                    char subSep = '-';

                    string egos = string.Empty;
                    foreach (Vertex ego in group.Egos)
                    {
                        egos += ego.Name + subSep;
                    }
                    egos = egos.TrimEnd(subSep);
                    int maxLehgth = 200 - this.PATH.Length - gdfName.Length; //260 je maximalni velikost jmena souboru
                    if (egos.Length > maxLehgth)
                    {
                        egos = egos.Substring(0, maxLehgth - 3) + "...";
                    }

                    gdfName = this.getCorrectFilename(group.Id + " " +
                        group.AllVertices.Count + " " +
                        egos + " " +
                        group.Egos.Count + subSep +
                        group.Liaisons.Count + subSep +
                        group.CoLiaisons.Count + " " +
                        gdfName + ".gdf");

                    gdfName = System.IO.Path.Combine(this.PATH, gdfName);

                    this.saveGdfGroup(group, gdfName);
                }
            }
        }

        private void saveSuperSubOverlappingZones()
        {
            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");

            string outputFileSub = System.IO.Path.Combine(this.PATH, "sub_zones " + name);
            string outputFileSuper = System.IO.Path.Combine(this.PATH, "super_zones " + name);
            string outputFileOverlapping = System.IO.Path.Combine(this.PATH, "overlapping_zones " + name);

            System.IO.StreamWriter swSub = new System.IO.StreamWriter(outputFileSub);
            System.IO.StreamWriter swSuper = new System.IO.StreamWriter(outputFileSuper);
            System.IO.StreamWriter swOverlapping = new System.IO.StreamWriter(outputFileOverlapping);

            string sep = ";";

            foreach (DependencyZone zone in this.zones.AllZones)
            {
                if (zone.AllVertices.Count >= MIN_OVERLAPPING_ZONES)
                {
                    DependencyZoneRelationships relationship = this.zones.Relationships[zone];

                    Dictionary<int, DependencyZone> subZones = relationship.SubZones;
                    if (subZones.Count > 0)
                    {
                        swSub.Write(zone.Id + sep + zone.AllVertices.Count + sep + subZones.Count);
                        foreach (int number in subZones.Keys)
                        {
                            swSub.Write(sep + number);
                        }
                        swSub.WriteLine();
                    }

                    Dictionary<int, DependencyZone> superZones = relationship.SuperZones;
                    if (superZones.Count > 0)
                    {
                        swSuper.Write(zone.Id + sep + zone.AllVertices.Count + sep + superZones.Count);
                        foreach (int number in superZones.Keys)
                        {
                            swSuper.Write(sep + number);
                        }
                        swSuper.WriteLine();
                    }

                    Dictionary<int, DependencyZone> overlappingZones = relationship.OverlappingZones;
                    if (overlappingZones.Count > 0)
                    {
                        swOverlapping.Write(zone.Id + sep + zone.AllVertices.Count + sep + overlappingZones.Count);
                        foreach (int number in overlappingZones.Keys)
                        {
                            swOverlapping.Write(sep + number);
                        }
                        swOverlapping.WriteLine();
                    }
                }
            }

            swSub.Close();
            swSuper.Close();
            swOverlapping.Close();

            string gdfName = System.IO.Path.GetFileNameWithoutExtension(this.fileName);
            string outputFile = System.IO.Path.Combine(this.PATH, "_zones_hierarchy " + gdfName + ".gdf");

            if (!this.LargeScaleCheckBox.Checked)
            {
                this.saveGdfZonesHierarchy(outputFile);
            }
        }

        private void saveVertices(string filename)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);
            StringBuilder sb = new StringBuilder();

            string sep = ";";
            sw.WriteLine("Id;NAME;DEGREE;DEP-DEGREE;CC;ONE-WAY-DEPENDENCY;ONE-WAY-INDEPENDENCY;TWO-WAY-DEPENDENCY;TWO-WAY-INDEPENDENCY;PROMINENCY;ZONE-ID;ZONE-SIZE;ZONE-CATEGORY;ZONE-DENSITY;" +
                "MEMBERSHIP;INNER-MEMBERSHIP;LIAISONSHIP;CO-LIAISONSHIP;SUPER;SUB;OVERLAP;ZONE-MODULARITY");

            List<int> Ids = new List<int>();
            foreach (Vertex v in this.network.Vertices.Values)
            {
                Ids.Add(v.Id);
            }
            Ids.Sort();

            foreach (int id in Ids)
            {
                ZonesProgressBar.PerformStep();

                Vertex v = this.network.Vertices[id];
                DependencyZone zone = this.zones.GetZone(v);
                if (zone != null)
                {
                    Vertex.Prominency prominency = v.GetProminency();

                    /////////////////////////////////////////////
                    this.statistics.MaxDegree = Math.Max(this.statistics.MaxDegree, v.AdjacentsCount);
                    if (prominency.GetValue() == 1)
                    {
                        this.statistics.ProminentsCount += 1;
                    }
                    else if (prominency.GetValue() > 0)
                    {
                        this.statistics.SubProminentsCount += 1;
                    }
                    this.statistics.SumMembreshipCount += this.zones.Memberships[v].Membership.Count;
                    this.statistics.MaxMembershipCount = Math.Max(this.statistics.MaxMembershipCount, this.zones.Memberships[v].Membership.Count);
                    this.statistics.SumLiaisonshipCount += this.zones.Memberships[v].Liaisonship.Count;
                    this.statistics.MaxLiaisonshipCount = Math.Max(this.statistics.MaxLiaisonshipCount, this.zones.Memberships[v].Liaisonship.Count);
                    this.statistics.SumCoLiaisonshipCount += this.zones.Memberships[v].CoLiaisonship.Count;
                    this.statistics.MaxCoLiaisonshipCount = Math.Max(this.statistics.MaxCoLiaisonshipCount, this.zones.Memberships[v].CoLiaisonship.Count);
                    this.statistics.SumOwDep += prominency.OneWayDependency;
                    this.statistics.MaxOwDep = Math.Max(this.statistics.MaxOwDep, prominency.OneWayDependency);
                    this.statistics.SumOwIndep += prominency.OneWayIndependency;
                    this.statistics.MaxOwIndep = Math.Max(this.statistics.MaxOwIndep, prominency.OneWayIndependency);
                    this.statistics.SumTwDep += prominency.TwoWayDependency;
                    this.statistics.MaxTwDep = Math.Max(this.statistics.MaxTwDep, prominency.TwoWayDependency);
                    this.statistics.SumTwIndep += prominency.TwoWayIndependency;
                    this.statistics.MaxTwIndep = Math.Max(this.statistics.MaxTwIndep, prominency.TwoWayIndependency);
                    /////////////////////////////////////////////

                    string cc = string.Empty;
                    string depDegree = string.Empty;
                    //cc = v.GetClusteringCoefficient().ToString();
                    if (!this.LargeScaleCheckBox.Checked)
                    {
                        cc = v.GetClusteringCoefficient().ToString();
                        //depDegree = v.GetDependencyDegree().ToString();
                    }
                    sb.Append(v.Id + sep + v.Name + sep + v.AdjacentsCount + sep + depDegree + sep + cc + sep);
                    sb.Append(prominency.OneWayDependency + sep + prominency.OneWayIndependency + sep);
                    sb.Append(prominency.TwoWayDependency + sep + prominency.TwoWayIndependency + sep);
                    sb.Append(prominency.GetValue() + sep);
                    sb.Append(zone.Id + sep + zone.AllVertices.Count + sep);
                    sb.Append(this.getCategoryText(zone) + sep + Network.GetDensity(zone.AllVertices) + sep);
                    sb.Append(this.zones.Memberships[v].Membership.Count + sep);
                    sb.Append(this.zones.Memberships[v].InnerMembership.Count + sep);
                    sb.Append(this.zones.Memberships[v].Liaisonship.Count + sep);
                    sb.Append(this.zones.Memberships[v].CoLiaisonship.Count + sep);

                    string super = string.Empty;
                    string sub = string.Empty;
                    string overlap = string.Empty;
                    if (this.zones.Relationships.Count > 0)
                    {
                        int sum = this.zones.Relationships[zone].SuperZones.Values.Count;
                        super = sum.ToString();

                        sum = 0;
                        foreach (DependencyZone z in this.zones.Relationships[zone].SubZones.Values)
                        {
                            if (z.AllVertices.Contains(v))
                            {
                                sum += 1;
                            }
                        }
                        sub = sum.ToString();

                        sum = 0;
                        foreach (DependencyZone z in this.zones.Relationships[zone].OverlappingZones.Values)
                        {
                            if (z.AllVertices.Contains(v))
                            {
                                sum += 1;
                            }
                        }
                        overlap = sum.ToString();
                    }

                    sb.Append(super + sep);
                    sb.Append(sub + sep);
                    sb.Append(overlap + sep);
                    sb.Append(this.zones.GetZone(v).Modularity.ZoneValue);

                    sw.WriteLine(sb.ToString());

                    sb.Clear();
                }

            }

            sw.Close();
        }

        //private double GetHarmonicMean(double x, double y)
        //{
        //    if (x * y == 0)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return 2 / (1 / x + 1 / y);
        //    }
        //}

        private string getCorrectFilename(string filename)
        {
            char[] chars = { '/', '\\', '*', ':', '?', '#', '<', '>',};
            string output = filename;
            foreach (char ch in chars)
            {
                output = output.Replace(ch, '_');
            }

            return output;
        }

        private void saveGdfGroup(IDependencyGroup group, string filename)
        {
            double GDF_MIN = 1e-45;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);

            sw.WriteLine("nodedef>name VARCHAR,label VARCHAR,dep-degree DOUBLE,color VARCHAR");
            HashSet<Vertex> neighborhood = new HashSet<Vertex>(this.network.Vertices.Values);
            //HashSet<Vertex> neighborhood = this.network.GetConnectedComponent(zone.Ego);
            //HashSet<Vertex> neighborhood = new HashSet<Vertex>(zone.AllVertices);
            //foreach (Vertex v in zone.AllVertices)
            //{
            //    foreach (Vertex adj in v.AdjacentVertices)
            //    {
            //        neighborhood.Add(adj);
            //    }
            //}
            foreach (Vertex v in neighborhood)
            {
                string color = GREEN;
                if (group.AllVertices.Contains(v))
                {
                    if (group.Egos.Contains(v))
                    {
                        color = RED; ;
                    }
                    else if (group.Liaisons.Contains(v))
                    {
                        color = BLUE;
                    }
                    else if (group.CoLiaisons.Contains(v))
                    {
                        color = CYAN;
                    }
                    else
                    {
                        color = YELLOW;
                    }
                }
                string name = v.Name;
                double size = v.AdjacentsCount;
                if (this.DepEdgeCheckBox.Checked)
                {
                    size = v.GetDependencyDegree();
                }

                Vertex.Prominency otwDep = v.GetProminency();
                double oneWayDependency = 1 + otwDep.OneWayDependency;
                double oneWayIndependency = 1 + otwDep.OneWayIndependency;
                double twoWayDependency = 1 + otwDep.TwoWayDependency;
                double twoWayIndependency = 1 + otwDep.TwoWayIndependency;

                sw.WriteLine("{0},{1},{2},{3}", v.Id, name, Convert.ToString(size).Replace(",", "."), color);
            }

            sw.WriteLine("edgedef>node1 VARCHAR,node2 VARCHAR,weight DOUBLE,label VARCHAR,directed BOOLEAN");
            foreach (Edge e in this.network.Edges)
            {
                if (neighborhood.Contains(e.VertexA) && neighborhood.Contains(e.VertexB))
                {
                    double weight = e.Weight;

                    if (this.DepEdgeCheckBox.Checked)
                    {
                        double wA = e.VertexA.GetDependencyOn(e.VertexB);
                        double wB = e.VertexB.GetDependencyOn(e.VertexA);
                        weight = Math.Sqrt(wA * wB);
                    }

                    if (weight < GDF_MIN)
                    {
                        weight = GDF_MIN;
                    }
                    sw.WriteLine("{0},{1},{2},{3},{4}", e.VertexA.Id, e.VertexB.Id,
                        Convert.ToString(weight).Replace(",", "."), Convert.ToString(weight).Replace(",", "."),
                        Convert.ToString(this.network.IsDirected));
                }
            }
            sw.Close();
        }

        private enum DependencyEdgeWeight
        {
            Unweighted, ArithmeticMean, GeometricMean, QuadraticMean, Maximum, Minimum
        }
        private void saveGdfDependencyNetwork(string filename, DependencyEdgeWeight weightType, bool degree)
        {
            double GDF_MIN = 1e-45;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);
            System.IO.StreamWriter swEdges = new System.IO.StreamWriter(filename.Replace("gdf", "csv"));

            sw.WriteLine("nodedef>name VARCHAR,label VARCHAR,dep-degree DOUBLE,prominency DOUBLE,color VARCHAR");
            foreach (Vertex v in this.network.Vertices.Values)
            {
                string color = GREEN;
                if (v.GetIndependency() == 1)
                {
                    color = RED;
                }
                string name = v.Name;

                double size;
                if (degree)
                {
                    size = v.AdjacentsCount;
                }
                else
                {
                    size = v.GetDependencyDegree();
                }

                Vertex.Prominency otwDep = v.GetProminency();
                double prominency = Math.Max(0, otwDep.GetValue());

                sw.WriteLine("{0},{1},{2},{3},{4}", v.Id, name, Convert.ToString(size).Replace(",", "."),
                    Convert.ToString(prominency).Replace(",", "."), color);
            }

            sw.WriteLine("edgedef>node1 VARCHAR,node2 VARCHAR,weight DOUBLE,label VARCHAR,directed BOOLEAN");
            foreach (Edge e in this.network.Edges)
            {
                double wA = e.VertexA.GetDependencyOn(e.VertexB);
                double wB = e.VertexB.GetDependencyOn(e.VertexA);
                double weight;
                if (weightType == DependencyEdgeWeight.ArithmeticMean)
                {
                    weight = (wA + wB) / 2;
                }
                else if (weightType == DependencyEdgeWeight.GeometricMean)
                {
                    weight = Math.Sqrt(wA * wB);
                }
                else if (weightType == DependencyEdgeWeight.QuadraticMean)
                {
                    weight = Math.Sqrt((wA * wA + wB * wB) / 2);
                }
                else if (weightType == DependencyEdgeWeight.Maximum)
                {
                    weight = Math.Max(wA, wB);
                }
                else if (weightType == DependencyEdgeWeight.Minimum)
                {
                    weight = Math.Max(wA, wB);
                }
                else
                {
                    weight = 1;
                }

                if (weight < GDF_MIN)
                {
                    weight = GDF_MIN;
                }
                sw.WriteLine("{0},{1},{2},{3},{4}", e.VertexA.Id, e.VertexB.Id,
                    Convert.ToString(weight).Replace(",", "."), Convert.ToString(weight).Replace(",", "."),
                    Convert.ToString(this.network.IsDirected));
                swEdges.WriteLine("{0};{1};{2}", e.VertexA.Id, e.VertexB.Id, Convert.ToString(weight));
            }
            sw.Close();
            swEdges.Close();
        }

        private void saveGdfDirectedDependencyNetwork(string filename)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);

            sw.WriteLine("nodedef>name VARCHAR,label VARCHAR,dep-degree DOUBLE,prominency DOUBLE,color VARCHAR");
            foreach (Vertex v in this.network.Vertices.Values)
            {
                string color = GREEN;
                if (v.GetIndependency() == 1)
                {
                    color = RED;
                }

                string name = v.Name;
                double size = v.GetDirectedDependencyDegree();

                Vertex.Prominency otwDep = v.GetProminency();
                double prominency = Math.Max(0, otwDep.GetValue());

                sw.WriteLine("{0},{1},{2},{3},{4}", v.Id, name, Convert.ToString(size).Replace(",", "."),
                    Convert.ToString(prominency).Replace(",", "."), color);
            }

            sw.WriteLine("edgedef>node1 VARCHAR,node2 VARCHAR,weight DOUBLE,label VARCHAR,directed BOOLEAN");
            foreach (Edge e in this.network.Edges)
            {
                double wA = e.VertexA.GetDependencyOn(e.VertexB) + 10e-12;
                double wB = e.VertexB.GetDependencyOn(e.VertexA) + 10e-12;

                int idFrom = e.VertexA.Id;
                int idTo = e.VertexB.Id;

                if (wA >= this.network.DependencyThreshold || wB >= this.network.DependencyThreshold)
                {
                    if (wA >= this.network.DependencyThreshold)
                    {
                        sw.WriteLine("{0},{1},{2},{3},{4}", idFrom, idTo,
                        Convert.ToString(1).Replace(",", "."), Convert.ToString(wA).Replace(",", "."),
                        Convert.ToString(true));
                    }
                    if (wB >= this.network.DependencyThreshold)
                    {
                        sw.WriteLine("{0},{1},{2},{3},{4}", idTo, idFrom,
                            Convert.ToString(1).Replace(",", "."), Convert.ToString(wB).Replace(",", "."),
                            Convert.ToString(true));
                    }
                }
                //else //doplni i nezavisle hrany
                //{
                //    sw.WriteLine("{0},{1},{2},{3},{4}", idTo, idFrom,
                //        Convert.ToString(1).Replace(",", "."), Convert.ToString(Math.Max(wA,wB)).Replace(",", "."),
                //        Convert.ToString(false));
                //}
            }
            sw.Close();
        }

        private void saveGdfOverlapsNetwork(string filename)
        {
            double GDF_MIN = 1e-45;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);

            sw.WriteLine("nodedef>name VARCHAR,label VARCHAR,dep-degree DOUBLE,prominency DOUBLE,color VARCHAR");
            foreach (Vertex v in this.network.Vertices.Values)
            {
                double prominencyValue = v.GetProminency().GetValue();

                string color = GREEN;
                if (prominencyValue == 1)
                {
                    color = RED;
                }
                else if (prominencyValue > 0)
                {
                    color = YELLOW;
                }

                string name = v.Name;
                double size = v.GetDependencyDegree();

                Vertex.Prominency otwDep = v.GetProminency();
                double prominency = Math.Max(0, otwDep.GetValue());

                sw.WriteLine("{0},{1},{2},{3},{4}", v.Id, name, Convert.ToString(size).Replace(",", "."),
                    Convert.ToString(prominency).Replace(",", "."), color);
            }

            sw.WriteLine("edgedef>node1 VARCHAR,node2 VARCHAR,weight DOUBLE,label VARCHAR,directed BOOLEAN");
            foreach (Edge e in this.network.Edges)
            {
                double wA = e.VertexA.GetDependencyOn(e.VertexB);
                double wB = e.VertexB.GetDependencyOn(e.VertexA);
                double weight = Math.Sqrt(wA * wB);

                if (weight < GDF_MIN)
                {
                    weight = GDF_MIN;
                }
                sw.WriteLine("{0},{1},{2},{3},{4}", e.VertexA.Id, e.VertexB.Id,
                    Convert.ToString(weight).Replace(",", "."), Convert.ToString(weight).Replace(",", "."),
                    Convert.ToString(this.network.IsDirected));
            }
            sw.Close();
        }

        private void saveGdfZonesHierarchy(string filename)
        {
            //double GDF_MIN = 1e-45;

            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);
            sw.WriteLine("nodedef>name VARCHAR,label VARCHAR,count INTEGER,color VARCHAR");

            foreach (DependencyZone zone in this.zones.AllZones)
            {
                DependencyZoneRelationships r = this.zones.Relationships[zone];

                ZoneCategory category = r.GetCategory();
                string color = string.Empty;
                if (category == ZoneCategory.TopZone)
                {
                    color = RED;
                }
                else if (category == ZoneCategory.BottomZone)
                {
                    color = GREEN;
                }
                else if (category == ZoneCategory.InnerZone)
                {
                    color = YELLOW;
                }
                else if (category == ZoneCategory.OutlyingZone)
                {
                    color = MAGENTA;
                }
                else if (category == ZoneCategory.IndependentZone)
                {
                    color = CYAN;
                }
                else if (category == ZoneCategory.OverlappingZone) //zatim nenastalo
                {
                    color = BLUE;
                }

                string name = r.Zone.Ego.Name;
                if (ZONE_DUPLICITY_FILTER == DependencyZone.DuplicityFilter.MultiEgo)
                {
                    int N = r.Zone.Egos.Count;
                    if (N > 1)
                    {
                        name += "+" + (N - 1);
                    }
;
                    if (r.AlternativeZones.Count > 0)
                    {
                        name += " ALT";
                    }
                }

                sw.WriteLine("{0},{1},{2},{3}", zone.Id, name, r.Zone.AllVertices.Count, color);
                //sw.WriteLine("{0},{1},{2},{3}", zone.Id, name, r.Zone.InnerZone.Count, color);
            }

            sw.WriteLine("edgedef>node1 VARCHAR,node2 VARCHAR,weight DOUBLE,label VARCHAR,directed BOOLEAN");
            HashSet<string> processedRelationships = new HashSet<string>();

            foreach (KeyValuePair<DependencyZone, DependencyZoneRelationships> relationshipKVPair in this.zones.Relationships)
            {
                DependencyZoneRelationships relationship = relationshipKVPair.Value;
                DependencyZone relZone = relationship.Zone;
                Dictionary<int, DependencyZone> nearestSuperZones = relationship.GetNearestSuperZones(this.zones.Relationships);

                foreach (DependencyZone superZone in nearestSuperZones.Values)
                {
                    double weight = relationship.Zone.AllVertices.Count;
                    //double weight = (double)relZone.InnerZone.Count / relZone.AllVertices.Count;
                    //weight = Math.Max(weight, GDF_MIN);
                    sw.WriteLine("{0},{1},{2},{3},{4}", relZone.Id, superZone.Id,
                        Convert.ToString(weight).Replace(",", "."), Convert.ToString(weight).Replace(",", "."), true);
                }

                foreach (DependencyZone alternativeZone in relationship.AlternativeZones.Values)
                {
                    double weight = relationship.Zone.AllVertices.Count;
                    //double weight = (double)relZone.InnerZone.Count / relZone.AllVertices.Count;
                    //weight = Math.Max(weight, GDF_MIN);
                    sw.WriteLine("{0},{1},{2},{3},{4}", relZone.Id, alternativeZone.Id,
                        Convert.ToString(weight).Replace(",", "."), Convert.ToString(weight).Replace(",", "."), true);
                }
            }

            sw.Close();
        }

        private bool isProcessed(HashSet<string> processed, int i, int j)
        {
            string key;
            if (i > j)
            {
                key = i + "_" + j;
            }
            else
            {
                key = j + "_" + i;
            }

            if (processed.Contains(key))
            {
                return true;
            }
            else
            {
                processed.Add(key);
                return false;
            }
        }

        private void GdfCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.DepEdgeCheckBox.Visible = this.GdfCheckBox.Checked;
            this.GdfMinNumericUpDown.Visible = this.GdfCheckBox.Checked;
        }

        private void ErdosRenyiModelButton_Click(object sender, EventArgs e)
        {
            ErdosRenyModel model = new ErdosRenyModel(RANDOM);
            this.processModel(model);
        }

        private void BarabasiAlbertModelButton_Click(object sender, EventArgs e)
        {
            BarabasiAlbertModel model = new BarabasiAlbertModel(RANDOM);
            this.processModel(model);
        }

        private void BianconiTriadicClosureModelButton_Click(object sender, EventArgs e)
        {
            BianconiTriadicClosureModel model = new BianconiTriadicClosureModel(RANDOM);
            this.processModel(model);
        }

        private void processModel(INetworkModel model)
        {
            char[] sep = { ' ' };

            string[] stringParameters = this.ParameterTextBox.Text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            double[] parameters = model.StringParametersToDouble(stringParameters);
            int n;
            if (!int.TryParse(this.VerticesTextBox.Text, out n))
            {
                n = 1000;
            }
            if (n < 0)
            {
                n = 1;
            }

            string filename;
            DialogResult result = this.getModelNetworkFilename(model.Name + ".csv", out filename);

            if (result == DialogResult.OK)
            {
                this.setLoadingProgress();

                this.fullNetwork = model.GenerateNetwork(n, parameters);
                this.saveFullNetwork(filename, true, true, false);

                this.VerticesTextBox.Text = n.ToString();

                StringBuilder sb = new StringBuilder();
                foreach (double p in parameters)
                {
                    sb.Append(p.ToString() + " ");
                }
                this.ParameterTextBox.Text = sb.ToString().Trim();

                this.PathTextBox.Text = filename;

                this.fileName = filename;

                this.setNetworkProgress();
            }
        }

        private DialogResult getModelNetworkFilename(string modelName, out string filename)
        {
            SaveFileDialog.FileName = modelName;
            SaveFileDialog.Filter = "CSV|*.csv";
            SaveFileDialog.Title = "Save network";
            DialogResult result = SaveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                filename = SaveFileDialog.FileName;
            }
            else
            {
                filename = string.Empty;
            }

            return result;
        }

        private void saveFullNetwork(string filename, bool weighted, bool dep, bool depDirected)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename);

            foreach (Edge edge in this.fullNetwork.Edges)
            {
                if (weighted)
                {
                    if (!dep)
                    {
                        sw.Write(edge.VertexA.Id + ";" + edge.VertexB.Id);
                        sw.Write(";" + edge.Weight);
                        sw.WriteLine();
                    }
                    else
                    {
                        if (!this.fullNetwork.IsDirected && depDirected)
                        {
                            sw.Write(edge.VertexA.Id + ";" + edge.VertexB.Id);
                            double weight = edge.VertexA.GetDependencyOn(edge.VertexB);
                            sw.Write(";" + weight);
                            sw.WriteLine();
                            sw.Write(edge.VertexB.Id + ";" + edge.VertexA.Id);
                            weight = edge.VertexB.GetDependencyOn(edge.VertexA);
                            sw.Write(";" + weight);
                            sw.WriteLine();
                        }
                        else
                        {
                            sw.Write(edge.VertexA.Id + ";" + edge.VertexB.Id);
                            double weight = Math.Sqrt(edge.VertexA.GetDependencyOn(edge.VertexB)
                                * edge.VertexB.GetDependencyOn(edge.VertexA));
                            sw.Write(";" + weight);
                            sw.WriteLine();
                        }
                    }
                }
            }

            sw.Close();
        }

        private Object coverLock = new object();
        private Object overlapLock = new object();
        private void saveCoversOverlaps()
        {
            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");

            string outputFileCovers = System.IO.Path.Combine(this.PATH, "covers " + name);
            string outputFileOverlaps = System.IO.Path.Combine(this.PATH, "overlaps " + name);

            System.IO.StreamWriter swCovers = new System.IO.StreamWriter(outputFileCovers);
            System.IO.StreamWriter swOverlaps = new System.IO.StreamWriter(outputFileOverlaps);

            string sep = ";";

            string header = "ZONE A;ZONE B;A SIZE;B SIZE;OVERLAP SIZE;OVERLAP RATIO;A DENSITY;B DENSITY;A-O DENSITY;B-O DENSITY;O DENSITY;O DENSITY RATIO;N-O DENSITY RATIO;SUB-ZONES;MAX SUB-ZONE;MAX SIZE;MAX RATIO";
            swCovers.WriteLine(header);
            swOverlaps.WriteLine(header);

            int minOverlappingZones = 2;
            if (!this.LargeScaleCheckBox.Checked)
            {
                minOverlappingZones = MIN_OVERLAPPING_ZONES;
            }
            else 
            {
                minOverlappingZones = MIN_INTRAOVERLAP; //(int)Math.Round(Math.Log(this.network.Vertices.Count));
            }

            ZonesProgressBar.PerformStep();

            int covers = 0;
            int overlaps = 0;
            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(this.zones.AllZones, options, zone =>
            {
                DependencyZoneRelationships relationship = this.zones.Relationships[zone];
               Dictionary<int, DependencyZone> subZones = relationship.SubZones;
               foreach (DependencyZone subZone in subZones.Values)
               {
                   if (zone.Id < subZone.Id &&
                       zone.AllVertices.Count >= minOverlappingZones && subZone.AllVertices.Count >= minOverlappingZones)
                   {
                       DependencyZone.ZonesOverlap overlap = new DependencyZone.ZonesOverlap(zone, subZone);
                       if (overlap.Type == DependencyZone.ZonesOverlapType.Cover)
                       {
                           lock (coverLock)
                           {
                               covers += 1;
                           }
                       }
                   }
               }
               Dictionary<int, DependencyZone> overlappingZones = relationship.OverlappingZones;
               foreach (DependencyZone overlappingZone in overlappingZones.Values)
               {
                   if (zone.Id < overlappingZone.Id &&
                       zone.AllVertices.Count >= minOverlappingZones && overlappingZone.AllVertices.Count >= minOverlappingZones)
                   {
                       DependencyZone.ZonesOverlap overlap = new DependencyZone.ZonesOverlap(zone, overlappingZone);
                       if (overlap.Type == DependencyZone.ZonesOverlapType.Overlap)
                       {
                           lock (overlapLock)
                           {
                               overlaps += 1;

                                /////////////////////////////////////////////
                                this.statistics.OverlapsCount += 1;
                                this.statistics.SumOverlapSize += overlap.Overlap.Count;
                                this.statistics.MaxOverlapSize = Math.Max(this.statistics.MaxOverlapSize, overlap.Overlap.Count);
                                /////////////////////////////////////////////
                            }
                        }
                   }
               }
           });

            ZonesProgressBar.PerformStep();

            StringBuilder[] coverArray = new StringBuilder[covers];
            StringBuilder[] overlapArray = new StringBuilder[overlaps];
            covers = 0;
            overlaps = 0;

            options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(this.zones.AllZones, options, zone =>
            {
                DependencyZoneRelationships relationship = this.zones.Relationships[zone];
                Dictionary<int, DependencyZone> subZones = relationship.SubZones;
                foreach (DependencyZone subZone in subZones.Values)
                {
                    if (zone.Id < subZone.Id &&
                        zone.AllVertices.Count >= minOverlappingZones && subZone.AllVertices.Count >= minOverlappingZones)
                    {
                        DependencyZone.ZonesOverlap overlap = new DependencyZone.ZonesOverlap(zone, subZone);
                        if (overlap.Type == DependencyZone.ZonesOverlapType.Cover)
                        {
                            StringBuilder sbCover = new StringBuilder();
                            sbCover.Append(zone.Id + sep + subZone.Id);
                            sbCover.Append(sep + zone.AllVertices.Count + sep + subZone.AllVertices.Count + sep + overlap.Overlap.Count);
                            sbCover.Append(sep + overlap.GetOverlapRatio());
                            sbCover.Append(sep + overlap.ZoneADensity + sep + overlap.ZoneBDensity);
                            sbCover.Append(sep + overlap.ZoneAExceptOverlapDensity + sep + overlap.ZoneBExceptOverlapDensity + sep + overlap.OverlapDensity);

                            string densityRatio = overlap.GetDensityRatio(false).ToString();
                            if (densityRatio == "-1")
                            {
                                densityRatio = "NA";
                            }
                            string densityRatioSkipOverlap = overlap.GetDensityRatio(true).ToString();
                            if (densityRatioSkipOverlap == "-1")
                            {
                                densityRatioSkipOverlap = "NA";
                            }
                            sbCover.Append(sep + densityRatio + sep + densityRatioSkipOverlap);

                            HashSet<DependencyZone> oSubZones = new HashSet<DependencyZone>();
                            DependencyZone maxZone = null;
                            foreach (DependencyZone oSubZone in this.zones.Relationships[subZone].SubZones.Values)
                            {
                                if (overlap.Overlap.Intersect(oSubZone.AllVertices).Count() == oSubZone.AllVertices.Count)
                                {
                                    oSubZones.Add(oSubZone);
                                    if (maxZone == null || oSubZone.AllVertices.Count > maxZone.AllVertices.Count)
                                    {
                                        maxZone = oSubZone;
                                    }
                                }
                            }

                            if (maxZone != null)
                            {
                                sbCover.Append(sep + oSubZones.Count + sep + maxZone.Id + sep + maxZone.AllVertices.Count);

                                double ratio = (double)maxZone.AllVertices.Count / subZone.AllVertices.Count;
                                sbCover.Append(sep + ratio);
                            }
                            else
                            {
                                sbCover.Append(sep + oSubZones.Count);
                            }

                            lock (coverLock)
                            {
                                coverArray[covers] = sbCover;
                                covers += 1;
                            }
                        }
                    }

                }

                Dictionary<int, DependencyZone> overlappingZones = relationship.OverlappingZones;
                foreach (DependencyZone overlappingZone in overlappingZones.Values)
                {
                    if (zone.Id < overlappingZone.Id &&
                        zone.AllVertices.Count >= minOverlappingZones && overlappingZone.AllVertices.Count >= minOverlappingZones)
                    {
                        DependencyZone.ZonesOverlap overlap = new DependencyZone.ZonesOverlap(zone, overlappingZone);
                        if (overlap.Type == DependencyZone.ZonesOverlapType.Overlap)
                        {
                            StringBuilder sbOverlap = new StringBuilder();
                            sbOverlap.Append(zone.Id + sep + overlappingZone.Id);
                            sbOverlap.Append(sep + zone.AllVertices.Count + sep + overlappingZone.AllVertices.Count + sep + overlap.Overlap.Count);
                            sbOverlap.Append(sep + overlap.GetOverlapRatio());
                            sbOverlap.Append(sep + overlap.ZoneADensity + sep + overlap.ZoneBDensity);
                            sbOverlap.Append(sep + overlap.ZoneAExceptOverlapDensity + sep + overlap.ZoneBExceptOverlapDensity + sep + overlap.OverlapDensity);

                            string densityRatio = overlap.GetDensityRatio(false).ToString();
                            if (densityRatio == "-1")
                            {
                                densityRatio = "NA";
                            }
                            string densityRatioSkipOverlap = overlap.GetDensityRatio(true).ToString();
                            if (densityRatioSkipOverlap == "-1")
                            {
                                densityRatioSkipOverlap = "NA";
                            }
                            sbOverlap.Append(sep + densityRatio + sep + densityRatioSkipOverlap);

                            HashSet<DependencyZone> oSubZones = new HashSet<DependencyZone>();
                            DependencyZone maxZone = null;
                            foreach (DependencyZone oSubZone in this.zones.Relationships[overlappingZone].SubZones.Values)
                            {
                                if (overlap.Overlap.Intersect(oSubZone.AllVertices).Count() == oSubZone.AllVertices.Count)
                                {
                                    oSubZones.Add(oSubZone);
                                    if (maxZone == null || oSubZone.AllVertices.Count > maxZone.AllVertices.Count)
                                    {
                                        maxZone = oSubZone;
                                    }
                                }
                            }

                            if (maxZone != null)
                            {
                                sbOverlap.Append(sep + oSubZones.Count + sep + maxZone.Id + sep + maxZone.AllVertices.Count);

                                int min = Math.Min(zone.AllVertices.Count, overlappingZone.AllVertices.Count);
                                double ratio = (double)maxZone.AllVertices.Count / min;
                                sbOverlap.Append(sep + ratio);
                            }
                            else
                            {
                                sbOverlap.Append(sep + oSubZones.Count);
                            }

                            lock (overlapLock)
                            {
                                overlapArray[overlaps] = sbOverlap;
                                overlaps += 1;

                                /////////////////////////////////////////////
                                if (maxZone != null)
                                {
                                    this.statistics.ZoneInOverlapsCount += 1;
                                    this.statistics.SumZoneInOverlapsSize += maxZone.AllVertices.Count;
                                    this.statistics.MaxZoneInOverlapsSize = Math.Max(this.statistics.MaxZoneInOverlapsSize, maxZone.AllVertices.Count);
                                }
                                /////////////////////////////////////////////
                            }
                        }
                    }
                }
            });

            ZonesProgressBar.PerformStep();

            this.Covers = 0;
            this.Overlaps = 0;
            foreach (StringBuilder sbc in coverArray)
            {
                this.Covers += 1;
                swCovers.WriteLine(sbc.ToString());
            }

            foreach (StringBuilder sbo in overlapArray)
            {
                this.Overlaps += 1;
                swOverlaps.WriteLine(sbo.ToString());
            }




            ///////////////////////////////////////////////////////
            //foreach (DependentZone zone in this.zones.AllZones)
            //{
            //    ZonesProgressBar.PerformStep();

            //    DependentZoneRelationships relationship = this.zones.Relationships[zone];
            //    Dictionary<int, DependentZone> subZones = relationship.SubZones;
            //    foreach (DependentZone subZone in subZones.Values)
            //    {
            //        if (zone.Id < subZone.Id &&
            //            zone.AllVertices.Count >= minOverlappingZones && subZone.AllVertices.Count >= minOverlappingZones)
            //        {
            //            DependentZone.ZonesOverlap overlap = new DependentZone.ZonesOverlap(zone, subZone);
            //            if (overlap.Type == DependentZone.ZonesOverlapType.Cover)
            //            {
            //                swCovers.Write(zone.Id + sep + subZone.Id);
            //                swCovers.Write(sep + zone.AllVertices.Count + sep + subZone.AllVertices.Count + sep + overlap.Overlap.Count);
            //                swCovers.Write(sep + overlap.GetOverlapRatio());
            //                swCovers.Write(sep + overlap.ZoneADensity + sep + overlap.ZoneBDensity);
            //                swCovers.Write(sep + overlap.ZoneAExceptOverlapDensity + sep + overlap.ZoneBExceptOverlapDensity + sep + overlap.OverlapDensity);

            //                string densityRatio = overlap.GetDensityRatio(false).ToString();
            //                if (densityRatio == "-1")
            //                {
            //                    densityRatio = "NA";
            //                }
            //                string densityRatioSkipOverlap = overlap.GetDensityRatio(true).ToString();
            //                if (densityRatioSkipOverlap == "-1")
            //                {
            //                    densityRatioSkipOverlap = "NA";
            //                }
            //                swCovers.Write(sep + densityRatio + sep + densityRatioSkipOverlap);

            //                HashSet<DependentZone> oSubZones = new HashSet<DependentZone>();
            //                DependentZone maxZone = null;
            //                foreach (DependentZone oSubZone in this.zones.Relationships[subZone].SubZones.Values)
            //                {
            //                    if (overlap.Overlap.Intersect(oSubZone.AllVertices).Count() == oSubZone.AllVertices.Count)
            //                    {
            //                        oSubZones.Add(oSubZone);
            //                        if (maxZone == null || oSubZone.AllVertices.Count > maxZone.AllVertices.Count)
            //                        {
            //                            maxZone = oSubZone;
            //                        }
            //                    }
            //                }

            //                if (maxZone != null)
            //                {
            //                    swCovers.Write(sep + oSubZones.Count + sep + maxZone.Id + sep + maxZone.AllVertices.Count);

            //                    double ratio = (double)maxZone.AllVertices.Count / subZone.AllVertices.Count;
            //                    swCovers.WriteLine(sep + ratio);
            //                }
            //                else
            //                {
            //                    swCovers.WriteLine(sep + oSubZones.Count);
            //                }
            //            }
            //        }
            //    }

            //    Dictionary<int, DependentZone> overlappingZones = relationship.OverlappingZones;
            //    foreach (DependentZone overlappingZone in overlappingZones.Values)
            //    {
            //        if (zone.Id < overlappingZone.Id &&
            //            zone.AllVertices.Count >= minOverlappingZones && overlappingZone.AllVertices.Count >= minOverlappingZones)
            //        {
            //            DependentZone.ZonesOverlap overlap = new DependentZone.ZonesOverlap(zone, overlappingZone);
            //            if (overlap.Type == DependentZone.ZonesOverlapType.Overlap)
            //            {
            //                swOverlaps.Write(zone.Id + sep + overlappingZone.Id);
            //                swOverlaps.Write(sep + zone.AllVertices.Count + sep + overlappingZone.AllVertices.Count + sep + overlap.Overlap.Count);
            //                swOverlaps.Write(sep + overlap.GetOverlapRatio());
            //                swOverlaps.Write(sep + overlap.ZoneADensity + sep + overlap.ZoneBDensity);
            //                swOverlaps.Write(sep + overlap.ZoneAExceptOverlapDensity + sep + overlap.ZoneBExceptOverlapDensity + sep + overlap.OverlapDensity);

            //                string densityRatio = overlap.GetDensityRatio(false).ToString();
            //                if (densityRatio == "-1")
            //                {
            //                    densityRatio = "NA";
            //                }
            //                string densityRatioSkipOverlap = overlap.GetDensityRatio(true).ToString();
            //                if (densityRatioSkipOverlap == "-1")
            //                {
            //                    densityRatioSkipOverlap = "NA";
            //                }
            //                swOverlaps.Write(sep + densityRatio + sep + densityRatioSkipOverlap);

            //                HashSet<DependentZone> oSubZones = new HashSet<DependentZone>();
            //                DependentZone maxZone = null;
            //                foreach (DependentZone oSubZone in this.zones.Relationships[overlappingZone].SubZones.Values)
            //                {
            //                    if (overlap.Overlap.Intersect(oSubZone.AllVertices).Count() == oSubZone.AllVertices.Count)
            //                    {
            //                        oSubZones.Add(oSubZone);
            //                        if (maxZone == null || oSubZone.AllVertices.Count > maxZone.AllVertices.Count)
            //                        {
            //                            maxZone = oSubZone;
            //                        }
            //                    }
            //                }

            //                if (maxZone != null)
            //                {
            //                    swOverlaps.Write(sep + oSubZones.Count + sep + maxZone.Id + sep + maxZone.AllVertices.Count);

            //                    int min = Math.Min(zone.AllVertices.Count, overlappingZone.AllVertices.Count);
            //                    double ratio = (double)maxZone.AllVertices.Count / min;
            //                    swOverlaps.WriteLine(sep + ratio);
            //                }
            //                else
            //                {
            //                    swOverlaps.WriteLine(sep + oSubZones.Count);
            //                }
            //            }
            //        }
            //    }
            //}

            swCovers.Close();
            swOverlaps.Close();
        }

        private void GdfOverlapButton_Click(object sender, EventArgs e)
        {
            if (this.ZxCCheckBox.Checked)
            {
                if (this.zoneToCommunities == null || this.zoneToCommunities.Communities.AllCommunities.Count == 0)
                {
                    this.ZxCCheckBox.Checked = false;

                    MessageBox.Show("Communities are not detected...");
                    return;
                }
            }

            if (this.zones == null)
            {
                MessageBox.Show("Zones are not detected...");
                return;
            }

            char[] sep = { ' ' };
            string[] values = this.OverlapTextBox.Text.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length < 2 || values.Length > 2)
            {
                MessageBox.Show("Incorrect zone IDs...");
                return;
            }
            
            int grooupAid;
            if (!int.TryParse(values[0], out grooupAid))
            {
                MessageBox.Show("Incorrect zone IDs...");
                return;
            }

            int groupBid;
            if (!this.ZxCCheckBox.Checked)
            {
                if (!int.TryParse(values[1], out groupBid))
                {
                    MessageBox.Show("Incorrect zone IDs...");
                    return;
                }
            }
            else
            {
                if (!int.TryParse(values[1], out groupBid))
                {
                    MessageBox.Show("Incorrect community IDs...");
                    return;
                }
            }

            DependencyZone zoneA = this.zones.GetZone(grooupAid);
            if (!this.ZxCCheckBox.Checked)
            {
                DependencyZone zoneB = this.zones.GetZone(groupBid);
                if (zoneA == null || zoneB == null)
                {
                    MessageBox.Show("Incorrect zone IDs...");
                    return;
                }

                this.saveGdfZoneZoneOverlapNetwork(zoneA, zoneB);
            }
            else
            {

                if (!this.zoneToCommunities.Communities.AllCommunities.ContainsKey(groupBid) || zoneA == null)
                {
                    MessageBox.Show("Incorrect zone or community IDs...");
                    return;
                }

                List<int> community = this.zoneToCommunities.Communities.AllCommunities[groupBid];
                this.saveGdfZoneCommunityOverlapNetwork(zoneA, groupBid, community);
            }

            MessageBox.Show("Done...");
        }

        private void saveGdfZoneZoneOverlapNetwork(DependencyZone zoneA, DependencyZone zoneB)
        {
            double GDF_MIN = 1e-45;

            string name = System.IO.Path.GetFileName(this.fileName).Replace(".csv", ".gdf");
            name = name.Replace(".gml", ".gdf");
            string outputFile = System.IO.Path.Combine(this.PATH, "overlap_" + zoneA.Id + "_"  + zoneB.Id + " " + name);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);

            DependencyZone.ZonesOverlap overlap = new DependencyZone.ZonesOverlap(zoneA, zoneB);
            HashSet<Vertex> component = this.network.GetConnectedComponent(zoneA.Ego);

            sw.WriteLine("nodedef>name VARCHAR,label VARCHAR,dep-degree DOUBLE,color VARCHAR");
            foreach (Vertex v in component)
            {
                name = v.Name;
                string color = GRAY;
                if (overlap.LargeZone.Egos.Contains(v))
                {
                    color = BLUE;
                    if (overlap.Overlap.Contains(v))
                    {
                        name = "+" + name;
                    }
                }
                else if (overlap.SmallZone.Egos.Contains(v))
                {
                    color = CYAN;
                    if (overlap.Overlap.Contains(v))
                    {
                        name = "+" + name;
                    }
                }
                else if (overlap.Overlap.Contains(v))
                {
                    color = RED;
                }
                else if (overlap.LargeZone.AllVertices.Contains(v))
                {
                    color = GREEN;
                }
                else if (overlap.SmallZone.AllVertices.Contains(v))
                {
                    color = YELLOW;
                }

                double size = v.GetDependencyDegree();
                sw.WriteLine("{0},{1},{2},{3}", v.Id, name, Convert.ToString(size).Replace(",", "."), color);
            }

            sw.WriteLine("edgedef>node1 VARCHAR,node2 VARCHAR,weight DOUBLE,label VARCHAR,directed BOOLEAN");
            foreach (Edge e in this.network.Edges)
            {
                if (component.Contains(e.VertexA) && component.Contains(e.VertexB))
                {
                    double wA = e.VertexA.GetDependencyOn(e.VertexB);
                    double wB = e.VertexB.GetDependencyOn(e.VertexA);
                    double weight = Math.Sqrt(wA * wB);

                    if (weight < GDF_MIN)
                    {
                        weight = GDF_MIN;
                    }
                    sw.WriteLine("{0},{1},{2},{3},{4}", e.VertexA.Id, e.VertexB.Id,
                        Convert.ToString(weight).Replace(",", "."), Convert.ToString(weight).Replace(",", "."),
                        Convert.ToString(this.network.IsDirected));
                }
            }
            sw.Close();
        }

        private void saveGdfZoneCommunityOverlapNetwork(DependencyZone zoneA, int communityId, List<int> community)
        {
            double GDF_MIN = 1e-45;

            string name = System.IO.Path.GetFileName(this.fileName).Replace(".csv", ".gdf");
            name = name.Replace(".gml", ".gdf");
            string outputFile = System.IO.Path.Combine(this.PATH, "overlap_ZxC_" + zoneA.Id + "_" + communityId + " " + name);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);

            HashSet<Vertex> com = new HashSet<Vertex>();
            foreach (int id in community)
            {
                com.Add(this.network.GetVertexById(id));
            }
            HashSet<Vertex> overlap = new HashSet<Vertex>(zoneA.AllVertices.Intersect(com));
            HashSet<Vertex> component = this.network.GetConnectedComponent(zoneA.Ego);

            sw.WriteLine("nodedef>name VARCHAR,label VARCHAR,dep-degree DOUBLE,color VARCHAR");
            foreach (Vertex v in component)
            {
                name = v.Name;
                string color = GRAY;
                if (zoneA.Egos.Contains(v))
                {
                    color = BLUE;
                    if (overlap.Contains(v))
                    {
                        name = "+" + name;
                    }
                }
                else if (overlap.Contains(v))
                {
                    color = RED;
                }
                else if (zoneA.AllVertices.Contains(v))
                {
                    color = GREEN;
                }
                else if (com.Contains(v))
                {
                    color = YELLOW;
                }

                double size = v.GetDependencyDegree();
                sw.WriteLine("{0},{1},{2},{3}", v.Id, name, Convert.ToString(size).Replace(",", "."), color);
            }

            sw.WriteLine("edgedef>node1 VARCHAR,node2 VARCHAR,weight DOUBLE,label VARCHAR,directed BOOLEAN");
            foreach (Edge e in this.network.Edges)
            {
                if (component.Contains(e.VertexA) && component.Contains(e.VertexB))
                {
                    double wA = e.VertexA.GetDependencyOn(e.VertexB);
                    double wB = e.VertexB.GetDependencyOn(e.VertexA);
                    double weight = Math.Sqrt(wA * wB);

                    if (weight < GDF_MIN)
                    {
                        weight = GDF_MIN;
                    }
                    sw.WriteLine("{0},{1},{2},{3},{4}", e.VertexA.Id, e.VertexB.Id,
                        Convert.ToString(weight).Replace(",", "."), Convert.ToString(weight).Replace(",", "."),
                        Convert.ToString(this.network.IsDirected));
                }
            }
            sw.Close();
        }

        private void saveCommunities()
        {
            if (this.zoneToCommunities.Communities.AllCommunities.Count == 0)
            {
                return;
            }

            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");

            string outputFile = System.IO.Path.Combine(this.PATH, "communities " + name);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);

            sw.Write("COMMUNITY;SIZE;CATEGORY;ZONE;SIZE;OVERLAP;FIT;COMM_DEP;ZONE_DEP;");
            sw.WriteLine(this.zoneToCommunities.Communities.Modularity);

            string sep = ";";

            /////////////////////////////////////////////
            this.statistics.Modularity = this.zoneToCommunities.Communities.Modularity;
            /////////////////////////////////////////////

            foreach (KeyValuePair<int, List<int>> community in this.zoneToCommunities.Communities.AllCommunities)
            {
                ZoneToCommunities.ZoneFitResult fitMax = new ZoneToCommunities.ZoneFitResult(null);
                foreach (ZoneToCommunities.ZoneFitResult fit in this.ZoneFitResults.Values)
                {
                    if (community.Key == fit.CommunityId && fit.Value > fitMax.Value)
                    {
                        fitMax = fit;
                    }
                }
                
                if (fitMax.Group != null && fitMax.Group.AllVertices.Count >= MIN_OVERLAPPING_ZONES) // k nekterym komunitam nemusi existovat zony (zony lepe sedi s jinymi komunitami)
                {
                    sw.Write(community.Key + sep + community.Value.Count);

                    sw.Write(sep + this.getCategoryText(fitMax.Group) + sep + fitMax.Group.Id + sep +
                                        fitMax.Group.AllVertices.Count + sep + fitMax.OverlapCount + sep + fitMax.Value);

                    /////////////////////////////////////////////
                    List<Vertex> vertices = new List<Vertex>();
                    foreach (int id in community.Value)
                    {
                        vertices.Add(this.network.GetVertexById(id));
                    }
                    this.statistics.SumCommunityEmbeddedness += Network.GetEmbeddedness(vertices);
                    this.statistics.CommunitiesCount += 1;
                    this.statistics.SumCommunitySize += community.Value.Count;
                    this.statistics.MaxCommunitySize = Math.Max(this.statistics.MaxCommunitySize, community.Value.Count);
                    this.statistics.SumCommZoneSize += fitMax.Group.AllVertices.Count;
                    this.statistics.MaxCommZoneSize = Math.Max(this.statistics.MaxCommZoneSize, fitMax.Group.AllVertices.Count);
                    this.statistics.SumCommZoneEmbeddedness += Network.GetEmbeddedness(fitMax.Group.AllVertices);
                    this.statistics.SumFit += fitMax.Value;
                    this.statistics.MaxFit = Math.Max(this.statistics.MaxFit, fitMax.Value);
                    this.statistics.SumCommZoneOverlapSize += fitMax.OverlapCount;
                    /////////////////////////////////////////////

                    sw.Write(sep + Network.GetGroupDependency(vertices).DependencyScore);
                    sw.Write(sep + fitMax.Group.Dependency.DependencyScore);

                    if (!this.LargeScaleCheckBox.Checked)
                    {
                        foreach (int id in community.Value)
                        {
                            sw.Write(sep + id);
                        }
                    }

                    sw.WriteLine();
                }
            }

            sw.Close();
        }

        private void saveStatistics()
        {
            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");

            string outputFile = System.IO.Path.Combine(this.PATH, "statistics " + name);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);


            sw.WriteLine("NodesCount;EdgesCount;ZonesCount;MaxZoneSize;AvgZoneSize;MaxInnerZoneSize;AvgInnerZoneSize;MaxOuterZoneSize;AvgOuterZoneSize;TrivialZonesCount;DyadZonesCount;TriadZonesCount;" +
                "MultiEgoZonesCount;MaxMultiEgoZoneEgos;AvgMultiEgoZoneEgos;AltZonesCount;MaxSubZonesCount;AvgSubZonesCount;MaxOverlappingZonesCount;AvgOverlappingZonesCount;" +
                "AvgEmbeddedness;TopZonesCount;InnerZonesCount;BottomZonesCount;IndependentZonesCount;MaxDegree;ProminentsCount;SubProminentsCount;" +
                "MaxMembershipCount;AvgMembreshipCount;MaxLiaisonshipCount;AvgLiaisonshipCount;MaxCoLiaisonshipCount;AvgCoLiaisonshipCount;" +
                "MaxOwDep;AvgOwDep;MaxOwIndep;AvgOwIndep;MaxTwDep;AvgTwDep;MaxTwIndep;AvgTwIndep;OverlapsCount;MaxOverlapSize;AvgOverlapSize;ZoneInOverlapsCount;" +
                "MaxZoneInOverlapsSize;AvgZoneInOverlapsSize;CommunitiesCount;MaxCommunitySize;AvgCommunitySize;MaxCommZoneSize;AvgCommZoneSize;MaxFit;AvgFit;AvgCommZoneOverlapSize;" +
                "Modularity;AvgCommunityEmbeddedness;AvgCommZoneEmbeddedness");

            string sep = ";";
            sw.Write(this.statistics.NodesCount + sep);
            sw.Write(this.statistics.EdgesCount + sep);
            sw.Write(this.statistics.ZonesCount + sep);
            sw.Write(this.statistics.MaxZoneSize  + sep);
            sw.Write((double)this.statistics.SumZoneSize / this.zones.AllZones.Count + sep);
            sw.Write(this.statistics.MaxInnerZoneSize + sep);
            sw.Write((double)this.statistics.SumInnerZoneSize / this.zones.AllZones.Count + sep);
            sw.Write(this.statistics.MaxOuterZoneSize + sep);
            sw.Write((double)this.statistics.SumOuterZoneSize / this.zones.AllZones.Count + sep);
            sw.Write(this.statistics.TrivialZonesCount + sep);
            sw.Write(this.statistics.DyadZonesCount + sep);
            sw.Write(this.statistics.TriadZonesCount + sep);
            sw.Write(this.statistics.MultiEgoZonesCount + sep);
            sw.Write(this.statistics.MaxMultiEgoZoneEgos + sep);
            if ( this.statistics.MultiEgoZonesCount > 0)
            {
                sw.Write((double)this.statistics.SumMultiEgoZoneEgos / this.statistics.MultiEgoZonesCount + sep);
            }
            else
            {
                sw.Write(0 + sep);
            }
            sw.Write(this.statistics.AltZonesCount + sep);
            sw.Write(this.statistics.MaxSubZonesCount + sep);
            sw.Write((double)this.statistics.SumSubZonesCount / this.zones.AllZones.Count + sep);
            sw.Write(this.statistics.MaxOverlappingZonesCount + sep);
            sw.Write((double)this.statistics.SumOverlappingZonesCount / this.zones.AllZones.Count + sep);
            if (this.statistics.NonTrivialEmbeddednessCount > 0)
            {
                sw.Write(this.statistics.SumEmbeddedness / this.statistics.NonTrivialEmbeddednessCount + sep);
            }
            else
            {
                sw.Write(0 + sep);
            }
            sw.Write(this.statistics.TopZonesCount + sep);
            sw.Write(this.statistics.InnerZonesCount + sep);
            sw.Write(this.statistics.BottomZonesCount + sep);
            sw.Write(this.statistics.IndependentZonesCount + sep);
            sw.Write(this.statistics.MaxDegree + sep);
            sw.Write(this.statistics.ProminentsCount + sep);
            sw.Write(this.statistics.SubProminentsCount + sep);
            sw.Write(this.statistics.MaxMembershipCount + sep);
            sw.Write((double)this.statistics.SumMembreshipCount / this.network.Vertices.Count + sep);
            sw.Write(this.statistics.MaxLiaisonshipCount + sep);
            sw.Write((double)this.statistics.SumLiaisonshipCount / this.network.Vertices.Count + sep);
            sw.Write(this.statistics.MaxCoLiaisonshipCount + sep);
            sw.Write((double)this.statistics.SumCoLiaisonshipCount / this.network.Vertices.Count + sep);
            sw.Write(this.statistics.MaxOwDep + sep);
            sw.Write((double)this.statistics.SumOwDep / this.network.Vertices.Count + sep);
            sw.Write(this.statistics.MaxOwIndep + sep);
            sw.Write((double)this.statistics.SumOwIndep / this.network.Vertices.Count + sep);
            sw.Write(this.statistics.MaxTwDep + sep);
            sw.Write((double)this.statistics.SumTwDep / this.network.Vertices.Count + sep);
            sw.Write(this.statistics.MaxTwIndep + sep);
            sw.Write((double)this.statistics.SumTwIndep / this.network.Vertices.Count + sep);
            sw.Write(this.statistics.OverlapsCount + sep);
            sw.Write(this.statistics.MaxOverlapSize + sep);
            if (this.statistics.OverlapsCount > 0)
            {
                sw.Write((double)this.statistics.SumOverlapSize / this.statistics.OverlapsCount + sep);
            }
            else
            {
                sw.Write(0 + sep);
            }
            sw.Write(this.statistics.ZoneInOverlapsCount + sep);
            sw.Write(this.statistics.MaxZoneInOverlapsSize + sep);
            if (this.statistics.ZoneInOverlapsCount > 0)
            {
                sw.Write((double)this.statistics.SumZoneInOverlapsSize / this.statistics.ZoneInOverlapsCount + sep);
            }
            else
            {
                sw.Write(0 + sep);
            }
            sw.Write(this.statistics.CommunitiesCount + sep);
            sw.Write(this.statistics.MaxCommunitySize + sep);
            sw.Write((double)this.statistics.SumCommunitySize / this.statistics.CommunitiesCount + sep);
            sw.Write(this.statistics.MaxCommZoneSize + sep);
            sw.Write((double)this.statistics.SumCommZoneSize / this.statistics.CommunitiesCount + sep);
            sw.Write(this.statistics.MaxFit + sep);
            sw.Write(this.statistics.SumFit / this.statistics.CommunitiesCount + sep);
            sw.Write((double)this.statistics.SumCommZoneOverlapSize / this.statistics.CommunitiesCount + sep);
            sw.Write(this.statistics.Modularity + sep);
            sw.Write(this.statistics.SumCommunityEmbeddedness / this.statistics.CommunitiesCount + sep);
            sw.Write(this.statistics.SumCommZoneEmbeddedness / this.statistics.CommunitiesCount);

            sw.WriteLineAsync();
            sw.Close();
        }

        /////kody pro testovani a specialni ulohy
        //private void saveSB_MixedVertices()
        //{
        //    if (this.network == null)
        //    {
        //        return;
        //    }

        //    string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");

        //    string outputFile = System.IO.Path.Combine(this.PATH, "SB_MixedVertices " + name);
        //    System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);

        //    sw.WriteLine("Id;NAME;Berkeley;Stanford;SCORE;AUTHORITY");

        //    string sep = ";";

        //    foreach (Vertex v in this.network.Vertices.Values)
        //    {
        //        int S = 0;
        //        int B = 0;
        //        foreach (Vertex adj in v.GetAdjacents())
        //        {
        //            if (adj.Name.Contains(".S."))
        //            {
        //                S += 1;
        //            }
        //            else if (adj.Name.Contains(".B."))
        //            {
        //                B += 1;
        //            }
        //        }

        //        if (S * B > 0)
        //        {
        //            sw.Write(v.Id + sep + v.Name + sep + B + sep + S + sep);

        //            double prominency = v.GetProminency().GetValue();
        //            string authority = string.Empty;
        //            if (prominency == 1)
        //            {
        //                authority = "ouner";
        //            }
        //            else if (prominency > 0)
        //            {
        //                authority = "inner";
        //            }


        //            sw.WriteLine(Math.Sqrt(B * S) + sep + authority);
        //        }
        //    }

        //    sw.Close();
        //}

        private void ProgressLabel_Click(object sender, EventArgs e)
        {
            //this.saveSB_MixedVertices();
            double dep = this.fullNetwork.GetVertexById(166).GetDependencyOn(this.fullNetwork.GetVertexById(84));
        }

        private void GroundTruthTextBox_DoubleClick(object sender, EventArgs e)
        {
            this.groundTruthName = string.Empty;
            this.GroundTruthTextBox.Text = this.groundTruthName;

            this.CommunityDetectionCheckBox.Text = "Community detection (Louvain)";
            this.CommunityDetectionCheckBox.Checked = false;
        }

        private void GroundTruthButton_Click(object sender, EventArgs e)
        {
            this.OpenFileDialog.Filter = "CSV files (*.csv)|*.csv";
            if (OpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.groundTruthName = OpenFileDialog.FileName;
                GroundTruthTextBox.Text = this.groundTruthName;

                this.CommunityDetectionCheckBox.Text = "Community detection (ground-truth)";
                this.CommunityDetectionCheckBox.Checked = true;
            }


            //// test maximalni velikosti komunity
            //Communities ccc = new Communities(GROUND_TRUTH_NAME, this.fullNetwork); 

        }

        private void DefaultSettingButton_Click(object sender, EventArgs e)
        {
            this.ProminencyFilterComboBox.SelectedIndex = Convert.ToInt32(WeightedNetwork.DependencyZones.ProminencyFilter.None);
            this.DuplicityFilterComboBox.SelectedIndex = Convert.ToInt32(DependencyZone.DuplicityFilter.MultiEgo);
            this.GroupingStrategyComboBox.SelectedIndex = Convert.ToInt32(ZoneGroups.GroupingStrategy.Outer);

            this.NetDepNumericUpDown.Value = Convert.ToDecimal(0.5);
            this.GroupDepNumericUpDown.Value = Convert.ToDecimal(0);
            this.MinSizeNumericUpDown.Value = Convert.ToDecimal(1);
            this.MaxSizeNumericUpDown.Value = Convert.ToDecimal(0);
        }

        object triadLock = new object();
        private void DependencyClosureButton_Click(object sender, EventArgs e)
        {
            if (this.fullNetwork == null)
            {
                MessageBox.Show("Network does not exist...");
                return;
            }

            if (this.NetworkTypeComboBox.SelectedIndex == 0)
            {
                this.network = this.fullNetwork.GetLargestComponentNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 1)
            {
                this.network = this.fullNetwork.GetNoOutliersNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 2)
            {
                this.network = this.fullNetwork;
            }

            // vypocet matice zavislosti
            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(this.network.Edges, options, de =>
            {
                de.VertexA.IsDependentOn(de.VertexB);
                de.VertexB.IsDependentOn(de.VertexA);
            });

            // vypocet triad
            LinkedList<Triad> triadList = new LinkedList<Triad>();
            Parallel.ForEach(this.network.Vertices.Values, options, v =>
           {
               List<Vertex> adjacents = new List<Vertex>(v.AdjacentVertices);
               for (int i = 0; i < adjacents.Count - 1; i++)
               {
                   for (int j = i + 1; j < adjacents.Count; j++)
                   {
                       Triad triad = new Triad(v, adjacents[i], adjacents[j]);
                       lock (triadLock)
                       {
                           triadList.AddLast(triad);
                       }
                   }
               }
           });

            Dictionary<string, int[]> dependencyTriads = new Dictionary<string, int[]>();
            Dictionary<string, int[]> prominencyTriads = new Dictionary<string, int[]>();
            foreach (Triad triad in triadList)
            {
                if (!dependencyTriads.ContainsKey(triad.DependencyConfiguration))
                {
                    dependencyTriads.Add(triad.DependencyConfiguration, new int[2]);
                }
                if (triad.Closure)
                {
                    dependencyTriads[triad.DependencyConfiguration][0] += 1;
                }
                else
                {
                    dependencyTriads[triad.DependencyConfiguration][1] += 1;
                }

                if (!prominencyTriads.ContainsKey(triad.ProminencyConfiguration))
                {
                    prominencyTriads.Add(triad.ProminencyConfiguration, new int[2]);
                }
                if (triad.Closure)
                {
                    prominencyTriads[triad.ProminencyConfiguration][0] += 1;
                }
                else
                {
                    prominencyTriads[triad.ProminencyConfiguration][1] += 1;
                }
            }

            this.completeProminencyTriads(prominencyTriads);
            this.saveTriads(prominencyTriads, "prominencyTriads");

            string folder = System.IO.Path.GetDirectoryName(this.fileName);
            System.Diagnostics.Process.Start(folder);
        }

        private void completeDependencyTriads(Dictionary<string, int[]> dependencyTriads)
        {
            List<string> missingConfigurations = new List<string>();
            foreach (string configuration in Triad.AllDependencyConfigurations)
            {
                if (!dependencyTriads.Keys.Contains(configuration))
                {
                    missingConfigurations.Add(configuration);
                }
            }

            foreach (string configuration in missingConfigurations)
            {
                int[] values = { 0, 0 };
                dependencyTriads.Add(configuration, values);
            }
        }

        private void completeProminencyTriads(Dictionary<string, int[]> prominencyTriads)
        {
            List<string> missingConfigurations = new List<string>();
            foreach (string configuration in Triad.AllProminencyConfigurations)
            {
                if (!prominencyTriads.Keys.Contains(configuration))
                {
                    missingConfigurations.Add(configuration);
                }
            }

            foreach (string configuration in missingConfigurations)
            {
                int[] values = { 0, 0 };
                prominencyTriads.Add(configuration, values);
            }
        }

        private void saveTriads(Dictionary<string, int[]> triads, string triadName)
        {
            string folderName = System.IO.Path.GetDirectoryName(this.fileName);
            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");
            string outputFile = System.IO.Path.Combine(folderName, triadName + " " + name);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);

            int closureTrue = 0;
            int closureFalse = 0;

            sw.WriteLine("CONFIGURATION;TRUE;FALSE;FRACTION");

            foreach (KeyValuePair<string, int[]> triad in triads)
            {
                double frac = 0;
                if (triad.Value[0] + triad.Value[1] > 0)
                {
                    frac = (double)triad.Value[0] / (triad.Value[0] + triad.Value[1]);

                }
                sw.WriteLine(triad.Key + ";" + triad.Value[0] + ";" + triad.Value[1] + ";" + frac);

                closureTrue += triad.Value[0];
                closureFalse += triad.Value[1];
            }

            double x = (double)closureTrue / (closureTrue + closureFalse);
            sw.WriteLine(";;;;;TOTAL;" + closureTrue + ";" + closureFalse + ";" + (double)closureTrue / (closureTrue + closureFalse));

            sw.Close();
        }






        //   UKAZKA VOLANI   ///////////////////////////////////////////////////////////////////
        private void UkazkaButton_Click(object sender, EventArgs e)
        {
            char[] sep = { ';', '\t', ' ' };

            /////////////////////////////////////////////////////////////////////////////////////
            MessageBox.Show("Načtení sítě karate...");

            Network network = new Network(false);
            System.IO.StreamReader sr = new System.IO.StreamReader(@"..\..\..\edges karate.csv");
            string s;
            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();

                string[] line = s.Split(sep);

                int idA = Convert.ToInt32(line[0]);
                int idB = Convert.ToInt32(line[1]);
                Vertex vA;
                if (!network.Vertices.TryGetValue(idA, out vA))
                {
                    vA = new Vertex(network, idA, Convert.ToString(idA), 1);
                    network.Vertices.Add(idA, vA);
                }
                Vertex vB;
                if (!network.Vertices.TryGetValue(idB, out vB))
                {
                    vB = new Vertex(network, idB, Convert.ToString(idB), 1);
                    network.Vertices.Add(idB, vB);
                }

                double w = 1;
                if (line.Length == 3)
                {
                    w = double.Parse(line[2], invC);
                }

                network.CreateEdge(vA, vB, w);
            }

            sr.Close();
            /////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////
            MessageBox.Show("Výpočet matice závislosti...");

            network.DependencyThreshold = NETWORK_DEPENDENCY_THRESHOLD;
            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(network.Edges, options, de =>
            {
                de.VertexA.IsDependentOn(de.VertexB);
                de.VertexB.IsDependentOn(de.VertexA);
            });
            /////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////
            MessageBox.Show("Detekce zón...");

            int minSize = 1;
            int maxSize = network.Vertices.Count;
            WeightedNetwork.DependencyZones zones = new WeightedNetwork.DependencyZones(network, minSize, maxSize, true,
                DependencyZone.DuplicityFilter.All, WeightedNetwork.DependencyZones.ProminencyFilter.AllProminents, 0); // -1,..,1 Kvalitni zony jsou >= 0
            //zones.CalculateMemberships(); //Uz se nemusi volat
            /////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////
            //Vypis zon
            StringBuilder sb = new StringBuilder();
            foreach (DependencyZone zone in zones.AllZones)
            {
                foreach (Vertex v in zone.AllVertices)
                {
                    sb.Append(v.Id + " ");
                }
                sb.Append(zone.GetQuality().ToString() + "\n\r");
            }

            MessageBox.Show(sb.ToString());
            /////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////
            //Vypis skupin zon
            sb = new StringBuilder();
            bool nonZonesOnly = false; //true ignoruje skupiny, ktere obsahyji jen jednu zonu (jsou zaroven zonou)
            ZoneGroups zoneGroups = new ZoneGroups(zones, ZoneGroups.GroupingStrategy.DependentOuter, nonZonesOnly, 0); // -1,..,1 Kvalitni zony jsou >= 0

            foreach (ZoneGroup group in zoneGroups.AllGroups)
            {
                foreach (Vertex v in group.AllVertices)
                {
                    sb.Append(v.Id + " ");
                }
                sb.Append(group.GetQuality().ToString() + "\n\r");
            }

            MessageBox.Show(sb.ToString());
            /////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////
            MessageBox.Show("Načtení sítě lesmis...");

            network = new Network(false);
            sr = new System.IO.StreamReader(@"..\..\..\edges lesmis.csv");
            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();

                string[] line = s.Split(sep);

                int idA = Convert.ToInt32(line[0]);
                int idB = Convert.ToInt32(line[1]);
                Vertex vA;
                if (!network.Vertices.TryGetValue(idA, out vA))
                {
                    vA = new Vertex(network, idA, Convert.ToString(idA), 1);
                    network.Vertices.Add(idA, vA);
                }
                Vertex vB;
                if (!network.Vertices.TryGetValue(idB, out vB))
                {
                    vB = new Vertex(network, idB, Convert.ToString(idB), 1);
                    network.Vertices.Add(idB, vB);
                }

                double w = 1;
                if (line.Length == 3)
                {
                    w = double.Parse(line[2], invC);
                }

                network.CreateEdge(vA, vB, w);
            }

            sr.Close();
            /////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////
            MessageBox.Show("Vytvoření matice sosuednosti...");
            // pocet vrcholu se nemusi zadavat
            AdjacencyMatrixSample sample = new AdjacencyMatrixSample(false, new WeightSimilarityStrategy(), new LogRepresentativenessStrategy());
            foreach(Edge edge in network.Edges)
            {
                sample.AddEdge(edge.VertexA.Id, edge.VertexB.Id, edge.Weight);
            };
            /////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////
            MessageBox.Show("Vytvoření redukované sítě...");

            AdjacencyMatrixSample.LRNET_EXTENDED_SETTING = true;
            sample.CalculateRepresentativeness();
            Network reducedNetwork = sample.GetReducedNetwork(1, 0);
            /////////////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////////////
            System.IO.StreamWriter sw = new System.IO.StreamWriter(@"..\..\..\edges lesmis reduced.csv");
            foreach (Edge edge in reducedNetwork.Edges)
            {
                sw.WriteLine(edge.VertexA.Id + ";" + edge.VertexB.Id + ";" + edge.Weight);
            }

            sw.Close();

            MessageBox.Show("Saved to 'edges lesmis reduced.csv'");
            /////////////////////////////////////////////////////////////////////////////////////

        }

        private void SaveProminencyTransformationMatrix(int[,] resultMatrix, Dictionary<string, int[]> promin)
        {
            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");
            string outputFile = System.IO.Path.Combine(this.PATH, "ttMatrix " + name);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);

            string header = " ; ; ;";
            foreach(string value in Triad.AllProminencyConfigurations)
            {
                header += value + ";";
            }
            sw.WriteLine(header);

            for (int i = 0; i < resultMatrix.GetLength(0); i++)
            {
                string line = promin[Triad.AllProminencyConfigurations[i]][0] + ";" + promin[Triad.AllProminencyConfigurations[i]][1]  + ";" + Triad.AllProminencyConfigurations[i] + ";";
                for (int j = 0; j < resultMatrix.GetLength(1); j++)
                {
                    line += resultMatrix[i, j] + ";";
                }
                sw.WriteLine(line);
            }
            sw.WriteLine();
            sw.WriteLine();
            sw.WriteLine();

            for (int i = 0; i < resultMatrix.GetLength(0); i++)
            {
                string line = Triad.AllProminencyConfigurations[i] + ";";
                for (int j = 0; j < resultMatrix.GetLength(1); j++)
                {
                    double percentil = 0;
                    int triadsWithoutClosure = promin[Triad.AllProminencyConfigurations[i]][1];
                    if (triadsWithoutClosure != 0 && resultMatrix[i, j] > 0)
                    {
                        percentil = ((double)(resultMatrix[i, j]) / triadsWithoutClosure)*100;
                    }

                    line += percentil + ";";
                }
                sw.WriteLine(line);
            }

            sw.Close();
        }

        private void SaveProminencyTransformationNetwork(int[,] resultMatrix, Dictionary<string, int> list)
        {
            string name = System.IO.Path.GetFileName(this.fileName).Replace(".gml", ".csv");
            string outputFile = System.IO.Path.Combine(this.PATH, "ttNetwork " + name);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile);

            int value = 0;
            foreach (string from in Triad.AllProminencyConfigurations)
            {
                foreach (string to in Triad.AllProminencyConfigurations)
                {
                    value = resultMatrix[list[from], list[to]];
                    sw.WriteLine(from + ";" + to + ";" + value + ";");
                }
            }

            sw.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (this.fullNetwork == null)
            {
                MessageBox.Show("Network does not exist...");
                return;
            }

            if (this.NetworkTypeComboBox.SelectedIndex == 0)
            {
                this.network = this.fullNetwork.GetLargestComponentNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 1)
            {
                this.network = this.fullNetwork.GetNoOutliersNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 2)
            {
                this.network = this.fullNetwork;
            }

            // vypocet matice zavislosti
            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(this.network.Edges, options, de =>
            {
                de.VertexA.IsDependentOn(de.VertexB);
                de.VertexB.IsDependentOn(de.VertexA);
            });

            // vypocet triad
            LinkedList<Triad> triadList = new LinkedList<Triad>();
            Parallel.ForEach(this.network.Vertices.Values, options, v =>
            {
                List<Vertex> adjacents = new List<Vertex>(v.AdjacentVertices);
                for (int i = 0; i < adjacents.Count - 1; i++)
                {
                    for (int j = i + 1; j < adjacents.Count; j++)
                    {
                        Triad triad = new Triad(v, adjacents[i], adjacents[j]);
                        lock (triadLock)
                        {
                            triadList.AddLast(triad);
                        }
                    }
                }
            });

            Dictionary<string, int[]> dependencyTriads = new Dictionary<string, int[]>();
            Dictionary<string, int[]> prominencyTriads = new Dictionary<string, int[]>();
            foreach (Triad triad in triadList)
            {
                if (!dependencyTriads.ContainsKey(triad.DependencyConfiguration))
                {
                    dependencyTriads.Add(triad.DependencyConfiguration, new int[2]);
                }
                if (triad.Closure)
                {
                    dependencyTriads[triad.DependencyConfiguration][0] += 1;
                }
                else
                {
                    dependencyTriads[triad.DependencyConfiguration][1] += 1;
                }

                if (!prominencyTriads.ContainsKey(triad.ProminencyConfiguration))
                {
                    prominencyTriads.Add(triad.ProminencyConfiguration, new int[2]);
                }
                if (triad.Closure)
                {
                    prominencyTriads[triad.ProminencyConfiguration][0] += 1;
                }
                else
                {
                    prominencyTriads[triad.ProminencyConfiguration][1] += 1;
                }
            }


            Dictionary<string, int> ProminencyConfigurationIndexes = new Dictionary<string, int>();

            int ConfCounter = 0;
            foreach (string conf in Triad.AllProminencyConfigurations)
            {
                ProminencyConfigurationIndexes.Add(conf, ConfCounter);
                ConfCounter++;
            }

            int[,] resultMatrix = new int[ConfCounter, ConfCounter];

            System.IO.Directory.CreateDirectory(this.PATH + "/snapshots");
            /// for each triad
            int id = 0; 
            foreach (Triad triad in triadList)
            {
                if (triad.Closure)
                    continue;

                List<Vertex.ProminencyType> originalValues = new List<Vertex.ProminencyType>();
                originalValues.Add(triad.VertexA.GetProminencyType());
                originalValues.Add(triad.VertexB.GetProminencyType());
                originalValues.Add(triad.VertexX.GetProminencyType());

                var result = triad.ComputeTriadProminencyChange();

                if (result is null)
                {
                    triad.Clean();
                    continue;
                }

                this.saveProminencyNet(triad, result, id, originalValues);
                triad.Clean();
                id++;
            }
        }

        public void saveProminencyNet(Triad triad, string[] configurations, int id, List<Vertex.ProminencyType>  originalValues)
        {
            var triedNodesId = triad.VertexX.Id + "-" + triad.VertexA.Id + triad.VertexB.Id;
            string outputFile = System.IO.Path.Combine(this.PATH, "snapshots/" +configurations[0] + "-" + configurations[1] + "-" + triedNodesId + ".gdf");
            var sw = new System.IO.StreamWriter(outputFile);

            //sw.WriteLine("Id;Label;aa;bb");
            sw.WriteLine("nodedef>name VARCHAR, label VARCHAR, prominencyType VARCHAR,  originalProminencyType VARCHAR, color VARCHAR, x DOUBLE,y DOUBLE");
            Dictionary<int, Vertex> neighbours = GetAdjacentVertices(triad);

            foreach (KeyValuePair<int, Vertex> kvp in neighbours)
            {
                Vertex.ProminencyType vertexType = Vertex.ProminencyType.NonProminent;
                Vertex.ProminencyType originalVertexType = Vertex.ProminencyType.NonProminent;

                if (kvp.Value.Id == triad.VertexA.Id)
                {
                    vertexType = triad.VertexA.GetProminencyType();
                    originalVertexType = originalValues[0];
                }

                if (kvp.Value.Id == triad.VertexB.Id)
                {
                    vertexType = triad.VertexB.GetProminencyType();
                    originalVertexType = originalValues[1];
                }

                if (kvp.Value.Id == triad.VertexX.Id)
                {
                    vertexType = triad.VertexX.GetProminencyType();
                    originalVertexType = originalValues[2];
                }
                
                // Name 
                sw.Write(kvp.Value.Id + ",");

                if(kvp.Value.Id == triad.VertexX.Id)
                {
                    sw.Write(kvp.Value.Id + "-X-" + GetProminencyToString(originalVertexType) + "-" + GetProminencyToString(vertexType) + ",");
                    sw.Write("\"" + GetProminencyToString(vertexType) + "\",");
                    sw.Write("\"" + GetProminencyToString(originalVertexType) + "\",");
                    sw.Write("'0,204,0',");

                }
                else if (kvp.Value.Id == triad.VertexA.Id || kvp.Value.Id == triad.VertexB.Id)
                {
                    sw.Write(kvp.Value.Id + "-" + GetProminencyToString(originalVertexType) + "-" + GetProminencyToString(vertexType) + ",");
                    sw.Write("\"" + GetProminencyToString(vertexType) + "\",");
                    sw.Write("\"" + GetProminencyToString(originalVertexType) + "\",");
                    sw.Write("'0,255,0',");
                }
                else
                {
                    sw.Write(kvp.Value.Id + ",");
                    sw.Write("\"null\",");
                    sw.Write("\"null\",");
                    sw.Write("'255,215,0',");
                }
                sw.Write("0,0,");
                sw.WriteLine();
            }

            var adjancentEdges = GetAdjacentEdges(triad);

            sw.WriteLine("edgedef>node1 VARCHAR,node2 VARCHAR,weight DOUBLE,original BOOLEAN, directed BOOLEAN, dependency DOUBLE, label VARCHAR");

            foreach (Edge e in adjancentEdges)
            {
                var original = true;
                string color = GREEN;

                if(e.VertexA.Id == triad.VertexA.Id && e.VertexB.Id == triad.VertexB.Id || e.VertexA.Id == triad.VertexB.Id && e.VertexB.Id == triad.VertexA.Id)
                    original = false;

                var dep = e.VertexA.GetDependencyOn(e.VertexB);

                var dependendentOnly = true;
                if(dependendentOnly)
                {
                    if (dep >= 0.5)
                        sw.WriteLine("{0},{1},{2},{3},{4},{5},{5}", e.VertexA.Id, e.VertexB.Id, 1, original, true, dep);

                    dep = e.VertexB.GetDependencyOn(e.VertexA);
                    if (dep >= 0.5)
                        sw.WriteLine("{0},{1},{2},{3},{4},{5},{5}", e.VertexB.Id, e.VertexA.Id, 1, original, true, dep);
                }
                else
                {
                    sw.WriteLine("{0},{1},{2},{3},{4},{5},{5}", e.VertexA.Id, e.VertexB.Id, 1, original, true, dep);
                    dep = e.VertexB.GetDependencyOn(e.VertexA);
                    sw.WriteLine("{0},{1},{2},{3},{4},{5},{5}", e.VertexB.Id, e.VertexA.Id, 1, original, true, dep);
                }
            }

            sw.Close();
        }
        private string GetProminencyToString(Vertex.ProminencyType type)
        {
            if (type == Vertex.ProminencyType.NonProminent)
                return "N";
            if (type == Vertex.ProminencyType.WeaklyProminent)
                return "W";
            if (type == Vertex.ProminencyType.StronglyProminent)
                return "S";

            return "NA";
        }

        private static Dictionary<int, Vertex> GetAdjacentVertices(Triad triad)
        {
            Dictionary<int, Vertex> neighbours = new Dictionary<int, Vertex>();
            foreach (Vertex vertex in triad.VertexA.AdjacentVertices)
            {
                if (neighbours.ContainsKey(vertex.Id))
                    continue;

                neighbours.Add(vertex.Id, vertex);
            }

            foreach (Vertex vertex in triad.VertexB.AdjacentVertices)
            {
                if (neighbours.ContainsKey(vertex.Id))
                    continue;

                neighbours.Add(vertex.Id, vertex);
            }

            foreach (Vertex vertex in triad.VertexX.AdjacentVertices)
            {
                if (neighbours.ContainsKey(vertex.Id))
                    continue;

                neighbours.Add(vertex.Id, vertex);
            }

            return neighbours;
        }

        private static List<Edge> GetAdjacentEdges(Triad triad)
        {
            List<Edge> neighbours = new List<Edge>();
            foreach (Edge edge in triad.VertexA.AdjacentEdges)
            {
                neighbours.Add(edge);
            }

            foreach (Edge edge in triad.VertexB.AdjacentEdges)
            {
                neighbours.Add(edge);
            }


            foreach (Edge edge in triad.VertexX.AdjacentEdges)
            {
                neighbours.Add(edge);
            }

            return neighbours;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.fullNetwork == null)
            {
                MessageBox.Show("Network does not exist...");
                return;
            }

            if (this.NetworkTypeComboBox.SelectedIndex == 0)
            {
                this.network = this.fullNetwork.GetLargestComponentNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 1)
            {
                this.network = this.fullNetwork.GetNoOutliersNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 2)
            {
                this.network = this.fullNetwork;
            }

            // vypocet matice zavislosti
            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(this.network.Edges, options, de =>
            {
                de.VertexA.IsDependentOn(de.VertexB);
                de.VertexB.IsDependentOn(de.VertexA);
            });

            // vypocet triad
            LinkedList<Triad> triadList = new LinkedList<Triad>();
            Parallel.ForEach(this.network.Vertices.Values, options, v =>
            {
                List<Vertex> adjacents = new List<Vertex>(v.AdjacentVertices);
                for (int i = 0; i < adjacents.Count - 1; i++)
                {
                    for (int j = i + 1; j < adjacents.Count; j++)
                    {
                        Triad triad = new Triad(v, adjacents[i], adjacents[j]);
                        lock (triadLock)
                        {
                            triadList.AddLast(triad);
                        }
                    }
                }
            });

            Dictionary<string, int[]> dependencyTriads = new Dictionary<string, int[]>();
            Dictionary<string, int[]> prominencyTriads = new Dictionary<string, int[]>();

            foreach (string conf in Triad.AllProminencyConfigurations)
            {
                prominencyTriads.Add(conf, new int[2]);
            }

            foreach (Triad triad in triadList)
            {
                if (!dependencyTriads.ContainsKey(triad.DependencyConfiguration))
                {
                    dependencyTriads.Add(triad.DependencyConfiguration, new int[2]);
                }
                if (triad.Closure)
                {
                    dependencyTriads[triad.DependencyConfiguration][0] += 1;
                }
                else
                {
                    dependencyTriads[triad.DependencyConfiguration][1] += 1;
                }

                if (!prominencyTriads.ContainsKey(triad.ProminencyConfiguration))
                {
                    prominencyTriads.Add(triad.ProminencyConfiguration, new int[2]);
                }
                if (triad.Closure)
                {
                    prominencyTriads[triad.ProminencyConfiguration][0] += 1;
                }
                else
                {
                    prominencyTriads[triad.ProminencyConfiguration][1] += 1;
                }
            }


            Dictionary<string, int> ProminencyConfigurationIndexes = new Dictionary<string, int>();

            int ConfCounter = 0;
            foreach (string conf in Triad.AllProminencyConfigurations)
            {
                ProminencyConfigurationIndexes.Add(conf, ConfCounter);
                ConfCounter++;
            }

            int[,] resultMatrix = new int[ConfCounter, ConfCounter];

            int id = 0;
            /// for each triad
            foreach (Triad triad in triadList)
            {
                if (triad.Closure)
                    continue;

                var result = triad.ComputeTriadProminencyChange();

                if (result is null)
                {
                    id++;
                    triad.Clean();
                    continue;
                }

                var ConfA = ProminencyConfigurationIndexes[result[0]];
                var ConfB = ProminencyConfigurationIndexes[result[1]];
                triad.Clean();
                resultMatrix[ConfA, ConfB]++;
                id++;
            }

            var sumOfClosures = 0f;
            var sumOfChanged = 0f;

            for (int i = 0; i < resultMatrix.GetLength(0); i++)
            {
                sumOfClosures += prominencyTriads[Triad.AllProminencyConfigurations[i]][0];
                for (int j = 0; j < resultMatrix.GetLength(1); j++)
                {
                    sumOfChanged += resultMatrix[i, j];
                }
            }
            var percentile = sumOfChanged / sumOfClosures * 100;
            MessageBox.Show("Percentile is :" + percentile);

            this.SaveProminencyTransformationMatrix(resultMatrix, prominencyTriads);
            this.completeProminencyTriads(prominencyTriads);

            string folder = System.IO.Path.GetDirectoryName(this.fileName);
            System.Diagnostics.Process.Start(folder);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.fullNetwork == null)
            {
                MessageBox.Show("Network does not exist...");
                return;
            }

            if (this.NetworkTypeComboBox.SelectedIndex == 0)
            {
                this.network = this.fullNetwork.GetLargestComponentNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 1)
            {
                this.network = this.fullNetwork.GetNoOutliersNetwork();
            }
            else if (this.NetworkTypeComboBox.SelectedIndex == 2)
            {
                this.network = this.fullNetwork;
            }

            // vypocet matice zavislosti
            var options = new ParallelOptions { MaxDegreeOfParallelism = Network.MAX_DEGREE_OF_PARALLELISM };
            Parallel.ForEach(this.network.Edges, options, de =>
            {
                de.VertexA.IsDependentOn(de.VertexB);
                de.VertexB.IsDependentOn(de.VertexA);
            });

            // vypocet triad
            LinkedList<Triad> triadList = new LinkedList<Triad>();
            Parallel.ForEach(this.network.Vertices.Values, options, v =>
            {
                List<Vertex> adjacents = new List<Vertex>(v.AdjacentVertices);
                for (int i = 0; i < adjacents.Count - 1; i++)
                {
                    for (int j = i + 1; j < adjacents.Count; j++)
                    {
                        Triad triad = new Triad(v, adjacents[i], adjacents[j]);
                        lock (triadLock)
                        {
                            triadList.AddLast(triad);
                        }
                    }
                }
            });

            Dictionary<string, int[]> dependencyTriads = new Dictionary<string, int[]>();
            Dictionary<string, int[]> prominencyTriads = new Dictionary<string, int[]>();

            foreach (string conf in Triad.AllProminencyConfigurations)
            {
                prominencyTriads.Add(conf, new int[2]);
            }

            foreach (Triad triad in triadList)
            {
                if(!dependencyTriads.ContainsKey(triad.DependencyConfiguration))
                {
                    dependencyTriads.Add(triad.DependencyConfiguration, new int[2]);
                }
                if (triad.Closure)
                {
                    dependencyTriads[triad.DependencyConfiguration][0] += 1;
                }
                else
                {
                    dependencyTriads[triad.DependencyConfiguration][1] += 1;
                }

                if (triad.Closure)
                {
                    prominencyTriads[triad.ProminencyConfiguration][0] += 1;
                }
                else
                {
                    prominencyTriads[triad.ProminencyConfiguration][1] += 1;
                }
            }


            Dictionary<string, int> ProminencyConfigurationIndexes = new Dictionary<string, int>();

            int ConfCounter = 0;
            foreach (string conf in Triad.AllProminencyConfigurations)
            {
                ProminencyConfigurationIndexes.Add(conf, ConfCounter);
                ConfCounter++;
            }

            int[,] resultMatrix = new int[ConfCounter, ConfCounter];

            int id = 0;
            /// for each triad
            foreach (Triad triad in triadList)
            {
                if (!triad.Closure)
                    continue;

                var result = triad.ComputeTriadProminencyChangeOnDelete();

                if (result is null)
                {
                    id++;
                    triad.CleanAfterDelete();
                    continue;
                }

                var ConfA = ProminencyConfigurationIndexes[result[0]];
                var ConfB = ProminencyConfigurationIndexes[result[1]];
                triad.CleanAfterDelete();
                resultMatrix[ConfA, ConfB]++;
                id++;
            }

            var sumOfClosures = 0f;
            var sumOfChanged = 0f;

            for (int i = 0; i < resultMatrix.GetLength(0); i++)
            {
                sumOfClosures += prominencyTriads[Triad.AllProminencyConfigurations[i]][0];
                for (int j = 0; j < resultMatrix.GetLength(1); j++)
                {
                    sumOfChanged += resultMatrix[i, j];
                }
            }
            var percentile = sumOfChanged / sumOfClosures * 100;
            MessageBox.Show("Percentile is :" + percentile);

            this.SaveProminencyTransformationMatrix(resultMatrix, prominencyTriads);
            this.completeProminencyTriads(prominencyTriads);
        }
    }
}
