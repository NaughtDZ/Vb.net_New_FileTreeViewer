Public Class Form1
    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        ListBox1.Items.Clear()
        For Each file As String In GetFileList(False, e) 'GFL will return as Collection Or Readonly Collection(Depend showhidden true of false),But every orhter things like "listbox" or else,have it OWN Collection like "Lisbox.ObjectCollection"
            ListBox1.Items.Add(My.Computer.FileSystem.GetName(file))
        Next
    End Sub

    Private Sub TreeView1_AfterExpand(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterExpand
        'In this Sub,"e" is which node you click
        GetNextDir(False, e)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GetRootDir("", TreeView1, False)
    End Sub

    Public Sub GetRootDir(ByVal FromWhere As String, UseTreeview As TreeView, ShowHidden As Boolean)
        'Using In FormLoad or when you need initialization. If "FromWhere" is "",will start on Drive.Or start with input(Need full path).
        'UseTreeView is which TreeView you need load.
        'ShowHidden = true will add HiddenDir in TreeView ,may cause Insufficient authority Bug.
        UseTreeview.Nodes.Clear()
        If FromWhere = "" Then
            Dim Drives = IO.Directory.GetLogicalDrives
            For Each DriveName As String In Drives
                If My.Computer.FileSystem.GetDriveInfo(DriveName).IsReady = True Then UseTreeview.Nodes.Add(DriveName, DriveName)
            Next
        Else
            Dim RootDir = My.Computer.FileSystem.GetDirectories(FromWhere)
            For Each DirName As String In RootDir
                If ShowHidden = False Then
                    If (My.Computer.FileSystem.GetDirectoryInfo(DirName).Attributes And FileAttribute.Hidden) <> FileAttribute.Hidden Then UseTreeview.Nodes.Add(DirName, My.Computer.FileSystem.GetName(DirName))
                Else
                    UseTreeview.Nodes.Add(DirName, My.Computer.FileSystem.GetName(DirName))
                End If
            Next
        End If
        '---Read II Level dir to make "+" in Treeview---
        Dim secdirs As ObjectModel.ReadOnlyCollection(Of String)
        For RootnodeNum As Integer = 0 To UseTreeview.Nodes.Count - 1
            secdirs = My.Computer.FileSystem.GetDirectories(UseTreeview.Nodes.Item(RootnodeNum).Name)
            For Each secdirpath As String In secdirs
                If ShowHidden = False Then
                    If (My.Computer.FileSystem.GetDirectoryInfo(secdirpath).Attributes And FileAttribute.Hidden) <> FileAttribute.Hidden Then UseTreeview.Nodes.Item(RootnodeNum).Nodes.Add(secdirpath, My.Computer.FileSystem.GetName(secdirpath))
                Else
                    UseTreeview.Nodes.Item(RootnodeNum).Nodes.Add(secdirpath, My.Computer.FileSystem.GetName(secdirpath))
                End If
            Next
        Next
    End Sub

    Public Sub GetNextDir(ByVal ShowHidden As Boolean, Whichnode As TreeViewEventArgs)
        Dim nextdirs As ObjectModel.ReadOnlyCollection(Of String)
        For nodeNumb As Integer = 0 To Whichnode.Node.Nodes.Count - 1
            nextdirs = My.Computer.FileSystem.GetDirectories(Whichnode.Node.Nodes.Item(nodeNumb).Name) 'Goddamnit.Can Mircosoft make it more shotter? I use "My",because MS said it more faster then "IO"
            For Each nextdirname As String In nextdirs
                If ShowHidden = False Then
                    If (My.Computer.FileSystem.GetDirectoryInfo(nextdirname).Attributes And FileAttribute.Hidden) <> FileAttribute.Hidden Then Whichnode.Node.Nodes.Item(nodeNumb).Nodes.Add(nextdirname, My.Computer.FileSystem.GetName(nextdirname), My.Computer.FileSystem.GetName(nextdirname))
                Else
                    Whichnode.Node.Nodes.Item(nodeNumb).Nodes.Add(nextdirname, My.Computer.FileSystem.GetName(nextdirname), My.Computer.FileSystem.GetName(nextdirname))
                End If

            Next
        Next
    End Sub

    Public Function GetFileList(ByVal ShowHideen As Boolean, Whichnode As TreeViewEventArgs)
        Dim waitHideenCheckFileList As ObjectModel.ReadOnlyCollection(Of String)
        Dim checkdone As New Collection
        waitHideenCheckFileList = My.Computer.FileSystem.GetFiles(Whichnode.Node.Name)
        If ShowHideen = False Then
            For Each filepath As String In waitHideenCheckFileList
                If (My.Computer.FileSystem.GetFileInfo(filepath).Attributes And FileAttribute.Hidden) <> FileAttribute.Hidden Then checkdone.Add(filepath)
            Next
            Return checkdone
        Else
            Return waitHideenCheckFileList
        End If
    End Function
End Class
