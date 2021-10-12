if WScript.Arguments.Count < 2 Then
    WScript.Echo "Please specify the source and the destination files. Usage: ExcelToCsv <xls/xlsx source file> <csv destination file>"
    Wscript.Quit
End If

csv_format = 6

Set objFSO = CreateObject("Scripting.FileSystemObject")

src_file = objFSO.GetAbsolutePathName(Wscript.Arguments.Item(0))
dest_file = objFSO.GetAbsolutePathName(WScript.Arguments.Item(1))

Dim oExcel
Set oExcel = CreateObject("Excel.Application")

Dim oBook
Set oBook = oExcel.Workbooks.Open(src_file)

rem If Dir("d:\Programming\UnityProjects\RotoShoot\RotoShootUnityProject\Assets\Resources\mySheet.csv") <> "" Then
rem     Kill "d:\Programming\UnityProjects\RotoShoot\RotoShootUnityProject\Assets\Resources\mySheet.csv"
rem End If

oBook.SaveAs dest_file, csv_format

oBook.Close False
oExcel.Quit