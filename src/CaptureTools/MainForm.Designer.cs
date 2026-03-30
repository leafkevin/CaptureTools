namespace CaptureTools
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRoot;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelHotkey;
        private System.Windows.Forms.Label labelNowHotkey;
        private System.Windows.Forms.TextBox _txtNowHotkey;
        private System.Windows.Forms.Label labelSetHotkey;
        private System.Windows.Forms.ComboBox _cmbHotkey;
        private System.Windows.Forms.Button buttonApplyHotkey;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelCommands;
        private System.Windows.Forms.Label labelWidth;
        private System.Windows.Forms.NumericUpDown _numWidth;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.NumericUpDown _numHeight;
        private System.Windows.Forms.CheckBox _chkAddFunc;
        private System.Windows.Forms.Button buttonCapture;
        private System.Windows.Forms.Button buttonCaptureS;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonGetRange;
        private System.Windows.Forms.Button buttonGetOffset;
        private System.Windows.Forms.Button buttonGetClipOffset;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Button buttonTestClip;
        private System.Windows.Forms.Button buttonCopyOffset;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelAscii;
        private System.Windows.Forms.TextBox _txtMyPic;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelAsciiButtons;
        private System.Windows.Forms.Button buttonTrimLeft;
        private System.Windows.Forms.Button buttonTrimRight;
        private System.Windows.Forms.Button buttonTrimUp;
        private System.Windows.Forms.Button buttonTrimDown;
        private System.Windows.Forms.Button buttonUpdateAscii;
        private System.Windows.Forms.SplitContainer splitContainerRight;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTopInputs;
        private System.Windows.Forms.Label labelClipText;
        private System.Windows.Forms.TextBox _txtClipText;
        private System.Windows.Forms.Label labelOffset;
        private System.Windows.Forms.TextBox _txtOffset;
        private System.Windows.Forms.TextBox _txtScr;
        private System.Windows.Forms.Label labelHelp;

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
            tableLayoutPanelRoot = new TableLayoutPanel();
            flowLayoutPanelHotkey = new FlowLayoutPanel();
            labelNowHotkey = new Label();
            _txtNowHotkey = new TextBox();
            labelSetHotkey = new Label();
            _cmbHotkey = new ComboBox();
            buttonApplyHotkey = new Button();
            flowLayoutPanelCommands = new FlowLayoutPanel();
            labelWidth = new Label();
            _numWidth = new NumericUpDown();
            labelHeight = new Label();
            _numHeight = new NumericUpDown();
            _chkAddFunc = new CheckBox();
            buttonCapture = new Button();
            buttonCaptureS = new Button();
            buttonTest = new Button();
            buttonCopy = new Button();
            buttonGetRange = new Button();
            buttonGetOffset = new Button();
            buttonGetClipOffset = new Button();
            buttonPaste = new Button();
            buttonTestClip = new Button();
            buttonCopyOffset = new Button();
            splitContainerMain = new SplitContainer();
            tableLayoutPanelAscii = new TableLayoutPanel();
            _txtMyPic = new TextBox();
            flowLayoutPanelAsciiButtons = new FlowLayoutPanel();
            buttonTrimLeft = new Button();
            buttonTrimRight = new Button();
            buttonTrimUp = new Button();
            buttonTrimDown = new Button();
            buttonUpdateAscii = new Button();
            splitContainerRight = new SplitContainer();
            tableLayoutPanelTopInputs = new TableLayoutPanel();
            labelClipText = new Label();
            labelOffset = new Label();
            _txtClipText = new TextBox();
            _txtOffset = new TextBox();
            _txtScr = new TextBox();
            labelHelp = new Label();
            tableLayoutPanelRoot.SuspendLayout();
            flowLayoutPanelHotkey.SuspendLayout();
            flowLayoutPanelCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_numWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            tableLayoutPanelAscii.SuspendLayout();
            flowLayoutPanelAsciiButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).BeginInit();
            splitContainerRight.Panel1.SuspendLayout();
            splitContainerRight.Panel2.SuspendLayout();
            splitContainerRight.SuspendLayout();
            tableLayoutPanelTopInputs.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanelRoot
            // 
            tableLayoutPanelRoot.ColumnCount = 1;
            tableLayoutPanelRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelRoot.Controls.Add(flowLayoutPanelHotkey, 0, 0);
            tableLayoutPanelRoot.Controls.Add(flowLayoutPanelCommands, 0, 1);
            tableLayoutPanelRoot.Controls.Add(splitContainerMain, 0, 2);
            tableLayoutPanelRoot.Controls.Add(labelHelp, 0, 3);
            tableLayoutPanelRoot.Dock = DockStyle.Fill;
            tableLayoutPanelRoot.Location = new Point(0, 0);
            tableLayoutPanelRoot.Margin = new Padding(2);
            tableLayoutPanelRoot.Name = "tableLayoutPanelRoot";
            tableLayoutPanelRoot.Padding = new Padding(8);
            tableLayoutPanelRoot.RowCount = 4;
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle());
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle());
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            tableLayoutPanelRoot.Size = new Size(924, 680);
            tableLayoutPanelRoot.TabIndex = 0;
            // 
            // flowLayoutPanelHotkey
            // 
            flowLayoutPanelHotkey.AutoSize = true;
            flowLayoutPanelHotkey.Controls.Add(labelNowHotkey);
            flowLayoutPanelHotkey.Controls.Add(_txtNowHotkey);
            flowLayoutPanelHotkey.Controls.Add(labelSetHotkey);
            flowLayoutPanelHotkey.Controls.Add(_cmbHotkey);
            flowLayoutPanelHotkey.Controls.Add(buttonApplyHotkey);
            flowLayoutPanelHotkey.Dock = DockStyle.Fill;
            flowLayoutPanelHotkey.Location = new Point(10, 10);
            flowLayoutPanelHotkey.Margin = new Padding(2);
            flowLayoutPanelHotkey.Name = "flowLayoutPanelHotkey";
            flowLayoutPanelHotkey.Size = new Size(904, 31);
            flowLayoutPanelHotkey.TabIndex = 0;
            flowLayoutPanelHotkey.WrapContents = false;
            // 
            // labelNowHotkey
            // 
            labelNowHotkey.AutoSize = true;
            labelNowHotkey.Location = new Point(2, 0);
            labelNowHotkey.Margin = new Padding(2, 0, 2, 0);
            labelNowHotkey.Name = "labelNowHotkey";
            labelNowHotkey.Padding = new Padding(0, 6, 0, 0);
            labelNowHotkey.Size = new Size(56, 23);
            labelNowHotkey.TabIndex = 0;
            labelNowHotkey.Text = "截屏热键";
            // 
            // _txtNowHotkey
            // 
            _txtNowHotkey.Location = new Point(62, 2);
            _txtNowHotkey.Margin = new Padding(2);
            _txtNowHotkey.Name = "_txtNowHotkey";
            _txtNowHotkey.ReadOnly = true;
            _txtNowHotkey.Size = new Size(99, 23);
            _txtNowHotkey.TabIndex = 1;
            // 
            // labelSetHotkey
            // 
            labelSetHotkey.AutoSize = true;
            labelSetHotkey.Location = new Point(165, 0);
            labelSetHotkey.Margin = new Padding(2, 0, 2, 0);
            labelSetHotkey.Name = "labelSetHotkey";
            labelSetHotkey.Padding = new Padding(8, 6, 0, 0);
            labelSetHotkey.Size = new Size(64, 23);
            labelSetHotkey.TabIndex = 2;
            labelSetHotkey.Text = "设置热键";
            // 
            // _cmbHotkey
            // 
            _cmbHotkey.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbHotkey.Items.AddRange(new object[] { "", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "PrintScreen", "Scroll", "Pause" });
            _cmbHotkey.Location = new Point(233, 2);
            _cmbHotkey.Margin = new Padding(2);
            _cmbHotkey.Name = "_cmbHotkey";
            _cmbHotkey.Size = new Size(113, 25);
            _cmbHotkey.TabIndex = 3;
            // 
            // buttonApplyHotkey
            // 
            buttonApplyHotkey.AutoSize = true;
            buttonApplyHotkey.Location = new Point(350, 2);
            buttonApplyHotkey.Margin = new Padding(2);
            buttonApplyHotkey.Name = "buttonApplyHotkey";
            buttonApplyHotkey.Size = new Size(52, 27);
            buttonApplyHotkey.TabIndex = 4;
            buttonApplyHotkey.Text = "应用";
            buttonApplyHotkey.Click += buttonApplyHotkey_Click;
            // 
            // flowLayoutPanelCommands
            // 
            flowLayoutPanelCommands.AutoSize = true;
            flowLayoutPanelCommands.Controls.Add(labelWidth);
            flowLayoutPanelCommands.Controls.Add(_numWidth);
            flowLayoutPanelCommands.Controls.Add(labelHeight);
            flowLayoutPanelCommands.Controls.Add(_numHeight);
            flowLayoutPanelCommands.Controls.Add(_chkAddFunc);
            flowLayoutPanelCommands.Controls.Add(buttonCapture);
            flowLayoutPanelCommands.Controls.Add(buttonCaptureS);
            flowLayoutPanelCommands.Controls.Add(buttonTest);
            flowLayoutPanelCommands.Controls.Add(buttonCopy);
            flowLayoutPanelCommands.Controls.Add(buttonGetRange);
            flowLayoutPanelCommands.Controls.Add(buttonGetOffset);
            flowLayoutPanelCommands.Controls.Add(buttonGetClipOffset);
            flowLayoutPanelCommands.Controls.Add(buttonPaste);
            flowLayoutPanelCommands.Controls.Add(buttonTestClip);
            flowLayoutPanelCommands.Controls.Add(buttonCopyOffset);
            flowLayoutPanelCommands.Dock = DockStyle.Fill;
            flowLayoutPanelCommands.Location = new Point(10, 45);
            flowLayoutPanelCommands.Margin = new Padding(2);
            flowLayoutPanelCommands.Name = "flowLayoutPanelCommands";
            flowLayoutPanelCommands.Size = new Size(904, 62);
            flowLayoutPanelCommands.TabIndex = 1;
            // 
            // labelWidth
            // 
            labelWidth.AutoSize = true;
            labelWidth.Location = new Point(2, 0);
            labelWidth.Margin = new Padding(2, 0, 2, 0);
            labelWidth.Name = "labelWidth";
            labelWidth.Padding = new Padding(0, 6, 0, 0);
            labelWidth.Size = new Size(32, 23);
            labelWidth.TabIndex = 0;
            labelWidth.Text = "宽度";
            // 
            // _numWidth
            // 
            _numWidth.Location = new Point(38, 2);
            _numWidth.Margin = new Padding(2);
            _numWidth.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _numWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numWidth.Name = "_numWidth";
            _numWidth.Size = new Size(56, 23);
            _numWidth.TabIndex = 1;
            _numWidth.Value = new decimal(new int[] { 71, 0, 0, 0 });
            // 
            // labelHeight
            // 
            labelHeight.AutoSize = true;
            labelHeight.Location = new Point(98, 0);
            labelHeight.Margin = new Padding(2, 0, 2, 0);
            labelHeight.Name = "labelHeight";
            labelHeight.Padding = new Padding(8, 6, 0, 0);
            labelHeight.Size = new Size(40, 23);
            labelHeight.TabIndex = 2;
            labelHeight.Text = "高度";
            // 
            // _numHeight
            // 
            _numHeight.Location = new Point(142, 2);
            _numHeight.Margin = new Padding(2);
            _numHeight.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _numHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numHeight.Name = "_numHeight";
            _numHeight.Size = new Size(56, 23);
            _numHeight.TabIndex = 3;
            _numHeight.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // _chkAddFunc
            // 
            _chkAddFunc.AutoSize = true;
            _chkAddFunc.Checked = true;
            _chkAddFunc.CheckState = CheckState.Checked;
            _chkAddFunc.Location = new Point(202, 2);
            _chkAddFunc.Margin = new Padding(2);
            _chkAddFunc.Name = "_chkAddFunc";
            _chkAddFunc.Padding = new Padding(8, 6, 0, 0);
            _chkAddFunc.Size = new Size(147, 27);
            _chkAddFunc.TabIndex = 4;
            _chkAddFunc.Text = "附加 FindText() 函数";
            // 
            // buttonCapture
            // 
            buttonCapture.AutoSize = true;
            buttonCapture.Location = new Point(353, 2);
            buttonCapture.Margin = new Padding(2);
            buttonCapture.Name = "buttonCapture";
            buttonCapture.Size = new Size(52, 27);
            buttonCapture.TabIndex = 5;
            buttonCapture.Text = "抓图";
            buttonCapture.Click += buttonCapture_Click;
            // 
            // buttonCaptureS
            // 
            buttonCaptureS.AutoSize = true;
            buttonCaptureS.Location = new Point(409, 2);
            buttonCaptureS.Margin = new Padding(2);
            buttonCaptureS.Name = "buttonCaptureS";
            buttonCaptureS.Size = new Size(66, 27);
            buttonCaptureS.TabIndex = 6;
            buttonCaptureS.Text = "截屏抓图";
            buttonCaptureS.Click += buttonCaptureS_Click;
            // 
            // buttonTest
            // 
            buttonTest.AutoSize = true;
            buttonTest.Location = new Point(479, 2);
            buttonTest.Margin = new Padding(2);
            buttonTest.Name = "buttonTest";
            buttonTest.Size = new Size(52, 27);
            buttonTest.TabIndex = 7;
            buttonTest.Text = "测试";
            buttonTest.Click += buttonTest_Click;
            // 
            // buttonCopy
            // 
            buttonCopy.AutoSize = true;
            buttonCopy.Location = new Point(535, 2);
            buttonCopy.Margin = new Padding(2);
            buttonCopy.Name = "buttonCopy";
            buttonCopy.Size = new Size(52, 27);
            buttonCopy.TabIndex = 8;
            buttonCopy.Text = "复制";
            buttonCopy.Click += buttonCopy_Click;
            // 
            // buttonGetRange
            // 
            buttonGetRange.AutoSize = true;
            buttonGetRange.Location = new Point(591, 2);
            buttonGetRange.Margin = new Padding(2);
            buttonGetRange.Name = "buttonGetRange";
            buttonGetRange.Size = new Size(90, 27);
            buttonGetRange.TabIndex = 9;
            buttonGetRange.Text = "获取屏幕范围";
            buttonGetRange.Click += buttonGetRange_Click;
            // 
            // buttonGetOffset
            // 
            buttonGetOffset.AutoSize = true;
            buttonGetOffset.Location = new Point(685, 2);
            buttonGetOffset.Margin = new Padding(2);
            buttonGetOffset.Name = "buttonGetOffset";
            buttonGetOffset.Size = new Size(90, 27);
            buttonGetOffset.TabIndex = 10;
            buttonGetOffset.Text = "获取相对坐标";
            buttonGetOffset.Click += buttonGetOffset_Click;
            // 
            // buttonGetClipOffset
            // 
            buttonGetClipOffset.AutoSize = true;
            buttonGetClipOffset.Location = new Point(779, 2);
            buttonGetClipOffset.Margin = new Padding(2);
            buttonGetClipOffset.Name = "buttonGetClipOffset";
            buttonGetClipOffset.Size = new Size(97, 27);
            buttonGetClipOffset.TabIndex = 11;
            buttonGetClipOffset.Text = "获取相对坐标2";
            buttonGetClipOffset.Click += buttonGetClipOffset_Click;
            // 
            // buttonPaste
            // 
            buttonPaste.AutoSize = true;
            buttonPaste.Location = new Point(2, 33);
            buttonPaste.Margin = new Padding(2);
            buttonPaste.Name = "buttonPaste";
            buttonPaste.Size = new Size(52, 27);
            buttonPaste.TabIndex = 12;
            buttonPaste.Text = "粘贴";
            buttonPaste.Click += buttonPaste_Click;
            // 
            // buttonTestClip
            // 
            buttonTestClip.AutoSize = true;
            buttonTestClip.Location = new Point(58, 33);
            buttonTestClip.Margin = new Padding(2);
            buttonTestClip.Name = "buttonTestClip";
            buttonTestClip.Size = new Size(52, 27);
            buttonTestClip.TabIndex = 13;
            buttonTestClip.Text = "测试2";
            buttonTestClip.Click += buttonTestClip_Click;
            // 
            // buttonCopyOffset
            // 
            buttonCopyOffset.AutoSize = true;
            buttonCopyOffset.Location = new Point(114, 33);
            buttonCopyOffset.Margin = new Padding(2);
            buttonCopyOffset.Name = "buttonCopyOffset";
            buttonCopyOffset.Size = new Size(52, 27);
            buttonCopyOffset.TabIndex = 14;
            buttonCopyOffset.Text = "复制2";
            buttonCopyOffset.Click += buttonCopyOffset_Click;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(10, 111);
            splitContainerMain.Margin = new Padding(2);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(tableLayoutPanelAscii);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(splitContainerRight);
            splitContainerMain.Size = new Size(904, 305);
            splitContainerMain.SplitterDistance = 729;
            splitContainerMain.SplitterWidth = 3;
            splitContainerMain.TabIndex = 2;
            // 
            // tableLayoutPanelAscii
            // 
            tableLayoutPanelAscii.ColumnCount = 1;
            tableLayoutPanelAscii.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelAscii.Controls.Add(_txtMyPic, 0, 0);
            tableLayoutPanelAscii.Controls.Add(flowLayoutPanelAsciiButtons, 0, 1);
            tableLayoutPanelAscii.Dock = DockStyle.Fill;
            tableLayoutPanelAscii.Location = new Point(0, 0);
            tableLayoutPanelAscii.Margin = new Padding(2);
            tableLayoutPanelAscii.Name = "tableLayoutPanelAscii";
            tableLayoutPanelAscii.RowCount = 2;
            tableLayoutPanelAscii.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelAscii.RowStyles.Add(new RowStyle());
            tableLayoutPanelAscii.Size = new Size(729, 305);
            tableLayoutPanelAscii.TabIndex = 0;
            // 
            // _txtMyPic
            // 
            _txtMyPic.Dock = DockStyle.Fill;
            _txtMyPic.Font = new Font("Consolas", 9F, FontStyle.Bold);
            _txtMyPic.Location = new Point(2, 2);
            _txtMyPic.Margin = new Padding(2);
            _txtMyPic.Multiline = true;
            _txtMyPic.Name = "_txtMyPic";
            _txtMyPic.ScrollBars = ScrollBars.Both;
            _txtMyPic.Size = new Size(725, 266);
            _txtMyPic.TabIndex = 0;
            _txtMyPic.WordWrap = false;
            // 
            // flowLayoutPanelAsciiButtons
            // 
            flowLayoutPanelAsciiButtons.AutoSize = true;
            flowLayoutPanelAsciiButtons.Controls.Add(buttonTrimLeft);
            flowLayoutPanelAsciiButtons.Controls.Add(buttonTrimRight);
            flowLayoutPanelAsciiButtons.Controls.Add(buttonTrimUp);
            flowLayoutPanelAsciiButtons.Controls.Add(buttonTrimDown);
            flowLayoutPanelAsciiButtons.Controls.Add(buttonUpdateAscii);
            flowLayoutPanelAsciiButtons.Dock = DockStyle.Fill;
            flowLayoutPanelAsciiButtons.Location = new Point(2, 272);
            flowLayoutPanelAsciiButtons.Margin = new Padding(2);
            flowLayoutPanelAsciiButtons.Name = "flowLayoutPanelAsciiButtons";
            flowLayoutPanelAsciiButtons.Size = new Size(725, 31);
            flowLayoutPanelAsciiButtons.TabIndex = 1;
            // 
            // buttonTrimLeft
            // 
            buttonTrimLeft.AutoSize = true;
            buttonTrimLeft.Location = new Point(2, 2);
            buttonTrimLeft.Margin = new Padding(2);
            buttonTrimLeft.Name = "buttonTrimLeft";
            buttonTrimLeft.Size = new Size(52, 27);
            buttonTrimLeft.TabIndex = 0;
            buttonTrimLeft.Text = "左删";
            buttonTrimLeft.Click += buttonTrimLeft_Click;
            // 
            // buttonTrimRight
            // 
            buttonTrimRight.AutoSize = true;
            buttonTrimRight.Location = new Point(58, 2);
            buttonTrimRight.Margin = new Padding(2);
            buttonTrimRight.Name = "buttonTrimRight";
            buttonTrimRight.Size = new Size(52, 27);
            buttonTrimRight.TabIndex = 1;
            buttonTrimRight.Text = "右删";
            buttonTrimRight.Click += buttonTrimRight_Click;
            // 
            // buttonTrimUp
            // 
            buttonTrimUp.AutoSize = true;
            buttonTrimUp.Location = new Point(114, 2);
            buttonTrimUp.Margin = new Padding(2);
            buttonTrimUp.Name = "buttonTrimUp";
            buttonTrimUp.Size = new Size(52, 27);
            buttonTrimUp.TabIndex = 2;
            buttonTrimUp.Text = "上删";
            buttonTrimUp.Click += buttonTrimUp_Click;
            // 
            // buttonTrimDown
            // 
            buttonTrimDown.AutoSize = true;
            buttonTrimDown.Location = new Point(170, 2);
            buttonTrimDown.Margin = new Padding(2);
            buttonTrimDown.Name = "buttonTrimDown";
            buttonTrimDown.Size = new Size(52, 27);
            buttonTrimDown.TabIndex = 3;
            buttonTrimDown.Text = "下删";
            buttonTrimDown.Click += buttonTrimDown_Click;
            // 
            // buttonUpdateAscii
            // 
            buttonUpdateAscii.AutoSize = true;
            buttonUpdateAscii.Location = new Point(226, 2);
            buttonUpdateAscii.Margin = new Padding(2);
            buttonUpdateAscii.Name = "buttonUpdateAscii";
            buttonUpdateAscii.Size = new Size(52, 27);
            buttonUpdateAscii.TabIndex = 4;
            buttonUpdateAscii.Text = "更新";
            buttonUpdateAscii.Click += buttonUpdateAscii_Click;
            // 
            // splitContainerRight
            // 
            splitContainerRight.Dock = DockStyle.Fill;
            splitContainerRight.Location = new Point(0, 0);
            splitContainerRight.Margin = new Padding(2);
            splitContainerRight.Name = "splitContainerRight";
            splitContainerRight.Orientation = Orientation.Horizontal;
            // 
            // splitContainerRight.Panel1
            // 
            splitContainerRight.Panel1.Controls.Add(tableLayoutPanelTopInputs);
            // 
            // splitContainerRight.Panel2
            // 
            splitContainerRight.Panel2.Controls.Add(_txtScr);
            splitContainerRight.Size = new Size(172, 305);
            splitContainerRight.SplitterDistance = 216;
            splitContainerRight.SplitterWidth = 3;
            splitContainerRight.TabIndex = 0;
            // 
            // tableLayoutPanelTopInputs
            // 
            tableLayoutPanelTopInputs.ColumnCount = 2;
            tableLayoutPanelTopInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tableLayoutPanelTopInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanelTopInputs.Controls.Add(labelClipText, 0, 0);
            tableLayoutPanelTopInputs.Controls.Add(labelOffset, 1, 0);
            tableLayoutPanelTopInputs.Controls.Add(_txtClipText, 0, 1);
            tableLayoutPanelTopInputs.Controls.Add(_txtOffset, 1, 1);
            tableLayoutPanelTopInputs.Dock = DockStyle.Fill;
            tableLayoutPanelTopInputs.Location = new Point(0, 0);
            tableLayoutPanelTopInputs.Margin = new Padding(2);
            tableLayoutPanelTopInputs.Name = "tableLayoutPanelTopInputs";
            tableLayoutPanelTopInputs.RowCount = 2;
            tableLayoutPanelTopInputs.RowStyles.Add(new RowStyle());
            tableLayoutPanelTopInputs.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelTopInputs.Size = new Size(172, 216);
            tableLayoutPanelTopInputs.TabIndex = 0;
            // 
            // labelClipText
            // 
            labelClipText.AutoSize = true;
            labelClipText.Location = new Point(2, 0);
            labelClipText.Margin = new Padding(2, 0, 2, 0);
            labelClipText.Name = "labelClipText";
            labelClipText.Size = new Size(56, 17);
            labelClipText.TabIndex = 0;
            labelClipText.Text = "文字数据";
            // 
            // labelOffset
            // 
            labelOffset.AutoSize = true;
            labelOffset.Location = new Point(113, 0);
            labelOffset.Margin = new Padding(2, 0, 2, 0);
            labelOffset.Name = "labelOffset";
            labelOffset.Size = new Size(49, 34);
            labelOffset.TabIndex = 1;
            labelOffset.Text = "偏移/范围";
            // 
            // _txtClipText
            // 
            _txtClipText.Dock = DockStyle.Fill;
            _txtClipText.Location = new Point(2, 36);
            _txtClipText.Margin = new Padding(2);
            _txtClipText.Multiline = true;
            _txtClipText.Name = "_txtClipText";
            _txtClipText.ScrollBars = ScrollBars.Vertical;
            _txtClipText.Size = new Size(107, 178);
            _txtClipText.TabIndex = 2;
            // 
            // _txtOffset
            // 
            _txtOffset.Dock = DockStyle.Fill;
            _txtOffset.Location = new Point(113, 36);
            _txtOffset.Margin = new Padding(2);
            _txtOffset.Name = "_txtOffset";
            _txtOffset.Size = new Size(57, 23);
            _txtOffset.TabIndex = 3;
            // 
            // _txtScr
            // 
            _txtScr.AcceptsReturn = true;
            _txtScr.AcceptsTab = true;
            _txtScr.Dock = DockStyle.Fill;
            _txtScr.Font = new Font("Consolas", 9F);
            _txtScr.Location = new Point(0, 0);
            _txtScr.Margin = new Padding(2);
            _txtScr.Multiline = true;
            _txtScr.Name = "_txtScr";
            _txtScr.ScrollBars = ScrollBars.Both;
            _txtScr.Size = new Size(172, 86);
            _txtScr.TabIndex = 0;
            _txtScr.WordWrap = false;
            _txtScr.TextChanged += _txtScr_TextChanged;
            // 
            // labelHelp
            // 
            labelHelp.Dock = DockStyle.Fill;
            labelHelp.Location = new Point(10, 418);
            labelHelp.Margin = new Padding(2, 0, 2, 0);
            labelHelp.Name = "labelHelp";
            labelHelp.Padding = new Padding(4);
            labelHelp.Size = new Size(904, 254);
            labelHelp.TabIndex = 3;
            labelHelp.Text = "说明：\r\n- “抓图”会打开固定尺寸选区。\r\n- “截屏抓图”会打开自由拖拽选区。\r\n- 捕获窗口内可做二值化、裁边、导出、绑定窗口、保存图片等操作。\r\n- 当前迁移版本优先保证工作流闭环与模板格式兼容。";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(924, 680);
            Controls.Add(tableLayoutPanelRoot);
            Margin = new Padding(2);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "抓图生成字库及找字代码";
            tableLayoutPanelRoot.ResumeLayout(false);
            tableLayoutPanelRoot.PerformLayout();
            flowLayoutPanelHotkey.ResumeLayout(false);
            flowLayoutPanelHotkey.PerformLayout();
            flowLayoutPanelCommands.ResumeLayout(false);
            flowLayoutPanelCommands.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_numWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numHeight).EndInit();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            tableLayoutPanelAscii.ResumeLayout(false);
            tableLayoutPanelAscii.PerformLayout();
            flowLayoutPanelAsciiButtons.ResumeLayout(false);
            flowLayoutPanelAsciiButtons.PerformLayout();
            splitContainerRight.Panel1.ResumeLayout(false);
            splitContainerRight.Panel2.ResumeLayout(false);
            splitContainerRight.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).EndInit();
            splitContainerRight.ResumeLayout(false);
            tableLayoutPanelTopInputs.ResumeLayout(false);
            tableLayoutPanelTopInputs.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
