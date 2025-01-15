using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using System.Drawing;
namespace WinDocIntel2025Jan {
    public partial class Form1 : Form {
        AzureKeyCredential akc;
        DocumentAnalysisClient dac;
        AnalyzeResult ar;
        bool drawRect = false; bool drawTbl = false; bool draw1Lin = false;
        int scale=70;
        string nl;
        public Form1() {
            InitializeComponent();
        }
        private async void button1_Click(object sender, EventArgs e) {
            textBox1.Text = $"H:{pictureBox1.Height}, W:{pictureBox1.Width}, L:{pictureBox1.Left}, T:{pictureBox1.Top}";
            /*Uri uriPdf = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-layout.pdf");
            AnalyzeDocumentOperation ado = await dac.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-layout", uriPdf);
            AnalyzeResult ar = ado.Value; */
            foreach (DocumentPage dp in ar.Pages) {
                textBox1.Text += nl + dp.Lines.Count.ToString() + nl;
            }
            IReadOnlyList<PointF> lpf = ar.Pages[0].Lines[0].BoundingPolygon; //.ToList<PointF>();
            //textBox1.Text += $"({ar.Pages[0].Lines[0].BoundingPolygon[0].X},{ar.Pages[0].Lines[0].BoundingPolygon[0].Y})";  works
            textBox1.Text += lpf.Count.ToString(); //4
            //textBox1.Text += String.Format("({1},{2}), ", lpf[1].X, lpf[1].Y);   //Index error
            for (int i = 0; i < lpf.Count; i++)
                textBox1.Text += $"({lpf[i].X},{lpf[i].Y})";
            //textBox1.Text += i + ", ";  works
            /*textBox1.Text += String.Format("({1},{2}), ", ar.Pages[0].Lines[0].BoundingPolygon[0].X,
                ar.Pages[0].Lines[0].BoundingPolygon[0].Y);  index error - it must have problems w/ String.Format*/
            float flH = lpf[2].Y - lpf[1].Y;
            float flW = lpf[1].X - lpf[0].X;
            textBox1.Text += $"{nl}X:{lpf[0].X}  Y:{lpf[0].Y}  W:{flW}  H:{flH}";
            //int scale = 50;
            int iH = (int)(flH * scale);
            int iW = (int)(flW * scale);
            int iX = (int)(lpf[0].X * scale);
            int iY = (int)(lpf[0].Y * scale);
            textBox1.Text += $"{nl}iX:{iX}  iY:{iY}  iH:{iH}  iW:{iW}";
            //pictureBox1.Paint += PictureBox1_Paint2;  I unsuccessfully tried adding an event handler 
        }
        /*private void PictureBox1_Paint(object sender, PaintEventArgs e) {
            //throw new NotImplementedException();
        } */
        private void pictureBox1_Paint(object sender, PaintEventArgs e) { //Paint seems to happen when the form is loaded
            if (drawRect) {
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
            }
            if (drawTbl) {
                Pen dulum = new Pen(Color.Green);
                IReadOnlyList<PointF> lpf;
                //int scale = 70;
                foreach (DocumentTable dt in ar.Tables)
                    foreach (BoundingRegion br in dt.BoundingRegions) {
                        lpf = br.BoundingPolygon;
                        e.Graphics.DrawRectangle(dulum, lpf[0].X * scale, lpf[0].Y * scale,
                            (lpf[1].X - lpf[0].X) * scale, (lpf[2].Y - lpf[1].Y) * scale);
                    }
            }
            if (draw1Lin) {
                IReadOnlyList<PointF> bp = ar.Pages[0].Lines[(int)comboBox1.SelectedItem].BoundingPolygon;
                Pen nsylvania = new Pen(Color.Blue);
                RectangleF rct = new RectangleF(bp[3].X*scale, bp[1].Y*scale, (bp[2].X-bp[3].X)*scale, (bp[3].Y-bp[0].Y)*scale);
                e.Graphics.DrawRectangle(nsylvania, rct);   
            }
        }
        private void Form1_Load(object sender, EventArgs e) {
            nl = Environment.NewLine;
            string leKey = Environment.GetEnvironmentVariable("formRecog_key");
            string endPnt = Environment.GetEnvironmentVariable("formRecog_url");
            akc = new AzureKeyCredential(leKey);
            dac = new DocumentAnalysisClient(new Uri(endPnt), akc);
            Uri uriPdf = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-layout.pdf");
            AnalyzeDocumentOperation ado = dac.AnalyzeDocumentFromUri(WaitUntil.Completed, "prebuilt-layout", uriPdf);
            ar = ado.Value;
            for (int Ψ = 0; Ψ < ar.Pages[0].Lines.Count; Ψ++)
                comboBox1.Items.Add(Ψ); //works like a big arfing bow-wow
            //comboBox1.DataSource = ar.Pages[0].Lines.Count;
        }
        private void button2_Click(object sender, EventArgs e) {
            textBox1.Text += $"# of Tables:{ar.Tables.Count}{nl}   # of Bounding Regions for 1st table:" +
                $"{ar.Tables[0].BoundingRegions.Count}   # of Bounding Polygons for 1st table: " +
                $"{ar.Tables[0].BoundingRegions[0].BoundingPolygon.Count}";
            //ar.Tables[0].BoundingRegions[0].BoundingPolygon[0].X
            //pictureBox1.Invalidate();  //doesn't remove rectangles drawn
            //pictureBox1.Refresh();    //doesn't remove rectangles drawn
            /*System.IO.FileNotFoundException:
            pictureBox1.Image = Image.FromFile("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-layout.pdf"); */
            /* System.ArgumentException: 'Parameter is not valid.':
            pictureBox1.Load("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-layout.pdf"); */
            //pictureBox1.Image = Image.FromFile("SecMicrosoft.jpg"); //doesn't remove rectangles drawn
            drawRect = false;
            pictureBox1.Refresh();
            //Graphics grf = Graphics.FromImage(pictureBox1.Image);
            textBox1.Text += nl + "Points in the 1st table's polygon: ";
            foreach (PointF pf in ar.Tables[0].BoundingRegions[0].BoundingPolygon) {
                textBox1.Text += $"({pf.X},{pf.Y}), ";
            }
            IReadOnlyList<PointF> lpf = ar.Tables[0].BoundingRegions[0].BoundingPolygon;
            textBox1.Text += nl + $"X:{lpf[0].X}   Y:{lpf[0].Y}   W:{lpf[1].X - lpf[0].X}   H:{lpf[2].Y - lpf[1].Y}";
            /* Pen dulum = new Pen(Color.Green);
            IReadOnlyList<PointF> lpf = ar.Tables[0].BoundingRegions[0].BoundingPolygon;
            grf.DrawRectangle(dulum, lpf[0].X, lpf[0].Y, lpf[1].X- lpf[0].X, lpf[2].Y- lpf[1].Y);
            pictureBox1.Refresh();      not drawing rectangle  */
            drawTbl = true; drawRect = false; draw1Lin = false;
            pictureBox1.Refresh();
        }
        private void button3_Click(object sender, EventArgs e) {
            //System.Drawing.Graphics grp = Graphics.FromImage(pictureBox1.Image);
            drawRect = true; drawTbl = false; draw1Lin = false;
            pictureBox1.Refresh();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            //textBox1.Text = e.ToString(); //System.EventArgs
            //textBox1.Text = comboBox1.SelectedItem.ToString(); works
            textBox1.Text = ar.Pages[0].Lines[(int)comboBox1.SelectedItem].Content;
            draw1Lin=true; drawTbl=false; drawRect=false;
            pictureBox1.Refresh();
        }
    }
}
