using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Interop;
using System.Threading;

namespace iiiwave.MatManLib
{
    public partial class DropDownRefControl : UserControl
    {
        private Excel.Application   m_application;
	    private Excel.Workbook      m_workbook;
	    private Excel.Worksheet     m_worksheet;

	    private RefEditState        m_displayState;
	    private bool                m_isValidAddress;

	    private bool                m_useAddress;
	    public  event AfterResizeEventHandler OnAfterResize;
	    public  delegate void AfterResizeEventHandler(object sender, AfterResizeEventArgs e);

	    public  event BeforeResizeEventHandler OnBeforeResize;
	    public  delegate void BeforeResizeEventHandler(object sender, BeforeResizeEventArgs e);
        
	    private string[]            m_listOfValueStrings;
        
        public DropDownRefControl()
        {
            InitializeComponent();
        }

        private void _SheetChange(object Sh, Excel.Range Target)
	    {
		    this.m_worksheet = (Excel.Worksheet)Sh;

		    if (Target != null) 
            {
			    string address   =  ((Excel.Range)Target.Cells[1, 1]).Address[false, false, Excel.XlReferenceStyle.xlA1, false, Type.Missing];
			    string value     =  ((Excel.Range)Target.Cells[1, 1]).Value.ToString();

			    try 
                {
				    if (object.ReferenceEquals(this.ActiveControl, this.uxAddressTextBox)) 
                    {
					    this.uxAddressTextBox.Text = address;

					    // Find the index of the value if it exists - the hard way
					    //string[] values  =  this.uxValueDropDownList.Items.Cast<string>().ToArray(); 
					    //int      index   =  Array.FindIndex(values, p => p == value); // if item exists, index in the list is returned, else -1

					    int index = this.uxValueDropDownList.FindStringExact(value);

					    if (index > -1) 
                        {
						    this.uxValueDropDownList.SelectedIndex = index;
					    } 
                        else 
                        {
						    this.uxValueDropDownList.SelectedIndex = 0;
					    }
				    }

			    } 
                catch (Exception exp) 
                {
			    }
		    }
	    }

	    private void _CellSelectionChange(Excel.Range Target)
	    {
		    if (Target != null) 
            {
			    string address  =  ((Excel.Range)Target.Cells[1, 1]).Address[false, false, Excel.XlReferenceStyle.xlA1, false, Type.Missing];
			    string value    =  ((Excel.Range)Target.Cells[1, 1]).Value.ToString();

			    try 
                {
				    if (object.ReferenceEquals(this.ActiveControl, this.uxAddressTextBox)) 
                    {
					    this.uxAddressTextBox.Text = address;

					    // Find the index of the value if it exists - the hard way (keep for reference - debug only)
					    //string[] values  =  this.uxValueDropDownList.Items.Cast<string>().ToArray(); 
					    //int      index   =  Array.FindIndex(values, p => p == value); // if item exists, index in the list is returned, else -1

					    // Find the index of the value if it exists - the easy way
					    int index = this.uxValueDropDownList.FindStringExact(value);

					    if (index > -1) 
                        {
						    this.uxValueDropDownList.SelectedIndex = index;
					    } 
                        else 
                        {
						    this.uxValueDropDownList.SelectedIndex = 0;
					    }
				    }

			    } 
                catch (Exception exp) 
                {
			    }
		    }
	    }

