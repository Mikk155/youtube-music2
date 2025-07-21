using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using Spectre.Console;

class Program
{
    public static string Workspace =>
        Directory.GetCurrentDirectory();

    public static string MusicFolder =>
        Path.Combine( Workspace, "music" );

    public static bool HasExtension( string executable )
    {
        if( !File.Exists( Path.Combine( Workspace, $"{executable}.exe" ) ) )
        {
            AnsiConsole.MarkupLine( $"[#ffA500]{executable}.exe[/] was not found at [#ffA500]{Workspace}[/]\nDownloading [#ffA500]{Workspace}/{executable}.exe[/]" );
            return false;
        }
        return true;
    }

    static async Task Main( string[] args )
    {
        if( !HasExtension( "yt-dlp" ) )
        {
            await YoutubeDLSharp.Utils.DownloadYtDlp();
        }

        if( !HasExtension( "ffmpeg" ) )
        {
            await YoutubeDLSharp.Utils.DownloadFFmpeg();
        }
    }
}
