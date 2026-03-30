namespace WinFormsApp1
{
    partial class CaptureToolForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRoot;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelSelection;
        private System.Windows.Forms.TextBox _txtSelGray;
        private System.Windows.Forms.TextBox _txtSelColor;
        private System.Windows.Forms.TextBox _txtSelR;
        private System.Windows.Forms.TextBox _txtSelG;
        private System.Windows.Forms.TextBox _txtSelB;
        private System.Windows.Forms.CheckBox _chkModify;
        private System.Windows.Forms.CheckBox _chkMultiColor;
        private System.Windows.Forms.CheckBox _chkFindShape;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.SplitContainer splitContainerWork;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelGrid;
        private WinFormsApp1.PixelGridControl _grid;
        private System.Windows.Forms.VScrollBar _gridVScroll;
        private System.Windows.Forms.HScrollBar _gridHScroll;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPreview;
        private System.Windows.Forms.Panel _imagePanel;
        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSaved;
        private System.Windows.Forms.ListBox _savedImagesList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelSavedButtons;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelBindButtons;
        private System.Windows.Forms.TabControl tabControlModes;
        private System.Windows.Forms.TabPage tabPageGray;
        private System.Windows.Forms.TabPage tabPageGrayDiff;
        private System.Windows.Forms.TabPage tabPageColor;
        private System.Windows.Forms.TabPage tabPageColorPosition;
        private System.Windows.Forms.TabPage tabPageMultiColor;
        private System.Windows.Forms.TextBox _txtThreshold;
        private System.Windows.Forms.TextBox _txtGrayDiff;
        private System.Windows.Forms.TrackBar _trackSimilar1;
        private System.Windows.Forms.TrackBar _trackSimilar2;
        private System.Windows.Forms.TrackBar _trackSimilar3;
        private System.Windows.Forms.NumericUpDown _numDiffRgb2;
        private System.Windows.Forms.TextBox _txtColorList;
        private System.Windows.Forms.TextBox _txtComment;
        private System.Windows.Forms.Label labelComment;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCrop;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCommands;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tableLayoutPanelRoot = new TableLayoutPanel();
            flowLayoutPanelSelection = new FlowLayoutPanel();
            _txtSelGray = new TextBox();
            _txtSelColor = new TextBox();
            _txtSelR = new TextBox();
            _txtSelG = new TextBox();
            _txtSelB = new TextBox();
            _chkModify = new CheckBox();
            _chkMultiColor = new CheckBox();
            _chkFindShape = new CheckBox();
            _lblStatus = new Label();
            splitContainerWork = new SplitContainer();
            tableLayoutPanelGrid = new TableLayoutPanel();
            _grid = new PixelGridControl();
            _gridVScroll = new VScrollBar();
            _gridHScroll = new HScrollBar();
            tableLayoutPanelPreview = new TableLayoutPanel();
            _imagePanel = new Panel();
            _pictureBox = new PictureBox();
            tableLayoutPanelSaved = new TableLayoutPanel();
            _savedImagesList = new ListBox();
            flowLayoutPanelSavedButtons = new FlowLayoutPanel();
            flowLayoutPanelBindButtons = new FlowLayoutPanel();
            tabControlModes = new TabControl();
            tabPageGray = new TabPage();
            tabPageGrayDiff = new TabPage();
            tabPageColor = new TabPage();
            tabPageColorPosition = new TabPage();
            tabPageMultiColor = new TabPage();
            _txtThreshold = new TextBox();
            _txtGrayDiff = new TextBox();
            _trackSimilar1 = new TrackBar();
            _trackSimilar2 = new TrackBar();
            _trackSimilar3 = new TrackBar();
            _numDiffRgb2 = new NumericUpDown();
            _txtColorList = new TextBox();
            _txtComment = new TextBox();
            labelComment = new Label();
            flowLayoutPanelCrop = new FlowLayoutPanel();
            flowLayoutPanelCommands = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)splitContainerWork).BeginInit();
            splitContainerWork.Panel1.SuspendLayout();
            splitContainerWork.Panel2.SuspendLayout();
            splitContainerWork.SuspendLayout();
            tableLayoutPanelGrid.SuspendLayout();
            tableLayoutPanelPreview.SuspendLayout();
            _imagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_pictureBox).BeginInit();
            tableLayoutPanelSaved.SuspendLayout();
            tabControlModes.SuspendLayout();
            tabPageGray.SuspendLayout();
            tabPageGrayDiff.SuspendLayout();
            tabPageColor.SuspendLayout();
            tabPageColorPosition.SuspendLayout();
            tabPageMultiColor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_trackSimilar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_trackSimilar2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_trackSimilar3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numDiffRgb2).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanelRoot
            // 
            tableLayoutPanelRoot.ColumnCount = 1;
            tableLayoutPanelRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelRoot.Dock = DockStyle.Fill;
            tableLayoutPanelRoot.Padding = new Padding(10);
            tableLayoutPanelRoot.RowCount = 5;
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle());
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle());
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle());
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle());
            tableLayoutPanelRoot.Controls.Add(flowLayoutPanelSelection, 0, 0);
            tableLayoutPanelRoot.Controls.Add(splitContainerWork, 0, 1);
            tableLayoutPanelRoot.Controls.Add(tabControlModes, 0, 2);
            tableLayoutPanelRoot.Controls.Add(flowLayoutPanelCrop, 0, 3);
            tableLayoutPanelRoot.Controls.Add(flowLayoutPanelCommands, 0, 4);
            // 
            // flowLayoutPanelSelection
            // 
            flowLayoutPanelSelection.AutoSize = true;
            flowLayoutPanelSelection.Controls.Add(new Label { Text = "灰度", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            flowLayoutPanelSelection.Controls.Add(_txtSelGray);
            flowLayoutPanelSelection.Controls.Add(new Label { Text = "颜色", AutoSize = true, Padding = new Padding(12, 8, 0, 0) });
            flowLayoutPanelSelection.Controls.Add(_txtSelColor);
            flowLayoutPanelSelection.Controls.Add(new Label { Text = "R", AutoSize = true, Padding = new Padding(12, 8, 0, 0) });
            flowLayoutPanelSelection.Controls.Add(_txtSelR);
            flowLayoutPanelSelection.Controls.Add(new Label { Text = "G", AutoSize = true, Padding = new Padding(12, 8, 0, 0) });
            flowLayoutPanelSelection.Controls.Add(_txtSelG);
            flowLayoutPanelSelection.Controls.Add(new Label { Text = "B", AutoSize = true, Padding = new Padding(12, 8, 0, 0) });
            flowLayoutPanelSelection.Controls.Add(_txtSelB);
            flowLayoutPanelSelection.Controls.Add(_chkModify);
            flowLayoutPanelSelection.Controls.Add(_chkMultiColor);
            flowLayoutPanelSelection.Controls.Add(_chkFindShape);
            flowLayoutPanelSelection.Controls.Add(_lblStatus);
            flowLayoutPanelSelection.Dock = DockStyle.Fill;
            // 
            // _txtSelGray
            // 
            _txtSelGray.ReadOnly = true;
            _txtSelGray.Size = new Size(80, 30);
            // 
            // _txtSelColor
            // 
            _txtSelColor.ReadOnly = true;
            _txtSelColor.Size = new Size(140, 30);
            // 
            // _txtSelR
            // 
            _txtSelR.ReadOnly = true;
            _txtSelR.Size = new Size(60, 30);
            // 
            // _txtSelG
            // 
            _txtSelG.ReadOnly = true;
            _txtSelG.Size = new Size(60, 30);
            // 
            // _txtSelB
            // 
            _txtSelB.ReadOnly = true;
            _txtSelB.Size = new Size(60, 30);
            // 
            // _chkModify
            // 
            _chkModify.AutoSize = true;
            _chkModify.Padding = new Padding(12, 8, 0, 0);
            _chkModify.Text = "修改";
            // 
            // _chkMultiColor
            // 
            _chkMultiColor.AutoSize = true;
            _chkMultiColor.Padding = new Padding(12, 8, 0, 0);
            _chkMultiColor.Text = "多点找色";
            // 
            // _chkFindShape
            // 
            _chkFindShape.AutoSize = true;
            _chkFindShape.Padding = new Padding(12, 8, 0, 0);
            _chkFindShape.Text = "找形状";
            // 
            // _lblStatus
            // 
            _lblStatus.AutoSize = true;
            _lblStatus.Padding = new Padding(12, 8, 0, 0);
            // 
            // splitContainerWork
            // 
            splitContainerWork.Dock = DockStyle.Fill;
            splitContainerWork.SplitterDistance = 900;
            splitContainerWork.Panel1.Controls.Add(tableLayoutPanelGrid);
            splitContainerWork.Panel2.Controls.Add(tableLayoutPanelPreview);
            // 
            // tableLayoutPanelGrid
            // 
            tableLayoutPanelGrid.ColumnCount = 2;
            tableLayoutPanelGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelGrid.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelGrid.Dock = DockStyle.Fill;
            tableLayoutPanelGrid.RowCount = 2;
            tableLayoutPanelGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelGrid.RowStyles.Add(new RowStyle());
            tableLayoutPanelGrid.Controls.Add(_grid, 0, 0);
            tableLayoutPanelGrid.Controls.Add(_gridVScroll, 1, 0);
            tableLayoutPanelGrid.Controls.Add(_gridHScroll, 0, 1);
            // 
            // _grid
            // 
            _grid.Dock = DockStyle.Fill;
            // 
            // _gridVScroll
            // 
            _gridVScroll.Dock = DockStyle.Fill;
            // 
            // _gridHScroll
            // 
            _gridHScroll.Dock = DockStyle.Fill;
            // 
            // tableLayoutPanelPreview
            // 
            tableLayoutPanelPreview.ColumnCount = 1;
            tableLayoutPanelPreview.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelPreview.Dock = DockStyle.Fill;
            tableLayoutPanelPreview.RowCount = 3;
            tableLayoutPanelPreview.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelPreview.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanelPreview.RowStyles.Add(new RowStyle());
            tableLayoutPanelPreview.Controls.Add(_imagePanel, 0, 0);
            tableLayoutPanelPreview.Controls.Add(tableLayoutPanelSaved, 0, 1);
            tableLayoutPanelPreview.Controls.Add(flowLayoutPanelBindButtons, 0, 2);
            // 
            // _imagePanel
            // 
            _imagePanel.AutoScroll = true;
            _imagePanel.BorderStyle = BorderStyle.FixedSingle;
            _imagePanel.Controls.Add(_pictureBox);
            _imagePanel.Dock = DockStyle.Fill;
            // 
            // _pictureBox
            // 
            _pictureBox.Location = new Point(0, 0);
            _pictureBox.SizeMode = PictureBoxSizeMode.Normal;
            // 
            // tableLayoutPanelSaved
            // 
            tableLayoutPanelSaved.ColumnCount = 1;
            tableLayoutPanelSaved.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelSaved.Dock = DockStyle.Fill;
            tableLayoutPanelSaved.RowCount = 2;
            tableLayoutPanelSaved.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelSaved.RowStyles.Add(new RowStyle());
            tableLayoutPanelSaved.Controls.Add(_savedImagesList, 0, 0);
            tableLayoutPanelSaved.Controls.Add(flowLayoutPanelSavedButtons, 0, 1);
            // 
            // _savedImagesList
            // 
            _savedImagesList.Dock = DockStyle.Fill;
            // 
            // flowLayoutPanelSavedButtons
            // 
            flowLayoutPanelSavedButtons.AutoSize = true;
            flowLayoutPanelSavedButtons.Controls.Add(new Button { Text = "载入图片", AutoSize = true, Name = "buttonLoadImage" });
            flowLayoutPanelSavedButtons.Controls.Add(new Button { Text = "保存图片", AutoSize = true, Name = "buttonSaveImage" });
            flowLayoutPanelSavedButtons.Controls.Add(new Button { Text = "打开目录", AutoSize = true, Name = "buttonOpenDir" });
            flowLayoutPanelSavedButtons.Controls.Add(new Button { Text = "清空截图", AutoSize = true, Name = "buttonClearImages" });
            flowLayoutPanelSavedButtons.Dock = DockStyle.Fill;
            // 
            // flowLayoutPanelBindButtons
            // 
            flowLayoutPanelBindButtons.AutoSize = true;
            flowLayoutPanelBindButtons.Controls.Add(new Button { Text = "绑定窗口1", AutoSize = true, Name = "buttonBind0" });
            flowLayoutPanelBindButtons.Controls.Add(new Button { Text = "绑定窗口1+", AutoSize = true, Name = "buttonBind1" });
            flowLayoutPanelBindButtons.Controls.Add(new Button { Text = "绑定窗口2", AutoSize = true, Name = "buttonBind2" });
            flowLayoutPanelBindButtons.Controls.Add(new Button { Text = "绑定窗口2+", AutoSize = true, Name = "buttonBind3" });
            flowLayoutPanelBindButtons.Controls.Add(new Button { Text = "绑定窗口3", AutoSize = true, Name = "buttonBind4" });
            flowLayoutPanelBindButtons.Controls.Add(new Button { Text = "保存裁剪图", AutoSize = true, Name = "buttonSaveTrimmed" });
            flowLayoutPanelBindButtons.Dock = DockStyle.Fill;
            // 
            // tabControlModes
            // 
            tabControlModes.Controls.Add(tabPageGray);
            tabControlModes.Controls.Add(tabPageGrayDiff);
            tabControlModes.Controls.Add(tabPageColor);
            tabControlModes.Controls.Add(tabPageColorPosition);
            tabControlModes.Controls.Add(tabPageMultiColor);
            tabControlModes.Dock = DockStyle.Fill;
            tabControlModes.Height = 180;
            // 
            // tabPageGray
            // 
            tabPageGray.Text = "灰度阈值";
            tabPageGray.Controls.Add(new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Controls =
                {
                    new Label { Text = "阈值", AutoSize = true, Padding = new Padding(0, 8, 0, 0) },
                    _txtThreshold,
                    new Button { Text = "灰度阈值二值化", AutoSize = true, Name = "buttonGray" }
                }
            });
            _txtThreshold.Width = 80;
            // 
            // tabPageGrayDiff
            // 
            tabPageGrayDiff.Text = "灰度差值";
            tabPageGrayDiff.Controls.Add(new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Controls =
                {
                    new Label { Text = "灰度差", AutoSize = true, Padding = new Padding(0, 8, 0, 0) },
                    _txtGrayDiff,
                    new Button { Text = "灰度差值二值化", AutoSize = true, Name = "buttonGrayDiff" }
                }
            });
            _txtGrayDiff.Width = 80;
            _txtGrayDiff.Text = "50";
            // 
            // tabPageColor
            // 
            tabPageColor.Text = "颜色";
            var colorPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            colorPanel.RowStyles.Add(new RowStyle());
            colorPanel.RowStyles.Add(new RowStyle());
            _txtColorList.Dock = DockStyle.Top;
            colorPanel.Controls.Add(_txtColorList, 0, 0);
            var colorActions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            colorActions.Controls.Add(new Button { Text = "添加相似色", AutoSize = true, Name = "buttonAddSimilarity" });
            colorActions.Controls.Add(_trackSimilar1);
            colorActions.Controls.Add(new Button { Text = "添加偏色", AutoSize = true, Name = "buttonAddDiff" });
            colorActions.Controls.Add(_numDiffRgb2);
            colorActions.Controls.Add(new Button { Text = "撤销", AutoSize = true, Name = "buttonUndoColor" });
            colorActions.Controls.Add(new Button { Text = "颜色二值化", AutoSize = true, Name = "buttonColorBinary" });
            colorPanel.Controls.Add(colorActions, 0, 1);
            tabPageColor.Controls.Add(colorPanel);
            _trackSimilar1.Minimum = 0;
            _trackSimilar1.Maximum = 100;
            _trackSimilar1.Value = 90;
            _trackSimilar1.TickStyle = TickStyle.None;
            _trackSimilar1.Width = 140;
            _numDiffRgb2.Minimum = 0;
            _numDiffRgb2.Maximum = 255;
            _numDiffRgb2.Value = 50;
            // 
            // tabPageColorPosition
            // 
            tabPageColorPosition.Text = "颜色位置";
            tabPageColorPosition.Controls.Add(new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Controls =
                {
                    new Label { Text = "相似度", AutoSize = true, Padding = new Padding(0, 8, 0, 0) },
                    _trackSimilar2,
                    new Button { Text = "颜色位置二值化", AutoSize = true, Name = "buttonColorPosition" }
                }
            });
            _trackSimilar2.Minimum = 0;
            _trackSimilar2.Maximum = 100;
            _trackSimilar2.Value = 90;
            _trackSimilar2.TickStyle = TickStyle.None;
            _trackSimilar2.Width = 160;
            // 
            // tabPageMultiColor
            // 
            tabPageMultiColor.Text = "多色查找";
            tabPageMultiColor.Controls.Add(new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Controls =
                {
                    new Label { Text = "相似度", AutoSize = true, Padding = new Padding(0, 8, 0, 0) },
                    _trackSimilar3,
                    new Button { Text = "撤销颜色点", AutoSize = true, Name = "buttonUndoPoint" },
                    new Label { Text = "勾选“多点找色”后点击网格记录颜色点", AutoSize = true, Padding = new Padding(12, 8, 0, 0) }
                }
            });
            _trackSimilar3.Minimum = 0;
            _trackSimilar3.Maximum = 100;
            _trackSimilar3.Value = 90;
            _trackSimilar3.TickStyle = TickStyle.None;
            _trackSimilar3.Width = 160;
            // 
            // flowLayoutPanelCrop
            // 
            flowLayoutPanelCrop.AutoSize = true;
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "-上", AutoSize = true, Name = "buttonRepUp" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "上", AutoSize = true, Name = "buttonCutUp" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "上3", AutoSize = true, Name = "buttonCutUp3" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "-左", AutoSize = true, Name = "buttonRepLeft" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "左", AutoSize = true, Name = "buttonCutLeft" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "左3", AutoSize = true, Name = "buttonCutLeft3" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "自动", AutoSize = true, Name = "buttonAutoCrop" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "-右", AutoSize = true, Name = "buttonRepRight" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "右", AutoSize = true, Name = "buttonCutRight" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "右3", AutoSize = true, Name = "buttonCutRight3" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "-下", AutoSize = true, Name = "buttonRepDown" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "下", AutoSize = true, Name = "buttonCutDown" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "下3", AutoSize = true, Name = "buttonCutDown3" });
            flowLayoutPanelCrop.Controls.Add(new Button { Text = "重读", AutoSize = true, Name = "buttonReset" });
            flowLayoutPanelCrop.Controls.Add(labelComment);
            flowLayoutPanelCrop.Controls.Add(_txtComment);
            flowLayoutPanelCrop.Dock = DockStyle.Fill;
            labelComment.AutoSize = true;
            labelComment.Padding = new Padding(12, 8, 0, 0);
            labelComment.Text = "识别文字";
            _txtComment.Width = 220;
            // 
            // flowLayoutPanelCommands
            // 
            flowLayoutPanelCommands.AutoSize = true;
            flowLayoutPanelCommands.Controls.Add(new Button { Text = "分割添加", AutoSize = true, Name = "buttonSplitAdd" });
            flowLayoutPanelCommands.Controls.Add(new Button { Text = "整体添加", AutoSize = true, Name = "buttonAllAdd" });
            flowLayoutPanelCommands.Controls.Add(new Button { Text = "确定", AutoSize = true, Name = "buttonOk" });
            flowLayoutPanelCommands.Controls.Add(new Button { Text = "取消", AutoSize = true, Name = "buttonCancel" });
            flowLayoutPanelCommands.Dock = DockStyle.Fill;
            // 
            // CaptureToolForm
            // 
            AutoScaleDimensions = new SizeF(10F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1420, 900);
            Controls.Add(tableLayoutPanelRoot);
            MinimumSize = new Size(1280, 820);
            Name = "CaptureToolForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "图像二值化及分割";
            splitContainerWork.Panel1.ResumeLayout(false);
            splitContainerWork.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerWork).EndInit();
            splitContainerWork.ResumeLayout(false);
            tableLayoutPanelGrid.ResumeLayout(false);
            tableLayoutPanelPreview.ResumeLayout(false);
            tableLayoutPanelPreview.PerformLayout();
            _imagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_pictureBox).EndInit();
            tableLayoutPanelSaved.ResumeLayout(false);
            tableLayoutPanelSaved.PerformLayout();
            tabControlModes.ResumeLayout(false);
            tabPageGray.ResumeLayout(false);
            tabPageGrayDiff.ResumeLayout(false);
            tabPageColor.ResumeLayout(false);
            tabPageColorPosition.ResumeLayout(false);
            tabPageMultiColor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_trackSimilar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)_trackSimilar2).EndInit();
            ((System.ComponentModel.ISupportInitialize)_trackSimilar3).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numDiffRgb2).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
