using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CatVision.Common.Enum;
using CatVision.Common.Model;
using HalconDotNet;

namespace CatVision.Wpf.Views
{
    public partial class DrawROIForm : Form
    {
        HWindowControl hWindow = new HWindowControl();
        public double positionX, positionY;
        private HImage hv_image;
        private HObject xld;
        HTuple hv_imageWidth = new HTuple();
        HTuple hv_imageHeight = new HTuple();
        string str_imgSize = string.Empty;
        List<string> RoiTypes = EnumHelper.GetEnumStrs<ROIType>();
        public HImage Image
        {
            get { return this.hv_image; }
            set
            {
                if (!(value is null))
                {
                    try
                    {
                        if (!(hv_image is null)) { hv_image.Dispose(); }
                        hv_image = value;
                        hv_image.GetImageSize(out hv_imageWidth, out hv_imageHeight);
                        str_imgSize = string.Format("W:{0},H:{1}", hv_imageWidth, hv_imageHeight);
                        hWindow.HalconWindow.SetColor("red");
                        hWindow.HalconWindow.SetPart(new HTuple(0), new HTuple(0), hv_imageHeight, hv_imageWidth);
                        //hWindow.HalconWindow.DispImage(hv_image);
                        hWindow.HalconWindow.DispObj(hv_image);
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.Print(ex.Message); }
                }
            }
        }
        public DrawROIForm()
        {
            Roi = new ROI();
            InitializeComponent();
            foreach (string t in RoiTypes) { comboBoxType.Items.Add(t); }
            this.tableLayoutPanel1.Controls.Add(this.hWindow, 1, 0);
            this.hWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindow.MouseMove += HWindow_MouseMove;
        }
        public DrawROIForm(ROI roi)
        {
            if (roi is null) { roi = new ROI(); }
            Roi = roi;
            InitializeComponent();
            foreach (string t in RoiTypes) { comboBoxType.Items.Add(t); }
            this.tableLayoutPanel1.Controls.Add(this.hWindow, 1, 0);
            this.hWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindow.MouseMove += HWindow_MouseMove;
            SetTextboxValue();
        }
        private void HWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(Image is null))
            {
                try
                {
                    string str_value;
                    HOperatorSet.CountChannels(Image, out HTuple channel_count);
                    HOperatorSet.GetImageSize(Image, out hv_imageWidth, out hv_imageHeight);
                    //string str_imgSize = string.Format("[{0}(w)-{1}(h)] ", hv_imageWidth.I, hv_imageHeight.I);
                    hWindow.HalconWindow.GetMpositionSubPix( out positionY, out positionX, out _);
                    string str_position = string.Format("X: {0:0}, Y: {1:0} ", positionX, positionY);
                    bool _isXOut = (positionX < 0 || positionX >= hv_imageWidth);
                    bool _isYOut = (positionY < 0 || positionY >= hv_imageHeight);
                    if (!_isXOut && !_isYOut)
                    {
                        if (channel_count == 1)
                        {
                            double grayVal = Image.GetGrayval((int)positionY, (int)positionX);
                            str_value = string.Format("Gray: {0:0.0}", grayVal);
                        }
                        else if (channel_count == 3)
                        {
                            HImage _RedChannel = Image.AccessChannel(1);
                            HImage _GreenChannel = Image.AccessChannel(2);
                            HImage _BlueChannel = Image.AccessChannel(3);
                            double grayValRed = _RedChannel.GetGrayval((int)positionY, (int)positionX);
                            double grayValGreen = _GreenChannel.GetGrayval((int)positionY, (int)positionX);
                            double grayValBlue = _BlueChannel.GetGrayval((int)positionY,(int)positionX);
                            _RedChannel.Dispose();
                            _GreenChannel.Dispose();
                            _BlueChannel.Dispose();
                            str_value = string.Format("| {0:0}(R), {1:0}(G), {2:0}(B)", grayValRed, grayValGreen, grayValBlue);
                        }
                        else
                        {
                            str_value = "";
                        }
                        labelPos.Text = str_imgSize + " " + str_position + " " + str_value;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.Message);
                }
            }
        }
        public void HobjectToHimage(HObject hobject)
        {
            if (hobject == Image)
            {
                return;
            }
            if (hobject == null || !hobject.IsInitialized())
            {
                hWindow.HalconWindow.ClearWindow();
                return;
            }
            this.Image = new HImage(hobject);
        }

        public ROI Roi;
        private void buttonReadImg_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "所有图像文件 | *.bmp; *.pcx; *.png; *.jpg; *.gif;*.tif; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    HTuple ImagePath = openFileDialog.FileName;
                    HImage image = new HImage();
                    image.ReadImage(ImagePath);
                    this.HobjectToHimage(image);
                }
            }
            catch (HalconException ex)
            {
                throw ex;
            }
        }

        private void buttonDrawRoi_Click(object sender, EventArgs e)
        {
            if (comboBoxType.SelectedIndex < 0)
            {
                labelInfo.Text = "Select type first."; return;
            }
            Roi.roi_type = EnumHelper.GetEnum<ROIType>(comboBoxType.SelectedItem.ToString());
            if (Roi.roi_type == ROIType.circle)
            {
                hWindow.HalconWindow.DrawCircle(out double r, out double c, out double p);
                Roi.roi_type = ROIType.circle; Roi.row = r; Roi.column = c; Roi.phi = p;
            }
            else if (Roi.roi_type == ROIType.rectangle1)
            {
                hWindow.HalconWindow.DrawRectangle1(out double r, out double c, out double l1, out double l2);
                Roi.roi_type = ROIType.rectangle1; Roi.row = r; Roi.column = c; Roi.length1 = l1; Roi.length2 = l2;
            }
            else if (Roi.roi_type == ROIType.rectangle2)
            {
                hWindow.HalconWindow.DrawRectangle2(out double r, out double c, out double p, out double l1, out double l2);
                Roi.roi_type = ROIType.rectangle2; Roi.row = r; Roi.column = c; Roi.phi = p; Roi.length1 = l1; Roi.length2 = l2;
            }
            SetTextboxValue();
            DispRoi();
        }
        private void SetTextboxValue()
        {
            textBoxName.Text = Roi.roi_name;
            textBoxRow.Text = Roi.row.ToString();
            textBoxColumn.Text = Roi.column.ToString();
            if (Roi.roi_type == ROIType.circle)
            {
                comboBoxType.SelectedIndex = 0;
                textBoxPhi.Text = Roi.phi.ToString();
            }
            else if (Roi.roi_type == ROIType.rectangle1)
            {
                comboBoxType.SelectedIndex = 1;
                textBoxLength1.Text = Roi.length1.ToString();
                textBoxLength2.Text = Roi.length2.ToString();
            }
            else if (Roi.roi_type == ROIType.rectangle2)
            {
                comboBoxType.SelectedIndex = 2;
                textBoxPhi.Text = Roi.phi.ToString();
                textBoxLength1.Text = Roi.length1.ToString();
                textBoxLength2.Text = Roi.length2.ToString();
            }
        }
        private void DispRoi()
        {
            HOperatorSet.GenEmptyObj(out xld);
            if (Roi.roi_type == ROIType.circle)
            {
                HOperatorSet.GenCircleContourXld(out xld, Roi.row, Roi.column, Roi.phi, 0, 6.28318, "positive", 1);
            }
            else if (Roi.roi_type == ROIType.rectangle1)
            {
                double r = (Roi.row + Roi.length1) / 2;
                double c = (Roi.column + Roi.length2) / 2;
                double p = 0;
                double l1 = (Roi.length1 - Roi.row) / 2;
                double l2 = (Roi.length2 - Roi.column) / 2;
                HOperatorSet.GenRectangle2ContourXld(out xld, r, c, p, l1, l2);
            }
            else if (Roi.roi_type == ROIType.rectangle2)
            {
                HOperatorSet.GenRectangle2ContourXld(out xld, Roi.row, Roi.column, Roi.phi, Roi.length1, Roi.length2);
            }
            hWindow.HalconWindow.DispXld(new HXLD(xld));
        }
        private void buttonClearRoi_Click(object sender, EventArgs e)
        {
            hWindow.HalconWindow.ClearWindow();
            hWindow.HalconWindow.DispObj(hv_image);
        }

        private void DrawROIForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetRoiValue(true);
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            SetRoiValue();
        }

        private void buttonDisp_Click(object sender, EventArgs e)
        {
            DispRoi();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            Roi.roi_name = textBoxName.Text;
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxType.SelectedIndex >= 0)
            {
                Roi.roi_type = EnumHelper.GetEnum<ROIType>(comboBoxType.SelectedItem.ToString());
            }
        }

        private void SetRoiValue(bool isClosing = false)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                labelInfo.Text = "Write type first."; return;
            }
            else
            {
                try
                {
                    Roi.roi_name = textBoxName.Text;
                    if (double.TryParse(textBoxRow.Text, out double r)) Roi.row = r;
                    if (double.TryParse(textBoxColumn.Text, out double c)) Roi.column = c;
                    if (double.TryParse(textBoxPhi.Text, out double p)) Roi.phi = p;
                    if (double.TryParse(textBoxLength1.Text, out double l1)) Roi.length1 = l1;
                    if (double.TryParse(textBoxLength2.Text, out double l2)) Roi.length2 = l2;
                    labelInfo.Text = "Done.";
                }
                catch { labelInfo.Text = "Something error."; }
            }
            Image?.Dispose();
            xld?.Dispose();
        }
    }
}
