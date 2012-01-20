What it is: 
An app to scrape comics into a CBZ.

Status: 
Barely started.

Functional specification:

"I'm the user and I want to download webcomics and save as CBZ format,
so I can view them on my iPhone. [Deleted: PDF format, because too
much trouble.]

I want to view this on my smart phone. Viewing will be handled by some
other app I already have, like CloudReader. Ideally, I would like to
do this all on my phone, but working on the desktop at first will be fine.

"All the webcomics are in pages at URLs that increase sequentially, either just numbers or dates. I want to give the program one URL and have it automatically grab all extant comics. I will be asked where to save the output and what format I want. I want to be able to convert existing files between formats also."

Early architectural plan (and hence tests to write):

// functions to generate comic URLs
// including something to parse the HTML and find which <img> is correct
ComicUrl ParseComicUrl (string originalUrl);
List<ComicUrl> GenerateComicUrls (ComicUrl originalComicUrl);

// repository object to manage collection
Path Repository.Location;
Dictionary<ComicUrl, Path> Repository.Files;

// functions to download comics
bool Repository.DownloadSingle (ComicUrl url);
List<bool> Repository.DownloadComics (IEnumerable<ComicUrl> urls);

// repository utilities
bool Repository.RenameFiles();
bool Repository.Contains(ComicUrl url);

// function to compile comic
bool CompileComic(Dictionary<Dictionary<ComicUrl,string> repoFiles, string outputstring);

// function wrapping it all up
bool GetComics (string originalUrl, string outputstring);

// also
bool ConvertComic (string input, string output);
bool ExtractComic (string input, string output);


