Imports System.Windows.Forms


Class Program
    Shared Function Main As Integer
	Dim f As New Form1 ()
        If f.b1 Is Nothing Then
		Console.WriteLine ("Failed") 
		Return 1
	Else
		Console.WriteLine ("Success")
		Return 0
	End If
    End Function
End Class


Partial Class Form1

End Class


<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    Public b1 As Button

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        b1 = New Button
        b1.Text = "yippee#3"
        Me.Controls.Add(b1)
    End Sub
End Class

