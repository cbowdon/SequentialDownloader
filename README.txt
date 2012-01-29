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

Post-completion review:

Most of the above was met, with a nice responsive GUI that seems to work without error. It performs as fast as the Internet connection will allow and does not consume excessive resources. 

However, there was some specification drift as I realised that restricting this to just an image scraper was unnecessary. So the app's new functional spec would be: 

"I'm the user and I want to download remote files and save as zip archives. The remote files are at sequentially increasing URLs, either dates or just integers." 

Given this new spec, the ability to parse a webpage and estimate the source URL of a comic image is no longer required. I am unsure whether to remove it to avoid confusion, or to leave it as extra functionality.

Grabbing all extant comics from one URL is impractical and slow at best, so this requirement was dropped. I regret that I wasted time on this, and on automatically working out which was the first comic in a series.

Every modern operating system can zip and unzip, so the ability to convert between formats was left out of the GUI.
