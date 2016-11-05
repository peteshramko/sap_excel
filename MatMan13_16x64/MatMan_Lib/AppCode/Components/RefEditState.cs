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

namespace iiiwave.MatManLib
{
    public class AfterResizeEventArgs : EventArgs
    {
	    public RefEditState DisplayState 
        {
		    get 
            { 
                return m_DisplayState; 
            }
		    set 
            { 
                m_DisplayState = value; 
            }
	    }

	    private RefEditState m_DisplayState;
	
        public AfterResizeEventArgs(RefEditState _DisplayState)
	    {
		    this.DisplayState = _DisplayState;
	    }
    }

    public class BeforeResizeEventArgs : System.ComponentModel.CancelEventArgs
    {
	    public RefEditState DisplayState 
        {
		    get 
            { 
                return m_DisplayState; 
            }
		    set 
            { 
                m_DisplayState = value; 
            }
	    }

	    private RefEditState m_DisplayState;
	    public BeforeResizeEventArgs(RefEditState _DisplayState)
	    {
		    this.DisplayState = _DisplayState;
	    }
    }
    
    public struct RefEditState
    {
	    public Size             ParentClientSize;
	    public bool             IsParentMinimized;
	    public FormBorderStyle  ParentPrevBorder;
	    public bool             ShowParentControlBox;
	    public int              ControlPrevX;
	    public int              ControlPrevY;
	    public AnchorStyles     ControlAnchor;
	    public Control          ActualParent;
	    public int              ControlIndex;
    }
}
