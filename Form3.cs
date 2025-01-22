using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Data;
namespace WinDocIntel2025Jan {
    public partial class Form3 : Form {
        public Form3() {
            InitializeComponent();
        }
        AzureKeyCredential akc;
        DocumentAnalysisClient dac;
        AnalyzeResult ar;
        bool lines = false; bool tabl = false; bool line = false;
        int scale = 1;
        string nl = Environment.NewLine;
        private void Form3_Load(object sender, EventArgs e) {
            //nl = Environment.NewLine;
            string leKey = Environment.GetEnvironmentVariable("formRecog_key");
            string endPnt = Environment.GetEnvironmentVariable("formRecog_url");
            akc = new AzureKeyCredential(leKey);
            dac = new DocumentAnalysisClient(new Uri(endPnt), akc);
            //pictureBox1.Image = Image.FromFile("layout.png"); //SecMicrosoft.jpg works
            panel1.Controls.Add(pictureBox1);
            panel1.AutoScroll = true;
            string[] files = Directory.GetFiles(    //"*.png | *.jpg" doesn't work
                @"C:\Users\DBruening\source\repos\WinDocIntel2025Jan\bin\Debug\net8.0-windows", "*.png")
                //.Where(φ => φ.ToLower().Contains("layout"))
                //.Where(v => (v.ToLower().EndsWith(".png") || v.ToLower().EndsWith(".jpg")) &&
                //!v.ToLower().Contains(" - copy"))
                .Select(f => Path.GetFileName(f)).ToArray();
            for (int Ψ = 0; Ψ < files.Length; Ψ++)
                //textBox1.Text += files[Ψ] + nl; works
                comboBox1.Items.Add(files[Ψ]);
            comboBox1.Items.Add("read.png");
            comboBox1.Items.Add("insurance-card.png");
            comboBox1.Items.Add("w2.png");
            comboBox1.Items.Add("receipt.png");
            comboBox1.Items.Add("business_card.jpg");
            //pictureBox1.Load("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/rest-api/layout.png"); works
            /*parameter is not valid:
            pictureBox1.Load("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-layout.pdf"); */
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            //textBox1.Text = comboBox1.SelectedItem.ToString();
            //pictureBox1.Image = null;  //doesn't help with System.OutOfMemoryException
            //pictureBox's Image doesn't seem to be able to accept PDF
            lines = false; line = false;  //2025-01-17 added 5:54pm
            pictureBox1.Refresh(); //2025-01-17 added 5:39pm
            string strUri = "https://raw.githubusercontent.com/Azure-Samples/" +
                "cognitive-services-REST-api-samples/master/curl/form-recognizer/";
            string restDir = "rest-api/" + comboBox1.SelectedItem.ToString();
            if (comboBox1.SelectedItem.ToString() == "sample-layout.png") {
                pictureBox1.Image = Image.FromFile(comboBox1.SelectedItem.ToString());
                restDir = comboBox1.SelectedItem.ToString().Replace(".png", ".pdf");
                scale = 70;
            } else if (comboBox1.SelectedItem.ToString() == "invoice.png") {
                pictureBox1.Image = Image.FromFile(comboBox1.SelectedItem.ToString());
                restDir = "rest-api/" + comboBox1.SelectedItem.ToString().Replace(".png", ".pdf");
                scale = 72;
            } else {
                pictureBox1.Load(strUri + restDir); //1/17 6:15 sample-layout.png is going here for some reason
                scale = 1;
            }
            textBox1.Text = strUri + restDir;
            Uri uri = new Uri(strUri + restDir);

            /*Uri uri = new Uri("https://raw.githubusercontent.com/Azure-Samples/" +
                "cognitive-services-REST-api-samples/master/curl/form-recognizer/rest-api/layout.png"); */
            AnalyzeDocumentOperation ado = dac.AnalyzeDocumentFromUri(WaitUntil.Completed, "prebuilt-layout", uri);
            ar = ado.Value;
            comboBox2.Items.Clear();
            for (int δ = 0; δ < ar.Pages[0].Lines.Count; δ++)
                comboBox2.Items.Add(δ);
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e) {
            /* 2025-01-17 commented the C# below since it seems that drawPolygon is
             more efficient and broader than drawRectangle since it can draw slanted 
            rectangles (parallelograms) for business_card.jpg:
            if (lines) {
                IReadOnlyList<PointF> lpf; //.ToList<PointF>();
                for (int ξ = 0; ξ < ar.Pages[0].Lines.Count; ξ++) {
                    lpf = ar.Pages[0].Lines[ξ].BoundingPolygon;
                    float flH = lpf[2].Y - lpf[1].Y;
                    float flW = lpf[1].X - lpf[0].X;
                    //int scale = 70;
                    int iH = (int)(flH * scale);
                    int iW = (int)(flW * scale);
                    int iX = (int)(lpf[0].X * scale);
                    int iY = (int)(lpf[0].Y * scale);
                    //textBox1.Text += $"{ξ}:({iX},{iY}) W:{iW}  H:{iH}{nl}";
                    Rectangle rct = new Rectangle(iX, iY, iW, iH);
                    Pen pig = new Pen(Color.Purple);
                    switch (ξ % 4) {
                        case 0: pig = new Pen(Color.Blue); break;
                        case 1: pig = new Pen(Color.Green); break;
                        case 2: pig = new Pen(Color.Red); break;
                    }
                    //Parameter is not valid when AnalyzeDocumentFromUri and pictureBox1_Paint are async
                    //grp.DrawRectangle(pig, rct);
                    e.Graphics.DrawRectangle(pig, rct);
                } 
            }*/
            /* 2025-01-17 commented the C# below since it seems that drawPolygon is
                more efficient and broader than drawRectangle since it can draw slanted 
            rectangles (parallelograms) for business_card.jpg:
            if (line) {
                IReadOnlyList<PointF> lpf = ar.Pages[0].Lines[comboBox2.SelectedIndex].BoundingPolygon;
                float flH = lpf[2].Y - lpf[1].Y;
                float flW = lpf[1].X - lpf[0].X;
                //int scale = 70;
                int iH = (int)(flH * scale);
                int iW = (int)(flW * scale);
                int iX = (int)(lpf[0].X * scale);
                int iY = (int)(lpf[0].Y * scale);
                //textBox1.Text += $"{ξ}:({iX},{iY}) W:{iW}  H:{iH}{nl}";
                Rectangle rct = new Rectangle(iX, iY, iW, iH);
                Pen pig = new Pen(Color.Purple);
                pig.Width = 3;
                e.Graphics.DrawRectangle(pig, rct);
            } */
            if (line) {
                if (comboBox2.SelectedIndex > -1) {
                    PointF[] pfa = ar.Pages[0].Lines[comboBox2.SelectedIndex].BoundingPolygon.ToArray(); ; ; ; ;
                    for (int φ = 0; φ < pfa.Length; φ++) {
                        pfa[φ].X *= scale;
                        pfa[φ].Y *= scale;
                    }
                    Pen dulum = new Pen(Color.Purple);
                    dulum.Width = 4;
                    e.Graphics.DrawPolygon(dulum, pfa);
                }
            } else if (lines) {
                PointF[] pfa;
                int cntLin = 0;
                Pen pal;
                System.Drawing.Color clr;
                foreach (DocumentLine dl in ar.Pages[0].Lines) {
                    pfa = dl.BoundingPolygon.ToArray();
                    for (int ξ = 0; ξ < pfa.Length; ξ++) {
                        pfa[ξ].X *= scale;
                        pfa[ξ].Y *= scale;
                    }
                    switch (cntLin++ % 3) {
                        case 0: clr = System.Drawing.Color.Red; break;
                        case 1: clr = System.Drawing.Color.Green; break;
                        case 2: clr = System.Drawing.Color.Blue; break;
                        default: clr = System.Drawing.Color.Purple; break;
                    }
                    pal = new Pen(clr); pal.Width = 4;
                    e.Graphics.DrawPolygon(pal, pfa);
                }
            }
            else if (tabl) {
                Pen sylvania = new Pen(Color.Fuchsia);
                sylvania.Width = 4;
                int cntTbl = 0;
                foreach (DocumentTable dt in ar.Tables) {
                    switch (cntTbl++ % 3) {
                        case 0: sylvania.Color = Color.Red; break;
                        case 1: sylvania.Color = Color.Blue; break;
                        case 2: sylvania.Color = Color.Green; break;
                    }
                    foreach (BoundingRegion br in dt.BoundingRegions) {
                        //PointF[] pfa = br.BoundingPolygon.Select(λ => λ.X*scale, λ => λ.Y*scale).ToArray();
                        //IReadOnlyList<PointF> lpf = br.BoundingPolygon;
                        PointF[] pfa = br.BoundingPolygon.ToArray();
                        for (int λ=0; λ<pfa.Length; λ++) {
                            pfa[λ].X *= scale;
                            pfa[λ].Y *= scale;
                        }
                        e.Graphics.DrawPolygon(sylvania, pfa);
                        //textBox1.Text += "In tabl in pictureBox1_Paint";
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e) {
            /*Initially I assigned line and lines in the 1st line of this EH. Clicking the button the 
             1st time didn't draw the polygons, but clicking a 2nd time did.  Setting comboBox2's index 
            to -1 in this EH ran its SelectedIndexChanged EH, which set line to true and lines to false.
            Thus, I moved assigning line and lines below comboBox2.SelectedIndex=-1 and made 
            pictureBox1.Refresh() last */
            comboBox2.SelectedIndex = -1;
            textBox1.Text = "";
            lines = true; line = false; tabl = false;
            pictureBox1.Refresh();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            line = true; lines = false; tabl = false;
            if (comboBox2.SelectedIndex > -1)
                textBox1.Text = ar.Pages[0].Lines[comboBox2.SelectedIndex].Content;
            pictureBox1.Refresh();
        }
        private void button2_Click(object sender, EventArgs e) {
           line = false; lines = false; tabl = true;
            pictureBox1.Refresh();
        }
        /*private void button2_Click(object sender, EventArgs e) {
   IReadOnlyList<PointF> lpf = ar.Pages[0].Lines[0].BoundingPolygon;
   textBox1.Text = "Points in polygon: " + ar.Pages[0].Lines[0].BoundingPolygon.Count.ToString(); //4
   foreach(PointF pf in lpf)
       textBox1.Text += $"  ({pf.X},{pf.Y}) ";
   businessCard=true; lines=false; line=false;  
   pictureBox1.Refresh();
} */
    }
}