	    private void uxAddressTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	    {
		    if (e.KeyCode == Keys.F4) 
            {
	            //MessageBox.Show("You Pressed the: F4 Key")
	            string cellAddress = uxAddressTextBox.Text;
	            try 
                {
		            Excel.Range cell              =  m_worksheet.Range[cellAddress];
		            int         col               =  0;
		            int         row               =  0;
		            bool        isRowAbsolute     =  false;
		            bool        isColumnAbsolute  =  false;

		            if (cell.Address[true, false] == cellAddress) 
                    {
			            isRowAbsolute = true;
			            isColumnAbsolute = false;
		            } 
                    else if (cell.Address[false, true] == cellAddress) 
                    {
			            isRowAbsolute = false;
			            isColumnAbsolute = true;
		            } 
                    else if (cell.Address[true, true] == cellAddress) 
                    {
			            isRowAbsolute = true;
			            isColumnAbsolute = true;
		            } else if (cell.Address[false, false] == cellAddress) 
                    {
			            isRowAbsolute = false;
			            isColumnAbsolute = false;
		            }

		            if (!isRowAbsolute && !isColumnAbsolute) 
                    {
			            cellAddress = cell.Address[false, true];
		            } else if (!isRowAbsolute && isColumnAbsolute) 
                    {
			            cellAddress = cell.Address[true, false];
		            } else if (isRowAbsolute && !isColumnAbsolute) 
                    {
			            cellAddress = cell.Address[true, true];
		            } else if (isRowAbsolute && isColumnAbsolute) 
                    {
			            cellAddress = cell.Address[false, false];
		            }

		            uxAddressTextBox.Text = cellAddress;

	            } 
                catch (Exception ex) 
                {
		            MessageBox.Show(ex.Message);
	            }
            }

            base.OnKeyDown(e);
	    }

	    protected override void OnLoad(EventArgs e)
	    {
		    this.uxIsAddressCheckBox.Text = iiiwave.MatManLib.Localization.Localize.DropDownRefControl_uxIsAddressCheckBox_Text;

		    base.OnLoad(e);
	    }


	    protected override void OnHandleDestroyed(EventArgs e)
	    {
		    if (this.IsHandleCreated) 
            {
			    if (this.m_workbook != null) 
                {
				    this.m_workbook.SheetActivate     -=  _SheetActivate;
				    this.m_workbook.SheetDeactivate   -=  _SheetDeactivate;
				    this.m_workbook.SheetChange       -=  _SheetChange;
				    this.m_worksheet.SelectionChange  -=  _CellSelectionChange;
			    }

			    base.OnHandleDestroyed(e);
		    }
	    }

	    private void _SheetActivate(object Sh)
	    {
		    this.m_worksheet = (Excel.Worksheet)Sh;
		    this.m_worksheet.SelectionChange += _CellSelectionChange;
	    }

	    private void _SheetDeactivate(object Sh)
	    {
		    this.m_worksheet = (Excel.Worksheet)Sh;
		    this.m_worksheet.SelectionChange -= _CellSelectionChange;
	    }


