﻿Imports System.IO
Imports System.Text.RegularExpressions
Imports NHLGames.My.Resources

Namespace Utilities

    Public Class ConsoleRedirectStreamWriter
        Inherits TextWriter
        Private ReadOnly _output As RichTextBox = Nothing

        Public Sub New(output As RichTextBox)
            _output = output
        End Sub

        Public Overrides ReadOnly Property Encoding() As Text.Encoding
            Get
                Return Text.Encoding.UTF8
            End Get
        End Property
            
        Public Overrides Sub WriteLine(value As String)
            Dim lastError As String = Nothing
            Dim regex As New Regex($"\[cli\](\[(.*?)\])?")
            MyBase.WriteLine(value)

            _output.BeginInvoke(
                New Action(
                    Function()
                        Dim startIndex As Integer = -1
                        Dim length As Integer = -1
                        Dim type As OutputType = OutputType.Normal
                        Dim timestamp As String = String.Format(English.msgDateTimeNow,Now.ToString("HH:mm:ss"))

                        If value.ToLower().IndexOf(English.errorDetection, StringComparison.Ordinal) = 0 OrElse
                            value.ToLower().IndexOf(English.errorExceptionDetection, StringComparison.Ordinal) = 0 Then
                            type = OutputType.Error
                            startIndex = _output.TextLength
                            length = value.IndexOf(English.errorDoubleDot, StringComparison.Ordinal) + 2
                            _output.AppendText(vbCr)
                        ElseIf value.ToLower().IndexOf(English.errorCliStreamer, StringComparison.Ordinal) = 0 Then
                            value = English.msgStreamer & regex.Replace(value, String.Empty)
                            type = OutputType.Cli
                            startIndex = _output.TextLength
                            length = value.IndexOf(English.errorDoubleDot, StringComparison.Ordinal) + 2
                            _output.AppendText(vbCr)
                        ElseIf value.ToLower().IndexOf(English.errorWarning, StringComparison.Ordinal) = 0 Then
                            type = OutputType.Warning
                            startIndex = _output.TextLength
                            length = value.IndexOf(English.errorDoubleDot, StringComparison.Ordinal) + 2
                            _output.AppendText(vbCr)
                        ElseIf value.IndexOf(":", StringComparison.Ordinal) > -1 Then
                            type = OutputType.Status
                            startIndex = _output.TextLength
                            length = value.IndexOf(English.errorDoubleDot, StringComparison.Ordinal) + 2
                            _output.AppendText(vbCr)
                        End If

                        If type = OutputType.Error Then
                            lastError = value
                        End If

                        value = timestamp & value
                        startIndex += timestamp.Length
                        _output.AppendText(value.ToString() & vbCr)

                        If startIndex > -1 Then
                            _output.Select(startIndex, length)

                            If type = OutputType.Error Then
                                _output.SelectionColor = Color.Red
                            ElseIf type = OutputType.Status Then
                                _output.SelectionColor = Color.Lime
                            ElseIf type = OutputType.Warning Then
                                _output.SelectionColor = Color.Yellow
                            ElseIf type = OutputType.Cli Then
                                _output.SelectionColor = Color.DeepSkyBlue
                            End If

                            _output.Select(startIndex, length)
                            _output.SelectionFont = New Font(_output.Font, FontStyle.Bold)
                        End If

                        If lastError <> Nothing Then 
                            InvokeElement.MsgBoxRed( String.Format(NHLGamesMetro.RmText.GetString("msgErrorGeneralText"),vbCrLf, lastError),
                                                    NHLGamesMetro.RmText.GetString("msgFailure"),
                                                    MessageBoxButtons.OK)
                        End If

                        Return Nothing
                    End Function))
        End Sub

    End Class
End Namespace
