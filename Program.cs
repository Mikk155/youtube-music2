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

    public static void HasExtension( string executable )
    {
        if( !File.Exists( Path.Combine( Workspace, $"{executable}.exe" ) ) )
        {
            Console.WriteLine( $"ERROR: {executable}.exe was not found at {Workspace}" );
            Console.ReadLine();
            Environment.Exit(0);
        }
    }

    static async Task Main( string[] args )
    {
        HasExtension( "yt-dlp" );
        HasExtension( "ffmpeg" );
    }
}
