Imports System.IO
Imports System.Xml.Linq

Public Class Form1
    Private filePath As String = "C:\MZ_Story Tracker\Data\dat.xml"
    Private logPath As String = "C:\MZ_Story Tracker\Data\errorlog.txt"
    Private settingsPath As String = "C:\MZ_Story Tracker\settings.xml"

    ''' <summary>
    ''' Initializes the application, ensures the data directory exists, 
    ''' and loads the existing story list.
    ''' </summary>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSettings() ' <-- Add this first
        RefreshMapDropdown()

        Dim x As String = My.Application.Info.Version.ToString
        Me.Text = "MZ_Story Tracker v" & x

        Try
            Directory.CreateDirectory(Path.GetDirectoryName(filePath))
            If Not File.Exists(filePath) Then
                Dim doc As New XDocument(New XElement("Stories"))
                doc.Save(filePath)
            End If
            LoadList()
        Catch ex As Exception
            LogError("Initialization Error", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Appends system errors to a local text file for diagnostic purposes.
    ''' </summary>
    ''' <param name="context">The function or area where the error occurred.</param>
    ''' <param name="message">The specific exception message.</param>
    Private Sub LogError(context As String, message As String)
        Try
            Dim logMsg As String = $"{DateTime.Now} | {context} | {message}{Environment.NewLine}"
            File.AppendAllText(logPath, logMsg)
            MessageBox.Show($"An error occurred ({context}). Check the error log for details.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch
            ' If logging itself fails, just show a message box
            MessageBox.Show("Critical failure: Could not write to log file.")
        End Try
    End Sub

    Private Function IsFormValid() As Boolean
        If String.IsNullOrWhiteSpace(txTitle.Text) OrElse
           String.IsNullOrWhiteSpace(txMap.Text) OrElse
           String.IsNullOrWhiteSpace(txEvent.Text) Then
            MessageBox.Show("Please fill in the Title, Map, and Event fields.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function

    ''' <summary>
    ''' Updates the story list based on search criteria and completion status.
    ''' Filters results by title and optionally shows only completed or incomplete stories.
    ''' </summary>
    Private Sub LoadList(Optional titleFilter As String = "",
                     Optional showOnlyCompleted As Boolean = False,
                     Optional mapFilter As String = "All")
        Try
            lstRange.Items.Clear()
            Dim doc = XDocument.Load(filePath)

            Dim stories = From s In doc.Root.Elements("Story")
                          Let title = s.Element("Title").Value
                          Let mapName = s.Element("Map").Value
                          Let completed = s.Element("Completed").Value.ToLower() = "true"
                          Where (String.IsNullOrEmpty(titleFilter) OrElse title.ToLower().Contains(titleFilter.ToLower())) _
                      And (Not showOnlyCompleted OrElse completed) _
                      And (mapFilter = "All" OrElse mapName = mapFilter)
                          Select title

            For Each title In stories
                lstRange.Items.Add(title)
            Next

            lblCount.Text = $"Records found: {lstRange.Items.Count}"
        Catch ex As Exception
            LogError("LoadList Filter Error", ex.Message)
        End Try
    End Sub

    Private Sub RefreshMapDropdown()
        Try
            ' 1. Check if file exists first to avoid crash on very first load
            If Not File.Exists(filePath) Then Exit Sub

            Dim doc = XDocument.Load(filePath)

            ' 2. Get unique map names
            Dim uniqueMaps = (From s In doc.Root.Elements("Story")
                              Where s.Element("Map") IsNot Nothing
                              Select s.Element("Map").Value).Distinct().OrderBy(Function(m) m).ToList()

            ' 3. Detach handler to prevent recursive/premature calls to LoadList
            RemoveHandler cmbMapFilter.SelectedIndexChanged, AddressOf cmbMapFilter_SelectedIndexChanged

            cmbMapFilter.Items.Clear()
            cmbMapFilter.Items.Add("All")

            For Each m In uniqueMaps
                cmbMapFilter.Items.Add(m)
            Next

            ' 4. Ensure there is at least "All" before setting index
            If cmbMapFilter.Items.Count > 0 Then
                cmbMapFilter.SelectedIndex = 0
            End If

            ' 5. Re-attach handler
            AddHandler cmbMapFilter.SelectedIndexChanged, AddressOf cmbMapFilter_SelectedIndexChanged
        Catch ex As Exception
            LogError("Map Dropdown Error", ex.Message)
        End Try
    End Sub

    Private Sub cmbMapFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMapFilter.SelectedIndexChanged
        ' Safety Check: If nothing is selected (or list is empty), don't try to filter
        If cmbMapFilter.SelectedItem Is Nothing Then Exit Sub

        LoadList(txSearch.Text, cbShowOnlyCompleted.Checked, cmbMapFilter.SelectedItem.ToString())
    End Sub

    Private Sub cbShowOnlyCompleted_CheckedChanged(sender As Object, e As EventArgs) Handles cbShowOnlyCompleted.CheckedChanged
        ' Pass both the current search text and the checkbox state
        LoadList(txSearch.Text, cbShowOnlyCompleted.Checked)
    End Sub

    ''' <summary>
    ''' Validates form input and either creates a new record or updates 
    ''' an existing one based on the Title field.
    ''' </summary>
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not IsFormValid() Then Exit Sub

        Try
            Dim doc = XDocument.Load(filePath)
            Dim existing = doc.Root.Elements("Story").FirstOrDefault(Function(x) x.Element("Title").Value = txTitle.Text)

            If existing IsNot Nothing Then
                existing.Element("Map").Value = txMap.Text
                existing.Element("Event").Value = txEvent.Text
                existing.Element("Completed").Value = cbCompleted.Checked.ToString()
                existing.Element("Notes").Value = rtbNotes.Text
            Else
                Dim newStory As New XElement("Story",
                    New XElement("Title", txTitle.Text),
                    New XElement("Map", txMap.Text),
                    New XElement("Event", txEvent.Text),
                    New XElement("Completed", cbCompleted.Checked.ToString()),
                    New XElement("Notes", rtbNotes.Text)
                )
                doc.Root.Add(newStory)
            End If

            doc.Save(filePath)
            LoadList()
            MessageBox.Show("Data Saved Successfully!")
        Catch ex As Exception
            LogError("Save Error", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Retrieves and displays the detailed data for a story when its title 
    ''' is selected in the ListBox.
    ''' </summary>
    Private Sub lstRange_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstRange.SelectedIndexChanged
        If lstRange.SelectedItem Is Nothing Then Exit Sub

        Try
            Dim doc = XDocument.Load(filePath)
            Dim story = doc.Root.Elements("Story").FirstOrDefault(Function(x) x.Element("Title").Value = lstRange.SelectedItem.ToString())

            If story IsNot Nothing Then
                txTitle.Text = story.Element("Title").Value
                txMap.Text = story.Element("Map").Value
                txEvent.Text = story.Element("Event").Value
                cbCompleted.Checked = Boolean.Parse(story.Element("Completed").Value)
                rtbNotes.Text = story.Element("Notes").Value
            End If
        Catch ex As Exception
            LogError("Selection Error", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Removes the selected story from the XML database after user confirmation.
    ''' </summary>
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If lstRange.SelectedItem Is Nothing Then
            MessageBox.Show("Select a record to delete.")
            Return
        End If

        Try
            Dim result = MessageBox.Show("Are you sure you want to delete this?", "Confirm", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim doc = XDocument.Load(filePath)
                doc.Root.Elements("Story").Where(Function(x) x.Element("Title").Value = lstRange.SelectedItem.ToString()).Remove()
                doc.Save(filePath)
                ClearFields()
                LoadList()
            End If
        Catch ex As Exception
            LogError("Delete Error", ex.Message)
        End Try
    End Sub

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        ClearFields()
    End Sub

    Private Sub ClearFields()
        txTitle.Clear()
        txMap.Clear()
        txEvent.Clear()
        cbCompleted.Checked = False
        rtbNotes.Clear()
        lstRange.ClearSelected()
    End Sub

    ' Triggers every time you type a letter in the search bar for big lists
    Private Sub txSearch_TextChanged(sender As Object, e As EventArgs) Handles txSearch.TextChanged
        LoadList(txSearch.Text, cbShowOnlyCompleted.Checked)

    End Sub

    ''' <summary>
    ''' Captures the current X and Y screen coordinates of the form and 
    ''' persists them to a separate settings.xml file.
    ''' </summary>
    Private Sub SaveSettings()
        Try
            ' Save current X and Y coordinates
            Dim settings As New XDocument(
                New XElement("Settings",
                    New XElement("WindowLocation",
                        New XElement("X", Me.Location.X),
                        New XElement("Y", Me.Location.Y)
                    )
                )
            )
            settings.Save(settingsPath)
        Catch ex As Exception
            LogError("Save Settings Error", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Retrieves saved window coordinates from settings.xml and restores the form's 
    ''' position. Includes a safety check to ensure the coordinates are within 
    ''' the boundaries of currently active screens.
    ''' </summary>
    Private Sub LoadSettings()
        Try
            If File.Exists(settingsPath) Then
                Dim doc = XDocument.Load(settingsPath)
                Dim x = Integer.Parse(doc.Root.Element("WindowLocation").Element("X").Value)
                Dim y = Integer.Parse(doc.Root.Element("WindowLocation").Element("Y").Value)

                ' Set the location
                Dim newPoint As New Point(x, y)

                ' Safety check: Ensure the point is actually on a visible screen
                Dim isVisible As Boolean = False
                For Each scr In Screen.AllScreens
                    If scr.WorkingArea.Contains(newPoint) Then
                        isVisible = True
                        Exit For
                    End If
                Next

                If isVisible Then
                    Me.StartPosition = FormStartPosition.Manual
                    Me.Location = newPoint
                End If
            End If
        Catch ex As Exception
            ' We don't necessarily need a popup for settings failure, just log it
            LogError("Load Settings Error", ex.Message)
        End Try
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        SaveSettings()
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        frmAbout.Show()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        SetXmlData()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()

    End Sub

#Region "DEVELOPER FEATURE ONLY"

    Private Function SetXmlData()
        Try
            ' Word pools for random generation
            Dim titles() As String = {"Dragon", "Quest", "Shadow", "Kingdom", "Crystal", "Knight", "Lost", "Ancient", "Hero", "Legend"}
            Dim maps() As String = {
    "MAP001", "MAP002", "MAP003", "MAP004", "MAP005",
    "MAP006", "MAP007", "MAP008", "MAP009", "MAP010",
    "MAP011", "MAP012", "MAP013", "MAP014", "MAP015"
}
            Dim events() As String = {
    "001", "002", "003", "004", "005", "006", "007", "008", "009", "010",
    "011", "012", "013", "014", "015", "016", "017", "018", "019", "020",
    "021", "022", "023", "024", "025", "026", "027", "028", "029", "030",
    "031", "032", "033", "034", "035", "036", "037", "038", "039", "040",
    "041", "042", "043", "044", "045", "046", "047", "048", "049", "050"
}
            Dim notesPool() As String = {"Found a secret door.", "The party is tired.", "Gained a level.", "Found 100 gold."}

            Dim doc = XDocument.Load(filePath)
            Dim rnd As New Random()

            For i As Integer = 1 To 30
                ' Mix random words and append the index 'i' to keep titles unique
                Dim rTitle As String = titles(rnd.Next(titles.Length)) & " " & titles(rnd.Next(titles.Length)) & " " & i
                Dim rMap As String = maps(rnd.Next(maps.Length))
                Dim rEvent As String = events(rnd.Next(events.Length))
                Dim rCompleted As String = (rnd.Next(0, 2) = 0).ToString()
                Dim rNotes As String = notesPool(rnd.Next(notesPool.Length))

                Dim newStory As New XElement("Story",
                    New XElement("Title", rTitle),
                    New XElement("Map", rMap),
                    New XElement("Event", rEvent),
                    New XElement("Completed", rCompleted),
                    New XElement("Notes", rNotes)
                )
                doc.Root.Add(newStory)
            Next

            doc.Save(filePath)
            LoadList() ' Refresh the ListBox to show new data
            MessageBox.Show("30 sample records added!", "Debug Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            LogError("Fill Sample Error", ex.Message)
        End Try

    End Function

#End Region




End Class
