# SCLoader - Automated SoundCloud Download Tool

This is a Console application to periodically scan the personal SoundCloud "likes".
Downloads new tracks as mp3 and adds the cover image and ID3 tags automatically.

## Required Settings

The application requires a SoundCloud Account and a registered App under [http://soundcloud.com/you/apps](http://soundcloud.com/you/apps)

* SoundCloud User Name (Display Name)
* SoundCloud Password
* SoundCloud App ID
* SoundCloud App Client Secret

There are also some optional settings for the main application and custom configuration sections for included plug-ins available.

## Extending functionality

It is possible to create plug-ins to save the downloaded files or redirect the log output.
The plug-ins are cerated by implementing interfaces from SCLoaderShared project and loaded at runtime using MEF.

### Current storage provider (Interface IStorageProvider)

* LocalStorage (Simple file storage on local computer)

### Current logging provider (Interface ILogger)

* ConsoleLogger (Writes the output to the console window)

# License

MIT License (See LICENSE.txt and NOTICE.txt for details)

# Misc

Build to run at managed background worker providers like Microsoft Azure or AppHarbor.
Should also work on low energy devices like Raspberry Pi with Mono installed (not yet tested).
