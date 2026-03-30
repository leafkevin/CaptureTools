namespace WinFormsApp1
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
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
            _txtClipText = new TextBox();
            labelOffset = new Label();
            _txtOffset = new TextBox();
            _txtScr = new TextBox();
            labelHelp = new Label();
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
            ((System.ComponentModel.ISupportInitialize)_numWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_numHeight).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanelRoot
            // 
            tableLayoutPanelRoot.ColumnCount = 1;
            tableLayoutPanelRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelRoot.Dock = DockStyle.Fill;
            tableLayoutPanelRoot.Padding = new Padding(12);
            tableLayoutPanelRoot.RowCount = 4;
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle());
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle());
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            tableLayoutPanelRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            tableLayoutPanelRoot.Controls.Add(flowLayoutPanelHotkey, 0, 0);
            tableLayoutPanelRoot.Controls.Add(flowLayoutPanelCommands, 0, 1);
            tableLayoutPanelRoot.Controls.Add(splitContainerMain, 0, 2);
            tableLayoutPanelRoot.Controls.Add(labelHelp, 0, 3);
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
            flowLayoutPanelHotkey.WrapContents = false;
            // 
            // labelNowHotkey
            // 
            labelNowHotkey.AutoSize = true;
            labelNowHotkey.Padding = new Padding(0, 8, 0, 0);
            labelNowHotkey.Text = "截屏热键";
            // 
            // _txtNowHotkey
            // 
            _txtNowHotkey.ReadOnly = true;
            _txtNowHotkey.Size = new Size(140, 30);
            // 
            // labelSetHotkey
            // 
            labelSetHotkey.AutoSize = true;
            labelSetHotkey.Padding = new Padding(12, 8, 0, 0);
            labelSetHotkey.Text = "设置热键";
            // 
            // _cmbHotkey
            // 
            _cmbHotkey.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbHotkey.Items.AddRange(new object[] { "", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "PrintScreen", "Scroll", "Pause" });
            _cmbHotkey.Size = new Size(160, 32);
            // 
            // buttonApplyHotkey
            // 
            buttonApplyHotkey.AutoSize = true;
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
            // 
            // labelWidth
            // 
            labelWidth.AutoSize = true;
            labelWidth.Padding = new Padding(0, 8, 0, 0);
            labelWidth.Text = "宽度";
            // 
            // _numWidth
            // 
            _numWidth.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _numWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numWidth.Size = new Size(80, 30);
            _numWidth.Value = new decimal(new int[] { 71, 0, 0, 0 });
            // 
            // labelHeight
            // 
            labelHeight.AutoSize = true;
            labelHeight.Padding = new Padding(12, 8, 0, 0);
            labelHeight.Text = "高度";
            // 
            // _numHeight
            // 
            _numHeight.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _numHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numHeight.Size = new Size(80, 30);
            _numHeight.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // _chkAddFunc
            // 
            _chkAddFunc.AutoSize = true;
            _chkAddFunc.Checked = true;
            _chkAddFunc.CheckState = CheckState.Checked;
            _chkAddFunc.Padding = new Padding(12, 8, 0, 0);
            _chkAddFunc.Text = "附加 FindText() 函数";
            // 
            // buttonCapture
            // 
            buttonCapture.AutoSize = true;
            buttonCapture.Text = "抓图";
            buttonCapture.Click += buttonCapture_Click;
            // 
            // buttonCaptureS
            // 
            buttonCaptureS.AutoSize = true;
            buttonCaptureS.Text = "截屏抓图";
            buttonCaptureS.Click += buttonCaptureS_Click;
            // 
            // buttonTest
            // 
            buttonTest.AutoSize = true;
            buttonTest.Text = "测试";
            buttonTest.Click += buttonTest_Click;
            // 
            // buttonCopy
            // 
            buttonCopy.AutoSize = true;
            buttonCopy.Text = "复制";
            buttonCopy.Click += buttonCopy_Click;
            // 
            // buttonGetRange
            // 
            buttonGetRange.AutoSize = true;
            buttonGetRange.Text = "获取屏幕范围";
            buttonGetRange.Click += buttonGetRange_Click;
            // 
            // buttonGetOffset
            // 
            buttonGetOffset.AutoSize = true;
            buttonGetOffset.Text = "获取相对坐标";
            buttonGetOffset.Click += buttonGetOffset_Click;
            // 
            // buttonGetClipOffset
            // 
            buttonGetClipOffset.AutoSize = true;
            buttonGetClipOffset.Text = "获取相对坐标2";
            buttonGetClipOffset.Click += buttonGetClipOffset_Click;
            // 
            // buttonPaste
            // 
            buttonPaste.AutoSize = true;
            buttonPaste.Text = "粘贴";
            buttonPaste.Click += buttonPaste_Click;
            // 
            // buttonTestClip
            // 
            buttonTestClip.AutoSize = true;
            buttonTestClip.Text = "测试2";
            buttonTestClip.Click += buttonTestClip_Click;
            // 
            // buttonCopyOffset
            // 
            buttonCopyOffset.AutoSize = true;
            buttonCopyOffset.Text = "复制2";
            buttonCopyOffset.Click += buttonCopyOffset_Click;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.SplitterDistance = 560;
            splitContainerMain.Panel1.Controls.Add(tableLayoutPanelAscii);
            splitContainerMain.Panel2.Controls.Add(splitContainerRight);
            // 
            // tableLayoutPanelAscii
            // 
            tableLayoutPanelAscii.ColumnCount = 1;
            tableLayoutPanelAscii.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelAscii.Dock = DockStyle.Fill;
            tableLayoutPanelAscii.RowCount = 2;
            tableLayoutPanelAscii.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelAscii.RowStyles.Add(new RowStyle());
            tableLayoutPanelAscii.Controls.Add(_txtMyPic, 0, 0);
            tableLayoutPanelAscii.Controls.Add(flowLayoutPanelAsciiButtons, 0, 1);
            // 
            // _txtMyPic
            // 
            _txtMyPic.Dock = DockStyle.Fill;
            _txtMyPic.Font = new Font("Consolas", 9F, FontStyle.Bold);
            _txtMyPic.Multiline = true;
            _txtMyPic.ScrollBars = ScrollBars.Both;
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
            // 
            // buttonTrimLeft
            // 
            buttonTrimLeft.AutoSize = true;
            buttonTrimLeft.Text = "左删";
            buttonTrimLeft.Click += buttonTrimLeft_Click;
            // 
            // buttonTrimRight
            // 
            buttonTrimRight.AutoSize = true;
            buttonTrimRight.Text = "右删";
            buttonTrimRight.Click += buttonTrimRight_Click;
            // 
            // buttonTrimUp
            // 
            buttonTrimUp.AutoSize = true;
            buttonTrimUp.Text = "上删";
            buttonTrimUp.Click += buttonTrimUp_Click;
            // 
            // buttonTrimDown
            // 
            buttonTrimDown.AutoSize = true;
            buttonTrimDown.Text = "下删";
            buttonTrimDown.Click += buttonTrimDown_Click;
            // 
            // buttonUpdateAscii
            // 
            buttonUpdateAscii.AutoSize = true;
            buttonUpdateAscii.Text = "更新";
            buttonUpdateAscii.Click += buttonUpdateAscii_Click;
            // 
            // splitContainerRight
            // 
            splitContainerRight.Dock = DockStyle.Fill;
            splitContainerRight.Orientation = Orientation.Horizontal;
            splitContainerRight.SplitterDistance = 85;
            splitContainerRight.Panel1.Controls.Add(tableLayoutPanelTopInputs);
            splitContainerRight.Panel2.Controls.Add(_txtScr);
            // 
            // tableLayoutPanelTopInputs
            // 
            tableLayoutPanelTopInputs.ColumnCount = 2;
            tableLayoutPanelTopInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tableLayoutPanelTopInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanelTopInputs.Dock = DockStyle.Fill;
            tableLayoutPanelTopInputs.RowCount = 2;
            tableLayoutPanelTopInputs.RowStyles.Add(new RowStyle());
            tableLayoutPanelTopInputs.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelTopInputs.Controls.Add(labelClipText, 0, 0);
            tableLayoutPanelTopInputs.Controls.Add(labelOffset, 1, 0);
            tableLayoutPanelTopInputs.Controls.Add(_txtClipText, 0, 1);
            tableLayoutPanelTopInputs.Controls.Add(_txtOffset, 1, 1);
            // 
            // labelClipText
            // 
            labelClipText.AutoSize = true;
            labelClipText.Text = "文字数据";
            // 
            // _txtClipText
            // 
            _txtClipText.Dock = DockStyle.Fill;
            _txtClipText.Multiline = true;
            _txtClipText.ScrollBars = ScrollBars.Vertical;
            // 
            // labelOffset
            // 
            labelOffset.AutoSize = true;
            labelOffset.Text = "偏移/范围";
            // 
            // _txtOffset
            // 
            _txtOffset.Dock = DockStyle.Fill;
            // 
            // _txtScr
            // 
            _txtScr.AcceptsReturn = true;
            _txtScr.AcceptsTab = true;
            _txtScr.Dock = DockStyle.Fill;
            _txtScr.Font = new Font("Consolas", 9F, FontStyle.Regular);
            _txtScr.Multiline = true;
            _txtScr.ScrollBars = ScrollBars.Both;
            _txtScr.WordWrap = false;
            _txtScr.TextChanged += _txtScr_TextChanged;
            // 
            // labelHelp
            // 
            labelHelp.Dock = DockStyle.Fill;
            labelHelp.Padding = new Padding(6);
            labelHelp.Text = "说明：\r\n- “抓图”会打开固定尺寸选区。\r\n- “截屏抓图”会打开自由拖拽选区。\r\n- 捕获窗口内可做二值化、裁边、导出、绑定窗口、保存图片等操作。\r\n- 当前迁移版本优先保证工作流闭环与模板格式兼容。";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1320, 960);
            Controls.Add(tableLayoutPanelRoot);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "抓图生成字库及找字代码";
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
            ((System.ComponentModel.ISupportInitialize)_numWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)_numHeight).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
