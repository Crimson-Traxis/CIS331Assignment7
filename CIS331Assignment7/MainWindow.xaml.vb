'------------------------------------------------------------ 
'-                File Name : MainWindow.xaml.vb            - 
'-                Part of Project: Assignment 7             - 
'------------------------------------------------------------
'-                Written By: Trent Killinger               - 
'-                Written On: 3-12-17                       - 
'------------------------------------------------------------ 
'- File Purpose:                                            - 
'-                                                          - 
'- This file contains the main window                       -
'------------------------------------------------------------
'- Variable Dictionary                                      - 
'- clickPoint - last click location in source list view     -
'- sourceFileWatcher - watches source location for file     -
'-                     changes                              -
'- destinationFileWatcher - watches the destination location-
'-                          for file changes                -
'------------------------------------------------------------

Imports System.IO
Imports System.Windows.Threading

Class MainWindow

    Private clickPoint As Point
    Private sourceFileWatcher As FileSystemWatcher
    Private destinationFileWatcher As FileSystemWatcher

    '------------------------------------------------------------ 
    '-                Subprogram Name: New                      - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine creates the gui and instantiates default -
    '- member data/objects                                      -
    '------------------------------------------------------------ 
    '- Parameter Dictionary:                                    - 
    '- (None)                                                   - 
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (None)                                                   - 
    '------------------------------------------------------------
    Public Sub New()
        InitializeComponent()
        clickPoint = New Vector(0, 0)

        textBoxSourceDir.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        textBoxDestinationDir.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        sourceFileWatcher = New FileSystemWatcher(textBoxSourceDir.Text)
        destinationFileWatcher = New FileSystemWatcher(textBoxDestinationDir.Text)

        sourceFileWatcher.Path = textBoxSourceDir.Text
        destinationFileWatcher.Path = textBoxDestinationDir.Text

        sourceFileWatcher.NotifyFilter = NotifyFilters.FileName

        destinationFileWatcher.NotifyFilter = NotifyFilters.FileName

        AddHandler sourceFileWatcher.Changed, AddressOf sourceFileWatcher_FileChanged
        AddHandler destinationFileWatcher.Changed, AddressOf destinationFileWatcher_FileChanged

        AddHandler sourceFileWatcher.Created, AddressOf sourceFileWatcher_FileChanged
        AddHandler destinationFileWatcher.Created, AddressOf destinationFileWatcher_FileChanged

        AddHandler sourceFileWatcher.Deleted, AddressOf sourceFileWatcher_FileChanged
        AddHandler destinationFileWatcher.Deleted, AddressOf destinationFileWatcher_FileChanged

        sourceFileWatcher.EnableRaisingEvents = True
        destinationFileWatcher.EnableRaisingEvents = True
    End Sub

    '------------------------------------------------------------ 
    '-          Subprogram Name: sourceFileWatcher_FileChanged  - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine reloads the listview when directorys     -
    '- contents change                                          -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub sourceFileWatcher_FileChanged(sender As Object, e As FileSystemEventArgs)
        Dispatcher.Invoke(Sub()
                              ReloadList(textBoxSourceDir, listViewSourceDir)
                          End Sub)
    End Sub

    '------------------------------------------------------------ 
    '-     Subprogram Name: destinationFileWatcher_FileChanged  - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine reloads the listview when directorys     -
    '- contents change                                          -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub destinationFileWatcher_FileChanged(sender As Object, e As FileSystemEventArgs)
        Dispatcher.Invoke(Sub()
                              ReloadList(textBoxDestinationDir, listViewDestinationDir)
                          End Sub)
    End Sub

    '------------------------------------------------------------ 
    '-       Subprogram Name: sliderOverwrite_ValueChanged      - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine updates copy state label                 -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub sliderOverwrite_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles sliderOverwrite.ValueChanged
        If e.NewValue = 1 Then
            textBlockCopyState.Text = "Overwriting Existing Files"
        Else
            textBlockCopyState.Text = "Not Overwriting Existing Files"
        End If
    End Sub

    '------------------------------------------------------------ 
    '-       Subprogram Name: buttonSelectSource_Click          - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine opens and folder dialog to browse for a  -
    '- source folder                                            -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- folderBrowser - folder browser gui                       -
    '------------------------------------------------------------
    Private Sub buttonSelectSource_Click(sender As Object, e As RoutedEventArgs) Handles buttonSelectSource.Click
        Dim folderBrowser As System.Windows.Forms.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        folderBrowser.Description = "Browser for a folder"
        If folderBrowser.ShowDialog() = Forms.DialogResult.OK Then
            textBoxSourceDir.Text = folderBrowser.SelectedPath
        End If

    End Sub

    '------------------------------------------------------------ 
    '-       Subprogram Name: buttonSelectDestination_Click     - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine opens and folder dialog to browse for a  -
    '- destination folder                                       -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- folderBrowser - folder browser gui                       -
    '------------------------------------------------------------
    Private Sub buttonSelectDestination_Click(sender As Object, e As RoutedEventArgs) Handles buttonSelectDestination.Click
        Dim folderBrowser As System.Windows.Forms.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        folderBrowser.Description = "Browser for a folder"
        If folderBrowser.ShowDialog() = Forms.DialogResult.OK Then
            textBoxDestinationDir.Text = folderBrowser.SelectedPath
        End If
    End Sub

    '------------------------------------------------------------ 
    '-       Subprogram Name: textBoxSourceDir_TextChanged      - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine updates the listview with filenames  and -
    '- updates the filewatchers path                            -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub textBoxSourceDir_TextChanged(sender As Object, e As TextChangedEventArgs) Handles textBoxSourceDir.TextChanged
        Try
            sourceFileWatcher.Path = textBoxSourceDir.Text
        Catch ex As Exception

        End Try
        ReloadList(sender, listViewSourceDir)
    End Sub

    '------------------------------------------------------------ 
    '-     Subprogram Name: textBoxDestinationDir_TextChanged   - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine updates the listview with filenames  and -
    '- updates the filewatchers path                            -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub textBoxDestinationDir_TextChanged(sender As Object, e As TextChangedEventArgs) Handles textBoxDestinationDir.TextChanged
        Try
            destinationFileWatcher.Path = textBoxDestinationDir.Text
            destinationFileWatcher.EnableRaisingEvents = True
        Catch ex As Exception

        End Try
        ReloadList(sender, listViewDestinationDir)
    End Sub

    '-------------------------------------------------------------------- 
    '-  Subprogram Name: listViewSourceDir_PreviewMouseLeftButtonDown   - 
    '--------------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine saves the last left click point          -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub listViewSourceDir_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles listViewSourceDir.PreviewMouseLeftButtonDown
        clickPoint = e.GetPosition(listViewSourceDir)
    End Sub

    '------------------------------------------------------------ 
    '-     Subprogram Name: listViewSourceDir_MouseMove         - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine determines if the user wants to preform  -
    '- a drag operation. Then grabs all the selected files from -
    '- the listview                                             -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- position - current point of mouse                        -
    '- distance - distance between last click and current pos   -
    '- data - list of files to copy                             -
    '------------------------------------------------------------
    Private Sub listViewSourceDir_MouseMove(sender As Object, e As Input.MouseEventArgs) Handles listViewSourceDir.MouseMove
        Dim position As Point = e.GetPosition(listViewSourceDir)
        Dim distance As Vector = clickPoint - position
        If e.LeftButton And
            Math.Abs(distance.X) > SystemParameters.MinimumHorizontalDragDistance And
                Math.Abs(distance.Y) > SystemParameters.MinimumVerticalDragDistance Then
            Dim filesToCopy As List(Of String) = New List(Of String)
            For Each file As String In listViewSourceDir.SelectedItems
                filesToCopy.Add(file)
            Next
            Dim data As DataObject = New DataObject("filelist", filesToCopy)
            DragDrop.DoDragDrop(listViewSourceDir, data, DragDropEffects.All)
        End If
    End Sub

    '------------------------------------------------------------ 
    '-                Subprogram Name: ReloadList               - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine updates the listview                     -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- textbox –textbox that contains the location              - 
    '- listView – listview to update                            -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub ReloadList(textbox As TextBox, listView As ListView)
        listView.Items.Clear()
        Try
            For Each file As String In Directory.GetFiles(textbox.Text)
                listView.Items.Add(file)
            Next
        Catch ex As Exception

        End Try
    End Sub

    '------------------------------------------------------------ 
    '-     Subprogram Name: listViewDestinationDir_DragEnter    - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine determines if the control can import     -
    '- the data in the drop and updates the dragdrop effect     -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub listViewDestinationDir_DragEnter(sender As Object, e As DragEventArgs) Handles listViewDestinationDir.DragEnter
        If Not e.Data.GetDataPresent("filelist") Then
            e.Effects = DragDropEffects.None
        End If
    End Sub

    '------------------------------------------------------------ 
    '-     Subprogram Name: listViewDestinationDir_Drop         - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine determines if the control can import     -
    '- the data in the drop then imports the files list         -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- filesToCopy - list of files to copy                      -
    '- replace - replacing files?                               -
    '- mover - files mover window                               -
    '------------------------------------------------------------
    Private Sub listViewDestinationDir_Drop(sender As Object, e As DragEventArgs) Handles listViewDestinationDir.Drop
        If e.Data.GetDataPresent("filelist") Then
            Dim filesToCopy As List(Of String) = e.Data.GetData("filelist")
            Dim replace As Boolean = False
            If sliderOverwrite.Value = 1 Then
                replace = True
            End If
            Dim mover As FileMoverWindow = New FileMoverWindow(filesToCopy, textBoxDestinationDir.Text, replace)
            mover.ShowDialog()
        End If
    End Sub
End Class
