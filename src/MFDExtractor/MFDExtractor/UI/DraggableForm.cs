using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Common;

namespace MFDExtractor.UI
{
    /// <summary>
    ///     Extension of the basic Windows Form, allowing forms that do not posess a titlebar area to be draggable/moveable
    /// </summary>
    public class DraggableForm : Form
    {
        #region Declarations

        /// <SUMMARY>
        ///     Required designer variable.
        /// </SUMMARY>
        private readonly IContainer components = null;

        private bool drag;
        private bool draggable = true;
        private string exclude_list = "";
        private Point start_point = new Point(0, 0);

        #endregion

        #region Constructor , Dispose

        public DraggableForm()
        {
            InitializeComponent();
        }

        /// <SUMMARY>
        ///     Clean up any resources being used.
        /// </SUMMARY>
        /// true if managed resources should be disposed; otherwise, false.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Util.DisposeObject(components);
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <SUMMARY>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </SUMMARY>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DraggableForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "DraggableForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "DraggableForm";
            this.ResumeLayout(false);
        }

        #endregion

        #region Overriden Functions

        protected override void OnControlAdded(ControlEventArgs e)
        {
            //
            //Add Mouse Event Handlers for each control added into the form,
            //if Draggable property of the form is set to true and the control
            //name is not in the ExcludeList.Exclude list is the comma seperated
            //list of the Controls for which you do not require the mouse handler 
            //to be added. For Example a button.  
            //
            if (Draggable && (ExcludeList.IndexOf(e.Control.Name, StringComparison.OrdinalIgnoreCase) == -1))
            {
                e.Control.MouseDown += Control_MouseDown;
                e.Control.MouseUp += Control_MouseUp;
                e.Control.MouseMove += Control_MouseMove;
            }
            base.OnControlAdded(e);
        }

        #endregion

        #region Event Handlers

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (draggable)
            {
                //
                //On Mouse Down set the flag drag=true and 
                //Store the clicked point to the start_point variable
                //
                drag = true;
                start_point = new Point(e.X, e.Y);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (draggable)
            {
                //
                //Set the drag flag = false;
                //
                drag = false;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //
            //If drag = true, drag the form
            //
            if (drag && draggable)
            {
                var p1 = new Point(e.X, e.Y);
                Point p2 = PointToScreen(p1);
                var p3 = new Point(p2.X - start_point.X,
                                   p2.Y - start_point.Y);
                DesktopLocation = p3;
            }
            base.OnMouseMove(e);
        }

        #endregion

        #region Properties

        public string ExcludeList
        {
            set { exclude_list = value; }
            get { return exclude_list.Trim(); }
        }

        public bool Draggable
        {
            set { draggable = value; }
            get { return draggable; }
        }

        #endregion
    }
}