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
            IgnoreDownloadErrors = args.Contains( "-ignore" )
        };

        ytdl.FFmpegPath = Path.Combine( Workspace, "ffmpeg.exe" );

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
            RestrictFilenames = true,
            NoOverwrites = true,
            Output = Path.Combine( MusicFolder, "%(title)s.%(ext)s" )/*,
            PostprocessorArgs = "--remove-source-files"*/
        };

        Progress<DownloadProgress> progress = new Progress<DownloadProgress>( p => {
            AnsiConsole.MarkupLine( $"[#ff0]{p.Progress:P0}[/] of [#ff0]{p.TotalDownloadSize ?? "?"}[/]\n{p.DownloadSpeed ?? "--"} – [#ff0]{p.State}[/]" );
        } );

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += ( _, e ) => { cts.Cancel(); e.Cancel = true; };

        RunResult<string> Result = await ytdl.RunWithOptions( URL, opts, progress: progress, ct: cts.Token );

        foreach( string webm in Directory.GetFiles( MusicFolder, "*.webm" ) )
        {
            File.Delete( Path.Combine( MusicFolder, webm ) );
        }

        if( Result.Success )
        {
            AnsiConsole.Markup( $"[#ff0]All done![/]\n" );
        }
        else
        {
            Exit( Result.ErrorOutput.ToString() );
        }
    }
}
