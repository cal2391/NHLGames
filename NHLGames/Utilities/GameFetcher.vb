﻿Imports NHLGames.Objects

Namespace Utilities
    Public Class GameFetcher
        

        Public Shared Sub StreamingProgress
            NHLGamesMetro.FormInstance.spnStreaming.Visible = NHLGamesMetro.SpnStreamingVisible
            If NHLGamesMetro.SpnStreamingValue < NHLGamesMetro.FormInstance.spnStreaming.Maximum And NHLGamesMetro.SpnStreamingValue >= 0 Then
                NHLGamesMetro.FormInstance.spnStreaming.Value = NHLGamesMetro.SpnStreamingValue
            End If

            If  NHLGamesMetro.FormInstance.spnStreaming.Value > 0 Then
                NHLGamesMetro.FormInstance.spnStreaming.Visible = True
            Else
                NHLGamesMetro.SpnStreamingVisible = False
                NHLGamesMetro.FormInstance.spnStreaming.Visible = NHLGamesMetro.SpnStreamingVisible
            End If
        End Sub

        Public Shared Sub LoadingProgress
            NHLGamesMetro.FormInstance.spnLoading.Visible = NHLGamesMetro.SpnLoadingVisible
            If NHLGamesMetro.SpnLoadingValue < NHLGamesMetro.FormInstance.spnLoading.Maximum And NHLGamesMetro.SpnLoadingValue >= 0 Then
                NHLGamesMetro.FormInstance.spnLoading.Value = NHLGamesMetro.SpnLoadingValue
            End If

            If  NHLGamesMetro.FormInstance.spnLoading.Value > 0 Then
                InvokeElement.SetGameTabControls(False)
                NHLGamesMetro.FormInstance.lblNoGames.Visible = False
            Else
                NHLGamesMetro.SpnLoadingVisible = False
                NHLGamesMetro.FormInstance.spnLoading.Visible = NHLGamesMetro.SpnLoadingVisible
                InvokeElement.SetGameTabControls(True)
                If (NHLGamesMetro.FormInstance.flpGames.Controls.Count = 0) Then
                    NHLGamesMetro.FormInstance.lblNoGames.Visible = True
                Else
                    NHLGamesMetro.FormInstance.lblNoGames.Visible = False
                End If
            End If
        End Sub

        Public Shared Async Sub LoadGames()

            NHLGamesMetro.SpnLoadingValue = 1
            NHLGamesMetro.SpnLoadingVisible = True

            If Not NHLGamesMetro.FormLoaded Then
                NHLGamesMetro.SpnLoadingVisible = False
                NHLGamesMetro.SpnLoadingValue = 0
                Return
            End If

            NHLGamesMetro.GamesDict.Clear()

            Dim games As Game()
            Dim sortedGames As List(Of Game)

            InvokeElement.SetFormStatusLabel(NHLGamesMetro.RmText.GetString("msgLoadingGames"))

            Try
                Dim gm = new GameManager()
                games = Await gm.GetGamesAsync()
                gm.Dispose()

                NHLGamesMetro.SpnLoadingValue = NHLGamesMetro.spnLoadingMaxValue - 1
                Await Task.Delay(100)

                If games IsNot Nothing Then
                    sortedGames = SortGames(games)
                    AddGamesToDict(sortedGames)
                End If

                InvokeElement.NewGamesFound(NHLGamesMetro.GamesDict.Values.ToList())
                InvokeElement.SetFormStatusLabel(String.Format(NHLGamesMetro.RmText.GetString("msgGamesFound"), NHLGamesMetro.GamesDict.Values.Count.ToString()))

            Catch ex As Exception
                Console.WriteLine(ex.ToString())
                Return
            End Try

            NHLGamesMetro.SpnLoadingVisible = False
            NHLGamesMetro.SpnLoadingValue = 0
        End Sub

        Private Shared Function SortGames(games As Game()) As List(Of Game)
            If NHLGamesMetro.TodayLiveGamesFirst Then
                Return games.OrderBy(Of Boolean)(Function(val) val.IsUnplayable).
                    ThenBy(Of Boolean)(Function(val) val.IsFinal).
                    ThenBy(Of Long)(Function(val) val.GameDate.Ticks).ToList()
            Else
                Return games.OrderBy(Of Boolean)(Function(val) val.IsUnplayable).
                    ThenBy(Of Long)(Function(val) val.GameDate.Ticks).ToList()
            End If
        End Function

        Private Shared Sub AddGamesToDict(games As List(Of Game))
            For Each game As Game In games
                If NHLGamesMetro.GamesDict.Keys.Contains(game.GameId) Then Continue For
                NHLGamesMetro.GamesDict.Add(game.GameId, game)
            Next
        End Sub

    End Class
End Namespace
