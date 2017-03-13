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
        AddHandler fileMover.ProgressChanged, AddressOf FileMover_ReportProgress
        AddHandler fileMover.RunWorkerCompleted, AddressOf FileMover_WorkerCompleted

    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        If Not completedOperation Then
            Threading.Volatile.Write(pendingUserInput, True)
            If MessageBox.Show("Exit Operation?") = MessageBoxResult.Yes Then
                e.Cancel = True
                fileMover.CancelAsync()
                Me.IsEnabled = False
            End If
            Threading.Volatile.Write(pendingUserInput, True)
        End If
    End Sub

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
                              progressBarProgress.Maximum = totalSize / 10000.0
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
                                      fileMover.ReportProgress(info.Length)
                                  End Sub)
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
                Else
                    Try
                        File.Copy(info.FullName, newLocation & "\\" & info.Name, True)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message)
                    End Try
                End If
                Dispatcher.Invoke(Sub()
                                      fileMover.ReportProgress(info.Length)
                                  End Sub)
            End If
        Next
        While Threading.Volatile.Read(pendingUserInput)

        End While
    End Sub

    Private Sub FileMover_ReportProgress(sender As Object, e As ProgressChangedEventArgs)
        Dispatcher.Invoke(Sub()
                              Dim length As Double = e.UserState
                              progressBarProgress.Value += length / 10000.0
                          End Sub)
    End Sub

    Private Sub FileMover_WorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        completedOperation = True
        Me.Close()
    End Sub

    Private Sub Window_ContentRendered(sender As Object, e As EventArgs)
        fileMover.RunWorkerAsync()
    End Sub
End Class
