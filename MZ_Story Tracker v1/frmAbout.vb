Public Class frmAbout
    Private Sub tmrScroll_Tick(sender As Object, e As EventArgs) Handles tmrScroll.Tick
        ' This makes it move up
        lblScrollText.Top -= 1

        ' If the bottom of the label goes off the top of the panel, reset it
        If lblScrollText.Bottom < 0 Then
            lblScrollText.Top = pnlContainer.Height
        End If
    End Sub

    Private Sub frmAbout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 1. FORCE the label to wrap at the Panel's width
        lblScrollText.AutoSize = False
        lblScrollText.Width = pnlContainer.Width
        lblScrollText.TextAlign = ContentAlignment.TopCenter ' Keeps it looking professional

        ' 2. Assign the text (it will now wrap vertically instead of hanging off)
        lblScrollText.Text = "MZ Story Tracker v1.2" & vbCrLf &
    "The Ultimate Narrative Management Tool" & vbCrLf & vbCrLf &
    "OVERVIEW" & vbCrLf &
    "MZ Story Tracker is a specialized data management solution designed to streamline the creative workflow for writers, dungeon masters, and world-builders. Built on a high-performance XML-driven backend, this application provides a centralized hub for archiving critical narrative data—from world maps and pivotal events to granular story beats." & vbCrLf & vbCrLf &
    "KEY FEATURES" & vbCrLf &
    "* Dynamic Data Indexing: Instantaneously retrieve and edit records through an intuitive, real-time search interface." & vbCrLf &
    "* State Persistence: Integrated workspace memory ensures your customized UI layout and window position are preserved across sessions." & vbCrLf &
    "* Robust Data Integrity: Built with industrial-grade error handling and automated validation to ensure your story data remains secure." & vbCrLf &
    "* Modular Architecture: Lightweight and portable, storing all mission-critical assets within a dedicated local directory for easy backup." & vbCrLf & vbCrLf &
    "TECHNICAL SUPPORT" & vbCrLf &
    "{GITHUB LINK}"

        ' 3. Recalculate height so it doesn't cut off the bottom of the text
        Using g As Graphics = lblScrollText.CreateGraphics()
            Dim size As SizeF = g.MeasureString(lblScrollText.Text, lblScrollText.Font, lblScrollText.Width)
            lblScrollText.Height = CInt(Math.Ceiling(size.Height))
        End Using

        ' 4. Start the position at the bottom and launch
        lblScrollText.Top = pnlContainer.Height
        tmrScroll.Start()
    End Sub
End Class