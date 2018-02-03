using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace F16CPD.UI.Forms
{
    /// <summary>
    ///   Extension of the basic Windows Form, allowing forms that do not posess a titlebar area to be draggable/moveable
    /// </summary>
    public class DraggableForm : Form
    {
        #region Declarations

        protected object lockObject = new object();
        protected bool Drag;
        protected Point StartPoint = new Point(0, 0);

        /// <SUMMARY>
        ///   Required designer variable.
        /// </SUMMARY>
        protected IContainer components;

        protected bool draggable = true;
        protected string exclude_list = "";

        #endregion

        #region Constructor , Dispose

        public DraggableForm()
        {
            InitializeComponent();
        }

        /// <SUMMARY>
        ///   Clean up any resources being used.
        /// </SUMMARY>
        /// true if managed resources should be disposed; otherwise, false.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Common.Util.DisposeObject(components);
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <SUMMARY>
        ///   Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </SUMMARY>
        protected void InitializeComponent()
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

        protected void Control_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        protected void Control_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        protected void Control_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (draggable && Cursor == Cursors.SizeAll)
            {
                //
                //On Mouse Down set the flag drag=true and 
                //Store the clicked point to the start_point variable
                //
                Drag = true;
                StartPoint = new Point(e.X, e.Y);
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
                Drag = false;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //
            //If drag = true, drag the form
            //
            if (Drag && draggable)
            {
                var p1 = new Point(e.X, e.Y);
                var p2 = PointToScreen(p1);
                var p3 = new Point(p2.X - StartPoint.X,
                                   p2.Y - StartPoint.Y);
                DesktopLocation = p3;
                Location = p3;
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
            set
            {
                lock (lockObject)
                {
                    draggable = value;
                }
            }
            get { return draggable; }
        }

        #endregion
    }
}