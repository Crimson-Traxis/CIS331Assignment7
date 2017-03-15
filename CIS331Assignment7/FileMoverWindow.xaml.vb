'------------------------------------------------------------ 
'-                File Name : FileMoverWindow.xaml.vb       - 
'-                Part of Project: Assignment 7             - 
'------------------------------------------------------------
'-                Written By: Trent Killinger               - 
'-                Written On: 3-12-17                       - 
'------------------------------------------------------------ 
'- File Purpose:                                            - 
'-                                                          - 
'- This file contains the file copy window                  -
'------------------------------------------------------------
'- Variable Dictionary                                      - 
'- fileMover - background worker that moves the files       -
'- filesToMove - files to copy                              -
'- newLocation - location to copy files to                  -
'- replaceFiles - replace files?                            -
'- completedOperation - copy operation status               -
'- pendingUserInput - if the user has the exit dialog open  -
'------------------------------------------------------------

Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Threading

Public Class FileMoverWindow

    Private fileMover As BackgroundWorker
    Private filesToMove As List(Of String)
    Private newLocation As String
    Private replaceFiles As Boolean
    Private completedOperation As Boolean
    Private pendingUserInput As Boolean

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
    Public Sub New(filesToMove As List(Of String), newLocation As String, replaceFiles As Boolean)

        InitializeComponent()

        Me.filesToMove = filesToMove
        Me.newLocation = newLocation
        Me.replaceFiles = replaceFiles
        completedOperation = False
        pendingUserInput = False

        fileMover = New BackgroundWorker()
        fileMover.WorkerReportsProgress = True
        fileMover.WorkerSupportsCancellation = True

        AddHandler fileMover.DoWork, AddressOf FileMover_DoWork
        AddHandler fileMover.RunWorkerCompleted, AddressOf FileMover_WorkerCompleted

    End Sub

    '------------------------------------------------------------ 
    '-                Subprogram Name: Window_Closing           - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine determins if the user wants to exit then -
    '- alerts the background thread to cancel                   -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        If Not completedOperation Then
            Threading.Volatile.Write(pendingUserInput, True)
            If MessageBox.Show("Exit Operation?", "Exit Operation?", MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                e.Cancel = True
                fileMover.CancelAsync()
                Me.IsEnabled = False
            End If
            Threading.Volatile.Write(pendingUserInput, True)
        End If
    End Sub

    '------------------------------------------------------------ 
    '-                Subprogram Name: FileMover_DoWork         - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine does to work of copying the files to the -
    '- new location. It also will rename files if the user does -
    '- not want the files to be replaced                        -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- totalSize - total size of the files                      -
    '- bytesProcessed - bytes that have already been copied     -
    '- info - file information                                  -
    '- fileName - file name                                     -
    '------------------------------------------------------------
    Private Sub FileMover_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim totalSize As Double = 0
        Dim bytesProcessed As Double = 0
        For Each file As String In filesToMove
            Dim info As FileInfo = New FileInfo(file)
            totalSize += info.Length
        Next

        Dispatcher.Invoke(Sub()
                              textBlockTotalBytesToProcess.Text = totalSize
                              textBlockTotalFiles.Text = filesToMove.Count
                              progressBarProgress.Maximum = totalSize
                          End Sub)

        For index As Integer = 0 To filesToMove.Count - 1

            If fileMover.CancellationPending Then
                Exit For
            End If

            Dim fileName As String = filesToMove(index)
            Dim info As FileInfo = New FileInfo(fileName)

            Dispatcher.Invoke(Sub()
                                  textBlockFileIndexProc.Text = index + 1
                                  textBlockProcessingFile.Text = "Processing File: " & info.FullName
                              End Sub)
            If replaceFiles Then
                Try
                    File.Copy(info.FullName, newLocation & "\\" & info.Name, True)
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
                Dispatcher.Invoke(Sub()
                                      progressBarProgress.Value += info.Length
                                  End Sub)
                bytesProcessed += info.Length
            Else
                Dim renameInt As Integer = 1
                If File.Exists(newLocation & "\\" & info.Name) Then
                    While File.Exists(newLocation & "\\" & info.Name & " (" & renameInt & ")")
                        renameInt += 1
                    End While
                    Try
                        File.Copy(info.FullName, newLocation & "\\" & info.Name & " (" & renameInt & ")")
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                    End Try
                    bytesProcessed += info.Length
                Else
                    Try
                        File.Copy(info.FullName, newLocation & "\\" & info.Name, True)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                    End Try
                    bytesProcessed += info.Length
                End If
                Dispatcher.Invoke(Sub()
                                      progressBarProgress.Value += info.Length
                                      textBlockBytesProcessed.Text = bytesProcessed
                                  End Sub)
            End If
            System.Threading.Thread.Sleep(1000)
        Next
        While Threading.Volatile.Read(pendingUserInput)

        End While
    End Sub

    '------------------------------------------------------------ 
    '-       Subprogram Name: FileMover_WorkerCompleted         - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine closes the window and sets completed     -
    '- operation to true                                        -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub FileMover_WorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        completedOperation = True
        Me.Close()
    End Sub

    '------------------------------------------------------------ 
    '-       Subprogram Name: Window_ContentRendered            - 
    '------------------------------------------------------------
    '-                Written By: Trent Killinger               - 
    '-                Written On: 3-12-17                       - 
    '------------------------------------------------------------
    '- Subprogram Purpose:                                      - 
    '-                                                          - 
    '- This subroutine starts the background worker             -
    '------------------------------------------------------------
    '- Parameter Dictionary:                                    - 
    '- sender – Identifies which particular control raised the  - 
    '-          click event                                     - 
    '- e – Holds the EventArgs object sent to the routine       -    
    '------------------------------------------------------------ 
    '- Local Variable Dictionary:                               - 
    '- (none)                                                   -
    '------------------------------------------------------------
    Private Sub Window_ContentRendered(sender As Object, e As EventArgs)
        fileMover.RunWorkerAsync()
    End Sub
End Class