	    private void uxRefEditButton_Click(object sender, EventArgs e)
	    {
		    BeforeResizeEventArgs args = new BeforeResizeEventArgs(this.m_displayState);
		    if (OnBeforeResize != null) 
            {
			    OnBeforeResize(this, args);
		    }
		    if (args.Cancel) 
            {
			    return;
		    }

		    // Hides - Shows
		    foreach (Control c in ParentForm.Controls) 
            {
			    c.Visible = this.m_displayState.IsParentMinimized;
		    }

		    // ensure control is visible
		    this.Visible = true;

		    // fix for Tab Controls et al
		    if (this.m_displayState.ActualParent == null) 
            {
			    this.m_displayState.ActualParent = this.Parent;
		    }

		    if (!this.m_displayState.IsParentMinimized) 
            {
			    if (!object.ReferenceEquals(this.m_displayState.ActualParent.GetType(), typeof(Form))) 
                {
				    this.m_displayState.ControlIndex = this.m_displayState.ActualParent.Controls.IndexOf(this);
				    this.ParentForm.Controls.Add(this);
			    }

			    // Set the button's image to minimized
			    this.uxRefEditButton.Image = Properties.Resources.imgMinimized;

			    // Store the current state of the form
			    this.m_displayState.ParentClientSize = ParentForm.ClientSize;
			    this.m_displayState.ParentPrevBorder = ParentForm.FormBorderStyle;
			    this.m_displayState.ShowParentControlBox = ParentForm.ControlBox;

			    // Store the state of the control
			    this.m_displayState.ControlPrevX = this.Left;
			    this.m_displayState.ControlPrevY = this.Top;
			    this.m_displayState.ControlAnchor = this.Anchor;

			    // Set the new state minimized state of the form
			    this.ParentForm.ClientSize = new Size(this.Width, this.Height);
			    this.ParentForm.FormBorderStyle = FormBorderStyle.FixedDialog;
			    this.ParentForm.ControlBox = false;

			    // set location of control
			    this.Anchor = AnchorStyles.Left;
			    this.Left = 0;
			    this.Top = 0;

			    // Revised minimized status
			    this.m_displayState.IsParentMinimized = true;

			    this.uxAddressTextBox.Focus();
		    } 
            else 
            {
			    // Parent is not minimized
			    if (!object.ReferenceEquals(this.m_displayState.ActualParent.GetType(), typeof(Form))) 
                {
				    this.ParentForm.Controls.Remove(this);
				    this.m_displayState.ActualParent.Controls.Add(this);
				    this.m_displayState.ActualParent.Controls.SetChildIndex(this, this.m_displayState.ControlIndex);
			    }

			    this.uxRefEditButton.Image = Properties.Resources.imgMaximized;

			    // Set the form to the stored state
			    this.ParentForm.ClientSize = this.m_displayState.ParentClientSize;
			    this.ParentForm.FormBorderStyle = this.m_displayState.ParentPrevBorder;
			    this.ParentForm.ControlBox = this.m_displayState.ShowParentControlBox;

			    // Set the control to the stored state
			    this.Anchor = this.m_displayState.ControlAnchor;
			    this.Left = this.m_displayState.ControlPrevX;
			    this.Top = this.m_displayState.ControlPrevY;

			    // Revised minimized status
			    this.m_displayState.IsParentMinimized = false;

			    this.uxAddressTextBox.Focus();
		    }

		    if (OnAfterResize != null) 
            {
			    OnAfterResize(this, new AfterResizeEventArgs(this.m_displayState));
		    }
	    }

	    private void uxAddressTextBox_TextChanged(object sender, EventArgs e)
	    {
		    string address = this.uxAddressTextBox.Text;
		    try 
            {
			    if (this.m_worksheet.Range[address] != null) 
                {
				    this.m_isValidAddress = true;
			    } 
                else 
                {
				    this.m_isValidAddress = false;
			    }
		    } 
            catch (Exception exp) 
            {
			    this.m_isValidAddress = false;
		    }
	    }

	    //Private Sub uxIsAddressCheckBox_CheckedChanged( sender As Object,  e As EventArgs) Handles uxIsAddressCheckBox.CheckedChanged
	    //	If(Me.uxIsAddressCheckBox.Checked) Then
	    //		Me.m_useAddress = True 
	    //		Me.uxValueDropDownList.Enabled = False
	    //		Me.uxAddressTextBox.Enabled = True
	    //	Else
	    //	    Me.uxAddressTextBox.Text = String.Empty 
	    //		Me.m_useAddress = False
	    //		Me.uxValueDropDownList.Enabled = True
	    //		Me.uxAddressTextBox.Enabled = False
	    //	End If
	    //End Sub

