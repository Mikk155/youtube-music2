# youtube-music2

- ### Download music from a whole youtube playlist

---

> Run the program by double clicking and it'll ask for a youtube URL. it could be either a playlist or a single video.

---

> On executing the program it'll download two executables.
> - **yt-dlp.exe**
>     - This is in charge of the actual download of a youtube video
> - **ffmpeg.exe**
>     - This is in charge of the audio conversion

---

> Additional parameters if the program is run through a terminal.
> - **-ignore**
>   - Ignore download errors and keep downloading if it's a playlist.
> - **-hideprogress**
>   - Hide progress count. Ideally it should use only a single line to display the progress but i didn't make it work for now so this prevents the terminal from being spamed with the progress messages.
> - **-format "(Argument)"**
>   - Specify a format. if not set it will be .mp3. These formats must be one of:
>       - best
>       - aac
>       - flac
>       - mp3
>       - m4a
>       - opus
>       - vorbis
>       - wav

---

> To cancel a download at any time just close the terminal or you can press CTRL+C to stop it propertly.

---

> ### As far as i know youtube links are either ``youtube.com/`` or ``youtu.be/`` if i missed something Please let me know.
