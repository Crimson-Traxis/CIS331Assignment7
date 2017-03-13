Imports System.IO
Imports Microsoft.Win32

Class MainWindow

    Private clickPoint As Point

    Public Sub New()
        InitializeComponent()
        clickPoint = New Vector(0, 0)
    End Sub

    Private Sub sliderOverwrite_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles sliderOverwrite.ValueChanged
        If e.NewValue = 1 Then
            textBlockCopyState.Text = "Overwriting Existing Files"
        Else
            textBlockCopyState.Text = "Not Overwriting Existing Files"
        End If
    End Sub

    Private Sub buttonSelectSource_Click(sender As Object, e As RoutedEventArgs) Handles buttonSelectSource.Click
        Dim folderBrowser As System.Windows.Forms.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        folderBrowser.Description = "Browser for a folder"
        If folderBrowser.ShowDialog() = Forms.DialogResult.OK Then
            textBoxSourceDir.Text = folderBrowser.SelectedPath
        End If

    End Sub

    Private Sub buttonSelectDestination_Click(sender As Object, e As RoutedEventArgs) Handles buttonSelectDestination.Click
        Dim folderBrowser As System.Windows.Forms.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        folderBrowser.Description = "Browser for a folder"
        If folderBrowser.ShowDialog() = Forms.DialogResult.OK Then
            textBoxDestinationDir.Text = folderBrowser.SelectedPath
        End If
    End Sub

    Private Sub textBoxSourceDir_TextChanged(sender As Object, e As TextChangedEventArgs) Handles textBoxSourceDir.TextChanged
        ReloadList(sender, listViewSourceDir)
    End Sub

    Private Sub textBoxDestinationDir_TextChanged(sender As Object, e As TextChangedEventArgs) Handles textBoxDestinationDir.TextChanged
        ReloadList(sender, listViewDestinationDir)
    End Sub

    Private Sub listViewSourceDir_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles listViewSourceDir.PreviewMouseLeftButtonDown
        clickPoint = e.GetPosition(listViewSourceDir)
    End Sub

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

    Private Sub ReloadList(textbox As TextBox, listView As ListView)
        listView.Items.Clear()
        Dim filesList As List(Of String) = New List(Of String)()
        Try
            filesList = Directory.GetFiles(textbox.Text).ToList()

        Catch ex As Exception

        End Try
        For Each file As String In filesList
            listView.Items.Add(file)
        Next
    End Sub

    Private Sub listViewDestinationDir_DragEnter(sender As Object, e As DragEventArgs) Handles listViewDestinationDir.DragEnter
        If Not e.Data.GetDataPresent("filelist") Then
            e.Effects = DragDropEffects.None
        End If
    End Sub

    Private Sub listViewDestinationDir_Drop(sender As Object, e As DragEventArgs) Handles listViewDestinationDir.Drop
        If e.Data.GetDataPresent("filelist") Then
            Dim filesToCopy As List(Of String) = e.Data.GetData("filelist")
            Dim replace As Boolean = False
            If sliderOverwrite.Value = 1 Then
                replace = True
            End If
            Dim mover As FileMoverWindow = New FileMoverWindow(filesToCopy, textBoxDestinationDir.Text, replace)
            mover.ShowDialog()
            ReloadList(textBoxDestinationDir, listViewDestinationDir)
        End If
    End Sub
End Class
