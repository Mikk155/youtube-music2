using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using Spectre.Console;

class Program
{
    static bool ShowProgress;

    static string Workspace =>
        Directory.GetCurrentDirectory();

    static string MusicFolder =>
        Path.Combine( Workspace, "music" );

    static bool HasExtension( string executable )
    {
        if( !File.Exists( Path.Combine( Workspace, $"{executable}.exe" ) ) )
        {
            AnsiConsole.MarkupLine( $"[#ffA500]{executable}.exe[/] was not found at [#ffA500]{Workspace}[/]\nDownloading [#ffA500]{Workspace}/{executable}.exe[/]" );
            return false;
        }
        return true;
    }

    static void Exit( string? Message = null, int ExitCode = 1 )
    {
        if( Message is not null )
        {
            AnsiConsole.MarkupLine( $"[#ff0000]{Message}[/]" );
        }
        Environment.Exit(ExitCode);
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

        string? URL = args.Length > 0 ? args[0] : null;

        while( true )
        {
            if( string.IsNullOrWhiteSpace( URL ) || ( !URL.Contains( "youtube.com/" ) && !URL.Contains( "youtu.be/" ) ))
            {
                AnsiConsole.MarkupLine( $"Input a valid [#ffA500]youtube[/] playlist or video url" );
                URL = Console.ReadLine();
                continue;
            }

            break;
        }

        YoutubeDL ytdl = new YoutubeDL{
            OutputFolder = MusicFolder,
            FFmpegPath = Path.Combine( Workspace, "ffmpeg.exe" ),
            YoutubeDLPath = Path.Combine( Workspace, "yt-dlp.exe" ),
            IgnoreDownloadErrors = args.Contains( "-ignore" )
        };

        ShowProgress = !args.Contains( "-hideprogress" );

        Directory.CreateDirectory( ytdl.OutputFolder );

        AudioConversionFormat AudioFormat = AudioConversionFormat.Mp3;

        int CustomAudioFormat = Array.IndexOf( args, "-format" );

        if( CustomAudioFormat >= 0 && CustomAudioFormat < args.Length - 1 )
        {
            string CustomAudioFormatName = args[ CustomAudioFormat + 1 ];

            switch( CustomAudioFormatName )
            {
                case "0":
                case "best": {
                    AudioFormat = AudioConversionFormat.Best;
                    break;
                }
                case "1":
                case "aac": {
                    AudioFormat = AudioConversionFormat.Aac;
                    break;
                }
                case "2":
                case "flac": {
                    AudioFormat = AudioConversionFormat.Flac;
                    break;
                }
                case "3":
                case "mp3": {
                    AudioFormat = AudioConversionFormat.Mp3;
                    break;
                }
                case "4":
                case "m4a": {
                    AudioFormat = AudioConversionFormat.M4a;
                    break;
                }
                case "5":
                case "opus": {
                    AudioFormat = AudioConversionFormat.Opus;
                    break;
                }
                case "6":
                case "vorbis": {
                    AudioFormat = AudioConversionFormat.Vorbis;
                    break;
                }
                case "7":
                case "wav": {
                    AudioFormat = AudioConversionFormat.Wav;
                    break;
                }
                default:
                {
                    Exit( $"Invalid audio format \"[#ff0]{CustomAudioFormatName}[/]\"" );
                    break;
                }
            }
        }

        OptionSet opts = new OptionSet{
            ExtractAudio = true,
            AudioFormat = AudioFormat,
            AudioQuality = 0,
            NoOverwrites = true,
            Output = Path.Combine( MusicFolder, "%(title)s.%(ext)s" )/*,
            PostprocessorArgs = "--remove-source-files"*/
        };

        AnsiConsole.MarkupLine( $"Fetching [#ffA500]{URL}[/]" );

        string PlayListName = ( await ytdl.RunVideoDataFetch( URL ) ).Data?.Title ?? "Downloading";

        AnsiConsole.MarkupLine( $"Start of download [#ffA500]{PlayListName}[/]" );

        Progress<DownloadProgress> progress = new Progress<DownloadProgress>( p =>
        {
            string speed = p.DownloadSpeed ?? "--";
            string progress = $"{p.Progress:P0}";
            string Totalsize = p.TotalDownloadSize ?? "?";

            string? FileName = p.Data;

            if( FileName is not null && !string.IsNullOrWhiteSpace( FileName ) )
            {
                AnsiConsole.MarkupLine( $"Starting download [#ffA500]{Path.GetFileNameWithoutExtension(FileName)}.{AudioFormat.ToString()}[/]" );
            }

            switch( p.State )
            {
                case DownloadState.Downloading:
                {
                    if( ShowProgress )
                    {
                        // AnsiConsole.MarkupLine( $"speed: [#f00]{speed}[/] – [#ff0]{progress}[/] of [#ff0]{Totalsize}[/] time: [#00f]{p.ETA}[/]" );
                        AnsiConsole.MarkupLine( $"[#ff0]{progress}[/] of [#f00]{Totalsize}[/]" );
                        // -TODO Display the progress on the same line? I give up for today :)
                    }
                    break;
                }
                case DownloadState.PostProcessing:
                {
                    if( ShowProgress )
                    {
                        AnsiConsole.MarkupLine( $"[#f1f]Finished downloading[/]" );
                    }
                    break;
                }
                case DownloadState.PreProcessing:
                case DownloadState.Success:
                case DownloadState.Error:
                case DownloadState.None:
                default:
                {
                    break;
                }
            }
        } );

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => { cts.Cancel(); e.Cancel = true; };

        RunResult<string> result = await ytdl.RunWithOptions( URL, opts, progress: progress, ct: cts.Token );

        if( result.Success )
        {
            AnsiConsole.MarkupLine( $"[#0f0]Download complete![/]" );
        }
        else
        {
            Exit(result.ErrorOutput.ToString());
        }
    }
}