	    private void uxIsAddressCheckBox_CheckedChanged(object sender, EventArgs e)
	    {
		    if ((this.uxIsAddressCheckBox.Checked)) 
            {
			    this.uxAddressTextBox.Enabled = true;
			    this.uxAddressTextBox.Focus();

			    try 
                {
				    Excel.Range myRange = (Excel.Range)this.m_application.Selection;
				    if (myRange != null) 
                    {
					    try 
                        {
						    string address = ((Excel.Range)myRange.Cells[1, 1]).Address[false, false, Excel.XlReferenceStyle.xlA1, false, Type.Missing];
						    string value = ((Excel.Range)myRange.Cells[1, 1]).Value.ToString();

						    this.uxAddressTextBox.Text = address;

					    } 
                        catch (Exception exp) 
                        {
						    //MessageBox.Show("Address: " & address & " Value: " & value & "  " & exp.Message)
					    }
				    }

			    } 
                catch (Exception ex) 
                {
			    }


			    this.uxValueDropDownList.Enabled = false;
			    this.uxValueDropDownList.Text = string.Empty;
			    this.m_useAddress = true;
		    } 
            else 
            {
			    this.uxAddressTextBox.Text = string.Empty;
			    this.uxAddressTextBox.Enabled = false;
			    this.uxValueDropDownList.Enabled = true;
			    this.m_useAddress = false;
		    }
	    }

	    [Browsable(true)]
	    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	    public Excel.Application _xlAPP 
        {
		    get 
            { 
                return this.m_application; 
            }
		    set 
            {
			    this.m_application = value;

			    if (this.m_application != null) 
                {
				    m_workbook    = (Excel.Workbook)m_application.ActiveWorkbook;
				    m_worksheet   = (Excel.Worksheet)m_application.ActiveSheet;

				    this.m_workbook.SheetActivate     +=  this._SheetActivate;
				    this.m_workbook.SheetDeactivate   +=  this._SheetDeactivate;
				    this.m_worksheet.SelectionChange  +=  this._CellSelectionChange;
				    this.m_workbook.SheetChange       +=  this._SheetChange;
			    }
		    }
	    }

	    [Browsable(true)]
	    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	    public string[] ListOfValueStrings 
        {
		    get 
            {
			    IEnumerable<string> stringsVar = this.uxValueDropDownList.Items.Cast<string>();
			    return stringsVar.ToArray();
		    }
		    set 
            { 
                this.uxValueDropDownList.Items.AddRange(value); 
            }
	    }

	    [Browsable(true)]
	    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	    public string ParameterLabelText 
        {
		    get 
            { 
                return this.uxParameterLabel.Text; 
            }
		    set 
            { 
                this.uxParameterLabel.Text = value; 
            }
	    }

	    [Browsable(true)]
	    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	    public string AddressText 
        {
		    get 
            { 
                return this.uxAddressTextBox.Text; 
            }
		    set 
            { 
                this.uxAddressTextBox.Text = value; 
            }
	    }

	    public bool IsValidAddress 
        {
		    get 
            {
			    this.m_isValidAddress = false;

			    try 
                {
				    if (this.m_worksheet.Range[this.uxAddressTextBox.Text] != null) 
                    {
					    this.m_isValidAddress = true;
				    }
			    } 
                catch (Exception exp) 
                {
			    }

			    return this.m_isValidAddress;
		    }
	    }

	    public bool UseAddress 
        {
		    get 
            { 
                return this.m_useAddress; 
            }
		    set 
            {
			    if (value != true) 
                {
				    this.m_useAddress = false;
				    this.uxIsAddressCheckBox.Checked = false;
			    } 
                else 
                {
				    this.m_useAddress = true;
				    this.uxIsAddressCheckBox.Checked = true;
			    }
		    }
	    }

	    public string SelectedItem 
        {
		    get 
            {
			    if (this.uxValueDropDownList.SelectedItem != null) 
                {
				    return this.uxValueDropDownList.SelectedItem.ToString();
			    }
			    return string.Empty;
		    }
    //Dim i As Integer = 0
    //For Each s As String In Me.uxValueDropDownList.Items
    //	If Me.uxValueDropDownList.Items(i).ToString().Chars(0).Equals(Value)
			    //End If 
			    //	i = i + 1
			    //Next
		    set 
            { 
                this.uxValueDropDownList.SelectedItem = value; 
            }
	    }

	    public int SetSelectedIndex 
        {
		    set 
            { 
                this.uxValueDropDownList.SelectedIndex = value; 
            }
	    }

    }
}
