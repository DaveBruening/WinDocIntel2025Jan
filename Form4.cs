using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Data;
namespace WinDocIntel2025Jan {
    public partial class Form4 : Form {
        public Form4() {
            InitializeComponent();
        }
        AzureKeyCredential akc;
        DocumentAnalysisClient dac;
        AnalyzeResult ar;
        bool picBoxNull = true;
        string nl = Environment.NewLine;
        private void Form4_Load(object sender, EventArgs e) {
            string leKey = Environment.GetEnvironmentVariable("formRecog_key");
            string endPnt = Environment.GetEnvironmentVariable("formRecog_url");
            akc = new AzureKeyCredential(leKey);
            dac = new DocumentAnalysisClient(new Uri(endPnt), akc);
            //pictureBox1.Image = Image.FromFile("layout.png"); //SecMicrosoft.jpg works
            panel1.Controls.Add(pictureBox1);
            comboBox1.Items.Add("read.png");
            comboBox1.Items.Add("insurance-card.png");
            comboBox1.Items.Add("w2.png");
            comboBox1.Items.Add("receipt.png");
            comboBox1.Items.Add("business_card.jpg");
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            //textBox1.Text = comboBox1.SelectedItem.ToString();
            /* Image URIs and model IDs from: 
             https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/
             quickstarts/get-started-sdks-rest-api?view=doc-intel-3.0.0&preserve-view=
             true&pivots=programming-language-rest-api */
            string strModel ="";
            switch (comboBox1.SelectedItem.ToString()) {
                case "read.png": strModel= "prebuilt-read"; break;
                case "insurance-card.png": 
                    strModel= "prebuilt-healthInsuranceCard.us"; break;
                case "w2.png": strModel = "prebuilt-tax.us.w2"; break;
                case "receipt.png": strModel = "prebuilt-receipt"; break;
                case "business_card.jpg": strModel= "prebuilt-businessCard"; break;
                default: strModel = "WTF!?"; break;
            }
            string strUri = "https://raw.githubusercontent.com/Azure-Samples/" +
                "cognitive-services-REST-api-samples/master/curl/form-recognizer/"
                + "rest-api/" + comboBox1.SelectedItem.ToString();
            pictureBox1.Load(strUri);
            AnalyzeDocumentOperation ado =
                dac.AnalyzeDocumentFromUri(WaitUntil.Completed, strModel, new Uri(strUri));
            ar = ado.Value;
        }
        private void button2_Click(object sender, EventArgs e) {
            IEnumerable<DocumentWord> idw;
            textBox1.Text = "Like, the # of words, or something: " +
                $"{ar.Pages[0].Words.Count}{nl}";
            string ltr = "";
            for (int ß=26; ß>0; ß--) {
                ltr = System.Text.Encoding.ASCII.GetString(new byte[] { System.Convert.ToByte(64 + ß) });
                idw = ar.Pages[0].Words.Where(w => w.Content.ToUpper().StartsWith(ltr));
                textBox1.Text += $"# words that start with {ltr}: {idw.Count()}{nl}";
                foreach (DocumentWord dw in idw) {
                    textBox1.Text += String.Format("\t{0} - {1}{2}", dw.Content, dw.Confidence, nl);
                }
            }
            picBoxNull = false;
            pictureBox1.Refresh();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e) {
            if (!picBoxNull) {
                Pen sive = new Pen(Color.Purple);
                sive.Width = 3;
                int cnt=0;
                
                foreach (DocumentWord dw in ar.Pages[0].Words) {
                    PointF[] pfa = dw.BoundingPolygon.ToArray();
                    switch (cnt++ % 4) {
                        case 0: sive.Color= Color.Red; break;
                        case 1: sive.Color= Color.Green; break;
                        case 2: sive.Color= Color.Blue; break;
                        default: sive.Color= Color.Purple; break;
                    }
                    e.Graphics.DrawPolygon(sive, pfa);
                }
            }
        }
    }
}
